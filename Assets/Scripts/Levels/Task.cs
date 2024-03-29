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
        public string MeetingTopic {get; private set;}

        /// <summary>
        /// Task constructor - initialize dictionary and topic
        /// </summary>
        public Task(){
            taskDictionary.Add(0, "Brew coffee"); // Done with audio
            taskDictionary.Add(1, "Talk in meeting"); // Done with audio
            taskDictionary.Add(2, "Get food at entry"); // Done with audio
            taskDictionary.Add(3, "Fix laptop"); // done with audio
            taskDictionary.Add(4, "Get device in hall"); // done with audio
            taskDictionary.Add(5, "Create statements"); // done with audio
            taskDictionary.Add(6, "Shoo paparazzi"); // done
            taskDictionary.Add(7, "Get worker on task"); // Done with audio
            taskDictionary.Add(8, "Shoo a temporal rift"); // done with audio
            taskDictionary.Add(9, "Heat up food"); // done with audio
            taskDictionary.Add(10, "Prepare paperwork"); // Done with audio
            taskDictionary.Add(11, "Turn on the lights"); // done with audio
            taskDictionary.Add(12, "Hang TVs back up"); // done with audio
            taskDictionary.Add(13, "Shoo party ninjas"); // done
            taskDictionary.Add(14, "Inspect extinguisher"); // done with audio
            taskDictionary.Add(15, "Preside over trial"); // done with audio
            taskDictionary.Add(16, "Treat a sick burn"); // done with audio
            taskDictionary.Add(17, "Catch Maya the dog"); // done with audio
            taskDictionary.Add(18, "Check fridge"); // done with audio
            taskDictionary.Add(19, "Take out the trash"); // done with audio
            taskDictionary.Add(20, "Water cooler gossip"); // done with audio

            Random rnd = new Random();
            int topicId = rnd.Next(0, 12);
            MeetingTopic = topicId == 0 ? "Progress" : topicId == 1 ? "How To Work" : topicId == 2 ? "Your Day" : topicId == 3 ? "My Love Life's DOA" :
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