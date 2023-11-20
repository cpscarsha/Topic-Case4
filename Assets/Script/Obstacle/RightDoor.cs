using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightDoor : MonoBehaviour
{
    private Player g_player;
    // Start is called before the first frame update
    void Start()
    {
        g_player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenDoor(){
        
    }
}
