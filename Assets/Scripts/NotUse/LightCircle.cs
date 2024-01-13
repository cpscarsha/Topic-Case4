using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightCircle : MonoBehaviour
{
    public float g_intensity = 10f;
    public float g_radius = 1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.GetChild(0).GetComponent<UnityEngine.Rendering.Universal.Light2D>().intensity = g_intensity;
        transform.GetChild(1).GetComponent<UnityEngine.Rendering.Universal.Light2D>().intensity = g_intensity;
    }
}
