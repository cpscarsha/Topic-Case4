using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode.Components;
using Unity.Netcode;

public class Ball : NetworkBehaviour
{
    public Kinematic g_kinematic;
    int g_stop_time = 0;
    private Vector3 g_last_position;
    // Start is called before the first frame update
    void Start()
    {
        g_kinematic = GetComponent<Kinematic>();
    }

    // Update is called once per frame
    void Update()
    {
        // if(!IsServer)return;
        if(g_last_position == transform.position)g_stop_time+=1;
        else g_stop_time = 0;
        if(g_stop_time > 100){
            Reset();
            g_stop_time = 0;
        }
        
        if(g_kinematic.CheckCollisionIn(Vector2.right, 0.2f)){
            foreach(RaycastHit2D i in g_kinematic.g_collision_result){
                try{
                    if(i.collider.CompareTag("Obstacle")){
                        g_kinematic.velocity = new Vector2(-Mathf.Abs(g_kinematic.velocity.x), g_kinematic.velocity.y); 
                        GetComponent<ObjectSync>().ResetTickDifferent();
                    }
                }
                catch{}
            }
        }
        if(g_kinematic.CheckCollisionIn(Vector2.left, 0.1f)){
            foreach(RaycastHit2D i in g_kinematic.g_collision_result){
                try{
                    if(i.collider.CompareTag("Obstacle")){
                        g_kinematic.velocity = new Vector2(Mathf.Abs(g_kinematic.velocity.x), g_kinematic.velocity.y);
                        GetComponent<ObjectSync>().ResetTickDifferent();
                    }
                }
                catch{}
            }
        }
        if(g_kinematic.CheckCollisionIn(Vector2.up, 0.1f)){
            foreach(RaycastHit2D i in g_kinematic.g_collision_result){
                try{
                    if(i.collider.CompareTag("Obstacle")){
                        g_kinematic.velocity = new Vector2(g_kinematic.velocity.x, -Mathf.Abs(g_kinematic.velocity.y)); 
                        g_kinematic.now_gravity = -g_kinematic.now_gravity;
                        GetComponent<ObjectSync>().ResetTickDifferent();
                    }
                }
                catch{}
            }
        }
        if(g_kinematic.CheckCollisionIn(Vector2.down, 0.1f)){
            foreach(RaycastHit2D i in g_kinematic.g_collision_result){
                try{
                    if(i.collider.CompareTag("Obstacle")){
                        g_kinematic.velocity = new Vector2(g_kinematic.velocity.x, Mathf.Abs(g_kinematic.velocity.y)); 
                        g_kinematic.now_gravity = -g_kinematic.now_gravity;
                        GetComponent<ObjectSync>().ResetTickDifferent();
                    }
                }
                catch{}
            }
        }
        if(g_kinematic.HasCollision(0.05f)){
            foreach(RaycastHit2D i in g_kinematic.g_collision_result){
                try{
                    if(i.collider.CompareTag("Player")){
                        Hit(Mathf.Atan2(transform.position.y - i.collider.transform.position.y, transform.position.x - i.collider.transform.position.x), 2.4f);
                        GetComponent<ObjectSync>().ResetTickDifferent();
                    }
                    else if(i.collider.CompareTag("Ground")){
                        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                        if(transform.position.x > 0){
                            if(players[0].GetComponent<Player>().g_is_host)transform.position = players[0].transform.position + new Vector3(0, 1, 0);
                            else transform.position = players[1].transform.position + new Vector3(0, 1, 0);
                            GetComponent<ObjectSync>().ResetTickDifferent();
                        }
                        else{
                            if(players[1].GetComponent<Player>().g_is_host)transform.position = players[0].transform.position + new Vector3(0, 1, 0);
                            else transform.position = players[1].transform.position + new Vector3(0, 1, 0);
                            GetComponent<ObjectSync>().ResetTickDifferent();
                        }
                        g_kinematic.velocity = Vector2.zero;
                    }
                }
                catch{}
            }
        }
        g_last_position = transform.position;
    }
    public void Reset(){
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if(transform.position.x > 0 || players.Length == 1){
            if(players[0].GetComponent<Player>().g_is_host)transform.position = players[0].transform.position + new Vector3(0, 1, 0);
            else transform.position = players[1].transform.position + new Vector3(0, 1, 0);
            GetComponent<ObjectSync>().ResetTickDifferent();
        }
        else{
            if(players[1].GetComponent<Player>().g_is_host)transform.position = players[0].transform.position + new Vector3(0, 1, 0);
            else transform.position = players[1].transform.position + new Vector3(0, 1, 0);
            GetComponent<ObjectSync>().ResetTickDifferent();
        }
        g_kinematic.velocity = Vector2.zero; 
    }
    public void Hit(float direct, float force){
        g_kinematic.velocity = new Vector2(force*Mathf.Cos(direct), force*Mathf.Sin(direct));
        g_kinematic.ResetGravity();
    }
}
