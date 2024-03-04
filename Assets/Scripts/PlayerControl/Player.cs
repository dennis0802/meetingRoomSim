using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

namespace PlayerControl{
    [DisallowMultipleComponent]
    public class Player : MonoBehaviour {
        /// <summary>
        /// Player jump sound
        /// </summary> 
        [Tooltip("Sound to play when jumping")]
        [SerializeField]
        private AudioSource jumpSound;

        /// <summary>
        /// Player pointer object
        /// </summary>
        [Tooltip("Player pointer object")]
        [SerializeField]
        private Image playerPointer;

        /// <summary>
        /// Text object to store the name of the object being viewed
        /// </summary>
        [Tooltip("Text object to store the name of the object being viewed")]
        [SerializeField]
        private TextMeshProUGUI objectNameText;

        /// <summary>
        /// The object the player can interact with (ie. in line of sight)
        /// </summary>
        [Tooltip("The object the player can interact with (ie. in line of sight")]
        [SerializeField]
        private GameObject interactableObject;
        
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
        private bool isGrounded, isRunning, canInteract;

        /// <summary>
        /// Values for player movement physics
        /// </summary>  
        private float gravity = -9.81f, rotationSpeed = 5f, jumpHeight = 2f;

        /// <summary>
        /// Player current velocity
        /// </summary> 
        private Vector3 playerVelocity;
        
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
                Debug.Log("Player wants to interact with " + interactableObject.name);
            }

            // Jumping
            else if(playerJump.triggered && isGrounded){
                playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravity);
                jumpSound.Play();
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
    }
}
