using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Realms;
using Realms.Sync;
using System.Linq;
using Database;

namespace Database{
    [DisallowMultipleComponent]
    public class RealmController : MonoBehaviour {
        static public RealmController Instance;

        private Realm _realm;
        private App _realmApp;
        private User _realmUser;

        [SerializeField] private string _realmAppId = "meetingroomsim-waabv";

        async void Awake(){
            DontDestroyOnLoad(gameObject);
            Instance = this;
            if(_realm == null){
                _realmApp = App.Create(new AppConfiguration(_realmAppId));
                // Still need to login
                if(_realmApp.CurrentUser == null){
                    _realmUser = await _realmApp.LogInAsync(Credentials.Anonymous());
                    _realm = await Realm.GetInstanceAsync(new FlexibleSyncConfiguration(_realmUser));
                    var query = _realm.All<GameDataModel>().Where(d => d.UserId == _realmUser.Id);
                    await query.SubscribeAsync();
                }
                // Already logged in
                else{
                    _realmUser = _realmApp.CurrentUser;
                    _realm = Realm.GetInstance(new FlexibleSyncConfiguration(_realmUser));
                    var query = _realm.All<GameDataModel>().Where(d => d.UserId == _realmUser.Id);
                    await query.SubscribeAsync();
                }
            }
        }

        void OnDisable(){
            if(_realm != null){
                _realm.Dispose();
            }
        }

        public bool IsRealmReady(){
            return _realm != null;
        }

        /// <summary>
        /// Get or create game data
        /// </summary>
        private GameDataModel GetOrCreateGameData(){
            var gameDataModel = _realm.All<GameDataModel>().Where(d => d.UserId == _realmUser.Id).FirstOrDefault();

            if(gameDataModel == null){
                _realm.Write(() => {
                    gameDataModel = _realm.Add(new GameDataModel(){
                        UserId = _realmUser.Id,
                        Score = 0,
                        X = 0,
                        Y = 0
                    });
                });
            }
            return gameDataModel;
        }

        public int GetScore(){
            GameDataModel gameDataModel = GetOrCreateGameData();
            return gameDataModel.Score;
        }

        public Vector2 GetPosition(){
            GameDataModel gameDataModel = GetOrCreateGameData();
            return new Vector2(gameDataModel.X, gameDataModel.Y);
        }

        public void IncreaseScore(int value){
            GameDataModel gameDataModel = GetOrCreateGameData();
            _realm.Write(() => {
                gameDataModel.Score += value;
            });
        }

        public void SetPosition(Vector2 position){
            GameDataModel gameDataModel = GetOrCreateGameData();
            _realm.Write(() => {
                gameDataModel.X = position.x;
                gameDataModel.Y = position.y;            
            });
        }
    }
}