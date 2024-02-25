using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;

    void Awake(){
        DontDestroyOnLoad(this.gameObject);
        if(Instance == null){
            Instance = this;
        }
        else{
            Destroy(gameObject);
        }
    }
}