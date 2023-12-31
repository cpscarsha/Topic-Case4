using Unity.Netcode;
using UnityEngine;

public class PlayerTransformSync : NetworkBehaviour
{
    private NetworkVariable<Vector3> _syncPos = new();
    private NetworkVariable<Quaternion> _syncRota = new();
    private NetworkVariable<Vector3> _syncScale = new();


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
        transform.localScale = _syncScale.Value;
    }

    private void UploadTransform()
    {
        if (IsServer)
        {
            _syncPos.Value = transform.position;
            _syncRota.Value = transform.rotation;
            _syncScale.Value = transform.localScale;
        }
        else
        {
            UploadTransformServerRpc(transform.position, transform.rotation, transform.localScale);
        }
    }

    [ServerRpc]
    private void UploadTransformServerRpc(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        _syncPos.Value = position;
        _syncRota.Value = rotation;
        _syncScale.Value = scale;
    }
}