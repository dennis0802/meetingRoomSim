using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using Levels;
using System.Linq;

namespace PlayerControl{
    [DisallowMultipleComponent]
    public class Player : MonoBehaviour {
        /// <summary>
        /// Player pointer object
        /// </summary>
        [Tooltip("Player pointer object")]
        [SerializeField]
        private Image playerPointer;

        /// <summary>
        /// Level manager
        /// </summary> 
        [Tooltip("Level manager")]
        [SerializeField]
        private LevelManager levelManager;

        /// <summary>
        /// Shop menus
        /// </summary> 
        [Tooltip("Shop menus")]
        [SerializeField]
        private ShopMenu personalShop, facilitiesShop;

        /// <summary>
        /// Text object to store the amount of office credit
        /// </summary>
        [Tooltip("Text object to store the amount of office credit")]
        [SerializeField]
        private TextMeshProUGUI creditText;

        /// <summary>
        /// Text object to store the name of the object being viewed
        /// </summary>
        [Tooltip("Text object to store the name of the object being viewed")]
        [SerializeField]
        private TextMeshProUGUI objectNameText;

        /// <summary>
        /// Text object to store status (ie. on click, display text)
        /// </summary>
        [Tooltip("Text object to store status (ie. on click, display text)")]
        [SerializeField]
        private TextMeshProUGUI statusText;

        /// <summary>
        /// The object the player can interact with (ie. in line of sight)
        /// </summary>
        [Tooltip("The object the player can interact with (ie. in line of sight")]
        [SerializeField]
        private GameObject interactableObject;

        /// <summary>
        /// Audio clips for interaction
        /// </summary>
        [Tooltip("Audio clips for interaction")]
        [SerializeField]
        private AudioClip[] interactionClips;

        /// <summary>
        /// Audio source for footsteps
        /// </summary> 
        [Tooltip("Audio source for footsteps")]
        [SerializeField]
        private AudioSource footstepsSound;

        /// <summary>
        /// Audio source
        /// </summary> 
        private AudioSource audioSource;
        
        /// <summary>
        /// Player input actions
        /// </summary> 
        private InputAction playerMove, playerInteract, startRun, playerJump, endRun;

        /// <summary>
        /// Camera transform following the player
        /// </summary> 
        private Transform cameraTransform;

        /// <summary>
        /// Controller for the player
        /// </summary> 
        private CharacterController controller;

        /// <summary>
        /// Flags for player actions
        /// </summary> 
        private bool isGrounded, isRunning, canInteract, delayedTaskActive;

        /// <summary>
        /// Values for player movement physics
        /// </summary>  
        private float gravity = -9.81f, rotationSpeed = 5f, jumpHeight = 2f;

        /// <summary>
        /// Timers
        /// </summary>  
        private float timer = 0.0f;

        /// <summary>
        /// Player current velocity
        /// </summary> 
        private Vector3 playerVelocity;

        /// <summary>
        /// List of delayed tasks
        /// </summary>
        private List<DelayedTask> delayedTasks;

        /// <summary>
        /// List of tasks based on progress
        /// </summary>
        private List<int> finishedDelayTasks, inProgressTasks;

        /// <summary>
        /// Player input object
        /// </summary> 
        private PlayerInput playerInput;

        /// <summary>
        /// Temp variable for temporal rift
        /// </summary>
        private GameObject temporalRift;

        /// <summary>
        /// The amount of office credit the player has
        /// </summary>
        public int officeCredit = 0;

        /// <summary>
        /// Waiting time for tasks
        /// </summary>
        public float microwaveWait = 10.0f, coffeeWait = 12.0f;
        
        /// <summary>
        /// Player speed
        /// </summary>  
        public float playerSpeed, basePlayerSpeed = 3.0f;

        // Start is called before the first frame update
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            playerInput = GetComponent<PlayerInput>();
            playerInteract = playerInput.actions["Interact"];
            playerMove = playerInput.actions["Move"];
            playerJump = playerInput.actions["Jump"];
            startRun = playerInput.actions["RunStart"];
            endRun = playerInput.actions["RunEnd"];
            startRun.performed += x => PressSprint();
            endRun.performed += x => ReleaseSprint();
            cameraTransform = Camera.main.transform;
            controller = GetComponent<CharacterController>();
            audioSource = GetComponent<AudioSource>();
            inProgressTasks = new List<int>();
            delayedTasks = new List<DelayedTask>();
            finishedDelayTasks = new List<int>();
        }

        // Update is called once per frame
        private void Update()
        {
            // Stop falling
            isGrounded = controller.isGrounded;
            if(isGrounded && playerVelocity.y < 0){
                playerVelocity.y = 0.0f;
            }

            // Running - only when key is hit
            playerSpeed = isRunning ? basePlayerSpeed * 2 : basePlayerSpeed;

            // Movement
            Vector2 input = playerMove.ReadValue<Vector2>();
            Vector3 move = new Vector3(input.x,0,input.y);
            move = move.x * cameraTransform.right.normalized + move.z * cameraTransform.forward.normalized;
            move.y = 0.0f;
            controller.Move(move * Time.deltaTime * playerSpeed);
            footstepsSound.enabled = !input.Equals(Vector2.zero) && isGrounded;

            // Falling
            playerVelocity.y += gravity * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);

            // Rotation
            Quaternion targetRotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Player interaction
            if(canInteract && playerInteract.triggered && !PauseMenu.IsPaused && !LevelManager.InUpgrades){
                Interact();
            }

            // Jumping
            if(playerJump.triggered && isGrounded){
                playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravity);
                audioSource.PlayOneShot(interactionClips[0], 0.5f);
            }
            
            // Timing
            // Check if a delayed task is finished
            if(delayedTasks.Count > 0){
                for(int i = delayedTasks.Count - 1; i >= 0; i--){
                    DelayedTask task = delayedTasks[i];
                    task.AddProgress(Time.deltaTime);

                    if(task.IsTaskDone()){
                        int id = task.GetTaskId();
                        delayedTasks.Remove(task);

                        switch(id){
                            case 0:
                                statusText.text = "Coffee is ready.";
                                audioSource.PlayOneShot(interactionClips[5], 1.0f);
                                finishedDelayTasks.Add(id);
                                break;
                            case 3:
                                statusText.text = "Laptop is ready to be started.";
                                finishedDelayTasks.Add(id);
                                break;
                            case 8:
                                statusText.text = "Temporal rift closed.";
                                inProgressTasks.Remove(8);
                                DespawnTaskObject(temporalRift, 11, 1.0f, 8);
                                officeCredit += 25;
                                break;
                            case 9:
                                statusText.text = "Microwave is done.";
                                audioSource.PlayOneShot(interactionClips[5], 1.0f);
                                finishedDelayTasks.Add(id);
                                break;
                        }
                    }
                }
            }

            // Clearing status
            if(delayedTasks.Count() == 0 && statusText.text != ""){
                timer += Time.deltaTime;
                if(timer >= 5.0f){
                    statusText.text = "";
                    timer = 0.0f;
                }
            }

            creditText.text = "CREDITS: " + officeCredit.ToString();
        }

        void FixedUpdate(){
            RaycastHit hit;
            bool raycast = Physics.Raycast(cameraTransform.position, cameraTransform.TransformDirection(Vector3.forward), out hit, 3, 1<<8);
            interactableObject = raycast ? hit.collider.gameObject.CompareTag("Worker") || hit.collider.gameObject.CompareTag("FacilitiesIT") || 
                                           hit.collider.gameObject.CompareTag("GossipWorker") || hit.collider.gameObject.CompareTag("Ninjas") ||
                                           hit.collider.gameObject.CompareTag("Paparazzi") || hit.collider.gameObject.CompareTag("Maya")
                                         ? hit.collider.gameObject.transform.parent.gameObject : hit.collider.gameObject : null;
            objectNameText.text = raycast ? hit.collider.gameObject.CompareTag("FacilitiesIT") ? interactableObject.name + " (IT)" : 
                                            hit.collider.gameObject.CompareTag("GossipWorker") ? interactableObject.name + " (Pest)": interactableObject.name : "";
            canInteract = raycast;
            playerPointer.color = raycast ? Color.green : Color.white;
        }

        /// <summary>
        /// Toggle running when shift key is hit
        /// </summary>
        private void PressSprint(){
            isRunning = true;
        }

        /// <summary>
        /// Toggle running when shift key is released
        /// </summary>
        private void ReleaseSprint(){
            isRunning = false;
        }

        /// <summary>
        /// Despawn a task object
        /// </summary>
        /// <param name="target">The target game object to despawn</param>
        /// <param name="clipNum">The associated audio clip number to play</param>
        /// <param name="volume">The associated audio clip volume</param>
        /// <param name="taskId">The associated task id to remove</param>
        private void DespawnTaskObject(GameObject target, int clipNum, float volume, int taskId){
            target.SetActive(false);
            audioSource.PlayOneShot(interactionClips[clipNum], volume);
            if(levelManager.taskIds.Contains(taskId)){
                levelManager.taskIds.Remove(taskId);
            }
        }

        /// <summary>
        /// Get player's office credit
        /// </summary>
        /// <returns>The amount of office credits the player has.</returns>
        public int GetCredits(){
            return officeCredit;
        }

        /// <summary>
        /// Set player's office credit
        /// </summary>
        /// <param name="credits">The new amount of credits</param>
        public void SetCredits(int credits){
            this.officeCredit = credits;
        }

        /// <summary>
        /// Interact with an object
        /// </summary>
        private void Interact(){
            if(interactableObject.CompareTag("FoodBox") && levelManager.taskIds.Contains(2)){
                statusText.text = "Food collected.";
                DespawnTaskObject(interactableObject.transform.parent.gameObject, 1, 0.5f, 2);
                officeCredit += 5;
            }

            else if(interactableObject.CompareTag("PersonalOffice")){
                levelManager.OpenUpgrades(0);
            }

            else if(interactableObject.CompareTag("FacilitiesIT")){
                levelManager.OpenUpgrades(1);
            }

            else if(interactableObject.CompareTag("Pen") && levelManager.taskIds.Contains(5)){
                statusText.text = "Statements on controversy made.";
                audioSource.PlayOneShot(interactionClips[2], 0.5f);
                levelManager.taskIds.Remove(5);
                officeCredit += 2;
            }

            else if(interactableObject.CompareTag("PaperStack") && levelManager.taskIds.Contains(10)){
                statusText.text = "Paperwork prepared.";
                DespawnTaskObject(interactableObject, 2, 0.5f, 10);
                officeCredit += 2;
            }

            else if(interactableObject.CompareTag("Mic") && levelManager.taskIds.Contains(1)){
                statusText.text = "Gave stunning input.";
                audioSource.PlayOneShot(interactionClips[12], 1.0f);
                levelManager.taskIds.Remove(1);
                officeCredit += 2;
            }

            else if(interactableObject.CompareTag("Gavel") && levelManager.taskIds.Contains(15)){
                int selected = Random.Range(0, 2);
                statusText.text = selected == 0 ? "The meeting was not guilty of internal theft." : "The meeting was guilty of internal theft.";
                audioSource.PlayOneShot(interactionClips[15], 1.0f);
                levelManager.taskIds.Remove(15);
                officeCredit += 10;
            }

            else if(interactableObject.CompareTag("Garbage") && levelManager.taskIds.Contains(19)){
                statusText.text = "Took out the trash.";
                audioSource.PlayOneShot(interactionClips[14], 1.0f);
                levelManager.taskIds.Remove(19);
                officeCredit += 2;
            }

            else if(interactableObject.CompareTag("Device") && levelManager.taskIds.Contains(4)){
                statusText.text = "Device collected.";
                DespawnTaskObject(interactableObject, 3, 0.5f, 4);
                officeCredit += 2;
            }

            else if(interactableObject.CompareTag("Fridge") && levelManager.taskIds.Contains(18)){
                statusText.text = "Fridge checked.";
                audioSource.PlayOneShot(interactionClips[7], 1.0f);
                levelManager.taskIds.Remove(18);
                officeCredit += 2;
            }

            else if(interactableObject.CompareTag("FirstAid") && levelManager.taskIds.Contains(16)){
                statusText.text = "Sick burn treated.";
                audioSource.PlayOneShot(interactionClips[7], 1.0f);
                levelManager.taskIds.Remove(16);
                officeCredit += 5;
            }

            else if(interactableObject.CompareTag("Extinguisher") && levelManager.taskIds.Contains(14)){
                statusText.text = "Extinguisher inspected.";
                audioSource.PlayOneShot(interactionClips[8], 1.0f);
                levelManager.taskIds.Remove(14);
                officeCredit += 5;
            }

            else if(interactableObject.CompareTag("LightSwitch") && levelManager.taskIds.Contains(11)){
                statusText.text = "Lights on.";
                audioSource.PlayOneShot(interactionClips[6], 1.0f);
                levelManager.ToggleLights(true);
                levelManager.taskIds.Remove(11);
                officeCredit += 2;
            }

            else if(interactableObject.CompareTag("Worker") && levelManager.taskIds.Contains(7)){
                statusText.text = interactableObject.name + " disciplined.";
                Worker worker = interactableObject.GetComponent<Worker>();
                worker.OnTask = true;
                audioSource.PlayOneShot(interactionClips[13], 1.0f);
                levelManager.taskIds.Remove(7);
                officeCredit += 10;
            }

            else if(interactableObject.CompareTag("TV") && levelManager.taskIds.Contains(12)){
                statusText.text = "TVs placed back.";
                audioSource.PlayOneShot(interactionClips[1], 1.0f);
                levelManager.ToggleTVs(true);
                levelManager.taskIds.Remove(12);
                officeCredit += 10;
            }

            else if(interactableObject.CompareTag("GossipWorker") && levelManager.taskIds.Contains(20)){
                string gossip = "";
                int selected = Random.Range(0,10);
                Worker worker = interactableObject.GetComponent<Worker>();

                switch(selected){
                    case 0:
                        gossip = "Begrudgingly gossiped with ";
                        break;
                    case 1:
                        gossip = "Spoke about management with ";
                        break;
                    case 2:
                        gossip = "Spoke about past with ";
                        break;
                    case 3:
                        gossip = "Discussed TV shows with ";
                        break;
                    case 4:
                        gossip = "Got roasted by ";
                        levelManager.taskIds.Add(16);
                        break;
                    case 5:
                        gossip = "You roasted ";
                        break;
                    case 6:
                        gossip = "Made a deal with ";
                        officeCredit += 5;
                        break;
                    case 7:
                        gossip = "Got robbed by ";
                        officeCredit -= 20;
                        officeCredit = officeCredit < 0 ? 0 : officeCredit;
                        break;
                    case 8:
                        gossip = "Discussed future with ";
                        break;
                    case 9:
                        gossip = "Awkward silence with ";
                        break;
                }

                audioSource.PlayOneShot(interactionClips[13], 1.0f);
                statusText.text = gossip + " " + worker.name;
                levelManager.taskIds.Remove(20);
                officeCredit += 10;
            }

            // Start a delayed task only if it is not in progress yet.
            else if(!inProgressTasks.Contains(9) && interactableObject.CompareTag("Microwave") && levelManager.taskIds.Contains(9)){
                statusText.text = "Heating for " + microwaveWait.ToString() + "s...";
                audioSource.PlayOneShot(interactionClips[4], 1.0f);
                DelayedTask task = new DelayedTask(9, microwaveWait);
                inProgressTasks.Add(9);
                delayedTasks.Add(task);
            }

            else if(finishedDelayTasks.Contains(9) && interactableObject.CompareTag("Microwave") && levelManager.taskIds.Contains(9)){
                statusText.text = "Hot food collected.";
                levelManager.taskIds.Remove(9);
                inProgressTasks.Remove(9);
                finishedDelayTasks.Remove(9);
                officeCredit += 5;
            }

            else if(!inProgressTasks.Contains(0) && interactableObject.CompareTag("CoffeeMaker") && levelManager.taskIds.Contains(0)){
                statusText.text = "Brewing coffee...";
                audioSource.PlayOneShot(interactionClips[4], 1.0f);
                DelayedTask task = new DelayedTask(0, coffeeWait);
                inProgressTasks.Add(0);
                delayedTasks.Add(task);
            }

            else if(finishedDelayTasks.Contains(0) && interactableObject.CompareTag("CoffeeMaker") && levelManager.taskIds.Contains(0)){
                statusText.text = "Coffee brewed.";
                levelManager.taskIds.Remove(0);
                inProgressTasks.Remove(0);
                finishedDelayTasks.Remove(0);
                officeCredit += 5;
            }

            else if(!inProgressTasks.Contains(3) && interactableObject.CompareTag("Laptop") && levelManager.taskIds.Contains(3)){
                statusText.text = "Laptop restarted.";
                audioSource.PlayOneShot(interactionClips[9], 1.0f);
                DelayedTask task = new DelayedTask(3, 5.0f);
                inProgressTasks.Add(3);
                delayedTasks.Add(task);
            }

            else if(finishedDelayTasks.Contains(3) && interactableObject.CompareTag("Laptop") && levelManager.taskIds.Contains(3)){
                statusText.text = "Laptop started.";
                audioSource.PlayOneShot(interactionClips[10], 1.0f);
                levelManager.taskIds.Remove(3);
                inProgressTasks.Remove(3);
                finishedDelayTasks.Remove(3);
                officeCredit += 2;
            }

            else if(!inProgressTasks.Contains(8) && interactableObject.CompareTag("TemporalRift") && levelManager.taskIds.Contains(8)){
                statusText.text = "Delegating rift task force...";
                DelayedTask task = new DelayedTask(8, 10.0f);
                inProgressTasks.Add(8);
                delayedTasks.Add(task);
                temporalRift = interactableObject;
            }

            // The related task is not active
            else{
                statusText.text = "No problem here.";
            }
        }
    }
}
