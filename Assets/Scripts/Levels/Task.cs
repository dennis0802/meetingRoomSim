using System.Collections.Generic;
using System.Collections;
using System;

namespace Levels{
    public class Task {
        /// <summary>
        /// To index tasks
        /// </summary>
        public Dictionary<int, string> taskDictionary = new Dictionary<int, string>();

        /// <summary>
        /// Flavor text to describe why we're having a meeting
        /// </summary>
        public string meetingTopic;

        /// <summary>
        /// Task constructor - initialize dictionary and topic
        /// </summary>
        public Task(){
            taskDictionary.Add(0, "Get coffee");
            taskDictionary.Add(1, "Talk in meeting");
            taskDictionary.Add(2, "Collect catered food");
            taskDictionary.Add(3, "Ziptie wires");
            taskDictionary.Add(4, "Send devices");
            taskDictionary.Add(5, "Create statements");
            taskDictionary.Add(6, "Shoo paparazzi");
            taskDictionary.Add(7, "Get Michael on task");
            taskDictionary.Add(8, "Shoo a temporal rift");
            taskDictionary.Add(9, "Fix a technical issue");
            taskDictionary.Add(10, "Prepare paperwork");
            taskDictionary.Add(11, "Turn on the lights");
            taskDictionary.Add(12, "Hang TVs back up");
            taskDictionary.Add(13, "Shoo party ninjas");
            taskDictionary.Add(14, "Play Magic");
            taskDictionary.Add(15, "Stop a toxic joke");
            taskDictionary.Add(16, "Put out Dwight's fire");
            taskDictionary.Add(17, "Catch Maya the dog");

            Random rnd = new Random();
            int topicId = rnd.Next(0, 12);
            meetingTopic = topicId == 0 ? "Progress" : topicId == 1 ? "How To Work" : topicId == 2 ? "Your Day" : topicId == 3 ? "My Love Life's DOA" :
                           topicId == 4 ? "Dept. Day-to-Day" : topicId == 5 ? "Dept. Projections" : topicId == 6 ? "Call of Duty" : topicId == 7 ? "Company Projections" :
                           topicId == 8 ? "Knocking on a Door" : topicId == 9 ? "Unhinged Profs" : topicId == 10 ? "Jupiter Sales" :
                           topicId == 11 ? "Conspiracies" : "Undefined";
        }

        /// <summary>
        /// Generates a random task to assign the player
        /// </summary>
        /// <returns> The index of a task </returns>
        public int GenerateTask(){
            Random rnd = new Random();
            return rnd.Next(0, taskDictionary.Count);
        }
    }
}