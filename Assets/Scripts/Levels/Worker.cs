using System.Collections.Generic;
using UnityEngine;

namespace Levels{
    public class Worker : MonoBehaviour {
        /// <summary>
        /// Shirt colors
        /// </summary>
        [Header("Aesthetics")]
        [Tooltip("Shirt colors")]
        public List<Material> ShirtColors;

        /// <summary>
        /// Skin tones
        /// </summary>
        [Tooltip("Skin tones")]
        public List<Material> SkinTones;

        /// <summary>
        /// Hair colors
        /// </summary>
        [Tooltip("Skin tones")]
        public List<Material> HairColors;

        /// <summary>
        /// Gameobject list representing worker's hair
        /// </summary>
        [Tooltip("Gameobject list representing worker's hair")]
        public List<GameObject> WorkerHair;

        /// <summary>
        /// Gameobject representing worker's shirt
        /// </summary>
        [Tooltip("Gameobject representing worker's shirt")]
        public GameObject Shirt;

        /// <summary>
        /// Gameobject representing worker's head
        /// </summary>
        [Tooltip("Gameobject representing worker's head")]
        public GameObject Head;

        /// <summary>
        /// Is this worker on task?
        /// </summary>
        [Header("Gameplay")]
        [Tooltip("Is this worker on task?")]
        public bool OnTask = true;

        void Start(){
            // 0 will get male hair style, 1 will get female hairstyle, 2 will include all styles for other identification
            int selectedGender = Random.Range(0, 3);
            List<string> names = new List<string>(){
                "Michael", "Ryan", "Kevin", "Creed", "Stanley", "Pete", "Andy", "Clark", "Darryl", "Ben", "Peter", "John",
                "Angela", "Pam", "Phyllis", "Meredith", "Dakota", "Kathy", "Kelly", "Erin", "Brianna", "Teena"
            };

            int selectedName = selectedGender == 0 ? Random.Range(0,12) : selectedGender == 1 ? Random.Range(12, names.Count) : Random.Range(0, names.Count);
            gameObject.name = names[selectedName];

            int selectedShirt = Random.Range(0, ShirtColors.Count);
            Shirt.GetComponent<Renderer>().material = ShirtColors[selectedShirt];

            int selectedSkin = Random.Range(0, SkinTones.Count);
            Head.GetComponent<Renderer>().material = SkinTones[selectedSkin];

            int selectedHair = Random.Range(0, HairColors.Count);
            foreach(GameObject hair in WorkerHair){
                hair.GetComponent<Renderer>().material = HairColors[selectedHair];
            }
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