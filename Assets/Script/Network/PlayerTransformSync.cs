using System.ComponentModel;
using System.Threading;
using Unity.Netcode;
using UnityEngine;

public class PlayerTransformSync : NetworkBehaviour
{
    // private NetworkVariable<float> _syncScaleX = new();
    // private NetworkVariable<Vector2> _syncVelocity = new();
    // private NetworkVariable<bool> _startSync = new();
    private float _syncScaleX;
    private float _syncGravity;
    private Vector2 _syncVelocity;
    private Vector3 _syncPosition;
    
    private Kinematic g_kinematic;
    private NetworkTimer g_timer;

    private static readonly int buffer_size = 1000;
    private Buffer<Vector3> g_delay_position = new(buffer_size);
    private Buffer<Vector3> g_delay_velocity = new(buffer_size);
    // private Buffer<float> g_delay_gravity = new(1024);
    private int buffer_index = 0;

    public bool g_start_sync = false;
    private void Start(){
        g_timer = new NetworkTimer(60);
        g_kinematic = GetComponent<Kinematic>();
    }

    // private NetworkTimers g_timer(60);
    // private Queue<Vector3> g_position_queue;
    private void Update(){
        g_timer.Update(Time.deltaTime);
        if(!IsServer){
            SyncTransform();
        }
    }
    private void FixedUpdate()
    {
        if(!IsServer){
            if(g_timer.ShouldTick()){
                g_delay_position.Add(transform.position, buffer_index);
                g_delay_velocity.Add(g_kinematic.velocity, buffer_index);
                // g_delay_gravity.Add(g_kinematic.now_gravity, buffer_index);
                buffer_index++;
                if(buffer_index < 0)buffer_index = buffer_size;
            }
        }
        if(IsServer){
            if(g_start_sync){ // 主機取得玩家速度
                g_kinematic.velocity = _syncVelocity;
                Vector3 new_scale = transform.localScale;
                new_scale.x = _syncScaleX;
                transform.localScale = new_scale;
                g_kinematic.now_gravity = _syncGravity;
                g_start_sync = false;
                g_delay_position.Clear();
                g_delay_velocity.Clear();
                buffer_index = 0;
            }
            UploadOwnerTransformClientRpc(transform.position, g_kinematic.velocity, transform.localScale.x, g_kinematic.now_gravity);
        }
    }

    public void StartSync()
    {
        if (IsLocalPlayer) // 本地玩家上傳速度
        {
            if(IsServer){
                _syncVelocity = g_kinematic.velocity;
                _syncScaleX = transform.localScale.x;
                _syncGravity = g_kinematic.now_gravity;
            }
            else{
                UploadTransformServerRpc(g_kinematic.velocity, transform.localScale.x, g_kinematic.now_gravity);
            }
        }
    }

    public int delay_tick = 10;
    private void SyncTransform(){
        // if(IsOwner)Debug.Log(buffer_index - delay_tick + "/" + _syncPosition + "/" + g_delay_position.Get(buffer_index-delay_tick));
        if(IsOwner && buffer_index - delay_tick >= 0 && Vector3.Distance(_syncPosition, g_delay_position.Get(buffer_index-delay_tick)) >= 0.5f){
            transform.position = _syncPosition;
            g_kinematic.velocity = _syncVelocity;
            g_kinematic.now_gravity = _syncGravity;
        }
        if(!IsOwner && buffer_index - delay_tick >= 0 && (Vector3.Distance(_syncPosition, g_delay_position.Get(buffer_index-delay_tick)) >= 0.5f || Vector3.Distance(_syncVelocity, g_delay_velocity.Get(buffer_index-delay_tick)) >= 0.5f )){
            transform.position = _syncPosition;
            g_kinematic.velocity = _syncVelocity;
            g_kinematic.now_gravity = _syncGravity;
        }
        Vector3 new_scale = transform.localScale;
        new_scale.x = _syncScaleX;
        transform.localScale = new_scale;
        
    }

    [ClientRpc]
    private void UploadOwnerTransformClientRpc(Vector3 position, Vector2 velocity, float scale_x, float gravity){ // 上傳資料至客戶端 
        _syncPosition = position;
        _syncScaleX = scale_x;
        _syncVelocity = velocity;
        _syncGravity = gravity;
    }

    [ServerRpc]
    private void UploadTransformServerRpc(Vector2 velocity, float scale_x, float gravity)
    {
        _syncVelocity = velocity;
        _syncScaleX = scale_x;
        _syncGravity = gravity;
        g_start_sync = true;
    }
}