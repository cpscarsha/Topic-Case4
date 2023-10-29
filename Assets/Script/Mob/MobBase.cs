using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobBase : MonoBehaviour
{
    protected Player g_player;
    protected Rigidbody2D g_rigidbody;
    protected Kinematic g_self_kinematic;
    protected void VariableInit(){
        g_player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        g_rigidbody = GetComponent<Rigidbody2D>();
        g_self_kinematic = GetComponent<Kinematic>();
    }
    protected void SetDirect(bool is_right){ // 設定怪物朝向，當 is_right 時朝右，否則朝左
        Vector3 change_scale = transform.localScale;
        if(is_right && change_scale.x > 0){
            change_scale.x = -change_scale.x;
        }
        else if(!is_right && change_scale.x < 0){
            change_scale.x = -change_scale.x;
        }
        transform.localScale = change_scale;
    }
    protected bool GetDirect(){ // 取得朝向，true時向右
        return transform.localScale.x < 0;
    }
}
