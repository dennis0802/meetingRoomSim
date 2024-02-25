using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UI;

namespace UI{
    public class Settings : MonoBehaviour {

        [Tooltip("Text object for descriptions")]
        [SerializeField]
        private TextMeshProUGUI muteDesc, resText, windowText;

        [Tooltip("Slider to control volume")]
        [SerializeField]
        private Slider volumeSlider;

        // Flag variables for muting and full screen
        private bool isMuted, isFullScreen;

        void Start(){
            isMuted = PlayerPrefs.GetInt("IsMuted") == 1;
            isFullScreen = PlayerPrefs.GetInt("IsFullScreen") == 1;
            muteDesc.text = isMuted ? "Muted" : "Unmuted";
            SetResolutionText();

            AudioListener.pause = isMuted;
            if(!PlayerPrefs.HasKey("Volume")){
                PlayerPrefs.SetFloat("Volume", 1);
                volumeSlider.value = PlayerPrefs.GetFloat("Volume");
            }
            else{
                volumeSlider.value = PlayerPrefs.GetFloat("Volume");
            }
            windowText.text = isFullScreen ? "Toggle: Full-screen" : "Toggle: Windowed";
            AudioListener.pause = isMuted;
        }

        /// <summary>
        /// Mute game audio
        /// </summary>
        public void Mute(){
            isMuted = !isMuted;
            AudioListener.pause = isMuted;
            PlayerPrefs.SetInt("IsMuted", isMuted ? 1 : 0);
            muteDesc.text = isMuted ? "Muted" : "Unmuted";
        }

        /// <summary>
        /// Change the volume of the game
        /// </summary>
        public void ChangeVolume(){
            AudioListener.volume = volumeSlider.value;
            PlayerPrefs.SetFloat("Volume", volumeSlider.value);
        }

        /// <summary>
        /// Change the window mode between fullscreen and windowed
        /// </summary>
        public void ChangeWindowMode(){
            #if UNITY_EDITOR
                Debug.Log("This setting is only visible in build versions.");
            #endif

            isFullScreen = !isFullScreen;
            switch(PlayerPrefs.GetInt("Resolution")){
                case 2:
                    Screen.SetResolution(1920, 1080, isFullScreen);
                    break;
                case 1:
                    Screen.SetResolution(1080, 960, isFullScreen);
                    break;
                case 0:
                    Screen.SetResolution(640, 480, isFullScreen);
                    break;
                default:
                    return;
            }
            PlayerPrefs.SetInt("IsFullScreen", isFullScreen ? 1 : 0);
            windowText.text = isFullScreen ? "Toggle: Full-screen" : "Toggle: Windowed";
        }        

        /// <summary>
        /// Change the resolution of the window (size)
        /// </summary>
        public void ChangeResolution(int flag){
            #if UNITY_EDITOR
                Debug.Log("This setting is only visible in build versions.");
            #endif

            // 0 is high, 1 is medium, 2 is low
            switch(flag){
                case 2:
                    Screen.SetResolution(1920, 1080, PlayerPrefs.GetInt("IsFullScreen") == 1);
                    PlayerPrefs.SetInt("Resolution", flag);
                    break;
                case 1:
                    Screen.SetResolution(1080, 960, PlayerPrefs.GetInt("IsFullScreen") == 1);
                    PlayerPrefs.SetInt("Resolution", flag);
                    break;
                case 0:
                    Screen.SetResolution(640, 480, PlayerPrefs.GetInt("IsFullScreen") == 1);
                    PlayerPrefs.SetInt("Resolution", flag);
                    break;
                default:
                    return;
            }
            SetResolutionText();
        }

        /// <summary>
        /// Change the text of the selected resolution
        /// </summary>
        private void SetResolutionText(){
            Vector3 pos = resText.GetComponent<RectTransform>().localPosition;

            switch(PlayerPrefs.GetInt("Resolution")){
                case 2:
                    resText.GetComponent<RectTransform>().localPosition = new Vector3(pos.x, -30f, pos.z);
                    break;
                case 1:
                    resText.GetComponent<RectTransform>().localPosition = new Vector3(pos.x, 30f, pos.z);
                    break;
                case 0:
                    resText.GetComponent<RectTransform>().localPosition = new Vector3(pos.x, 90f, pos.z);
                    break;
                default:
                    resText.GetComponent<RectTransform>().localPosition = new Vector3(pos.x, -30f, pos.z);
                    PlayerPrefs.SetInt("Resolution", 2);
                    return;
            }
        }
    }
}
