using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode.Components;
using Unity.Netcode;

public class Ball : NetworkBehaviour
{
    public Kinematic g_kinematic;
    // Start is called before the first frame update
    void Start()
    {
        g_kinematic = GetComponent<Kinematic>();
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("afadf"+GetComponent<Kinematic>());
        if(g_kinematic.CheckCollisionIn(Vector2.down, 0.01f)){
            g_kinematic.velocity.x *= 0.2f;
        }
        if(g_kinematic.HasCollision(0.05f)){
            foreach(RaycastHit2D i in g_kinematic.g_collision_result){
                try{
                    if(i.collider.CompareTag("Player")){
                        // Hit(Mathf.PI/4 + (i.collider.transform.position.x<0?0:Mathf.PI/2), 1.6f);
                        Hit(Mathf.Atan2(transform.position.y - i.collider.transform.position.y, transform.position.x - i.collider.transform.position.x), 1.6f);
                    }
                    if(i.collider.CompareTag("Ground")){
                        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                        if(transform.position.x > 0){
                            if(players[0].GetComponent<Player>().g_is_host)transform.position = players[0].transform.position + new Vector3(0, 1, 0);
                            else transform.position = players[1].transform.position + new Vector3(0, 1, 0);
                        }
                        else{
                            if(players[1].GetComponent<Player>().g_is_host)transform.position = players[0].transform.position + new Vector3(0, 1, 0);
                            else transform.position = players[1].transform.position + new Vector3(0, 1, 0);
                        }
                        g_kinematic.velocity = Vector2.zero;
                    }
                }
                catch{}
            }
        }
    }

    public void Hit(float direct, float force){
        g_kinematic.velocity = new Vector2(force*Mathf.Cos(direct), force*Mathf.Sin(direct));
    }
}
