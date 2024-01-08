using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine;
using Unity.Netcode.Components;
using Unity.Netcode;
using Cinemachine;

public class Player : NetworkBehaviour
{
    public enum PlayerInput{
        LEFT,
        RIGHT,
        NOTHING
    };
    public bool a_attack_end = false;
    public int a_attack_end_level = 0;
    // public bool a_walk_end = false;
    public bool a_dodge_end = false;
    public bool a_is_sweeping = false;
    public bool a_is_idle = true;
    public GameObject g_gameover;
    public float g_health;
    public float g_max_health;
    
    // private Light2D g_self_light;
    private Animator g_self_animator;
    // private Rigidbody2D g_self_rigidbody;
    // private ParticleSystem g_death_particle;
    private bool g_death = false;
    private Kinematic g_self_kinematic;
    // private int g_attack_level = 0;
    public float g_attack = 0.2f;
    
    public float g_attack_cooldown = 5f;
    private float g_attack_cooldown_remaining = 0;
    public float g_attack_delay = 0.2f;
    public float g_attack_delay_remaining = 0;
    public GameObject g_main_idle;
    public float t_time;
    public GameObject g_ball;
    public Ball g_ball_object;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Player is Init");
        GameManager.g_is_connect = true;
        g_self_animator = GetComponent<Animator>();
        // g_self_rigidbody = GetComponent<Rigidbody2D>();
        g_self_kinematic = GetComponent<Kinematic>();
        g_main_idle = GameObject.Find("Main Idle System");
        g_gameover = GameObject.Find("Gameover");
        g_max_health = 100;
        g_health = 100;
        transform.position = new Vector3(1, 0, 0);
        if(IsServer){
            g_ball_object = Instantiate(g_ball).GetComponent<Ball>();
            g_ball_object.transform.position = new Vector3(-1, 1, 0);
            transform.position = new Vector3(-1, 0, 0);
        }
        if(IsOwner){
            GameObject.Find("Virtual Camera").GetComponent<CinemachineVirtualCamera>().Follow = transform;
        }
        // GetComponent<NetworkAnimator>().Animator = GetComponent<Animator>();
        // g_death_particle = GetComponent<ParticleSystem>();
        // g_self_light = GetComponent<Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        if(g_main_idle.GetComponent<MainIdleSystem>().g_game_start){
            t_time = Time.time;
            if(a_is_sweeping && IsCooldownFinish()){
                if(g_self_kinematic.CheckCollisionIn(GetDirect()?Vector2.right:Vector2.left, 0.25f)){
                    foreach(RaycastHit2D i in g_self_kinematic.g_collision_result){
                        try{
                            if(i.collider.CompareTag("Mob"))
                            {
                                i.collider.GetComponent<MobBase>().BeHit(GetDirect(), 10, g_attack);
                                StartCooldown();
                            }
                        }
                        catch{}
                    }
                }
            }

            if(g_self_kinematic.HasCollision(0.05f)){
                foreach(RaycastHit2D i in g_self_kinematic.g_collision_result){
                    try{
                        if(i.collider.CompareTag("Ball")){
                             g_ball_object.Hit(Mathf.PI/4 + (GetDirect()?0:Mathf.PI/2), 1.6f);
                            // g_ball_object.Hit(Mathf.Atan2(g_ball_object.transform.position.y - transform.position.y, g_ball_object.transform.position.x - transform.position.x), 1.3f);
                        }
                    }
                    catch{}
                }
            }
            // Debug.Log(g_self_animator.GetBool("isDodge"));
            CheckSlide();
            ExcuteLight();
            ExcuteAnimator();
        }
    }
    
    /*觸控觸發的函數*/
    private bool g_is_buffer_move = false;
    void Click(){ // 點擊螢幕時觸發
        if(!g_self_animator.GetBool("isDodge")){
            // g_self_animator.SetBool("isAttack", true); // 開始攻擊動畫
            // g_self_animator.SetBool("isWalk", false);
            // g_self_animator.SetBool("isDodge", false);
            // g_self_kinematic.velocity.x = (GetDirect()?1:-1)*2;
            int attack_level = g_self_animator.GetInteger("AttackLevel");
            if(attack_level < 3)attack_level++;
            g_self_animator.SetInteger("AttackLevel", attack_level);
        }
    }
    void TouchEnd(){
        if(g_self_animator.GetBool("isWalk")){
            g_self_animator.SetBool("isWalk", false);
            g_self_kinematic.velocity.x = 0;
        }
    }
    void SlideUp(){
        // if(g_self_kinematic.CheckCollisionIn(Vector2.down, 0.05f)){
        //     g_self_kinematic.velocity.y = 3;
        // }
    }
    void SlideDown(){
        // g_self_kinematic.velocity.y = -2;
    }
    void SlideRight(){
        g_self_animator.SetBool("isDodge", true);
        g_self_animator.SetBool("isWalk", false);
        g_self_animator.SetBool("isAttack", false);
        g_self_kinematic.velocity.x = 2;
        a_attack_end_level = 0;
        Debug.Log("c");
        g_self_animator.SetInteger("AttackLevel", 0);
        SetDirect(false);
    }
    void SlideLeft(){
        g_self_animator.SetBool("isDodge", true);
        g_self_animator.SetBool("isWalk", false);
        g_self_animator.SetBool("isAttack", false);
        g_self_kinematic.velocity.x = -2;
        a_attack_end_level = 0;
        Debug.Log("d");
        g_self_animator.SetInteger("AttackLevel", 0);
        SetDirect(true);
    }
    void KeepSlideUp(){
        // if(g_self_kinematic.CheckCollisionIn(Vector2.down, 0.05f)){
        //     g_self_kinematic.velocity.y = 3;
        // }
    }
    void KeepSlideDown(){

    }
    void KeepSlideRight(){
        if(!g_self_animator.GetBool("isDodge")){
            g_self_animator.SetBool("isWalk", true);
            g_self_kinematic.velocity.x = 1.8f;
            SetDirect(true);
        }
    }
    void KeepSlideLeft(){
        if(!g_self_animator.GetBool("isDodge")){
            g_self_animator.SetBool("isWalk", true);
            g_self_kinematic.velocity.x = -1.8f;
            SetDirect(false);
        }
    }
    void ShortSlideUp(){
        if(!g_self_animator.GetBool("isDodge")){
            if(g_self_kinematic.CheckCollisionIn(Vector2.down, 0.05f)){
                g_self_kinematic.velocity.y = 3;
            }
            if(g_is_buffer_move){
                g_is_buffer_move = false;
                g_self_kinematic.velocity.x = 0;
            }
        }
    }
    void ShortSlideDown(){
        if(!g_self_animator.GetBool("isDodge")){
            if(g_is_buffer_move){
                g_is_buffer_move = false;
                g_self_kinematic.velocity.x = 0;
            }
        }
    }
    void ShortSlideRight(){
        if(!g_self_animator.GetBool("isDodge")){
            g_self_kinematic.velocity.x = 1f;
            g_is_buffer_move = true;
        }
    }
    void ShortSlideLeft(){
        if(!g_self_animator.GetBool("isDodge")){
            g_self_kinematic.velocity.x = -1f;
            g_is_buffer_move = true;
        }
    }
    /*觸控觸發的函數*/

    
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

    private bool g_is_attack_ending = false;
    private void ExcuteAnimator(){ // 處理動畫造成的變數變化
        // if(a_attack_end){
        //     g_self_animator.SetBool("isAttack", false); // 關閉攻擊動畫
        //     g_self_kinematic.velocity.x = 0;
        //     a_attack_end = false;
        // }

        // if(a_walk_end){
        //     if(GetNowInput() == PlayerInput.NOTHING){
        //         g_self_animator.SetBool("isWalk", false);
        //         g_self_kinematic.velocity.x = 0;
        //         a_walk_end = false;
        //     }
        // }
        // if(g_self_animator.GetBool("isWalk") && GetEndedTouchInput() != PlayerInput.NOTHING){ //長按持續行走
        //     g_self_animator.SetBool("isWalk", false);
        //     g_self_kinematic.velocity.x = 0;
        //     a_walk_end = false;
        // }
        int attack_level = g_self_animator.GetInteger("AttackLevel");
        
        if(a_attack_end_level != 0 && a_attack_end_level >= attack_level && !g_is_attack_ending){
            g_attack_delay_remaining = g_attack_delay+Time.time;
            g_is_attack_ending = true;
            Debug.Log("a");
        }
        else if(a_attack_end_level == 3 || a_attack_end_level != 0 && g_is_attack_ending && a_attack_end_level >= attack_level && Time.time > g_attack_delay_remaining){
            attack_level = 0;
            a_attack_end_level = 0;
            g_is_attack_ending = false;
            g_self_animator.SetInteger("AttackLevel", 0);
            Debug.Log("b");
        }
        // else if(Time.time > g_attack_delay_remaining){
        //     g_attack_delay_remaining = 0;
        //     Debug.Log("d");
        // }
        
        
        
        if(a_dodge_end){
            g_self_kinematic.velocity.x = 0;
            a_dodge_end = false;
            g_self_animator.SetBool("isDodge", false);
        }
    }

    private void ExcuteLight(){
        if(g_death){
            GetComponent<UnityEngine.Rendering.Universal.Light2D>().intensity = 1.5f*Mathf.Abs((int)(100*Time.time)%200/5.0f-20);
        }
        else{
            float display_light = g_health/g_max_health;
            if(display_light < 0)display_light = 0;
            GetComponent<UnityEngine.Rendering.Universal.Light2D>().intensity = 3*display_light + 0.3f;
        }
    }
    // private PlayerInput GetNowInput(){
    //     if(Input.touches.Length > 0){
    //         Touch touch = Input.touches[0];
    //         Vector3 touch_position = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane));
    //         if(touch_position.x > Camera.main.transform.position.x){
    //             return PlayerInput.RIGHT;
    //         }
    //         else{
    //             return PlayerInput.LEFT;
    //         }
    //     }
    //     return PlayerInput.NOTHING;
    // }
    // private PlayerInput GetBeginTouchInput(){
    //     if(Input.touches.Length > 0){
    //         Touch touch = Input.touches[0];
    //         if(touch.phase == TouchPhase.Began){
    //             Vector3 touch_position = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane));
    //             if(touch_position.x > Camera.main.transform.position.x){
    //                 return PlayerInput.RIGHT;
    //             }
    //             else{
    //                 return PlayerInput.LEFT;
    //             }
    //         }
    //     }
    //     return PlayerInput.NOTHING;
    // }
    // private PlayerInput GetEndedTouchInput(){
    //     if(Input.touches.Length > 0){
    //         Touch touch = Input.touches[0];
    //         if(touch.phase == TouchPhase.Ended){
    //             Vector3 touch_position = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane));
    //             if(touch_position.x > Camera.main.transform.position.x){
    //                 return PlayerInput.RIGHT;
    //             }
    //             else{
    //                 return PlayerInput.LEFT;
    //             }
    //         }
    //     }
    //     return PlayerInput.NOTHING;
    // }

    private Vector2 g_touch_base_position;
    private float g_touch_begin_time = 0;
    private float g_touch_duration;
    private void CheckSlide(){
        if(Input.touches.Length > 0){
            Touch touch = Input.touches[0];
            g_touch_duration = Time.time-g_touch_begin_time;
            // Vector2 end_position = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane));
            Vector2 end_position = touch.position;
            float theata = Mathf.Atan2(end_position.y-g_touch_base_position.y, end_position.x-g_touch_base_position.x);
            // Debug.Log("base:"+g_touch_base_position+"|||"+end_position);
            if(touch.phase == TouchPhase.Began){
                // g_touch_base_position = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane));
                g_touch_base_position = touch.position;
                g_touch_begin_time = Time.time;
                g_touch_duration = 0;
            }
            else if(touch.phase == TouchPhase.Ended){
                TouchEnd();
                if(g_touch_duration < 0.15f){
                    if(Vector2.Distance(g_touch_base_position, end_position) < 0.3f){
                        Click();
                    }
                    else{
                        if(theata <= Mathf.PI*3/4 && theata >= Mathf.PI/4){
                            SlideUp();
                        }
                        else if(theata >= -Mathf.PI*3/4 && theata <= -Mathf.PI/4){
                            SlideDown();
                        }
                        else if(theata >= -Mathf.PI/4 && theata <= Mathf.PI/4){
                            SlideRight();
                        }
                        else if(theata >= Mathf.PI*3/4 || theata <= -Mathf.PI*3/4){
                            SlideLeft();
                        }
                    }
                }
            }
            else if(g_touch_duration > 0.15f){
                if(theata <= Mathf.PI*3/4 && theata >= Mathf.PI/4){
                    KeepSlideUp();
                }
                else if(theata >= -Mathf.PI*3/4 && theata <= -Mathf.PI/4){
                    KeepSlideDown();
                }
                else if(theata >= -Mathf.PI/4 && theata <= Mathf.PI/4){
                    KeepSlideRight();
                }
                else if(theata >= Mathf.PI*3/4 || theata <= -Mathf.PI*3/4){
                    KeepSlideLeft();
                }
            }
            else if(Vector2.Distance(g_touch_base_position, end_position) > 0.3f){
                if(theata <= Mathf.PI*3/4 && theata >= Mathf.PI/4){
                    ShortSlideUp();
                }
                else if(theata >= -Mathf.PI*3/4 && theata <= -Mathf.PI/4){
                    ShortSlideDown();
                }
                else if(theata >= -Mathf.PI/4 && theata <= Mathf.PI/4){
                    ShortSlideRight();
                }
                else if(theata >= Mathf.PI*3/4 || theata <= -Mathf.PI*3/4){
                    ShortSlideLeft();
                }
            }
        }     
    }
    
    private void StartCooldown(){
        g_attack_cooldown_remaining = Time.time + g_attack_cooldown;
    }
    private bool IsCooldownFinish(){
        return g_attack_cooldown_remaining < Time.time;
    }
    public void Death(){
        // g_death_particle.Play();
        g_gameover.SetActive(true);
        GetComponent<SpriteRenderer>().enabled = false;
        g_self_kinematic.enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<Animator>().enabled = false;
        g_death = true;
        // Destroy(gameObject, 1f); 
    }
    public bool IsDeath(){
        return g_death;
    }
}
