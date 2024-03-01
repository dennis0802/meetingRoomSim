using System.Collections;
using System.Collections.Generic;
using TMPro;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace PlayerControl{
    [DisallowMultipleComponent]
    public class PauseMenu : MonoBehaviour
    {
        /// <summary>
        /// Player input
        /// </summary>
        [Header("Pause Menu UI")]
        [Tooltip("Player input object")]
        [SerializeField]
        private PlayerInput playerInput;
        
        /// <summary>
        /// Button click
        /// </summary>        
        [Tooltip("Button click audio")]
        [SerializeField]
        private AudioSource buttonClick;

        /// <summary>
        /// Pause UI
        /// </summary>
        [Tooltip("UI for pause menu")]
        [SerializeField]
        private GameObject pauseMenuUI;

        /// <summary>
        /// Main game UI
        /// </summary>
        [Tooltip("UI for main game")]
        [SerializeField]
        private GameObject mainGameUI;

        /// <summary>
        /// Virtual camera for the game
        /// </summary>
        [Tooltip("Game camera")]
        [SerializeField]
        private CinemachineInputProvider gameCamInput;

        [Header("Loading UI")]
        [SerializeField]
        [Tooltip("Progress bar for loading")]
        private Slider slider;

        [SerializeField]
        [Tooltip("Loading object")]
        private GameObject loadingScreen;

        [SerializeField]
        [Tooltip("Percentage text object")]
        private TextMeshProUGUI progressText;

        /// <summary>
        /// Is the game paused?
        /// </summary>
        public static bool IsPaused;

        /// <summary>
        /// Pause action
        /// </summary>
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
            gameCamInput.enabled = true;
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
            gameCamInput.enabled = false;
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
            LoadLevel(0);
        }

        public void LoadLevel(int index){
            StartCoroutine(LoadAsync(index));
        }

        IEnumerator LoadAsync(int index){
            AsyncOperation operation = SceneManager.LoadSceneAsync(index);
            loadingScreen.SetActive(true);

            while(!operation.isDone){
                float progress = Mathf.Clamp01(operation.progress/0.9f);

                slider.value = progress;
                Debug.Log(progress * 100f + "%");
                progressText.text = progress * 100f + "%";

                yield return null;
            }
        }
    }
}

