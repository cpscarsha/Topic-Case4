using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MainIdleSystem : MonoBehaviour
{
    public bool g_game_start = false;
    public LightCircle g_halo;
    public CinemachineVirtualCamera g_camera;
    // public CinemachineVirtualCamera g_virtual_camera;
    // Start is called before the first frame update
    void Start()
    {
        g_camera.m_Lens.OrthographicSize = 0.35f;
        
    }

    // Update is called once per frame
    void Update()
    {   
        Debug.Log("connect:"+GameManager.g_is_connect);
        Random.seed = (int)Time.time*1000000;
        if(GameManager.g_is_connect && Input.touches.Length > 0 && Input.touches[0].phase == TouchPhase.Ended){
            g_game_start = true;
            // g_camera.Follow = GameObject.FindWithTag("Player").transform;
            // g_camera.m_Lens.OrthographicSize = 2.35f;
            // g_camera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = new Vector3(0, -0.63f, 0);
        }
        if(g_game_start){
            if(g_camera.m_Lens.OrthographicSize < 2.35f){
                g_camera.m_Lens.OrthographicSize += 2.35f*Time.deltaTime*(1*(0.1f+Mathf.Min(Mathf.Abs(g_camera.m_Lens.OrthographicSize - 0f),Mathf.Abs(g_camera.m_Lens.OrthographicSize - 2.35f)))/1.325f);
            }
            if(g_camera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y < 0.63){
                g_camera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y += 0.63f*Time.deltaTime*(1*(0.1f+Mathf.Min(Mathf.Abs(g_camera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y - 0f),Mathf.Abs(g_camera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y - 0.63f)))/0.315f);
            }
        }
        
    }
}
