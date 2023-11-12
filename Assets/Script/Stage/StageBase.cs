using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class StageBase : MonoBehaviour
{
    public LeftDoor g_left_door;
    public RightDoor g_right_door;
    public GameObject[] g_summon_mobs;
    public Vector2 g_summon_distance;
    public bool g_stage_is_begin = false;
    protected Player g_player;
    private bool g_right_door_is_summon = false;
    protected void NecessaryLoop(){
        if(!g_stage_is_begin && transform.GetChild(0).GetComponent<LeftDoor>().IsClose()){
            g_stage_is_begin = true;
            foreach(GameObject mob in g_summon_mobs){
                Instantiate(mob, new Vector3(g_player.transform.position.x+Random.Range(g_summon_distance.x, g_summon_distance.y), -0.6f, 0), new Quaternion(0, 0, 0, 0));
            }
        }
    }
    protected bool StageIsEnd(){
        return g_right_door_is_summon && transform.GetChild(1).GetComponent<RightDoor>().transform.position.x+1.6f < g_player.transform.position.x;
    }
    // Start is called before the first frame update
    protected void VariableInit(float max_health = 1.0f, float health = 1.0f){
        g_player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
    protected void StartStage(){
        Instantiate(g_left_door, new Vector3(g_player.transform.position.x+3.2f, g_player.transform.position.y+2.4f, 0), new Quaternion(0, 0, 0, 0)).transform.parent = transform;
    }
    protected void FinishStage(){
        if(!g_right_door_is_summon){
            Instantiate(g_right_door, new Vector3(g_player.transform.position.x+3.2f, g_player.transform.position.y+2.4f, 0), new Quaternion(0, 0, 0, 0)).transform.parent = transform;
            g_right_door_is_summon = true;    
        }
    }
    protected bool MobCleared(){
        GameObject[] mobs = GameObject.FindGameObjectsWithTag("Mob");
        return mobs.Length == 0;
    }
    
}
