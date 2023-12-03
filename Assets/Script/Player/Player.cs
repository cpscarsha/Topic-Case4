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
    public bool a_walk_end = false;
    public bool a_dodge_end = false;
    public bool a_is_sweeping = false;
    public GameObject g_gameover;
    public float g_health;
    public float g_max_health;
    
    // private Light2D g_self_light;
    private Animator g_self_animator;
    // private Rigidbody2D g_self_rigidbody;
    private ParticleSystem g_death_particle;
    private bool g_death = false;
    private Kinematic g_self_kinematic;
    public float g_attack = 0.2f;
    public float g_attack_cooldown = 0.2f;
    private float g_attack_cooldown_remaining = 0;
    // Start is called before the first frame update
    void Start()
    {
        g_self_animator = GetComponent<Animator>();
        // g_self_rigidbody = GetComponent<Rigidbody2D>();
        g_self_kinematic = GetComponent<Kinematic>();
        g_max_health = 100;
        g_health = 100;
        g_death_particle = GetComponent<ParticleSystem>();
        // g_self_light = GetComponent<Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(a_is_sweeping && IsCooldownFinish()){
            if(g_self_kinematic.CheckCollisionIn(GetDirect()?Vector2.right:Vector2.left, 0.4f)){
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
        // Debug.Log(g_self_kinematic.IsStuck());
        CheckSlide();
        ExcuteLight();
        ExcuteAnimator();
    }
    
    /*觸控觸發的函數*/
    private bool g_is_buffer_move = false;
    void Click(){ // 點擊螢幕時觸發
        g_self_animator.SetBool("isAttack", true); // 開始攻擊動畫
        g_self_animator.SetBool("isWalk", false);
        g_self_animator.SetBool("isDodge", false);
        g_self_kinematic.velocity.x = (GetDirect()?1:-1)*2;
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
        SetDirect(false);
    }
    void SlideLeft(){
        g_self_animator.SetBool("isDodge", true);
        g_self_animator.SetBool("isWalk", false);
        g_self_animator.SetBool("isAttack", false);
        g_self_kinematic.velocity.x = -2;
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
        g_self_animator.SetBool("isWalk", true);
        g_self_kinematic.velocity.x = 1.8f;
        SetDirect(true);
    }
    void KeepSlideLeft(){
        g_self_animator.SetBool("isWalk", true);
        g_self_kinematic.velocity.x = -1.8f;
        SetDirect(false);
    }
    void ShortSlideUp(){
        if(g_self_kinematic.CheckCollisionIn(Vector2.down, 0.05f)){
            g_self_kinematic.velocity.y = 3;
        }
        if(g_is_buffer_move){
            g_is_buffer_move = false;
            g_self_kinematic.velocity.x = 0;
        }
    }
    void ShortSlideDown(){
        if(g_is_buffer_move){
            g_is_buffer_move = false;
            g_self_kinematic.velocity.x = 0;
        }
    }
    void ShortSlideRight(){
        g_self_kinematic.velocity.x = 1f;
        g_is_buffer_move = true;
    }
    void ShortSlideLeft(){
        g_self_kinematic.velocity.x = -1f;
        g_is_buffer_move = true;
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
    private void ExcuteAnimator(){ // 處理動畫造成的變數變化
        if(a_attack_end){
            g_self_animator.SetBool("isAttack", false); // 關閉攻擊動畫
            g_self_kinematic.velocity.x = 0;
            a_attack_end = false;
        }

        if(a_walk_end){
            if(GetNowInput() == PlayerInput.NOTHING){
                g_self_animator.SetBool("isWalk", false);
                g_self_kinematic.velocity.x = 0;
                a_walk_end = false;
            }
        }
        if(g_self_animator.GetBool("isWalk") && GetEndedTouchInput() != PlayerInput.NOTHING){ //長按持續行走
                g_self_animator.SetBool("isWalk", false);
                g_self_kinematic.velocity.x = 0;
                a_walk_end = false;
        }

        if(a_dodge_end){
            g_self_animator.SetBool("isDodge", false);
            g_self_kinematic.velocity.x = 0;
            a_dodge_end = false;
        }
    }

    private void ExcuteLight(){
        if(g_death){
            GetComponent<UnityEngine.Rendering.Universal.Light2D>().intensity = 1.5f*Mathf.Abs((int)(100*Time.time)%200/5.0f-20);
        }
        else{
            float display_light = g_health/g_max_health;
            if(display_light < 0)display_light = 0;
            GetComponent<UnityEngine.Rendering.Universal.Light2D>().intensity = 2*display_light;
        }
    }
    private PlayerInput GetNowInput(){
        if(Input.touches.Length > 0){
            Touch touch = Input.touches[0];
            Vector3 touch_position = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane));
            if(touch_position.x > Camera.main.transform.position.x){
                return PlayerInput.RIGHT;
            }
            else{
                return PlayerInput.LEFT;
            }
        }
        return PlayerInput.NOTHING;
    }
    private PlayerInput GetBeginTouchInput(){
        if(Input.touches.Length > 0){
            Touch touch = Input.touches[0];
            if(touch.phase == TouchPhase.Began){
                Vector3 touch_position = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane));
                if(touch_position.x > Camera.main.transform.position.x){
                    return PlayerInput.RIGHT;
                }
                else{
                    return PlayerInput.LEFT;
                }
            }
        }
        return PlayerInput.NOTHING;
    }
    private PlayerInput GetEndedTouchInput(){
        if(Input.touches.Length > 0){
            Touch touch = Input.touches[0];
            if(touch.phase == TouchPhase.Ended){
                Vector3 touch_position = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane));
                if(touch_position.x > Camera.main.transform.position.x){
                    return PlayerInput.RIGHT;
                }
                else{
                    return PlayerInput.LEFT;
                }
            }
        }
        return PlayerInput.NOTHING;
    }

    private Vector2 g_touch_base_position;
    private float g_touch_begin_time = 0;
    private float g_touch_duration;
    private void CheckSlide(){
        if(Input.touches.Length > 0){
            Touch touch = Input.touches[0];
            g_touch_duration = Time.time-g_touch_begin_time;
            Vector2 end_position = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane));
            float theata = Mathf.Atan2(end_position.y-g_touch_base_position.y, end_position.x-g_touch_base_position.x);
            if(touch.phase == TouchPhase.Began){
                g_touch_base_position = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane));
                g_touch_begin_time = Time.time;
                g_touch_duration = 0;
            }
            else if(touch.phase == TouchPhase.Ended){
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
            else{
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
