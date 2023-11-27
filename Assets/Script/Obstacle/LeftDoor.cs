using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftDoor : MonoBehaviour
{
    private Kinematic g_self_kinematic;
    private Player g_player;
    private bool g_is_close = false;
    // Start is called before the first frame update
    void Start()
    {
        g_self_kinematic = GetComponent<Kinematic>();
        g_self_kinematic.gravity = 0;
        g_player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if(g_is_close && g_self_kinematic.CheckCollisionIn(Vector2.down, 0.1f)){
            g_self_kinematic.gravity = 0;
        }
        else if(!g_is_close && g_player.transform.position.x - 0.3f > transform.position.x){
            g_self_kinematic.gravity = 20;
            g_self_kinematic.velocity.y = -0.01f;
            g_is_close = true;
        }
        
    }
    public bool IsClose(){
        return g_is_close;
    }
}
