using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveJoystick : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject g_joy_limit;
    public GameObject g_joy_point;

    private float g_ctrl_theata = 0; // 搖桿角度[-PI, PI]
    private float g_ctrl_value = 0; // 搖桿與基點距離
    private float g_ctrl_max_distance = 120.0f; // 搖桿可控制的最大距離
    private bool g_ctrl_trigger = false; // 搖桿放開後變成True, 再次壓下變成False
    private bool g_is_enable = false; // 是否在使用搖桿
    public Vector3 g_base_position = new Vector3(0, 0, 0); // 搖桿基點
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.touches.Length > 0){
            Touch touch = Input.touches[0];
            if(touch.phase == TouchPhase.Began){
                // g_base_position = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane));
                g_base_position = touch.position;
                g_ctrl_trigger = false;
                g_is_enable = true;
            }
            else if(touch.phase == TouchPhase.Ended){
                g_ctrl_value = 0;
                g_ctrl_trigger = true;
                g_is_enable = false;
            }
            if(g_is_enable){
                //Vector3 now_position = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane));
                Vector3 now_position = touch.position;
                g_ctrl_theata = Mathf.Atan2(now_position.y - g_base_position.y,now_position.x - g_base_position.x);
                g_ctrl_value = Mathf.Sqrt((now_position.x - g_base_position.x)*(now_position.x - g_base_position.x)+(now_position.y - g_base_position.y)*(now_position.y - g_base_position.y));
                if(g_ctrl_value > g_ctrl_max_distance)g_ctrl_value = g_ctrl_max_distance;
                g_joy_point.GetComponent<RectTransform>().position = new Vector2(g_base_position.x+g_ctrl_value*Mathf.Cos(g_ctrl_theata),g_base_position.y+g_ctrl_value*Mathf.Sin(g_ctrl_theata));
            }
        }
        else{
            g_ctrl_value = 0;
            g_is_enable = false;
        }
        g_joy_limit.GetComponent<RectTransform>().position = g_base_position;
        g_joy_limit.SetActive(g_is_enable);
        g_joy_point.SetActive(g_is_enable);
    }

    public float GetTheata(){ // 取得搖桿角度[-PI, PI]
        return g_ctrl_theata;
    }
    public float GetValue(){ // 取得搖桿與基點距離
        return g_ctrl_value;
    }
    public bool GetTrigger(){ // 取得搖桿觸發(放開)
        if(g_ctrl_trigger){
            bool ans = g_ctrl_trigger;
            g_ctrl_trigger = false;
            return ans;
        }
        return g_ctrl_trigger;
    }
}
