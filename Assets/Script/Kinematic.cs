using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kinematic : MonoBehaviour
{
    public Vector2 velocity;
    private Collider2D g_self_collider;
    public RaycastHit2D[] g_collision_result;
    private Rigidbody2D g_self_rigidbody;
    void Start(){
        g_self_collider = GetComponent<Collider2D>();
        g_self_rigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate(){
        float now_speed = velocity.x;
        if(CheckCollisionIn(Vector2.right, now_speed*Time.fixedDeltaTime+0.05f) && now_speed > 0){
            velocity = Vector2.zero;
            Debug.Log("RIGHT");
        }
        else if(CheckCollisionIn(Vector2.left, -now_speed*Time.fixedDeltaTime+0.05f) && now_speed < 0){
            velocity = Vector2.zero;
            Debug.Log("LEFT");
        }
        g_self_rigidbody.position = new Vector2(g_self_rigidbody.position.x + velocity.x*Time.fixedDeltaTime, g_self_rigidbody.position.y);
    }
    
    public bool CheckCollisionIn(Vector2 direction, float distance){
        g_collision_result = new RaycastHit2D[2];
        return g_self_collider.Cast(direction, g_collision_result, distance) > 0;
    }
}
