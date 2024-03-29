using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using AI.Sensors;
using System.Linq;
using AI.States;
using Levels;

// Based off of coursework for Dr. Vasighizaker's COMP4770-2023W Artificial Intelligence for Games

namespace AI{
    /// <summary>
    /// Base class for all agents
    /// </summary>
    public abstract class BaseAgent : MonoBehaviour
    {
        public BaseSensor[] Sensors {get; private set;}

        /// <summary>
        /// NavMeshAgent of the agent
        /// </summary>
        [Tooltip("Navmesh agent controlling the agent")]
        [SerializeField]
        private NavMeshAgent NavMeshAgent;

        /// <summary>
        /// Current destination of the agent
        /// </summary>
        private Vector3 TargetDest;

        /// <summary>
        /// Current velocity of the agent
        /// </summary>
        public Vector3 Velocity {get; private set;}

        /// <summary>
        /// If the agent is moving
        /// </summary>
        public bool IsMoving {get; private set;}

        /// <summary>
        /// Current state of the agent
        /// </summary>
        public BaseState State {get; private set;}

        /// <summary>
        /// How far away this mutant can detect party members.
        /// </summary>
        public float DetectionRange { get; private set; }

        /// <summary>
        /// Time passed since last time the agent's mind made decisions
        /// </summary>
        public float DeltaTime { get; private set; }

        protected virtual void Start(){
            Setup();

            if(LevelManager.Mind != null){
                LevelManager.Mind.Enter(this);
            }

            if(State != null){
                State.Enter(this);
            }
        }

        /// <summary>
        /// Perform current state action
        /// </summary>
        public virtual void Perform(){
            if(LevelManager.Mind != null){
                LevelManager.Mind.Execute(this);
            }
            
            if(State != null){
                State.Execute(this);
            }

            DeltaTime = 0;
        }

        /// <summary>
        /// Read a sensor and receive given data piece.
        /// </summary>
        /// <typeparam name="TSensor">The sensor type to read.</typeparam>
        /// <typeparam name="TData">The expected data to return.</typeparam>
        /// <returns>The data piece if returned by the given sensor type, default otherwise.</returns>
        public TData Sense<TSensor, TData>() where TSensor : BaseSensor{
            // Get relevant sensors
            foreach(BaseSensor sensor in Sensors){
                if(sensor is not TSensor){
                    continue;
                }

                // If correct type and data returned, return it.
                object data = sensor.Sense();
                if(data is TData correctType){
                    return correctType;
                }
            }

            return default;
        }

        /// <summary>
        /// Setup the agent
        /// </summary>
        public void Setup(){
            LevelManager.AddAgent(this);

            // Find sensors
            List<BaseSensor> sensors = GetComponents<BaseSensor>().ToList();
            sensors.AddRange(GetComponentsInChildren<BaseSensor>());
            Sensors = sensors.Distinct().ToArray();

            foreach(BaseSensor sensor in Sensors){
                sensor.Agent = this;
            }
        }

        /// <summary>
        /// Set the state of the agent
        /// </summary>
        /// <typeparam name="T">The state to put the agent in</typeparam>
        public void SetState<T>() where T : BaseState{
            BaseState value = LevelManager.GetState<T>();

            if(State = value){
                return;
            }

            if(State != null){
                State.Exit(this);
            }

            State = value;

            if(State != null){
                State.Enter(this);
            }
        }

        /// <summary>
        /// Set the state of the agent
        /// </summary>
        /// <param name="dest">The destination to move the agent to<param>
        public void SetDestination(Vector3 dest){
            TargetDest = dest;
            NavMeshAgent.SetDestination(TargetDest);
        }

        /// <summary>
        /// Get the agent's current destination
        /// </summary>
        /// <returns>The target destination</returns>
        public Vector3 GetDestination(){
            return TargetDest;
        }

        /// <summary>
        /// Set the detection range of the agent
        /// </summary>
        /// <param name="detectionRange">The range this agent can detect up to<param>
        public void SetDetectionRange(float detectionRange){
            DetectionRange = detectionRange;
        }

        /// <summary>
        /// Get the agent's current velocity
        /// </summary>
        /// <returns>The agent's current velocity</returns>
        public Vector3 GetVelocity(){
            return NavMeshAgent.velocity;
        }

        /// <summary>
        /// Get the agent's max speed
        /// </summary>
        /// <returns>The agent's max speed</returns>
        public float GetSpeed(){
            return NavMeshAgent.speed;
        }

        /// <summary>
        /// Stop the agent (make its current position the destination)
        /// </summary>
        public void StopMoving(){
            SetDestination(transform.position);
        }

        /// <summary>
        /// Increase individual agent delta time
        /// </summary>
        public void IncreaseDeltaTime(){
            DeltaTime += Time.deltaTime;
        }

        /// <summary>
        /// Change agent rotation to look to a target.
        /// </summary>
        /// <param name="target"> The target to look towards </param>
        public void LookToTarget(Transform target){
            // Only rotate around the y-axis
            Vector3 targetRotation = new(target.position.x, transform.position.y, target.position.z);
            // For simplicity, assume instant look speed with Mathf.Infinity
            Vector3 rotation = Vector3.RotateTowards(transform.forward, targetRotation - transform.position, Mathf.Infinity * Time.deltaTime, 0.0f);

            // Face the target
            transform.rotation = rotation == Vector3.zero || float.IsNaN(rotation.x) || float.IsNaN(rotation.y) || float.IsNaN(rotation.z) ? transform.rotation 
                                    : Quaternion.LookRotation(rotation); 
        }
    }
}