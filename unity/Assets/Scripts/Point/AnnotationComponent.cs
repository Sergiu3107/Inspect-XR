using System;
using Unity.Netcode;
using UnityEngine;

public class AnnotationComponent : NetworkBehaviour
{
    public NetworkVariable<long> id = new NetworkVariable<long>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public void Initialize(long id)
    {
        if (IsServer)
        {
            this.id.Value = id;
        }
        else
        {
            Debug.LogWarning("Initialize called on client. This should only be called on server.");
        }
    }
}