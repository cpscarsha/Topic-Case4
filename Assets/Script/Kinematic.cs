using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor.Rendering;
using UnityEngine;
using Fusion;

public class Kinematic : NetworkBehaviour
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
    public override void FixedUpdateNetwork(){
        // try{
        //     if(!GetComponent<Player>().IsServer)return;
        // }
        // catch{}
        // if(CheckCollisionIn(Vector2.right, now_speed*Runner.DeltaTime+0.005f) && now_speed > 0){
        //     velocity = Vector2.zero;
        // }
        // else if(CheckCollisionIn(Vector2.left, -now_speed*Runner.DeltaTime+0.005f) && now_speed < 0){
        //     velocity = Vector2.zero;
        // }
        
        // if(!CheckCollisionIn(Vector2.down, gravity*Runner.DeltaTime+0.005f)){
        //     g_self_rigidbody.position = new Vector2(g_self_rigidbody.position.x, g_self_rigidbody.position.y-gravity*Runner.DeltaTime);
        // }
        // velocity.y -= gravity*Runner.DeltaTime;

        now_gravity -= gravity*Runner.DeltaTime;
        if(Mathf.Abs(knockback.x) > 1)knockback.x += (knockback.x>0?-1:1)*knockback_resistance*Runner.DeltaTime;
        else knockback.x = 0;
        if(Mathf.Abs(knockback.y) > 1)knockback.y += (knockback.y>0?-1:1)*knockback_resistance*Runner.DeltaTime;
        else knockback.y = 0;
        
        if((velocity.y + knockback.y) + now_gravity < 0 && CheckCollisionIn(Vector2.down, -(velocity.y + knockback.y+ now_gravity )*Runner.DeltaTime + 0.005f)){
            velocity.y = 0;
            knockback.y = 0;
            now_gravity = 0;
        }
        if((velocity.y + knockback.y) + now_gravity > 0 && CheckCollisionIn(Vector2.up, (velocity.y + knockback.y+ now_gravity )*Runner.DeltaTime + 0.005f)){
            velocity.y = 0;
            knockback.y = 0;
            now_gravity = 0;
        }
        // if((velocity.y + knockback.y - gravity)*Runner.DeltaTime + g_self_rigidbody.position.y + GetComponent<BoxCollider2D>().size.y/2.0f + g_self_collider.offset.y <= ground){
        //     velocity.y = 0;
        //     knockback.y = 0;
        //     now_gravity = 0;
        // }

        if((velocity.x + knockback.x > 0 && CheckCollisionIn(Vector2.right, (velocity.x + knockback.x)*Runner.DeltaTime+0.005f)) ^ (velocity.x + knockback.x < 0 && CheckCollisionIn(Vector2.left, -(velocity.x + knockback.x)*Runner.DeltaTime+0.005f))){
            velocity.x = 0;
            knockback.x = 0;
        }

        // if(g_self_rigidbody.position.y + velocity.y + knockback.y - g_self_collider.GetComponent<BoxCollider2D>().size.y/2 < ground){
        //     velocity.y = 0;
        // }

        

        GetComponent<NetworkTransform>.Teleport = new Vector2(g_self_rigidbody.position.x + (velocity.x + knockback.x)*Runner.DeltaTime, g_self_rigidbody.position.y + (velocity.y + knockback.y + now_gravity)*Runner.DeltaTime);
        // if(g_self_rigidbody.position.y <= -10 && GetComponent<MobBase>()){
        //     Destroy(GetComponent<MobBase>().gameObject);
        // }
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
