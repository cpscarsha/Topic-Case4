using UnityEngine;
using Unity.Netcode;

public class GameManager : MonoBehaviour
{
    public static bool g_is_connect = false;
    private void Update(){
        if (Input.GetKeyDown(KeyCode.O)){
            SlideUp();
        }
        if (Input.GetKeyDown(KeyCode.P)){
            SlideDown();
        }
        CheckSlide();
    }
    private void SlideUp(){
        if(!g_is_connect){
            NetworkManager.Singleton.StartHost();
        }
    }
    private void SlideDown(){
        NetworkManager.Singleton.StartClient();
    }

    private Vector3 touch_base_position = Vector3.zero;
    private float touch_begin_time = 0;
    private float touch_duration = 0;
    private void CheckSlide(){
        if(Input.touches.Length > 0){
            Touch touch = Input.touches[0];
            touch_duration = Time.time-touch_begin_time;
            Vector2 end_position = touch.position;
            float theata = Mathf.Atan2(end_position.y-touch_base_position.y, end_position.x-touch_base_position.x);
            if(touch.phase == TouchPhase.Began){
                touch_base_position = touch.position;
                touch_begin_time = Time.time;
                touch_duration = 0;
            }
            else if(touch.phase == TouchPhase.Ended){
                if(touch_duration < 0.15f){
                    if(theata <= Mathf.PI*3/4 && theata >= Mathf.PI/4){
                        SlideUp();
                    }
                    else if(theata >= -Mathf.PI*3/4 && theata <= -Mathf.PI/4){
                        SlideDown();
                    }
                }
            }
        }     
    }
}

