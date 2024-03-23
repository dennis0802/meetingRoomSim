using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;
using AI.Sensors;
using PlayerControl;

namespace AI.States{
    [CreateAssetMenu(menuName = "AI/States/Office Mind", fileName = "Office Mind")]
    public class OfficeMind : BaseState
    {
        /// <summary>
        /// When agent first enters the state
        /// </summary> 
        public override void Enter(BaseAgent agent){}

        /// <summary>
        /// When agent executes the state
        /// </summary> 
        public override void Execute(BaseAgent agent){
            // NOTE: All movement operations need velocity to be checked to avoid weird looking movements from the destination changing constantly
            switch(agent){
                // ---------------------------------------- MAYA MIND ------------------------------------------------------
                case Maya maya:
                    Maya m = agent as Maya;

                    Player nearestPlayer = m.Sense<PlayerSensor, Player>();

                    // "Annoy" the player by chasing them
                    if(nearestPlayer is not null){
                        m.SetDestination(nearestPlayer.transform.position);
                    }

                    else if(m.GetVelocity().magnitude < m.minStopSpeed){
                        float zComponent = m.transform.position.z >= -43f ? -70f : -15f;
                        m.SetDestination(new Vector3(m.transform.position.x, m.transform.position.y, zComponent));
                    }
                    break;

                // ---------------------------------------- NINJA MIND ------------------------------------------------------
                case PartyNinja partyNinja:
                    PartyNinja pn = agent as PartyNinja;

                    Player nearestPlayerPn = pn.Sense<PlayerSensor, Player>();

                    // Run away from the player
                    if(nearestPlayerPn is not null){
                        Vector3 interest = nearestPlayerPn.transform.position;
                        float xComponent = interest.x - pn.transform.position.x < -10.0f ? interest.x - 8.0f : pn.transform.position.x > 10.0f ? interest.x + 8.0f : pn.transform.position.x;
                        float zComponent = interest.z - pn.transform.position.z < -10.0f ? interest.z - 8.0f : pn.transform.position.z > 10.0f ? interest.z + 8.0f : pn.transform.position.z;

                        Vector3 target = new Vector3(xComponent, interest.y, zComponent);
                        pn.SetDestination(target);
                    }

                    else if(pn.GetVelocity().magnitude < pn.minStopSpeed){
                        float zComponent = pn.transform.position.z >= -43f ? -70f : -15f;
                        pn.SetDestination(new Vector3(pn.transform.position.x, pn.transform.position.y, zComponent));
                    }                   
                    break;
                
                // ---------------------------------------- PAPARAZZI MIND ------------------------------------------------------
                case Paparazzi paparazzi:
                    Paparazzi p = agent as Paparazzi;

                    Player nearestPlayerP = p.Sense<PlayerSensor, Player>();

                    // "Annoy" the player by chasing them
                    if(nearestPlayerP is not null){
                        Vector3 interest = nearestPlayerP.transform.position;
                        float xComponent = interest.x - p.transform.position.x < -10.0f ? interest.x + 2.0f : p.transform.position.x > 10.0f ? interest.x - 2.0f : p.transform.position.x;
                        float zComponent = interest.z - p.transform.position.z < -10.0f ? interest.z + 2.0f : p.transform.position.z > 10.0f ? interest.z - 2.0f : p.transform.position.z;

                        Vector3 target = new Vector3(xComponent, interest.y, zComponent);
                        p.SetDestination(target);
                    }

                    else if(p.GetVelocity().magnitude < p.minStopSpeed){
                        float zComponent = p.transform.position.z >= -43f ? -70f : -15f;
                        p.SetDestination(new Vector3(p.transform.position.x, p.transform.position.y, zComponent));
                    }                   
                    break;

                // ---------------------------------------- INTERN MIND ------------------------------------------------------
                case Intern intern:
                    Intern i = agent as Intern;
                    break;



            }
        }

        /// <summary>
        /// When agent exits the state
        /// </summary> 
        public override void Exit(BaseAgent agent){}
    }
}
