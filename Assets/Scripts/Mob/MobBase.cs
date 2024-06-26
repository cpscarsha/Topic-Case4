using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobBase : MonoBehaviour
{
    protected PlayerSingle g_player;
    protected Rigidbody2D g_rigidbody;
    protected Kinematic g_self_kinematic;
    private ParticleSystem g_death_particle;
    private bool g_death;
    public float g_max_health;
    public float g_health;
    protected void VariableInit(float max_health = 1.0f, float health = 1.0f){
        g_player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSingle>();
        g_rigidbody = GetComponent<Rigidbody2D>();
        g_self_kinematic = GetComponent<Kinematic>();
        g_max_health = max_health;
        g_health = health;
        g_death = false;
        g_death_particle = GetComponent<ParticleSystem>();
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
        if(!g_death && g_self_kinematic.CheckCollisionIn(GetDirect()?Vector2.right:Vector2.left, distance)){
            foreach(RaycastHit2D i in g_self_kinematic.g_collision_result){
                try{
                    if(i.collider.tag == "Player"){
                        i.collider.GetComponent<Kinematic>().knockback = new Vector2((GetDirect()?1:-1)*knockback, 0);
                        i.collider.GetComponent<PlayerSingle>().g_health -= damage;
                        if(i.collider.GetComponent<PlayerSingle>().g_health <= 0.001f){
                            i.collider.GetComponent<PlayerSingle>().Death();
                        }
                        result = true;
                    }
                }
                catch{}
            }
        }
        return result;
    }
    public void BeHit(bool attacker_is_right, float knockback, float damage){
        g_health -= damage;
        if(g_health <= 0.001f){
            Death();
        }
        g_self_kinematic.knockback = new Vector2((attacker_is_right?1:-1)*knockback, 0);
    }
    public void Death(){
        g_death_particle.Play();
        GetComponent<SpriteRenderer>().enabled = false;
        Destroy(g_self_kinematic);
        g_death = true;
        Destroy(gameObject, 1f); 
    }
    public bool IsDeath(){
        return g_death;
    }

    public Vector4 GetSummonRange(){ // 回傳(左下角x, 左下角y, x長度, y長度)
        Transform summon_range = transform.Find("SummonRange").transform;
        Vector2 summon_base_pos = summon_range.position;
        float width = summon_range.localScale.x/0.25f;
        float height = summon_range.localScale.y/0.25f;
        return new Vector4(summon_base_pos.x-0.25f*width/2, summon_base_pos.y-0.25f*height/2, width, height);
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
