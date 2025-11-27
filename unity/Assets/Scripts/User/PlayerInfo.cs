using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerInfo : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI playerName;

    private NetworkVariable<FixedString32Bytes> networkUsername = new NetworkVariable<FixedString32Bytes>(
        "", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            var username = LocalUserStorage.LoadUser().username;
            networkUsername.Value = username;
        }

        networkUsername.OnValueChanged += OnUsernameChanged;
        OnUsernameChanged("", networkUsername.Value);
    }

    private void OnUsernameChanged(FixedString32Bytes oldValue, FixedString32Bytes newValue)
    {
        playerName.text = newValue.ToString();
    }
}
