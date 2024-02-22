using UnityEngine;

public class CameraMenu : MonoBehaviour {
    private Animation anim;
    bool hasPlayed = false;

    void Start(){
        anim = GetComponent<Animation>();
    }

    void Update(){
        if(!hasPlayed){
            hasPlayed = true;
            //anim.Play("camera_to_modes");
        }
    }
}