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

    void FixedUpdate(){
        // if(CheckCollisionIn(Vector2.right, now_speed*Time.fixedDeltaTime+0.005f) && now_speed > 0){
        //     velocity = Vector2.zero;
        // }
        // else if(CheckCollisionIn(Vector2.left, -now_speed*Time.fixedDeltaTime+0.005f) && now_speed < 0){
        //     velocity = Vector2.zero;
        // }
        
        // if(!CheckCollisionIn(Vector2.down, gravity*Time.fixedDeltaTime+0.005f)){
        //     g_self_rigidbody.position = new Vector2(g_self_rigidbody.position.x, g_self_rigidbody.position.y-gravity*Time.fixedDeltaTime);
        // }
        velocity.y -= gravity*Time.fixedDeltaTime;
        
        if(Mathf.Abs(knockback.x) > 1)knockback.x += (knockback.x>0?-1:1)*knockback_resistance*Time.fixedDeltaTime;
        else knockback.x = 0;
        if(Mathf.Abs(knockback.y) > 1)knockback.y += (knockback.y>0?-1:1)*knockback_resistance*Time.fixedDeltaTime;
        else knockback.y = 0;
        
        if((velocity.y + knockback.y < 0 && CheckCollisionIn(Vector2.down, -(velocity.y + knockback.y)*Time.fixedDeltaTime+0.005f)) || (velocity.y + knockback.y > 0 && CheckCollisionIn(Vector2.up, (velocity.y + knockback.y)*Time.fixedDeltaTime+0.005f))){
            velocity.y = 0;
            knockback.y = 0;
        }
        if((velocity.y + knockback.y)*Time.fixedDeltaTime + g_self_rigidbody.position.y + GetComponent<BoxCollider2D>().size.y/2.0f + g_self_collider.offset.y <= ground){
            velocity.y = 0;
            knockback.y = 0;
        }

        if((velocity.x + knockback.x > 0 && CheckCollisionIn(Vector2.right, (velocity.x + knockback.x)*Time.fixedDeltaTime+0.005f)) || (velocity.x + knockback.x < 0 && CheckCollisionIn(Vector2.left, -(velocity.x + knockback.x)*Time.fixedDeltaTime+0.005f))){
            velocity.x = 0;
            knockback.x = 0;
        }

        // if(g_self_rigidbody.position.y + velocity.y + knockback.y - g_self_collider.GetComponent<BoxCollider2D>().size.y/2 < ground){
        //     velocity.y = 0;
        // }


        g_self_rigidbody.position = new Vector2(g_self_rigidbody.position.x + (velocity.x + knockback.x)*Time.fixedDeltaTime, g_self_rigidbody.position.y + (velocity.y + knockback.y)*Time.fixedDeltaTime);
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
    
    public bool CheckCollisionIn(Vector2 direction, float distance){
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
    public bool HasCollision(float distance){
        if(CheckCollisionIn(Vector2.up, distance))return true;
        if(CheckCollisionIn(Vector2.down, distance))return true;
        if(CheckCollisionIn(Vector2.right, distance))return true;
        if(CheckCollisionIn(Vector2.left, distance))return true;
        return false;
    }
    public bool IsStuck(){ // 被卡在牆裡
        Collider2D[] result = new Collider2D[2];
        ContactFilter2D filter = new();
        filter.NoFilter();
        try{
            return g_self_collider.OverlapCollider(filter, result) > 0;
        }
        catch{
            return true;
        }
    }
}
