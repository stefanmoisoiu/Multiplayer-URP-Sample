using System;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private int maxClientsExcludingHost = 3;
    [FoldoutGroup("UI")][SerializeField] private TMP_InputField lobbyNameInputField;
    [FoldoutGroup("UI")][SerializeField] private CanvasGroup[] canvasGroupsToDisableOnStart;
    [FoldoutGroup("UI")][SerializeField][Range(0, 1f)] private float canvasGroupAlpha = 0.5f;

    [FoldoutGroup("Events")][SerializeField] private UnityEvent uOnHost, uOnClient, uOnAny;
    public static Action OnHost, OnClient, OnAny;

    public async void CreateLobby()
    {
        HideCanvasGroups();
        try
        {
            RelayManager.RelayHostData relayHostData = await RelayManager.HostGame(maxClientsExcludingHost);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
            relayHostData.IPv4Address,
            relayHostData.Port,
            relayHostData.AllocationIDBytes,
            relayHostData.Key,
            relayHostData.ConnectionData
        );
            Debug.Log($"Hosted game at {relayHostData.IPv4Address}:{relayHostData.Port} with join code:{relayHostData.JoinCode}");
            GUIUtility.systemCopyBuffer = relayHostData.JoinCode;
            Debug.Log($"COPIED JOIN CODE");
            NetworkManager.Singleton.StartHost();

            uOnHost?.Invoke();
            OnHost?.Invoke();

            uOnAny?.Invoke();
            OnAny?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            ShowCanvasGroups();
            return;
        }
    }
    public async void JoinLobby()
    {
        if (string.IsNullOrEmpty(lobbyNameInputField.text)) return;

        HideCanvasGroups();

        try
        {
            RelayManager.RelayJoinData relayJoinData = await RelayManager.JoinGame(lobbyNameInputField.text);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
            relayJoinData.IPv4Address,
            relayJoinData.Port,
            relayJoinData.AllocationIDBytes,
            relayJoinData.Key,
            relayJoinData.ConnectionData,
            relayJoinData.HostConnectionData
        );
            NetworkManager.Singleton.StartClient();

            uOnClient?.Invoke();
            OnClient?.Invoke();

            uOnAny?.Invoke();
            OnAny?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            ShowCanvasGroups();
            return;
        }
    }

    private void HideCanvasGroups()
    {
        foreach (var canvasGroup in canvasGroupsToDisableOnStart)
        {
            canvasGroup.alpha = canvasGroupAlpha;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
    private void ShowCanvasGroups()
    {
        foreach (var canvasGroup in canvasGroupsToDisableOnStart)
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }
}
