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
        if(g_mob.IsDeath()){
            GetComponent<SpriteRenderer>().enabled = false;
        }
        float display_health = g_mob.GetHealthPercentage();
        if(display_health >= 1){
            display_health = 1;
        }
        if(display_health <= 0){
            display_health = 0;
        }
        float direct = (int)g_mob.transform.localScale.x;
        direct = Mathf.Abs(direct)/direct;
        if(direct < 0){
            g_fill.transform.localPosition = new Vector3(0.15f, 0, 0);
        }
        else{
            g_fill.transform.localPosition = new Vector3(-0.15f, 0, 0);
        }
        g_fill.transform.localScale = new Vector2(direct*display_health, 1);
        
    }
}
