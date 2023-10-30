using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MobBase
{
    public bool a_is_jump;
    public float g_attack_cooldown = 0.2f;
    private float g_self_attack_cooldown = 0;
    // Start is called before the first frame update
    void Start()
    {
        VariableInit();
    }

    // Update is called once per frame
    void Update()
    {
        g_self_kinematic.velocity = new Vector2(0, 0);
        bool is_right = g_player.transform.position.x > transform.position.x;
        SetDirect(is_right);
        if(a_is_jump){
            g_self_kinematic.velocity = new Vector2((is_right?1:-1)*0.6f, 0);
        }

        if(g_self_attack_cooldown < Time.time && g_self_kinematic.CheckCollisionIn(GetDirect()?Vector2.right:Vector2.left, 0.1f)){
            foreach(RaycastHit2D i in g_self_kinematic.g_collision_result){
                try{
                    if(i.collider.tag == "Player"){
                        i.collider.GetComponent<Kinematic>().knockback = new Vector2((GetDirect()?1:-1)*6, 0);
                        i.collider.GetComponent<Player>().g_health -= 10.0f;
                        g_self_attack_cooldown = Time.time + g_attack_cooldown;
                    }
                }
                catch{}
            }
        }
    }

}
