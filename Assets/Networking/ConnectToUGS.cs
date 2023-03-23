using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine.Events;
using System;
public class ConnectToUGS : MonoBehaviour
{
    [SerializeField] private UnityEvent UOnConnected;
    private Action OnConnected;
    private async void Start()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        OnConnected?.Invoke();
        UOnConnected?.Invoke();

        Debug.Log($"Connected to UGS successfully with user id: {AuthenticationService.Instance.PlayerId}");
    }
}
