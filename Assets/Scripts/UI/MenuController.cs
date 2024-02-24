using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI{
    public class MenuController : MonoBehaviour {

        /// <summary>
        /// Camera object containing the animations
        /// </summary>
        [SerializeField]
        [Tooltip("Camera object to use for animations")]
        private GameObject cameraObj;

        private Animation anim;
        // 0 = main, 1 = mode selection, 2 = about, 3 = scores/settings
        private int menuIndex = 0;

        void Start(){
            anim = cameraObj.GetComponent<Animation>();
        }

        /// <summary>
        /// Play the game
        /// </summary>
        public void PlayGame(){
            anim.Play("camera_to_modes");
            menuIndex = 1;
        }

        /// <summary>
        /// Open about menu
        /// </summary>
        public void AboutMenu(){
            anim.Play("camera_to_about");
            menuIndex = 2;
        }

        /// <summary>
        /// Open settings menu
        /// </summary>
        public void Settings(){
            anim.Play("camera_to_scores_settings");
            menuIndex = 3;
        }

        /// <summary>
        /// Open scores menu
        /// </summary>
        public void Scores(){
            anim.Play("camera_to_scores_settings");
            menuIndex = 3;
        }

        /// <summary>
        /// Return to main menu
        /// </summary>
        public void BackToMenu(){
            switch(menuIndex){
                case 1:
                    anim.Play("mode_to_default");
                    break;
                case 2:
                    anim.Play("about_to_default");
                    break;
                case 3:
                    anim.Play("score_settings_to_default");
                    break;
            }
            menuIndex = 0;
        }

        /// <summary>
        /// Exit the game
        /// </summary>
        public void ExitGame(){
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                UnityEngine.Application.Quit();
            #endif
        }
    }
}