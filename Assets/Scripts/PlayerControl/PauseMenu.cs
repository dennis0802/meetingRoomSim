using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace PlayerControl{
    public class PauseMenu : MonoBehaviour
    {
        [Tooltip("Player input object")]
        [SerializeField]
        private PlayerInput playerInput;
        
        [Tooltip("Button click audio")]
        [SerializeField]
        private AudioSource buttonClick;

        [Tooltip("UI for pause menu")]
        [SerializeField]
        private GameObject pauseMenuUI;

        [Tooltip("UI for main game")]
        [SerializeField]
        private GameObject mainGameUI;

        // To track if the game is paused.
        public static bool IsPaused;
        private InputAction pauseAction;

        void Start(){
            pauseAction = playerInput.actions["Pause"];
        }

        void Update(){
            if(SceneManager.GetActiveScene().buildIndex > 0 && pauseAction.triggered){
                if(IsPaused){
                    Resume();
                }
                else{
                    Pause();
                }
            }
        }

        /// <summary>
        /// Resume the game
        /// </summary>
        public void Resume(){
            buttonClick.Play();
            pauseMenuUI.SetActive(false);
            mainGameUI.SetActive(true);
            Time.timeScale = 1.0f;
            IsPaused = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        /// <summary>
        /// Pause the game
        /// </summary>
        public void Pause(){
            buttonClick.Play();
            pauseMenuUI.SetActive(true);
            mainGameUI.SetActive(false);
            Time.timeScale = 0.0f;
            IsPaused = true;
            Cursor.lockState = CursorLockMode.None;
        }

        /// <summary>
        /// Load the main menu
        /// </summary>
        public void LoadMenu(){
            IsPaused = false;
            pauseMenuUI.SetActive(false);
            Time.timeScale = 1.0f;
            SceneManager.LoadScene(0);
        }

        /// <summary>
        /// Quit the game
        /// </summary>
        public void QuitGame(){
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                UnityEngine.Application.Quit();
            #endif
        }
    }
}

