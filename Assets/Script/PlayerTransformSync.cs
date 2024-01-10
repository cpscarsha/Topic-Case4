using Unity.Netcode;
using UnityEngine;

public class PlayerTransformSync : NetworkBehaviour
{
    private NetworkVariable<Vector3> _syncPos = new();
    private NetworkVariable<Quaternion> _syncRota = new();
    private NetworkVariable<Vector3> _syncScale = new();
    private NetworkVariable<Vector2> _syncVelocity = new();

    private Kinematic g_kinematic;
    public bool g_sync_position = false;
    private void Start(){
        g_kinematic = GetComponent<Kinematic>();
    }

    private void Update()
    {
        if (IsServer)
        {
            UploadTransform();
        }
    }

    private void FixedUpdate()
    {
        if (!IsServer)
        {
            SyncTransform();
        }
    }

    public void SyncPositionTrigger(){
        foreach(GameObject i in GameObject.FindGameObjectsWithTag("Player")){
            i.GetComponent<PlayerTransformSync>().g_sync_position = true;
        }
    }

    private void SyncTransform()
    {
        transform.position = _syncPos.Value;
        transform.localScale = _syncScale.Value;
        
        // if(g_sync_position || _syncVelocity.Value == Vector2.zero){
        //     transform.position = _syncPos.Value;
        //     g_sync_position = false;
        // }
        // transform.rotation = _syncRota.Value;
        // transform.localScale = _syncScale.Value;
        // g_kinematic.velocity = _syncVelocity.Value;
    }

    private void UploadTransform()
    {
        if (IsServer)
        {
            _syncPos.Value = transform.position;
            _syncRota.Value = transform.rotation;
            _syncScale.Value = transform.localScale;
            _syncVelocity.Value = g_kinematic.velocity;
        }
        // else
        // {
        //     UploadTransformServerRpc(transform.position, transform.rotation, transform.localScale, g_kinematic.velocity);
        // }
    }

    // [ServerRpc]
    // private void UploadTransformServerRpc(Vector3 position, Quaternion rotation, Vector3 scale, Vector2 velocity)
    // {
    //     // if(Vector3.Distance(_syncPos.Value, position) > 0.16f || _syncVelocity.Value == Vector2.zero)
    //     _syncPos.Value = position;
    //     _syncRota.Value = rotation;
    //     _syncScale.Value = scale;
    //     _syncVelocity.Value = velocity;
    // }
}