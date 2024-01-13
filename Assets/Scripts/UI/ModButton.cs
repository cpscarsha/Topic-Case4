using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModButton : MonoBehaviour
{
    public Vector2 g_origin_scale;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown){
            transform.localScale = new Vector2(g_origin_scale.x * Screen.height / 1920f / 2, g_origin_scale.y * Screen.height / 1920f / 2);
            transform.GetChild(0).transform.position = new Vector2(0, transform.GetChild(0).transform.position.y);
            transform.GetChild(1).transform.position = new Vector2(0, transform.GetChild(1).transform.position.y);
        }
        else{
            // transform.localScale = new Vector2(g_origin_scale.x * Screen.width / 1080f / 2, g_origin_scale.y * Screen.width / 1080f / 2);
            transform.GetChild(0).transform.position = new Vector2(-410, transform.GetChild(0).transform.position.y);
            transform.GetChild(1).transform.position = new Vector2(-410, transform.GetChild(1).transform.position.y);
        }
    }
}
