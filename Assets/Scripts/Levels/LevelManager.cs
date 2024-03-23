using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Random=UnityEngine.Random;
using Cinemachine;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AI.States;
using AI;

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
        /// Personal upgrade screen
        /// </summary>
        [Tooltip("Personal upgrade screen")]
        [SerializeField]
        private GameObject personalUpgradeScreen;

        /// <summary>
        /// Facilities upgrade screen
        /// </summary>
        [Tooltip("Facilities upgrade screen")]
        [SerializeField]
        private GameObject facilitiesUpgradeScreen;

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
        /// Sound to play when generating a task
        /// </summary>
        [Tooltip("Sound to play when generating a task")]
        [SerializeField]
        private AudioClip taskSound;

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

        [Tooltip("Worker's laptop screen")]
        [SerializeField]
        private GameObject laptopScreen;

        [Tooltip("Default laptop screen material")]
        [SerializeField]
        private Material defaultScreen;

        [Tooltip("Workers in the scene")]
        [SerializeField]
        private List<GameObject> workers;

        [Tooltip("TVs in the scene")]
        [SerializeField]
        private List<GameObject> tvs;

        [Tooltip("TV original positions")]
        [SerializeField]
        private List<Vector3> tvStartingPositions;

        [Tooltip("Lights in the scene")]
        [SerializeField]
        private List<Light> lights;

        [Header("Agents")]
        [Tooltip("The mind or global state agents are in.")]
        [SerializeField]
        private BaseState mind;

        [Tooltip("The maximum number of agents that can be updated in a single frame.")]
        [Min(0)]
        [SerializeField]
        private int maxAgentsPerUpdate;

        /// <summary>
        /// Office credit
        /// </summary>
        private int officeCredit;

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
        /// Agent index
        /// </summary>
        private int _currentAgentIndex;

        /// <summary>
        /// Singleton manager
        /// </summary>
        protected static LevelManager Singleton;

        /// <summary>
        /// Mind reference
        /// </summary>
        public static BaseState Mind => Singleton.mind;

        /// <summary>
        /// Random position to move to
        /// </summary>
        public static Vector2 RandomPosition => Random.insideUnitCircle * 45;

        /// <summary>
        /// All agents
        /// </summary>
        public List<BaseAgent> Agents {get; private set;} = new();

        /// <summary>
        /// All states
        /// </summary>
        private static readonly Dictionary<Type, BaseState> RegisteredStates = new();

        /// <summary>
        /// Rate of patience loss
        /// </summary>
        [Header("Rates and Thresholds")]
        [Tooltip("Rate of patience lost")]
        [SerializeField]
        private float patienceRate = 1;

        /// <summary>
        /// Rate of patience loss while out the room
        /// </summary>
        [Tooltip("Rate of patience lost while out the room")]
        [SerializeField]
        private float outOfRoomRate = 1;

        /// <summary>
        /// Patience regain rate
        /// </summary>
        [Tooltip("Patience regain rate")]
        [SerializeField]
        private float regainBonus = 0;

        /// <summary>
        /// Threshold for equipment failure
        /// </summary>
        [Tooltip("Threshold for equipment failure")]
        [SerializeField]
        private float minFailureThreshold = 0.0f;

        /// <summary>
        /// Static flags
        /// </summary>
        public static bool GameLost, GameWon, InUpgrades;

        // Start is called before the first frame update
        void Start()
        {
            taskArchive = new Task();
            taskTopic.text = taskArchive.MeetingTopic;
            audioSource = GetComponent<AudioSource>();
            workers.AddRange(GameObject.FindGameObjectsWithTag("Worker"));
            workers = workers.Where(w => w.transform.parent == null).ToList();
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

                    if(!taskIds.Contains(3)){
                        laptopScreen.GetComponent<Renderer>().material = defaultScreen;
                    }
                    
                    taskActive = taskIds.Count > 0;
                    if(seconds > 0){
                        // Tasks
                        for(int i = 0; i < taskIds.Count; i++){
                            taskText[i].text = "• " + taskArchive.taskDictionary[taskIds[i]];
                        }

                        if(taskIds.Count < 12){
                            for(int i = taskIds.Count; i < 12; i++){
                                taskText[i].text = "• ";
                            }
                        }

                        // Add tasks every interval provided the list isn't full
                        if(timer >= taskInterval && taskIds.Count < 12){
                            int generated;
                            float threshold;
                            // Ensure duplicate of an active task does not get generated
                            while(true){
                                generated = taskArchive.GenerateTask();
                                threshold = Random.Range(0.0f, 1.0f);
                                
                                if((generated == 3 || generated == 11 || generated == 12) && !taskIds.Contains(generated) && threshold >= minFailureThreshold){
                                    break;
                                }
                                else if(!taskIds.Contains(generated)){
                                    break;
                                }
                            }
                            taskIds.Add(generated);

                            // Spawn associated object if applicable
                            int selectedWorker;
                            string task;
                            Worker worker;

                            switch(generated){
                                case 0:
                                    break;
                                case 1:
                                    break;
                                case 2:
                                    SpawnTaskObject(taskObjects[0], taskObjectSpawnClips[0], 0.5f);
                                    break;
                                case 3:
                                    selectedWorker = Random.Range(0, workers.Count);
                                    task = "Fix " + workers[selectedWorker].name + "'s laptop";
                                    taskArchive.taskDictionary[3] = task;
                                    laptopScreen.GetComponent<Renderer>().material.color = Color.blue;
                                    audioSource.PlayOneShot(taskObjectSpawnClips[4], 0.5f);
                                    break;
                                case 4:
                                    SpawnTaskObject(taskObjects[2], taskObjectSpawnClips[2], 0.5f);
                                    break;
                                case 6:
                                    SpawnDestructibleObject(taskObjects[5], new Vector3(15.0f, 0f, -70f), taskObjectSpawnClips[6], 0.5f);
                                    SpawnDestructibleObject(taskObjects[5], new Vector3(15.0f, 0f, -70f), taskObjectSpawnClips[6], 0.5f);
                                    break;
                                case 7:
                                    selectedWorker = Random.Range(0, workers.Count);
                                    task = "Get " + workers[selectedWorker].name + " on task";
                                    worker = workers[selectedWorker].GetComponent<Worker>();
                                    worker.OnTask = false;
                                    taskArchive.taskDictionary[7] = task;
                                    break;
                                case 8:
                                    SpawnTaskObject(taskObjects[3], taskObjectSpawnClips[5], 0.5f);
                                    break;
                                case 9:
                                    break;
                                case 10:
                                    SpawnTaskObject(taskObjects[1], taskObjectSpawnClips[1], 0.5f);
                                    break;
                                case 11:
                                    ToggleLights(false);
                                    audioSource.PlayOneShot(taskObjectSpawnClips[2], 1.0f);
                                    break;
                                case 12:
                                    ToggleTVs(false);
                                    audioSource.PlayOneShot(taskObjectSpawnClips[3], 0.5f);
                                    break;
                                case 13:
                                    SpawnDestructibleObject(taskObjects[6], new Vector3(15.0f, 0f, -66f), taskObjectSpawnClips[7], 0.5f);
                                    SpawnDestructibleObject(taskObjects[6], new Vector3(15.0f, 0f, -72f), taskObjectSpawnClips[7], 0.5f);
                                    break;
                                case 14:
                                    break;
                                case 15:
                                    break;
                                case 16:
                                    break;
                                case 17:
                                    SpawnDestructibleObject(taskObjects[4], new Vector3(15.0f, 0f, -73f), taskObjectSpawnClips[3], 0.5f);
                                    break;
                                case 18:
                                    break;
                                case 19:
                                    break;
                                case 20:
                                    break;
                                default:
                                    break;
                            }
                            audioSource.PlayOneShot(taskSound, 1.0f);
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
                            patienceSlider.value -= Time.deltaTime * patienceRate * 2 + Time.deltaTime * outOfRoomRate + Time.deltaTime * regainBonus;
                        }
                        else{
                            if(taskActive){
                                patienceSlider.value += Time.deltaTime * patienceRate * 2;
                            }
                            if(player.transform.position.x > 0f){
                                patienceSlider.value += Time.deltaTime * outOfRoomRate;
                            }
                        }
                    }
                    // Timer done, tasks active
                    else if(seconds <= 0 && taskActive){
                        for(int i = 0; i < taskIds.Count; i++){
                            taskText[i].text = "• " + taskArchive.taskDictionary[taskIds[i]];
                        }

                        if(taskIds.Count < 12){
                            for(int i = taskIds.Count; i < 12; i++){
                                taskText[i].text = "• ";
                            }
                        }
                        patienceSlider.value += 0.01f;
                    }
                    else{
                        PrepGameOver(winScreen, winSound);
                        GameWon = true;
                    }
                }

                // AI actions
                if(Time.timeScale != 0){
                    if(maxAgentsPerUpdate <= 0){
                        for(int i = 0; i < Agents.Count; i++){
                            try{
                                Agents[i].Perform();
                            }
                            catch(Exception e){
                                Debug.LogError(e);
                            }
                        }
                    }
                    else{
                        for(int i = 0; i < maxAgentsPerUpdate; i++){
                            try{
                                Agents[_currentAgentIndex].Perform();
                            }
                            catch(Exception e){
                                Debug.LogError(e);
                            }
                            NextAgent();
                        }
                    }
                }

                foreach(BaseAgent agent in Agents){
                    agent.IncreaseDeltaTime();
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

        /// <summary>
        /// Spawn a task object that will be "destroyed" on completion
        /// </summary>
        /// <param name="target">The new task object</param>
        /// <param name="clip">The audio clip to play</param>
        /// <param name="volume">The volume of the audio clip</param>
        void SpawnDestructibleObject(GameObject target, Vector3 pos, AudioClip clip, float volume){
            Instantiate(target, pos, Quaternion.identity); 
            audioSource.PlayOneShot(clip, volume);
        }

        /// <summary>
        /// Turn on/off the lights
        /// </summary>
        /// <param name="state">Is the light on or off?</param>
        public void ToggleLights(bool state){
            foreach(Light light in lights){
                light.enabled = state;
            }
        }

        /// <summary>
        /// Hang/drop the TVs
        /// </summary>
        /// <param name="state">Are the TVs hanging from the wall?</param>
        public void ToggleTVs(bool state){
            // Hanging: put the TVs back in their starting position
            if(state){
                int i = 0;
                foreach(GameObject tv in tvs){
                    Rigidbody r = tv.GetComponent<Rigidbody>();
                    r.useGravity = false;
                    r.isKinematic = true;
                    tv.transform.localPosition = tvStartingPositions[i];
                    i++;
                }
            }
            // Otherwise trigger gravity
            else{
                foreach(GameObject tv in tvs){
                    Rigidbody r = tv.GetComponent<Rigidbody>();
                    r.useGravity = true;
                    r.isKinematic = false;
                }
            }
        }

        /// <summary>
        /// Open an upgrades screen
        /// </summary>
        /// <param name="id">The id of the menu to open; 0 for personal, 1 for facilities</param>
        public void OpenUpgrades(int id){
            Time.timeScale = 0.0f;
            gameCamInput.enabled = false;
            //mainScreen.SetActive(false);
            if(id == 0){
                personalUpgradeScreen.SetActive(true);
            }
            else{
                facilitiesUpgradeScreen.SetActive(true);
            }
            audioSource.PlayOneShot(taskObjectSpawnClips[0], 0.5f);
            InUpgrades = true;
            Cursor.lockState = CursorLockMode.None;
        }

        /// <summary>
        /// Close an upgrades screen
        /// </summary>
        public void CloseUpgrades(){
            Time.timeScale = 1.0f;
            gameCamInput.enabled = true;
            audioSource.PlayOneShot(taskObjectSpawnClips[1], 1.0f);
            InUpgrades = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        /// <summary>
        /// Increment the maximum patience
        /// </summary>
        public void IncrementMaxPatience(){
            patienceSlider.maxValue += 50f;
        }

        /// <summary>
        /// Get the patience rate
        /// </summary>
        /// <returns>The patience rate</returns>
        public float GetPatienceRate(){
            return this.patienceRate;
        }

        /// <summary>
        /// Set the patience rate
        /// </summary>
        /// <param name="value">The new patience rate</param>
        public void SetPatienceRate(float value){
            this.patienceRate = value;
        }

        /// <summary>
        /// Get the room patience rate
        /// </summary>
        /// <returns>The room patience rate</returns>
        public float GetRoomPatienceRate(){
            return this.outOfRoomRate;
        }

        /// <summary>
        /// Set the room patience rate
        /// </summary>
        /// <param name="value">The new room patience rate</param>
        public void SetRoomPatienceRate(float value){
            this.outOfRoomRate = value;
        }

        /// <summary>
        /// Get the regain rate
        /// </summary>
        /// <returns>The regain rate</returns>
        public float GetRegainRate(){
            return this.regainBonus;
        }

        /// <summary>
        /// Set the regain rate
        /// </summary>
        /// <param name="value">The new regain patience rate</param>
        public void SetRegainRate(float value){
            this.regainBonus = value;
        }

        /// <summary>
        /// Get the failure threshold
        /// </summary>
        /// <returns>The failure threshold</returns>
        public float GetFailureThreshold(){
            return this.minFailureThreshold;
        }

        /// <summary>
        /// Set the failure threshold
        /// </summary>
        /// <param name="value">The new regain patience rate</param>
        public void SetFailureThreshold(float value){
            this.minFailureThreshold = value;
        }

        // ------------------------ STATES AND AI --------------------------------------------------------------
        /// <summary>
        /// Lookup a state type from the dictionary.
        /// </summary>
        /// <typeparam name="T">The type of state to register</typeparam>
        /// <returns>The state of the requested type.</returns>
        public static BaseState GetState<T>() where T : BaseState{
            return RegisteredStates.ContainsKey(typeof(T)) ? RegisteredStates[typeof(T)] : CreateState<T>();
        }

        /// <summary>
        /// Register a state type into the dictionary.
        /// </summary>
        /// <typeparam name="T">The type of state to register</typeparam>
        /// <returns>The state of the requested type.</returns>
        private static void RegisterState<T>(BaseState stateToAdd) where T : BaseState{
            RegisteredStates[typeof(T)] = stateToAdd;
        }

        /// <summary>
        /// Create a state type into the dictionary
        /// </summary>
        /// <typeparam name="T">The type of state to register</typeparam>
        /// <returns>The state of the requested type.</returns>
        private static BaseState CreateState<T>() where T : BaseState{
            RegisterState<T>(ScriptableObject.CreateInstance(typeof(T)) as BaseState);
            return RegisteredStates[typeof(T)];
        }

        /// <summary>
        /// Add an agent from the list of agents
        /// </summary>
        /// <param name="agent">The agent to add</param>
        public static void AddAgent(BaseAgent agent){
            if(Singleton.Agents.Contains(agent)){
                return;
            }

            Singleton.Agents.Add(agent);
        }

        /// <summary>
        /// Remove an agent from the list of agents
        /// </summary>
        /// <param name="agent">The agent to remove</param>
        public static void RemoveAgent(BaseAgent agent){
            if(!Singleton.Agents.Contains(agent)){
                return;
            }

            int index = Singleton.Agents.IndexOf(agent);
            Singleton.Agents.Remove(agent);
            if(Singleton._currentAgentIndex > index){
                Singleton._currentAgentIndex--;
            }
            if(Singleton._currentAgentIndex < 0 || Singleton._currentAgentIndex >= Singleton.Agents.Count){
                Singleton._currentAgentIndex = 0;
            }
        }

        /// <summary>
        /// Move to the next agent in context
        /// </summary>
        private void NextAgent(){
            _currentAgentIndex++;
            _currentAgentIndex = _currentAgentIndex >= Agents.Count ? 0 : _currentAgentIndex;
        }

        protected virtual void Awake(){
            if(Singleton == this){
                return;
            }

            if(Singleton != null){
                Destroy(gameObject);
                return;
            }
            Singleton = this;
        }
    }
}