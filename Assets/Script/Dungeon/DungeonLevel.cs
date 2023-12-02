using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonLevel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < transform.childCount; i++){
            MobBase mob = transform.GetChild(i).GetComponent<MobBase>();
            Vector4 summon_range = mob.GetSummonRange();
            int repect_count = 0;
            do{
                float x = 0.125f+(int)((summon_range.x-0.125f)/0.25f)*0.25f + Random.Range(0, (int)summon_range.z)*0.25f;
                float y = 0.125f+(int)((summon_range.y-0.125f)/0.25f)*0.25f + Random.Range(0, (int)summon_range.w)*0.25f;
                mob.transform.position = new Vector3(x, y, 0);            
            }while(repect_count++ < 100 && mob.GetComponent<Kinematic>().IsStuck());
            Debug.Log(repect_count);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
