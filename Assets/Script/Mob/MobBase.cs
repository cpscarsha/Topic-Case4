using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobBase : MonoBehaviour
{
    protected Player g_player;
    protected Rigidbody2D g_rigidbody;
    protected Kinematic g_self_kinematic;
    public float g_max_health;
    public float g_health;
    protected void VariableInit(float max_health = 1.0f, float health = 1.0f){
        g_player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        g_rigidbody = GetComponent<Rigidbody2D>();
        g_self_kinematic = GetComponent<Kinematic>();
        g_max_health = max_health;
        g_health = health;
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
    protected bool AttackPlayer(float distance, float knockback, float damage){ // 攻擊距離為distance以內的玩家，擊退初速度為knockback，若成功攻擊則回傳true
        bool result= false;
        if(g_self_kinematic.CheckCollisionIn(GetDirect()?Vector2.right:Vector2.left, distance)){
            foreach(RaycastHit2D i in g_self_kinematic.g_collision_result){
                try{
                    if(i.collider.tag == "Player"){
                        i.collider.GetComponent<Kinematic>().knockback = new Vector2((GetDirect()?1:-1)*knockback, 0);
                        i.collider.GetComponent<Player>().g_health -= damage;
                        result = true;
                    }
                }
                catch{}
            }
        }
        return result;
    }
    public void BeHit(bool attacker_is_right, float knockback, float damage){
        g_self_kinematic.knockback = new Vector2((attacker_is_right?1:-1)*knockback, 0);
        g_health -= damage;
    }
    // public float GetHealth(){
    //     return g_health;
    // }
    // public void SetHealth(float health){
    //     g_health = health;
    // }
    public float GetHealthPercentage(){
        return g_health/g_max_health;
    }
}
