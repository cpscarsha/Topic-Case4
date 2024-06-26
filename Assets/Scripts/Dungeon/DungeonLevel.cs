using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonLevel : MonoBehaviour
{
    public float[] g_summon_rate;
    private bool g_active = false;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < transform.childCount; i++){
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    void Update(){
        if(g_active){
            
        }
    }
    public void ChangePosition(){
        if(g_active){
            for(int i = 0; i < transform.childCount; i++){
                if((i < g_summon_rate.Length?g_summon_rate[i]:100) > Random.Range(0f, 99.99f)){    
                    MobBase mob = transform.GetChild(i).GetComponent<MobBase>();
                    Vector4 summon_range = mob.GetSummonRange();
                    int repect_count = 0;
                    do{
                        float x = 0.125f+Mathf.Ceil((summon_range.x-0.125f)/0.25f)*0.25f + Random.Range(0, (int)Mathf.Ceil(summon_range.z))*0.25f;
                        float y = -0.125f+Mathf.Ceil((summon_range.y+0.135f)/0.25f)*0.25f + Random.Range(0, (int)Mathf.Ceil(summon_range.w))*0.25f;
                        mob.GetComponent<Rigidbody2D>().position = new Vector2(x, y); // 直接改變transform.position會導致mob.GetComponent<Kinematic>()來不及變化
                    }while(repect_count++ < 100 && mob.GetComponent<Kinematic>().IsStuck());
                    if(repect_count == 101){
                        Debug.Log("die because repeat");
                        Destroy(mob.gameObject);
                    }
                }
                else{
                    Destroy(transform.GetChild(i).gameObject);
                    Debug.Log("die because rate");
                }
            }
            g_active = false;
        }
    }
    // Update is called once per frame
    public void Active(){
        for(int i = 0; i < transform.childCount; i++){
            transform.GetChild(i).gameObject.SetActive(true);
        }
        g_active = true;
        Invoke("ChangePosition", 0.02f);
    }
}
