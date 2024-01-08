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
    }

    public void Hit(float direct, float force){
        g_kinematic.velocity = new Vector2(force*Mathf.Cos(direct), force*Mathf.Sin(direct));
    }
}
