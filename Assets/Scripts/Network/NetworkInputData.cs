using UnityEngine;

public struct NetworkInputData
{
    public TouchPhase left_phase;
    public Vector3 left_position;
    public TouchPhase right_phase;
    public Vector3 right_position;
    public bool has_right;
    public bool has_left;

    public NetworkInputData(Touch[] touches){
        has_right = false;
        has_left = false;
        left_phase = new();
        left_position = new();
        right_phase = new();
        right_position = new();
        if(touches.Length > 0){
            foreach(Touch touch in touches){
                if(has_right && has_left)break;
                if(touch.position.x > Camera.main.pixelWidth/2){
                    if(has_right)continue;
                    has_right = true;
                    right_phase = touch.phase;
                    right_position = touch.position;
                }
                else{
                    if(has_left)continue;
                    has_left = true;
                    left_phase = touch.phase;
                    left_position = touch.position;
                }
            }
        }
    }
}