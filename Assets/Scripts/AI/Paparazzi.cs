using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AI{
    public class Paparazzi : BaseAgent
    {
        /// <summary>
        /// Min speed to be considered stopped
        /// </summary>
        [Header("AI")]
        public float minStopSpeed;

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
        /// Gender of the worker
        /// </summary>
        [Tooltip("Gender of the worker")]
        public Gender WorkerGender;

        protected override void Start(){
            base.Start();

            // 0 will get male hair style, 1 will get female hairstyle, 2 will include all styles for other identification
            WorkerGender = (Gender)(Random.Range(0,3));

            List<string> names = new List<string>(){
                "Kevin", "Kenny", "Kyle", "Chad", "Ross", "Gordon",
                "Karen", "Gale", "Courteney", "Rachel", "Donna"
            };

            int selectedName = WorkerGender == Gender.Male ? Random.Range(0,12) : WorkerGender == Gender.Female ? Random.Range(12, names.Count) : Random.Range(0, names.Count);
            gameObject.name = names[selectedName] + " (Paparazzi)";

            int selectedShirt = Random.Range(0, ShirtColors.Count);
            Shirt.GetComponent<Renderer>().material = ShirtColors[selectedShirt];

            int selectedSkin = Random.Range(0, SkinTones.Count);
            Head.GetComponent<Renderer>().material = SkinTones[selectedSkin];

            if(WorkerGender == Gender.Female){
                SetHairLength(false);
            }
            else if(WorkerGender == Gender.Other){
                SetHairLength(true);
            }

            if(WorkerGender != Gender.Female){
                int random = Random.Range(0, 100);
                if(random >= 49){
                    foreach(GameObject hair in WorkerHair){
                        hair.SetActive(false);
                    }
                }
            }

            int selectedHair = Random.Range(0, HairColors.Count);
            foreach(GameObject hair in WorkerHair){
                hair.GetComponent<Renderer>().material = HairColors[selectedHair];
            }
        }

                /// <summary>
        /// Set the worker's hair length
        /// </summary>
        /// <param name="randomize">Are all hairstyles available?</param>
        void SetHairLength(bool randomize){
            for(int i = 1; i < WorkerHair.Count && WorkerGender == Gender.Female; i++){
                if(i < WorkerHair.Count - 2){
                    WorkerHair[i].transform.localPosition = new Vector3(WorkerHair[i].transform.localPosition.x, 2.94f, WorkerHair[i].transform.localPosition.z);
                    WorkerHair[i].transform.localScale = new Vector3(1f, WorkerHair[i].transform.localScale.y, WorkerHair[i].transform.localScale.z);
                }
                else{
                    WorkerHair[i].SetActive(true);
                    WorkerHair[i].SetActive(true);
                }
            }

            if(randomize){
                int hairLength = Random.Range(0,2);
                for(int i = 1; i < WorkerHair.Count && hairLength == 1; i++){
                    if(i < WorkerHair.Count - 2){
                        WorkerHair[i].transform.localPosition = new Vector3(WorkerHair[i].transform.localPosition.x, 2.94f, WorkerHair[i].transform.localPosition.z);
                        WorkerHair[i].transform.localScale = new Vector3(1f, WorkerHair[i].transform.localScale.y, WorkerHair[i].transform.localScale.z);
                    }
                    else{
                        WorkerHair[i].SetActive(true);
                        WorkerHair[i].SetActive(true);
                    }
                }
            }
        }
    }

    public enum Gender{
        Male = 0,
        Female = 1,
        Other = 2
    }
}

