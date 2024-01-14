using System.ComponentModel;
using System.Threading;
using Unity.Netcode;
using UnityEngine;

public class PlayerTransformSync : NetworkBehaviour
{
    private float _syncScaleX;
    private float _syncGravity;
    private Vector2 _syncVelocity;
    private Vector3 _syncPosition;
    
    private Kinematic g_kinematic;
    private NetworkTimer g_timer;

    private static readonly int buffer_size = 1000;
    private Buffer<Vector3> g_delay_position = new(buffer_size);
    private Buffer<Vector3> g_delay_velocity = new(buffer_size);
    public int buffer_index = 0;
    public int tick_difference; // 用於計算主機和客戶端誤差時間

    public bool g_start_sync = false;
    private void Start(){
        g_timer = new NetworkTimer(45);
        g_kinematic = GetComponent<Kinematic>();
    }

    private void Update(){
        g_timer.Update(Time.deltaTime);
    }
    private void FixedUpdate()
    {
        if(g_timer.ShouldTick()){
            tick_difference += 1;// 用於計算主機和客戶端誤差時間
            if(!IsServer){
                // 將資料存入緩衝區
                g_delay_position.Add(transform.position, buffer_index);
                g_delay_velocity.Add(g_kinematic.velocity, buffer_index);
                buffer_index++;
                if(buffer_index < 0)buffer_index = buffer_size;
            }
            if(IsServer){
                if(g_start_sync){ // 客戶端主動上傳資料
                    g_kinematic.velocity = _syncVelocity;
                    Vector3 new_scale = transform.localScale;
                    new_scale.x = _syncScaleX;
                    transform.localScale = new_scale;
                    g_kinematic.now_gravity = _syncGravity;
                    g_start_sync = false;
                    tick_difference = 0;
                }
                // 傳送資料給主機端
                UploadOwnerTransformClientRpc(transform.position, g_kinematic.velocity, transform.localScale.x, g_kinematic.now_gravity, tick_difference);
            }
        }
    }

    public void StartSync()
    {
        if (IsLocalPlayer) // 本地玩家上傳速度
        {
            if(IsServer){ // 本地玩家為主機端的情況
                _syncVelocity = g_kinematic.velocity;
                _syncScaleX = transform.localScale.x;
                _syncGravity = g_kinematic.now_gravity;
            }
            else{
                tick_difference = 0; // 重製延遲計時
                // 上傳資料給伺服端
                UploadTransformServerRpc(g_kinematic.velocity, transform.localScale.x, g_kinematic.now_gravity);
            }
        }
    }

    public int delay_tick = 10;
    public int last_index = 0;
    public int error_counts = 0;
    private void SyncTransform(){ // 客戶端同步資料
        Debug.Log(Vector3.Distance(_syncPosition, g_delay_position.Get(buffer_index-delay_tick)));
        if(delay_tick >= 0){
            if(IsOwner && tick_difference > 40 && buffer_index - delay_tick >= 0 && Vector3.Distance(_syncPosition, g_delay_position.Get(buffer_index-delay_tick)) >= 0.1f){ // 如果自己是這個物件的擁有者且位置資料相差超過0.05代表客戶端資料有誤
                error_counts += 1;
                if(error_counts > 3){ // 連續發生4次錯誤就同步資料
                    if(last_index != buffer_index - delay_tick){
                        tick_difference -= delay_tick; 
                        transform.position = _syncPosition;
                        g_kinematic.velocity = _syncVelocity;
                        g_kinematic.now_gravity = _syncGravity;
                    }
                }
                last_index = buffer_index - delay_tick; // 計算上次的資料引索值(避免比較同一筆資料) 
            }
            else if(IsOwner){
                error_counts = 0;
            }
        }
        
        if(!IsOwner && buffer_index - delay_tick >= 0 && (Vector3.Distance(_syncPosition, g_delay_position.Get(buffer_index-delay_tick)) >= 0.1f || Vector3.Distance(_syncVelocity, g_delay_velocity.Get(buffer_index-delay_tick)) >= 0.1f )){// 如果自己不是擁有者(其他客戶端的玩家的資料)，位置或速度資料相差超過0.1就同步資料
            transform.position = _syncPosition;
            g_kinematic.velocity = _syncVelocity;
            g_kinematic.now_gravity = _syncGravity;
        }
        // 較不重要的資料直接同步
        Vector3 new_scale = transform.localScale;
        new_scale.x = _syncScaleX;
        transform.localScale = new_scale;
    }

    public void ResetTickDifferent(){
        tick_difference = 0;
        ResetTickDifferentServerRpc();
    }

    [ClientRpc]
    private void UploadOwnerTransformClientRpc(Vector3 position, Vector2 velocity, float scale_x, float gravity , int tick_difference){ // 上傳資料至客戶端 
        _syncPosition = position;
        _syncScaleX = scale_x;
        _syncVelocity = velocity;
        _syncGravity = gravity;
        delay_tick = this.tick_difference - tick_difference;
        SyncTransform();
    }

    [ServerRpc]
    private void UploadTransformServerRpc(Vector2 velocity, float scale_x, float gravity){ // 上傳資料至主機端 
        _syncVelocity = velocity;
        _syncScaleX = scale_x;
        _syncGravity = gravity;
        g_start_sync = true;
    }
    [ServerRpc]
    private void ResetTickDifferentServerRpc(){
        tick_difference = 0;
    }
}