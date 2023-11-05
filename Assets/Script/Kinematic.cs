using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class Kinematic : MonoBehaviour
{
    public Vector2 velocity;
    public Vector2 knockback;
    public float knockback_resistance = 0;
    public string[] avoid_collision_tags;
    public RaycastHit2D[] g_collision_result;
    private Collider2D g_self_collider;
    private Rigidbody2D g_self_rigidbody;
    void Start(){
        g_self_collider = GetComponent<Collider2D>();
        g_self_rigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate(){
        float now_speed = velocity.x;
        if(CheckCollisionIn(Vector2.right, now_speed*Time.fixedDeltaTime+0.005f) && now_speed > 0){
            velocity = Vector2.zero;
        }
        else if(CheckCollisionIn(Vector2.left, -now_speed*Time.fixedDeltaTime+0.005f) && now_speed < 0){
            velocity = Vector2.zero;
        }
        g_self_rigidbody.position = new Vector2(g_self_rigidbody.position.x + (velocity.x + knockback.x)*Time.fixedDeltaTime, g_self_rigidbody.position.y + (velocity.y + knockback.y)*Time.fixedDeltaTime);
        if(Mathf.Abs(knockback.x) > 1)knockback.x += (knockback.x>0?-1:1)*knockback_resistance*Time.fixedDeltaTime;
        else knockback.x = 0;
        if(Mathf.Abs(knockback.y) > 1)knockback.y += (knockback.y>0?-1:1)*knockback_resistance*Time.fixedDeltaTime;
        else knockback.y = 0;
    }
    
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
        return collision_num != 0;
    }
}
