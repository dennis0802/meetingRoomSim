using UnityEngine;

public class Portal : MonoBehaviour {
    void Update(){
        transform.Rotate(0,60*Time.deltaTime,0);
    }
}