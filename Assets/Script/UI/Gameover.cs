using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Gameover : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.touches.Length > 0 && Input.touches[0].phase == TouchPhase.Began){
            SceneManager.LoadScene("SampleScene");
        }
    }
}
