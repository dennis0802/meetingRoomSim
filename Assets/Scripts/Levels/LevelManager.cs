using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Levels{
    [DisallowMultipleComponent]
    public class LevelManager : MonoBehaviour
    {
        /// <summary>
        /// Text to display time
        /// </summary>
        [Header("Player UI")]
        [Tooltip("Text to display time")]
        [SerializeField]
        private TextMeshProUGUI timeText;

        /// <summary>
        /// Player
        /// </summary>
        [Tooltip("Player")]
        [SerializeField]
        private GameObject player;

        /// <summary>
        /// Slider to display patience
        /// </summary>
        [Tooltip("Slider to display patience")]
        [SerializeField]
        private Slider patienceSlider;

        /// <summary>
        /// Virtual camera for the game
        /// </summary>
        [Tooltip("Game camera")]
        [SerializeField]
        private CinemachineInputProvider gameCamInput;

        /// <summary>
        /// All task (bullet points) text objects"
        /// </summary>
        [Header("Tasks")]
        [Tooltip("All task (bullet points) text objects")]
        [SerializeField]
        private List<TextMeshProUGUI> taskText;

        /// <summary>
        /// Text object to display topic"
        /// </summary>
        [Tooltip("Text object to display topic")]
        [SerializeField]
        private TextMeshProUGUI taskTopic;

        /// <summary>
        /// List of task ids"
        /// </summary>
        [Tooltip("List of task ids")]
        [SerializeField]
        public List<int> taskIds;

        /// <summary>
        /// Game over screen
        /// </summary>
        [Header("Screens")]
        [Tooltip("Game over screen")]
        [SerializeField]
        private GameObject gameOverScreen;

        /// <summary>
        /// Winning screen
        /// </summary>
        [Tooltip("Winning screen")]
        [SerializeField]
        private GameObject winScreen;

        /// <summary>
        /// Main screen
        /// </summary>
        [Tooltip("Main screen")]
        [SerializeField]
        private GameObject mainScreen;

        /// <summary>
        /// Losing audio clip
        /// </summary>
        [Header("Audio")]
        [Tooltip("Losing audio clip")]
        [SerializeField]
        private AudioClip loseSound;

        /// <summary>
        /// Winning audio clip
        /// </summary>
        [Tooltip("Winning audio clip")]
        [SerializeField]
        private AudioClip winSound;

        /// <summary>
        /// Task object spawn audio clips
        /// </summary>
        [Tooltip("Task object spawn audio clips")]
        [SerializeField]
        private AudioClip[] taskObjectSpawnClips;

        /// <summary>
        /// Objects to spawn based on task
        /// </summary>
        [Header("Gameplay")]
        [Tooltip("Objects to spawn based on task")]
        [SerializeField]
        private List<GameObject> taskObjects;

        /// <summary>
        /// Audio source
        /// </sumamry>
        private AudioSource audioSource;

        /// <summary>
        /// Archive containing task data
        /// </summary>
        private Task taskArchive;

        /// <summary>
        /// Timing
        /// </summary>
        private float seconds, minutes, timer, taskInterval;

        /// <summary>
        /// Flags
        /// </summary>
        private bool setup, taskActive;

        /// <summary>
        /// Static flags
        /// </summary>
        public static bool GameLost, GameWon;

        // Start is called before the first frame update
        void Start()
        {
            taskArchive = new Task();
            taskTopic.text = taskArchive.MeetingTopic;
            audioSource = GetComponent<AudioSource>();
        }

        // Update is called once per frame
        void Update()
        {
            // For catch-up meeting
            if(SceneManager.GetActiveScene().buildIndex == 1 && !(GameLost || GameWon)){
                // Initialize
                if(!setup){
                    seconds = 300;
                    taskInterval = 15.0f;
                    setup = true;
                }
                else{
                    // Letting patience grow too high
                    if(patienceSlider.value >= 100f){
                        PrepGameOver(gameOverScreen, loseSound);
                        GameLost = true;
                    }

                    taskActive = taskIds.Count > 0;
                    if(seconds > 0){
                        // Tasks
                        for(int i = 0; i < taskIds.Count; i++){
                            taskText[i].text = "• " + taskArchive.taskDictionary[taskIds[i]];
                        }

                        // Add tasks every interval provided the list isn't full
                        if(timer >= taskInterval && taskIds.Count < 12){
                            int generated;
                            // Ensure duplicate of an active task does not get generated
                            while(true){
                                generated = taskArchive.GenerateTask();

                                if(!taskIds.Contains(generated)){
                                    break;
                                }
                            }
                            taskIds.Add(generated);

                            // Spawn associated object if applicable
                            switch(generated){
                                case 2:
                                    SpawnTaskObject(taskObjects[0], taskObjectSpawnClips[0], 0.5f);
                                    break;
                                case 4:
                                    SpawnTaskObject(taskObjects[3], taskObjectSpawnClips[0], 0.5f);
                                    break;
                                case 10:
                                    SpawnTaskObject(taskObjects[1], taskObjectSpawnClips[1], 0.5f);
                                    break;
                                default:
                                    break;
                            }
                            timer = 0.0f;
                        }

                        // Timer
                        timer += Time.deltaTime;
                        seconds -= Time.deltaTime;
                        minutes = (int)(seconds/60);
                        int displayedSeconds = ((int)seconds)%60;
                        timeText.text = displayedSeconds >= 10 ? "TIME: " + minutes + ":" + displayedSeconds : "TIME: " + minutes + ":0" + displayedSeconds;

                        // Increment patience if task is active or player is outside the room
                        if(!taskActive && player.transform.position.x <= 0f){
                            patienceSlider.value -= Time.deltaTime*4;
                        }
                        else{
                            if(taskActive){
                                patienceSlider.value += Time.deltaTime*2;
                            }
                            if(player.transform.position.x > 0f){
                                patienceSlider.value += Time.deltaTime*2;
                            }
                        }
                    }
                    // Timer done, tasks active
                    else if(seconds <= 0 && taskActive){
                        for(int i = 0; i < taskIds.Count; i++){
                            taskText[i].text = "• " + taskArchive.taskDictionary[taskIds[i]];
                        }
                        patienceSlider.value += 0.01f;
                    }
                    else{
                        PrepGameOver(winScreen, winSound);
                        GameWon = true;
                    }
                }
            }
        }

        /// <summary>
        /// Prepare to unload game objects to end game
        /// </summary>
        /// <param name="newScreen">The new screen to display to the player</param>
        /// <param name="clip">The audio clip to play</param>
        void PrepGameOver(GameObject newScreen, AudioClip clip){
            mainScreen.SetActive(false);
            newScreen.SetActive(true);
            audioSource.PlayOneShot(clip, 0.5f);
            gameCamInput.enabled = false;
            Cursor.lockState = CursorLockMode.None;
        }

        /// <summary>
        /// Spawn a task object
        /// </summary>
        /// <param name="target">The new task object</param>
        /// <param name="clip">The audio clip to play</param>
        /// <param name="volume">The volume of the audio clip</param>
        void SpawnTaskObject(GameObject target, AudioClip clip, float volume){
            target.SetActive(true);
            audioSource.PlayOneShot(clip, volume);
        }
    }
}