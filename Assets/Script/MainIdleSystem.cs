using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MainIdleSystem : MonoBehaviour
{
    public bool g_game_start = false;
    public bool g_is_single_player = false;
    public LightCircle g_halo;
    public CinemachineVirtualCamera g_camera;
    public GameObject g_single_player_perfab;
    public GameObject g_single_player_dungeon_perfab;
    public GameObject g_gameover;
    private bool g_dungeon_is_init = false;
    private float g_delay_start = 1;
    // public CinemachineVirtualCamera g_virtual_camera;
    // Start is called before the first frame update
    void Start()
    {
        g_camera.m_Lens.OrthographicSize = 0.35f;
        g_delay_start = Time.time + 1;
    }

    // Update is called once per frame
    [System.Obsolete]
    void Update()
    {   
        Random.seed = (int)Time.time*1000000;
        Debug.Log("connect:"+GameManager.g_is_connect);
        Debug.Log("start:"+g_game_start);
        if(Time.time > g_delay_start && GameManager.g_is_connect && Input.touches.Length > 0 && Input.touches[0].phase == TouchPhase.Ended){
            g_game_start = true;
            // g_camera.Follow = GameObject.FindWithTag("Player").transform;
            // g_camera.m_Lens.OrthographicSize = 2.35f;
            // g_camera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = new Vector3(0, -0.63f, 0);
        }
        if(g_game_start){
            if(!g_dungeon_is_init && g_is_single_player){
                Instantiate(g_single_player_dungeon_perfab).transform.position = new Vector3(5, 0, 0);
                g_dungeon_is_init = true;
            }
            if(g_camera.m_Lens.OrthographicSize < 1.5f){
                g_camera.m_Lens.OrthographicSize += 1.5f*Time.deltaTime*(1*(0.1f+Mathf.Min(Mathf.Abs(g_camera.m_Lens.OrthographicSize - 0f),Mathf.Abs(g_camera.m_Lens.OrthographicSize - 2.35f)))/1.325f);
            }
            if(g_camera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y < 0.63){
                g_camera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y += 0.63f*Time.deltaTime*(1*(0.1f+Mathf.Min(Mathf.Abs(g_camera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y - 0f),Mathf.Abs(g_camera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y - 0.63f)))/0.315f);
            }
        }
    }

    public void StartSinglePlayer(){
        g_camera.Follow = Instantiate(g_single_player_perfab).transform;
        g_is_single_player = true;
    }
}
