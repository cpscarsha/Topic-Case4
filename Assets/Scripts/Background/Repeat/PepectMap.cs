using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PepectMap : MonoBehaviour
{
    public GameObject[] g_map;
    public float g_map_scale;
    public float g_map_width;
    public GameObject g_camera;
    private int g_middle_map;
    // Start is called before the first frame update
    void Start()
    {
        g_middle_map = g_map.Length/2;
    }

    // Update is called once per frame
    void Update()
    {
        if(g_camera.transform.position.x > g_map[g_middle_map].transform.position.x+g_map_width/2*g_map_scale){ // 玩家向右走，超過臨界值
            g_map[0].transform.Translate(new Vector3(g_map_scale*g_map_width*g_map.Length, 0, 0)); // 將最左邊的地圖移到最右邊
            // 改變地圖順序
            GameObject temp = g_map[0];
            for(int i=0;i<g_map.Length-1;i++){
                g_map[i] = g_map[i+1];
            }
            g_map[g_map.Length-1] = temp;
        }
        else if(g_camera.transform.position.x < g_map[g_middle_map].transform.position.x-g_map_width/2*g_map_scale){ // 玩家向左走，超過臨界值
            g_map[g_map.Length-1].transform.Translate(new Vector3(-g_map_scale*g_map_width*g_map.Length, 0, 0)); // 將最右邊的地圖移到最左邊
            // 改變地圖順序
            GameObject temp = g_map[g_map.Length-1];
            for(int i=g_map.Length-1;i>0;i--){
                g_map[i] = g_map[i-1];
            }
            g_map[0] = temp;
        }
    }
}
