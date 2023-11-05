using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobHealthBar : MonoBehaviour
{
    private GameObject g_fill;
    public MobBase g_mob;

    // Start is called before the first frame update
    void Start()
    {
        g_fill = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        float display_health = g_mob.GetHealthPercentage();
        if(display_health >= 1){
            display_health = 1;
        }
        if(display_health <= 0){
            display_health = 0;
        }
        g_fill.transform.localScale = new Vector2(display_health, 1);
        
    }
}
