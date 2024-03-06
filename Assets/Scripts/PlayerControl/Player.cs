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
        /// Player speed
        /// </summary>  
        public float playerSpeed = 3.0f;

        /// <summary>
        /// Player input object
        /// </summary> 
        private PlayerInput playerInput;

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
            playerSpeed = isRunning ? 6.0f : 3.0f;

            // Movement
            Vector2 input = playerMove.ReadValue<Vector2>();
            Vector3 move = new Vector3(input.x,0,input.y);
            move = move.x * cameraTransform.right.normalized + move.z * cameraTransform.forward.normalized;
            move.y = 0.0f;
            controller.Move(move * Time.deltaTime * playerSpeed);

            // Falling
            playerVelocity.y += gravity * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);

            // Rotation
            Quaternion targetRotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Player interaction
            if(canInteract && playerInteract.triggered && !PauseMenu.IsPaused){
                if(interactableObject.CompareTag("FoodBox")){
                    statusText.text = "Food collected.";
                    DespawnTaskObject(interactableObject.transform.parent.gameObject, 1, 0.5f, 2);
                }

                else if(interactableObject.CompareTag("PaperStack")){
                    statusText.text = "Paperwork prepared.";
                    DespawnTaskObject(interactableObject, 2, 0.5f, 10);
                }

                else if(interactableObject.CompareTag("Device")){
                    statusText.text = "Device collected.";
                    DespawnTaskObject(interactableObject, 3, 0.5f, 4);
                }

                else if(interactableObject.CompareTag("Fridge") && levelManager.taskIds.Contains(18)){
                    statusText.text = "Fridge checked.";
                    levelManager.taskIds.Remove(18);
                }

                // Start a delayed task only if it is not in progress yet.
                else if(!inProgressTasks.Contains(9) && interactableObject.CompareTag("Microwave") && levelManager.taskIds.Contains(9)){
                    statusText.text = "Heating for 15s...";
                    DelayedTask task = new DelayedTask(9, 5.0f);
                    inProgressTasks.Add(9);
                    delayedTasks.Add(task);
                }

                else if(finishedDelayTasks.Contains(9) && interactableObject.CompareTag("Microwave") && levelManager.taskIds.Contains(9)){
                    statusText.text = "Hot food collected.";
                    levelManager.taskIds.Remove(9);
                    inProgressTasks.Remove(9);
                    finishedDelayTasks.Remove(9);
                }

                else if(!inProgressTasks.Contains(0) && interactableObject.CompareTag("CoffeeMaker") && levelManager.taskIds.Contains(0)){
                    statusText.text = "Brewing coffee...";
                    DelayedTask task = new DelayedTask(0, 10.0f);
                    inProgressTasks.Add(0);
                    delayedTasks.Add(task);
                }

                else if(finishedDelayTasks.Contains(0) && interactableObject.CompareTag("CoffeeMaker") && levelManager.taskIds.Contains(0)){
                    statusText.text = "Coffee brewed.";
                    levelManager.taskIds.Remove(0);
                    inProgressTasks.Remove(0);
                    finishedDelayTasks.Remove(0);
                }

                // The related task is not active
                else{
                    statusText.text = "No problem here.";
                }
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
                                break;
                            case 9:
                                statusText.text = "Microwave is done.";
                                break;
                        }

                        finishedDelayTasks.Add(id);
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
        }

        void FixedUpdate(){
            RaycastHit hit;
            bool raycast = Physics.Raycast(cameraTransform.position, cameraTransform.TransformDirection(Vector3.forward), out hit, 3, 1<<8);
            interactableObject = raycast ? hit.collider.gameObject : null;
            objectNameText.text = raycast ? interactableObject.name : "";
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
    }
}
