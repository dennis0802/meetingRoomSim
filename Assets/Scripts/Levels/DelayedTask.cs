
namespace Levels{
    /// <summary>
    /// Helper class for delayed tasks
    /// </summary>
    public class DelayedTask{
        /// <summary>
        /// Task id
        /// </summary>
        public int id;

        /// <summary>
        /// Progress done
        /// </summary>
        private float progressDone = 0.0f;

        /// <summary>
        /// Target time for task
        /// </summary>
        public float targetTime;

        /// <summary>
        /// Constructor for a delayed task
        /// </summary>
        /// <param name="id">The id of the task</param>
        /// <param name="targetTime">The amount of time that must be spent to finish the task</param>
        public DelayedTask(int id, float targetTime){
            this.id = id;
            this.targetTime = targetTime;
        }

        /// <summary>
        /// Add progress
        /// </summary>
        /// <param name="progression">The amount to progress</param>
        public void AddProgress(float progression){
            progressDone += progression;
        }

        /// <summary>
        /// Get task id
        /// </summary>
        /// <returns>The task id</returns>
        public int GetTaskId(){
            return id;
        }

        /// <summary>
        /// Is the task finished? (ie. Has enough time been spent?)
        /// </summary>
        /// <returns>True if target time is met, false otherwise</returns>
        public bool IsTaskDone(){
            return progressDone >= targetTime;
        }
    }
}
