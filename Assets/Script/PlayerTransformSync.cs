using Unity.Netcode;
using UnityEngine;

public class PlayerTransformSync : NetworkBehaviour
{
    private NetworkVariable<Vector3> _syncPos = new();
    private NetworkVariable<Quaternion> _syncRota = new();

    private void Update()
    {
        if (IsLocalPlayer)
        {
            UploadTransform();
        }
    }

    private void FixedUpdate()
    {
        if (!IsLocalPlayer)
        {
            SyncTransform();
        }
    }

    private void SyncTransform()
    {
        transform.position = _syncPos.Value;
        transform.rotation = _syncRota.Value;
    }

    private void UploadTransform()
    {
        if (IsServer)
        {
            _syncPos.Value = transform.position;
            _syncRota.Value = transform.rotation;
        }
        else
        {
            UploadTransformServerRpc(transform.position, transform.rotation);
        }
    }

    [ServerRpc]
    private void UploadTransformServerRpc(Vector3 position, Quaternion rotation)
    {
        _syncPos.Value = position;
        _syncRota.Value = rotation;
    }
}