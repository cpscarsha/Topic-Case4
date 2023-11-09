using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MobBase
{
    public bool a_is_jump;
    public float g_attack_cooldown = 0.2f;
    private float g_attack_cooldown_remaining = 0;
    // Start is called before the first frame update
    void Start()
    {
        VariableInit();
    }

    // Update is called once per frame
    void Update()
    {
        bool is_right = g_player.transform.position.x > transform.position.x;
        SetDirect(is_right);
        if(a_is_jump){
            g_self_kinematic.velocity.x = (is_right?1:-1)*0.6f;
        }
        else{
            g_self_kinematic.velocity.x = 0;
        }

        if(IsCooldownFinish()){
            if(AttackPlayer(0.05f, 6, 10)){
                StartCooldown();
            }
        }
    }

    private void StartCooldown(){
        g_attack_cooldown_remaining = Time.time + g_attack_cooldown;
    }
    private bool IsCooldownFinish(){
        return g_attack_cooldown_remaining < Time.time;
    }

}
