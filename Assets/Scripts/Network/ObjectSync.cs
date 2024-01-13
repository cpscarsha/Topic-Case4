using Unity.Netcode;
using UnityEngine;

public class ObjectSync : NetworkBehaviour
{
    private Vector2 _syncVelocity;
    private Vector3 _syncPosition;
    private float _syncGravity;
    
    private Kinematic g_kinematic;
    private NetworkTimer g_timer;
    private Buffer<Vector3> g_delay_position = new(1024);
    private int buffer_index = 0;

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
                g_delay_position.Add(transform.position, buffer_index);
                // g_delay_velocity = g_kinematic.velocity;
                buffer_index++;
            }
        }
        else{
            UploadOwnerTransformClientRpc(transform.position, g_kinematic.velocity, g_kinematic.now_gravity);
        }
    }
    private int delay_tick = 10;
    private void SyncTransform(){
        if(buffer_index - delay_tick < 0 || Vector3.Distance(_syncPosition, g_delay_position.Get(buffer_index-delay_tick)) >= 0.1f){
            transform.position = _syncPosition;
            g_kinematic.velocity = _syncVelocity;
            g_kinematic.now_gravity = _syncGravity;
            // Debug.Log(transform.position + "/" +g_kinematic.velocity+"/"+g_kinematic.gravity);
        }
    }

    [ClientRpc]
    private void UploadOwnerTransformClientRpc(Vector3 position, Vector2 velocity, float gravity){ // 上傳資料至客戶端 
        _syncPosition = position;
        _syncVelocity = velocity;
        _syncGravity = gravity;
    }
}