using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyContent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int pos_y = 375;
        for(int i=0;i<transform.childCount;i++){
            transform.GetChild(i).localPosition = new Vector3(-300, pos_y, 0);
            pos_y -= 140;
        }
    }
}
