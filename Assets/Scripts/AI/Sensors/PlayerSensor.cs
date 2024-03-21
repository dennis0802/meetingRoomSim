using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using PlayerControl;

namespace AI.Sensors{
    public class PlayerSensor : BaseSensor
    {
        /// <summary>
        /// Sense the nearest player to the agent.
        /// </summary>
        /// <returns>The transform of the nearest collectible or null if none available</returns>
        public override object Sense(){
            if(Agent is not Maya and not Paparazzi and not PartyNinja){
                return null;
            }

            Player[] players = FindObjectsOfType<Player>().Where(t => Equals(t.tag, "Player") && Vector3.Distance(Agent.transform.position, t.transform.position) < 20.0f).ToArray();

            if(players.Length == 0){
                return null;
            }

            return players.OrderBy(b => Vector3.Distance(Agent.transform.position, b.transform.position)).First();
        }
    }
}