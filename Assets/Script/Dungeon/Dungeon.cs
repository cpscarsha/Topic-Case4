using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dungeon : MonoBehaviour
{
    public LeftDoor g_left_door;
    public RightDoor g_right_door;
    public GameObject[] g_summon_mobs_scenes; // 長度決定最大波次
    public int level = 1; // 當前波次
    public bool g_stage_is_begin = false;
    public bool g_stage_is_end = false;
    private Player g_player;
    // Start is called before the first frame update
    void Start()
    {
        g_player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!g_stage_is_begin && g_left_door.IsClose()){
            g_stage_is_begin = true;
            level = 1;
            g_summon_mobs_scenes[level-1].gameObject.SetActive(true);
        }
        if(g_stage_is_begin && MobCleared()){
            if(level >= g_summon_mobs_scenes.Length){
                g_stage_is_end = true;
                g_right_door.OpenDoor();
            }
            else{
                level += 1;
                g_summon_mobs_scenes[level-1].gameObject.SetActive(true);
            }
        }
    }

    private bool MobCleared(){
        GameObject[] mobs = GameObject.FindGameObjectsWithTag("Mob");
        return mobs.Length == 0;
    }
}
