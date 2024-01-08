using Unity.Netcode;
using UnityEngine;

public class ObjectSync : NetworkBehaviour
{
    private NetworkVariable<Vector3> _syncPos = new();
    private NetworkVariable<Quaternion> _syncRota = new();
    private NetworkVariable<Vector3> _syncScale = new();
    private NetworkVariable<Vector2> _syncVelocity = new();

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

    public void SyncTransform()
    {
        if(_syncVelocity.Value == Vector2.zero)transform.position = _syncPos.Value;
        transform.rotation = _syncRota.Value;
        transform.localScale = _syncScale.Value;
        g_kinematic.velocity = _syncVelocity.Value;
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
    }
}