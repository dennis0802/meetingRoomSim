using UnityEngine;

namespace Levels{
    public class Worker : MonoBehaviour {
        /// <summary>
        /// Is this worker on task?
        /// </summary>
        public bool OnTask = true;

        void Start(){
            int selectedName = Random.Range(0,12);
            string charName = "";

            switch(selectedName){
                case 0:
                    charName = "Michael";
                    break;
                case 1:
                    charName = "Ryan";
                    break;
                case 2:
                    charName = "Kevin";
                    break;
                case 3:
                    charName = "Creed";
                    break;        
                case 4:
                    charName = "Stanley";
                    break;
                case 5:
                    charName = "Pete";
                    break;
                case 6:
                    charName = "Andy";
                    break;    
                case 7:
                    charName = "Roy";
                    break;
                case 8:
                    charName = "Darryl";
                    break;
                case 9:
                    charName = "Ben";
                    break;
                case 10:
                    charName = "Peter";
                    break;
                case 11:
                    charName = "Dwight";
                    break;
            }

            gameObject.name = charName;
        }

        void Update(){
            // If on task, face forward, otherwise rotate in place.
            if(!OnTask){
                transform.Rotate(0,60*Time.deltaTime,0);
            }
            else{
                Quaternion targetRotation = Quaternion.Euler(0, 0, 0);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
            }
        }
    }
}