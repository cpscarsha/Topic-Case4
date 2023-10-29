using System.Collections;
using System.Collections.Generic;
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
    private Animator g_self_animator;
    private Rigidbody2D g_self_rigidbody;
    private bool g_is_motion = false;
    // Start is called before the first frame update
    void Start()
    {
        g_self_animator = GetComponent<Animator>();
        g_self_rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInput now_input = GetInput();
        if(now_input == PlayerInput.LEFT || now_input == PlayerInput.RIGHT){
            g_self_animator.SetBool("isAttack", true); // 開始攻擊動畫
            Vector3 change_scale = transform.localScale;
            if(now_input == PlayerInput.LEFT){
                change_scale.x = -Mathf.Abs(change_scale.x);
                g_self_rigidbody.velocity = new Vector2(-2, 0);
            }
            else{
                change_scale.x = Mathf.Abs(change_scale.x);
                g_self_rigidbody.velocity = new Vector2(2, 0);
            }
            transform.localScale = change_scale;
        }

        ExcuteAnimator();
    }

    private void ExcuteAnimator(){ // 處理動畫造成的變數變化
        if(a_attack_end){
            g_self_animator.SetBool("isAttack", false); // 關閉攻擊動畫
            g_self_rigidbody.velocity = new Vector2(0, 0);
            a_attack_end = false;
        }
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
