using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnPlayers : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    private void Start()
    {
        if (NetworkManager.IsHost) GetComponent<NetworkObject>().Spawn();
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        SpawnPlayerServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayerServerRpc(ServerRpcParams serverRpcParams = default)
    {
        GameObject playerInstance = Instantiate(playerPrefab, transform.position, Quaternion.identity);
        playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId: serverRpcParams.Receive.SenderClientId);
    }
}
