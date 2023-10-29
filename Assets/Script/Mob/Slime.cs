using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MobBase
{
    public bool a_is_jump;
    private bool g_is_motion = false;
    // Start is called before the first frame update
    void Start()
    {
        VariableInit();
    }

    // Update is called once per frame
    void Update()
    {
        g_rigidbody.velocity = new Vector2(0, 0);
        bool is_right = g_player.transform.position.x > transform.position.x;
        SetDirect(is_right);
        if(a_is_jump){
            g_rigidbody.velocity = new Vector2((is_right?1:-1)*0.6f, 0);
        }
    }

}
