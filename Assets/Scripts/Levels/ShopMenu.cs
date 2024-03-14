using System.Collections.Generic;
using PlayerControl;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Levels{
    public class ShopMenu : MonoBehaviour {
        /// <summary>
        /// Buttons for the menu
        /// </summary>
        [Tooltip("Buttons for the menu")]
        [SerializeField]
        private List<Button> buttons;

        /// <summary>
        /// Costs of upgrades
        /// </summary>
        [Tooltip("Costs of upgrades")]
        [SerializeField]
        private List<int> upgradeCosts;

        /// <summary>
        /// Text objects storing costs
        /// </summary>
        [Tooltip("Text objects storing costs")]
        [SerializeField]
        private List<TextMeshProUGUI> costText;

        /// <summary>
        /// Text objects storing upgrades purchased
        /// </summary>
        [Tooltip("Text objects storing upgrades purchased")]
        [SerializeField]
        private List<TextMeshProUGUI> upgradeTimesText;

        /// <summary>
        /// Player
        /// </summary>
        [Tooltip("Player")]
        [SerializeField]
        private Player player;

        /// <summary>
        /// The amount of times an upgrade has been purchased
        /// </summary>
        public List<int> UpgradesPurchased = new List<int>(){0, 0, 0, 0};

        void Start(){
            if(gameObject.name.Equals("IT Upgrades")){

            }
            else if(gameObject.name.Equals("Personal Upgrades")){

            }
        }

        void Update(){
            int i = 0;
            foreach(Button button in buttons){
                button.interactable = !(player.GetCredits() < upgradeCosts[i]) && !(UpgradesPurchased[i] == 3);
                i++;
            }
            
            i = 0;
            foreach(TextMeshProUGUI textObj in costText){
                textObj.text = UpgradesPurchased[i] == 3 ? "MAX" : upgradeCosts[i].ToString();
                i++;
            }

            i = 0;
            foreach(TextMeshProUGUI textObj in upgradeTimesText){
                textObj.text = UpgradesPurchased[i] == 3 ? "MAX"  : UpgradesPurchased[i].ToString();
                i++;
            }
        }

        /// <summary>
        /// Process purchase; decrement player's credits and increase cost of next upgrade
        /// </summary>
        /// <param name="id">The id of the upgrade</param>
        public void ProcessPurchase(int id){
            player.SetCredits(player.GetCredits() - upgradeCosts[id]);
            upgradeCosts[id] += upgradeCosts[id];
            UpgradesPurchased[id]++;

            if(gameObject.name.Equals("PersonalUpgrades")){
                switch(id){
                    case 0:
                        player.basePlayerSpeed += 1.0f;
                        break;
                }
            }
        }
    }
}