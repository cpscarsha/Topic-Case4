using Unity.Netcode;
using UnityEngine;

public class ObjectSync : NetworkBehaviour
{
    private Vector2 _syncVelocity;
    private Vector3 _syncPosition;
    
    private Kinematic g_kinematic;
    private NetworkTimer g_timer;
    private Vector3 g_delay_position;
    private Vector3 g_delay_velocity;

    public bool g_start_sync = false;
    private void Start(){
        g_timer = new NetworkTimer(30);
        g_kinematic = GetComponent<Kinematic>();
    }

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
                g_delay_position = transform.position;
                g_delay_velocity = g_kinematic.velocity;
            }
        }
        else{
            UploadOwnerTransformClientRpc(transform.position, g_kinematic.velocity);
        }
    }

    private void SyncTransform(){
        if(Vector3.Distance(_syncPosition, g_delay_position) >= 0.5f || Vector3.Distance(_syncVelocity, g_delay_velocity) >= 0.5f){
            transform.position = _syncPosition;
            g_kinematic.velocity = _syncVelocity;
        }
        Vector3 new_scale = transform.localScale;
        transform.localScale = new_scale;
    }

    [ClientRpc]
    private void UploadOwnerTransformClientRpc(Vector3 position, Vector2 velocity){ // 上傳資料至客戶端 
        _syncPosition = position;
        _syncVelocity = velocity;
    }
}