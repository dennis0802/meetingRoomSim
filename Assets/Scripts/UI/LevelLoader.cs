using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace UI{
    [DisallowMultipleComponent]
    public class LevelLoader : MonoBehaviour {
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
