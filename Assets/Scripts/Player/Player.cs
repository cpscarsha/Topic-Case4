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
    public bool g_is_host = false;

    private Vector2[] g_touch_base_position;
    private float[] g_touch_begin_time;
    private float[] g_touch_duration;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Player is Init");
        GameManager.g_is_connect = true;
        g_self_animator = GetComponent<Animator>();
        // g_self_rigidbody = GetComponent<Rigidbody2D>();
        g_self_kinematic = GetComponent<Kinematic>();
        g_main_idle = GameObject.Find("Main Idle System");
        g_gameover = g_main_idle.GetComponent<MainIdleSystem>().g_gameover;
        g_max_health = 100;
        g_health = 100;
        transform.position = new Vector3(1, 0, 0);
        if(IsOwner){
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            Screen.autorotateToLandscapeLeft = false;
            Screen.autorotateToLandscapeRight = false;
            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;
            if(IsHost){
                g_is_host = IsHost;
                g_ball_object = Instantiate(g_ball).GetComponent<Ball>();
                g_ball_object.GetComponent<NetworkObject>().Spawn();
                g_ball_object.transform.position = new Vector3(-1, 1, 0);
                transform.position = new Vector3(-1, 0, 0);
            }
            GameObject.Find("Virtual Camera").GetComponent<CinemachineVirtualCamera>().Follow = transform;
        }
        g_touch_base_position = new Vector2[2];
        g_touch_begin_time = new float[2];
        g_touch_duration = new float[2];
        // GetComponent<NetworkAnimator>().Animator = GetComponent<Animator>();
        // g_death_particle = GetComponent<ParticleSystem>();
        // g_self_light = GetComponent<Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
        t_time = Time.time;
        if(a_is_sweeping && IsCooldownFinish()){
            if(g_self_kinematic.CheckCollisionIn(GetDirect()?Vector2.right:Vector2.left, 0.25f)){
                foreach(RaycastHit2D i in g_self_kinematic.g_collision_result){
                    try{
                        // if(i.collider.CompareTag("Mob"))
                        // {
                        //     i.collider.GetComponent<MobBase>().BeHit(GetDirect(), 10, g_attack);
                        //     StartCooldown();
                        // }
                        if(i.collider.CompareTag("Ball"))
                        {
                            i.collider.GetComponent<Ball>().Hit(Mathf.Atan2(i.collider.transform.position.y - transform.position.y, i.collider.transform.position.x - transform.position.x), 2.4f);
                            StartCooldown();
                        }
                    }
                    catch{}
                }
            }
        }

        // if(g_self_kinematic.IsStuck()){
        //     transform.position += new Vector3(0, 0.005f, 0); 
        //     Debug.Log("IsStuck");
        // }
        if(!IsOwner)return;
        if(g_main_idle.GetComponent<MainIdleSystem>().g_game_start){
            
            // Debug.Log(g_self_animator.GetBool("isDodge"));
            CheckSlide();
            ExcuteLight();
            ExcuteAnimator();
        }
    }

    public void SetVelocity(Vector2 velocity){
        if(g_self_kinematic.velocity != velocity){
            g_self_kinematic.velocity = velocity;
            GetComponent<PlayerTransformSync>().StartSync();
        }
    }
    public void SetVelocity(char x_y_z, float value){
        if(x_y_z == 'x'){
            if(g_self_kinematic.velocity.x != value){
                g_self_kinematic.velocity.x = value;
                GetComponent<PlayerTransformSync>().StartSync();
            }
        }
        else if(x_y_z == 'y'){
            if(g_self_kinematic.velocity.y != value){
                g_self_kinematic.velocity.y = value;
                GetComponent<PlayerTransformSync>().StartSync();
            }
        }
    }
    
    /*觸控觸發的函數*/
    private bool g_is_buffer_move = false;
    private bool g_is_right_touch = false;

    private void TouchSlide(Direct touch_direct, Direct slide_direct, float move_distance, float hold_time){
        if(touch_direct == Direct.LEFT){
            if(slide_direct == Direct.RIGHT && move_distance > 5f){
                g_self_animator.SetBool("isWalk", true);
                SetVelocity('x', 1.8f);
                SetDirect(true);
            }
            else if(slide_direct == Direct.LEFT && move_distance > 5f){
                g_self_animator.SetBool("isWalk", true);
                SetVelocity('x', -1.8f);
                SetDirect(false);
            }
        }
        else{
            if(slide_direct == Direct.UP && move_distance > 5f){
                if(g_self_kinematic.CheckCollisionIn(Vector2.down, 0.05f)){
                    SetVelocity('y', 3);
                }
            }
            if(slide_direct == Direct.LEFT && move_distance > 40f){
                g_self_animator.SetBool("isDodge", true);
                g_self_animator.SetBool("isWalk", false);
                g_self_animator.SetBool("isAttack", false);
                SetVelocity('x', -3f);
                a_attack_end_level = 0;
                g_self_animator.SetInteger("AttackLevel", 0);
                SetDirect(true);
            }
            if(slide_direct == Direct.RIGHT && move_distance > 40f){
                g_self_animator.SetBool("isDodge", true);
                g_self_animator.SetBool("isWalk", false);
                g_self_animator.SetBool("isAttack", false);
                SetVelocity('x', 3f);
                a_attack_end_level = 0;
                g_self_animator.SetInteger("AttackLevel", 0);
                SetDirect(false);
            }
        }
    }
    private void TouchEnd(Direct touch_direct, Direct slide_direct, float move_distance, float hold_time){
        // Debug.Log(touch_direct + "/" + slide_direct + "/" + move_distance + "/" + hold_time);

        if(touch_direct == Direct.RIGHT && hold_time <= 0.1f){ // 點擊
            int attack_level = g_self_animator.GetInteger("AttackLevel");
            if(attack_level < 3)attack_level++;
            g_self_animator.SetInteger("AttackLevel", attack_level);
        }
        if(touch_direct == Direct.LEFT){
            g_self_animator.SetBool("isWalk", false);
            SetVelocity('x', 0);
        }
    }
    /*觸控觸發的函數*/

    
    private void SetDirect(bool is_right){ // 設定朝向，當 is_right 時朝右，否則朝左
        Vector3 change_scale = transform.localScale;
        if(is_right && change_scale.x == -1){
            change_scale.x = 1;
            transform.localScale = change_scale;
            GetComponent<PlayerTransformSync>().StartSync();
        }
        else if(!is_right && change_scale.x == 1){
            change_scale.x = -1;
            transform.localScale = change_scale;
            GetComponent<PlayerTransformSync>().StartSync();
        }
    }
    public bool GetDirect(){ // 取得朝向，true時向右
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
            SetVelocity('x', 0);
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
    private enum Direct{
        UP = 0,
        DOWN = 1,
        LEFT = 2,
        RIGHT = 3
    } 
    private Direct AngleToDirect(float theata){
        if(theata <= Mathf.PI*3/4 && theata >= Mathf.PI/4){
            return Direct.UP;
        }
        else if(theata >= -Mathf.PI*3/4 && theata <= -Mathf.PI/4){
            return Direct.DOWN;
        }
        else if(theata >= -Mathf.PI/4 && theata <= Mathf.PI/4){
            return Direct.RIGHT;
        }
        else{
            return Direct.LEFT;
        }
    }
    private struct TouchData{
        public Vector3 touch_base_position;
        public Vector3 last_position;
        public float touch_time;
        public bool is_touch;
    }
    private TouchData right_data;
    private TouchData left_data;

    private void CheckSlide(){
        if(right_data.is_touch)right_data.touch_time += Time.deltaTime;
        if(left_data.is_touch)left_data.touch_time += Time.deltaTime;
        if(Input.touches.Length > 0){
            NetworkInputData data = new(Input.touches);
            if(data.has_right){
                if(data.right_phase == TouchPhase.Began){
                    right_data.is_touch = true;
                    right_data.touch_base_position = data.right_position;
                    right_data.touch_time = 0;
                }
                else if(data.right_phase == TouchPhase.Moved || data.right_phase == TouchPhase.Stationary){
                    if(!right_data.is_touch){
                        right_data.is_touch = true;
                        right_data.touch_base_position = data.right_position;
                        right_data.touch_time = 0;
                    }
                    float theata = Mathf.Atan2(data.right_position.y - right_data.touch_base_position.y, data.right_position.x - right_data.touch_base_position.x);
                    TouchSlide(Direct.RIGHT, AngleToDirect(theata), Vector3.Distance(data.right_position, right_data.touch_base_position), right_data.touch_time);
                }
                else{
                    right_data.is_touch = false;
                    float theata = Mathf.Atan2(data.right_position.y - right_data.touch_base_position.y, data.right_position.x - right_data.touch_base_position.x);
                    TouchEnd(Direct.RIGHT, AngleToDirect(theata), Vector3.Distance(data.right_position, right_data.touch_base_position), right_data.touch_time);
                }
                right_data.last_position = data.right_position;
            }
            if(data.has_left){
                if(data.left_phase == TouchPhase.Began){
                    left_data.is_touch = true;
                    left_data.touch_base_position = data.left_position;
                    left_data.touch_time = 0;
                }
                else if(data.left_phase == TouchPhase.Moved || data.left_phase == TouchPhase.Stationary){
                    if(!left_data.is_touch){
                        left_data.is_touch = true;
                        left_data.touch_base_position = data.left_position;
                        left_data.touch_time = 0;
                    }
                    float theata = Mathf.Atan2(data.left_position.y - left_data.touch_base_position.y, data.left_position.x - left_data.touch_base_position.x);
                    TouchSlide(Direct.LEFT, AngleToDirect(theata), Vector3.Distance(data.left_position, left_data.touch_base_position), left_data.touch_time);
                }
                else{
                    left_data.is_touch = false;
                    float theata = Mathf.Atan2(data.left_position.y - left_data.touch_base_position.y, data.left_position.x - left_data.touch_base_position.x);
                    TouchEnd(Direct.LEFT, AngleToDirect(theata), Vector3.Distance(data.left_position, left_data.touch_base_position), left_data.touch_time);
                }
                left_data.last_position = data.left_position;
            }
        }
        else{
            if(right_data.is_touch){
                right_data.is_touch = false;
                float theata = Mathf.Atan2(right_data.last_position.y - right_data.touch_base_position.y, right_data.last_position.x - right_data.touch_base_position.x);
                TouchEnd(Direct.RIGHT, AngleToDirect(theata), Vector3.Distance(right_data.last_position, right_data.touch_base_position), right_data.touch_time);
            }    
            if(left_data.is_touch){
                left_data.is_touch = false;
                float theata = Mathf.Atan2(left_data.last_position.y - left_data.touch_base_position.y, left_data.last_position.x - left_data.touch_base_position.x);
                TouchEnd(Direct.LEFT, AngleToDirect(theata), Vector3.Distance(left_data.last_position, left_data.touch_base_position), left_data.touch_time);
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
