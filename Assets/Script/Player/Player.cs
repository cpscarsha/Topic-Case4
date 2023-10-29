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
    public RaycastHit2D[] g_collision_result;
    private Animator g_self_animator;
    private Rigidbody2D g_self_rigidbody;
    private Collider2D g_self_collider;
    // Start is called before the first frame update
    void Start()
    {
        g_self_animator = GetComponent<Animator>();
        g_self_rigidbody = GetComponent<Rigidbody2D>();
        g_self_collider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInput now_input = GetInput();
        if(now_input == PlayerInput.LEFT || now_input == PlayerInput.RIGHT){
            g_self_animator.SetBool("isAttack", true); // 開始攻擊動畫
            SetDirect(now_input == PlayerInput.RIGHT);
            if(now_input == PlayerInput.LEFT){
                g_self_rigidbody.velocity = new Vector2(-2, 0);
            }
            else{
                g_self_rigidbody.velocity = new Vector2(2, 0);
            }
        }

        ExcuteAnimator();
    }
    void FixedUpdate(){
        FixCollision();
    }

    private void FixCollision(){
        float now_speed = g_self_rigidbody.velocity.x;
        if(CheckCollisionIn(Vector2.right, now_speed*Time.fixedDeltaTime+0.005f) && now_speed > 0){
            g_self_rigidbody.velocity = Vector2.zero;
        }
        else if(CheckCollisionIn(Vector2.left, -now_speed*Time.fixedDeltaTime+0.005f) && now_speed < 0){
            g_self_rigidbody.velocity = Vector2.zero;
        }
    }
    private bool CheckCollisionIn(Vector2 direction, float distance){
        g_collision_result = new RaycastHit2D[2];
        return g_self_collider.Cast(direction, g_collision_result, distance) > 0;
    }
    private void SetDirect(bool is_right){ // 設定怪物朝向，當 is_right 時朝右，否則朝左
        Vector3 change_scale = transform.localScale;
        if(is_right && change_scale.x < 0){
            change_scale.x = -change_scale.x;
        }
        else if(!is_right && change_scale.x > 0){
            change_scale.x = -change_scale.x;
        }
        transform.localScale = change_scale;
    }
    private void ExcuteAnimator(){ // 處理動畫造成的變數變化
        if(a_attack_end){
            g_self_animator.SetBool("isAttack", false); // 關閉攻擊動畫
            g_self_rigidbody.velocity = Vector2.zero;
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
