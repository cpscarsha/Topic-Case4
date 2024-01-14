using System.Collections;
using System.Collections.Generic;
// using Unity.VisualStudio.Editor;
using UnityEngine;


[ExecuteInEditMode]
public class ModButton : MonoBehaviour
{
    public Vector2 g_origin_scale;
    public Vector2 g_origin_position_0;
    public Vector2 g_origin_position_1;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown){
            transform.GetChild(0).transform.localScale = new Vector2(g_origin_scale.x * Screen.height / 1920f, g_origin_scale.y * Screen.height / 1920f);
            transform.GetChild(1).transform.localScale = new Vector2(g_origin_scale.x * Screen.height / 1920f, g_origin_scale.y * Screen.height / 1920f);
            transform.GetChild(0).transform.localPosition = new Vector2(Screen.width / 2, 100 + 50* Screen.height / 1920f);
            transform.GetChild(1).transform.localPosition = new Vector2(Screen.width / 2, 100 + 50* Screen.height / 1920f + 50 + 2 * 50* Screen.height / 1920f);
        }
        else{
            // transform.localScale = new Vector2(g_origin_scale.x * Screen.width / 1080f / 2, g_origin_scale.y * Screen.width / 1080f / 2);
            transform.GetChild(0).transform.localScale = new Vector2(1, 1);
            transform.GetChild(1).transform.localScale = new Vector2(1, 1);
            transform.GetChild(0).transform.localPosition = new Vector2(100 + 350, g_origin_position_0.y);
            transform.GetChild(1).transform.localPosition = new Vector2(100 + 350, g_origin_position_1.y);
        }
    }
}
