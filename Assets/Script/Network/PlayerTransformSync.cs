using Unity.Netcode;
using UnityEngine;

public class PlayerTransformSync : NetworkBehaviour
{
    // private NetworkVariable<float> _syncScaleX = new();
    // private NetworkVariable<Vector2> _syncVelocity = new();
    // private NetworkVariable<bool> _startSync = new();
    private float _syncScaleX;
    private Vector2 _syncVelocity;
    
    private Kinematic g_kinematic;
    public bool g_start_sync = false;
    private void Start(){
        g_kinematic = GetComponent<Kinematic>();
    }

    // private NetworkTimers g_timer(60);
    // private Queue<Vector3> g_position_queue;
    private void Update(){
        if(IsLocalPlayer){
            SyncOwnerTransformClientRpc(transform.position, transform.localScale.x);
            
        }
        if(IsOwner && !IsServer){
            SyncLocalTransformClientRpc(g_kinematic.velocity, transform.position, transform.localScale.x);
        }
    }
    private void FixedUpdate()
    {
        // if(IsLocalPlayer){
        //     if(g_timer.ShouldTick()){
        //         g_position_queue.Enqueue(PlayerTransformSync.position);
        //     }
        // }
        if(IsServer){
            if(g_start_sync){ // 主機取得玩家速度
                g_kinematic.velocity = _syncVelocity;
                Vector3 new_scale = transform.localScale;
                new_scale.x = _syncScaleX;
                transform.localScale = new_scale;
                g_start_sync = false;
            }
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

    [ClientRpc]
    private void SyncOwnerTransformClientRpc(Vector3 position, float scale_x){
        if(Vector3.Distance(position, transform.position) >= 0.05f)transform.position = position;
        Debug.Log(position + " / " +　transform.position);
        Vector3 new_scale = transform.localScale;
        new_scale.x = scale_x;
        transform.localScale = new_scale;
    }
    [ClientRpc]
    private void SyncLocalTransformClientRpc(Vector2 velocity, Vector3 position, float scale_x){
        g_kinematic.velocity = velocity;
        if(Vector3.Distance(position, transform.position) >= 0.05f){
            transform.position = position;
            
        }
        // Debug.Log(position + " / " +　transform.position);
        Vector3 new_scale = transform.localScale;
        new_scale.x = scale_x;
        transform.localScale = new_scale;
    }

    [ServerRpc]
    private void UploadTransformServerRpc(Vector2 velocity, float scale_x)
    {
        _syncVelocity = velocity;
        _syncScaleX = scale_x;
        g_start_sync = true;
    }
}