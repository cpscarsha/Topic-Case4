using Unity.Netcode;
using UnityEngine;

public class ObjectSync : NetworkBehaviour
{
    private NetworkVariable<Vector3> _syncPos = new();
    private NetworkVariable<Quaternion> _syncRota = new();
    private NetworkVariable<Vector3> _syncScale = new();
    private NetworkVariable<Vector2> _syncVelocity = new();

    public bool g_sync_position = false;
    private Kinematic g_kinematic;
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
        
    }

    public void SyncTransform()
    {
<<<<<<< Updated upstream:Assets/Script/PlayerTransformSync.cs
        transform.position = _syncPos.Value;
=======
        if(g_sync_position || _syncVelocity.Value == Vector2.zero || Vector3.Distance(transform.position, _syncPos.Value) > 0.2f){
            Debug.Log("sync pos");
            transform.position = _syncPos.Value;
            g_sync_position = false;
        }
        transform.rotation = _syncRota.Value;
>>>>>>> Stashed changes:Assets/Script/Network/ObjectSync.cs
        transform.localScale = _syncScale.Value;
        
        // if(g_sync_position || _syncVelocity.Value == Vector2.zero){
        //     transform.position = _syncPos.Value;
        //     g_sync_position = false;
        // }
        // transform.rotation = _syncRota.Value;
        // transform.localScale = _syncScale.Value;
        // g_kinematic.velocity = _syncVelocity.Value;
    }

    public void UploadTransform()
    {
        if (IsServer)
        {
            _syncPos.Value = transform.position;
            _syncRota.Value = transform.rotation;
            _syncScale.Value = transform.localScale;
            _syncVelocity.Value = g_kinematic.velocity;
        }
<<<<<<< Updated upstream:Assets/Script/PlayerTransformSync.cs
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
=======
    }
>>>>>>> Stashed changes:Assets/Script/Network/ObjectSync.cs
}