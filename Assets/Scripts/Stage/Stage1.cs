using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1 : StageBase
{
    public GameObject[] NextStage;
    // Start is called before the first frame update
    void Start()
    {
        VariableInit();
        StartStage();
    }

    // Update is called once per frame
    void Update()
    {
        NecessaryLoop();
        if(g_stage_is_begin && MobCleared()){
            FinishStage();
        }
        if(StageIsEnd()){
            Instantiate(NextStage[Random.Range(0, NextStage.Length)]);
            Destroy(gameObject);
        }
    }
}
