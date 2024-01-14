using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor.Rendering;
using UnityEngine;

public class Kinematic : MonoBehaviour
{
    public Vector2 velocity;
    public Vector2 knockback;
    public float gravity = 1f;
    public float knockback_resistance = 0;
    public float ground = 0f;
    public string[] avoid_collision_tags;
    public RaycastHit2D[] g_collision_result;
    private Collider2D g_self_collider;
    private Rigidbody2D g_self_rigidbody;
    void Start(){
        g_self_collider = GetComponent<Collider2D>();
        g_self_rigidbody = GetComponent<Rigidbody2D>();
    }
    public float now_gravity = 0;
    void FixedUpdate(){
        // try{
        //     if(!GetComponent<Player>().IsServer)return;
        // }
        // catch{}
        // if(CheckCollisionIn(Vector2.right, now_speed*Time.fixedDeltaTime+0.005f) && now_speed > 0){
        //     velocity = Vector2.zero;
        // }
        // else if(CheckCollisionIn(Vector2.left, -now_speed*Time.fixedDeltaTime+0.005f) && now_speed < 0){
        //     velocity = Vector2.zero;
        // }
        
        // if(!CheckCollisionIn(Vector2.down, gravity*Time.fixedDeltaTime+0.005f)){
        //     g_self_rigidbody.position = new Vector2(g_self_rigidbody.position.x, g_self_rigidbody.position.y-gravity*Time.fixedDeltaTime);
        // }
        // velocity.y -= gravity*Time.fixedDeltaTime;

        now_gravity -= gravity*Time.fixedDeltaTime; // 計算重力在這一幀造成的速度

        // 處理擊退衰減
        if(Mathf.Abs(knockback.x) > 1)knockback.x += (knockback.x>0?-1:1)*knockback_resistance*Time.fixedDeltaTime;
        else knockback.x = 0;
        if(Mathf.Abs(knockback.y) > 1)knockback.y += (knockback.y>0?-1:1)*knockback_resistance*Time.fixedDeltaTime;
        else knockback.y = 0;
        
        // 預測移動距離內是否有碰撞箱
        if(velocity.y + knockback.y + now_gravity < 0 && CheckCollisionIn(Vector2.down, -(velocity.y + knockback.y+ now_gravity )*Time.fixedDeltaTime + 0.005f)){
            velocity.y = 0;
            knockback.y = 0;
            now_gravity = 0;
        }
        if(velocity.y + knockback.y + now_gravity > 0 && CheckCollisionIn(Vector2.up, (velocity.y + knockback.y+ now_gravity )*Time.fixedDeltaTime + 0.005f)){
            velocity.y = 0;
            knockback.y = 0;
            now_gravity = 0;
        }
        if((velocity.x + knockback.x > 0 && CheckCollisionIn(Vector2.right, (velocity.x + knockback.x)*Time.fixedDeltaTime+0.005f)) ^ (velocity.x + knockback.x < 0 && CheckCollisionIn(Vector2.left, -(velocity.x + knockback.x)*Time.fixedDeltaTime+0.005f))){
            velocity.x = 0;
            knockback.x = 0;
        }

        // 位置根據速度進行改變
        g_self_rigidbody.position = new Vector2(g_self_rigidbody.position.x + (velocity.x + knockback.x)*Time.fixedDeltaTime, g_self_rigidbody.position.y + (velocity.y + knockback.y + now_gravity)*Time.fixedDeltaTime);
 
    }
    // public void GetForce(Vector2 force){
    //     velocity += force;
    // }
    // public void GetXForce(float force){
    //     velocity.x += force;
    // }
    // public void GetYForce(float force){
    //     velocity.y += force;
    // }
    public void ResetGravity(){
        now_gravity = 0;
    }
    public bool CheckCollisionIn(Vector2 direction, float distance){
        try{
        g_collision_result = new RaycastHit2D[5];
        int collision_num = g_self_collider.Cast(direction, g_collision_result, distance);
        RaycastHit2D[] temp = g_collision_result;
        foreach(string tag in avoid_collision_tags){
            for(int i=0;i<temp.Length;i++){
                if(!temp[i])break;
                if(temp[i].collider.tag.Equals(tag)){
                    collision_num -= 1;
                    temp[i] = new RaycastHit2D();
                }
            }
        }
        return collision_num > 0;
        }
        catch{return false;}
    }
    public bool HasCollision(float distance){
        RaycastHit2D[] collision_result = new RaycastHit2D[20];
        if(CheckCollisionIn(Vector2.up, distance))return true;
        for(int i=0;i<5;i++)collision_result[i] = g_collision_result[i];
        if(CheckCollisionIn(Vector2.down, distance))return true;
        for(int i=0;i<5;i++)collision_result[i+5] = g_collision_result[i];
        if(CheckCollisionIn(Vector2.right, distance))return true;
        for(int i=0;i<5;i++)collision_result[i+10] = g_collision_result[i];
        if(CheckCollisionIn(Vector2.left, distance))return true;
        for(int i=0;i<5;i++)collision_result[i+15] = g_collision_result[i];
        g_collision_result = collision_result;
        return false;
    }
    public bool IsStuck(){ // 被卡在牆裡
        return CheckCollisionIn(Vector2.down, 0.005f) && CheckCollisionIn(Vector2.up, 0.005f) && CheckCollisionIn(Vector2.right, 0.005f) && CheckCollisionIn(Vector2.left, 0.005f);
    }
}
