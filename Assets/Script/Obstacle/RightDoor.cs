using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightDoor : MonoBehaviour
{
    private Player g_player;
    private bool g_is_opening_door = false;
    // Start is called before the first frame update
    void Start()
    {
        g_player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if(g_is_opening_door && transform.position.y <= 4.263){
            transform.position += new Vector3(0, 1*Time.fixedDeltaTime, 0);
        }
    }

    public void OpenDoor(){
        g_is_opening_door = true;
    }
}
