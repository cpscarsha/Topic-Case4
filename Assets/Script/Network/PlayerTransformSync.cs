using Unity.Netcode;
using UnityEngine;

public class PlayerTransformSync : NetworkBehaviour
{
    // private NetworkVariable<float> _syncScaleX = new();
    // private NetworkVariable<Vector2> _syncVelocity = new();
    // private NetworkVariable<bool> _startSync = new();
    private float _syncScaleX;
    private Vector2 _syncVelocity;
    private Vector3 _syncPosition;
    
    private Kinematic g_kinematic;
    private NetworkTimer g_timer;
    private Vector3 g_delay_position;
    private Vector3 g_delay_velocity;

    public bool g_start_sync = false;
    private void Start(){
        g_timer = gameObject.AddComponent<NetworkTimer>();
        g_kinematic = GetComponent<Kinematic>();
    }

    // private NetworkTimers g_timer(60);
    // private Queue<Vector3> g_position_queue;
    private void Update(){
        if(!IsServer){
            SyncTransform();
        }
        // if(IsLocalPlayer && !IsServer){
        //     SyncLocalTransformClientRpc(g_kinematic.velocity, transform.position, transform.localScale.x);
        // }
    }
    private void FixedUpdate()
    {
        if(!IsServer){
            if(g_timer.ShouldTick()){
                g_delay_position = transform.position;
                g_delay_velocity = g_kinematic.velocity;
            }
        }
        if(IsServer){
            if(g_start_sync){ // 主機取得玩家速度
                g_kinematic.velocity = _syncVelocity;
                Vector3 new_scale = transform.localScale;
                new_scale.x = _syncScaleX;
                transform.localScale = new_scale;
                g_start_sync = false;
            }
            UploadOwnerTransformClientRpc(transform.position, g_kinematic.velocity, transform.localScale.x);
        }
    }

    public void StartSync()
    {
        if (IsLocalPlayer) // 本地玩家上傳速度
        {
            if(IsServer){
                _syncVelocity = g_kinematic.velocity;
                _syncScaleX = transform.localScale.x;
            }
            else{
                UploadTransformServerRpc(g_kinematic.velocity, transform.localScale.x);
            }
        }
    }

    private void SyncTransform(){
        if(Vector3.Distance(_syncPosition, g_delay_position) >= 0.5f || Vector3.Distance(_syncVelocity, g_delay_velocity) >= 0.5f){
            transform.position = _syncPosition;
            g_kinematic.velocity = _syncVelocity;
            Debug.Log(_syncPosition + "/" + g_delay_position);
        }
        Vector3 new_scale = transform.localScale;
        new_scale.x = _syncScaleX;
        transform.localScale = new_scale;
    }

    [ClientRpc]
    private void UploadOwnerTransformClientRpc(Vector3 position, Vector2 velocity, float scale_x){ // 上傳資料至客戶端 
        _syncPosition = position;
        _syncScaleX = scale_x;
        _syncVelocity = velocity;
    }

    [ServerRpc]
    private void UploadTransformServerRpc(Vector2 velocity, float scale_x)
    {
        _syncVelocity = velocity;
        _syncScaleX = scale_x;
        g_start_sync = true;
    }
}