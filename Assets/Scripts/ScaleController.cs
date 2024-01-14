using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]

public class ScaleController : MonoBehaviour
{
    public Vector2 g_origin_scale;
    public bool g_portrait_enable;
    public bool g_landscape_enable;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(g_portrait_enable && (Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown)){
            transform.localScale = new Vector2(g_origin_scale.x * Screen.height / 1920f / 2, g_origin_scale.y * Screen.height / 1920f / 2);
        }
        else{
            if(!g_landscape_enable)transform.localScale = new Vector2(1, 1);
            else transform.localScale = new Vector2(g_origin_scale.x * Screen.width / 1080f / 2, g_origin_scale.y * Screen.width / 1080f / 2);
        }
    }
}