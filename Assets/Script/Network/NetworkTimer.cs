using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkTimer : MonoBehaviour
{
    private float timer;
    public float g_min_time_between_ticks{get;}
    public int g_current_tick{get; private set;}

    public NetworkTimer(float server_tick_rate){
        g_min_time_between_ticks = 1f / server_tick_rate;
    }
    private void Update()
    {
        timer += Time.deltaTime;
    }

    public bool ShouldTick(){
        if(timer >= g_min_time_between_ticks){
            timer -= g_min_time_between_ticks;
            g_current_tick++;
            return true;
        }
        return false;
    }
}
