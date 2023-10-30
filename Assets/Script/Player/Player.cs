using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum PlayerInput{
        LEFT,
        RIGHT,
        NOTHING
    };
    public bool a_attack_end = false;
    public bool a_is_sweeping = false;
    public float g_health;
    // private Light2D g_self_light;
    private Animator g_self_animator;
    private Rigidbody2D g_self_rigidbody;
    
    private Kinematic g_self_kinematic;
    // Start is called before the first frame update
    void Start()
    {
        g_self_animator = GetComponent<Animator>();
        g_self_rigidbody = GetComponent<Rigidbody2D>();
        g_self_kinematic = GetComponent<Kinematic>();
        g_health = 100;
        // g_self_light = GetComponent<Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInput now_input = GetInput();
        if(now_input == PlayerInput.LEFT || now_input == PlayerInput.RIGHT){
            g_self_animator.SetBool("isAttack", true); // 開始攻擊動畫
            SetDirect(now_input == PlayerInput.RIGHT);
            if(now_input == PlayerInput.LEFT){
                g_self_kinematic.velocity = new Vector2(-2, 0);
            }
            else{
                g_self_kinematic.velocity = new Vector2(2, 0);
            }
        }
        if(a_is_sweeping){
            if(g_self_kinematic.CheckCollisionIn(GetDirect()?Vector2.right:Vector2.left, 0.3f)){
                foreach(RaycastHit2D i in g_self_kinematic.g_collision_result){
                    try{
                        if(i.collider.tag == "Mob"){
                        i.collider.GetComponent<Kinematic>().knockback = new Vector2((GetDirect()?1:-1)*10, 0);
                        }
                    }
                    catch{}
                }
            }
        }
        ExcuteLight();
        ExcuteAnimator();
    }
    
    
    private void SetDirect(bool is_right){ // 設定朝向，當 is_right 時朝右，否則朝左
        Vector3 change_scale = transform.localScale;
        if(is_right && change_scale.x < 0){
            change_scale.x = -change_scale.x;
        }
        else if(!is_right && change_scale.x > 0){
            change_scale.x = -change_scale.x;
        }
        transform.localScale = change_scale;
    }
    private bool GetDirect(){ // 取得朝向，true時向右
        return transform.localScale.x > 0;
    }
    private void ExcuteAnimator(){ // 處理動畫造成的變數變化
        if(a_attack_end){
            g_self_animator.SetBool("isAttack", false); // 關閉攻擊動畫
            g_self_kinematic.velocity = Vector2.zero;
            a_attack_end = false;
        }
    }
    private void ExcuteLight(){
        GetComponent<UnityEngine.Rendering.Universal.Light2D>().intensity = 2.8f*g_health*0.01f+1.2f;
    }
    private PlayerInput GetInput(){
        if(Input.touches.Length > 0){
            Touch touch = Input.touches[0];
            if(touch.phase == TouchPhase.Began){
                Vector3 touch_position = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane));
                if(touch_position.x > transform.position.x){
                    return PlayerInput.RIGHT;
                }
                else{
                    return PlayerInput.LEFT;
                }
            }
        }
        return PlayerInput.NOTHING;
    }
    
}
