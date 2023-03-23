using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Threading.Tasks;
using System;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;

public static class RelayManager
{
    /// <summary>
    /// HostGame allocates a Relay server and returns needed data to host the game
    /// </summary>
    /// <param name="maxConn">The maximum number of peer connections the host will allow</param>
    /// <returns>A Task returning the needed hosting data</returns>
    public static async Task<RelayHostData> HostGame(int maxConn)
    {

        //Ask Unity Services to allocate a Relay server
        Allocation allocation = await Unity.Services.Relay.RelayService.Instance.CreateAllocationAsync(maxConn);

        //Populate the hosting data
        RelayHostData data = new RelayHostData
        {
            // WARNING allocation.RelayServer is deprecated
            IPv4Address = allocation.RelayServer.IpV4,
            Port = (ushort)allocation.RelayServer.Port,

            AllocationID = allocation.AllocationId,
            AllocationIDBytes = allocation.AllocationIdBytes,
            ConnectionData = allocation.ConnectionData,
            Key = allocation.Key,
        };

        //Retrieve the Relay join code for our clients to join our party
        data.JoinCode = await Unity.Services.Relay.RelayService.Instance.GetJoinCodeAsync(data.AllocationID);

        return data;
    }
    /// <summary>
    /// Join a Relay server based on the JoinCode received from the Host or Server
    /// </summary>
    /// <param name="joinCode">The join code generated on the host or server</param>
    /// <returns>All the necessary data to connect</returns>
    public static async Task<RelayJoinData> JoinGame(string joinCode)
    {

        //Ask Unity Services for allocation data based on a join code
        JoinAllocation allocation = await Unity.Services.Relay.RelayService.Instance.JoinAllocationAsync(joinCode);

        //Populate the joining data
        RelayJoinData data = new RelayJoinData
        {
            // WARNING allocation.RelayServer is deprecated. It's best to read from ServerEndpoints.
            IPv4Address = allocation.RelayServer.IpV4,
            Port = (ushort)allocation.RelayServer.Port,
            AllocationID = allocation.AllocationId,
            AllocationIDBytes = allocation.AllocationIdBytes,
            ConnectionData = allocation.ConnectionData,
            HostConnectionData = allocation.HostConnectionData,
            Key = allocation.Key,
        };
        return data;
    }

    /// <summary>
    /// RelayHostData represents the necessary information
    /// for a Host to host a game on a Relay
    /// </summary>
    public struct RelayHostData
    {
        public string JoinCode;
        public string IPv4Address;
        public ushort Port;
        public Guid AllocationID;
        public byte[] AllocationIDBytes;
        public byte[] ConnectionData;
        public byte[] Key;
    }

    /// <summary>
    /// RelayHostData represents the necessary information
    /// for a Host to host a game on a Relay
    /// </summary>
    public struct RelayJoinData
    {
        public string IPv4Address;
        public ushort Port;
        public Guid AllocationID;
        public byte[] AllocationIDBytes;
        public byte[] ConnectionData;
        public byte[] HostConnectionData;
        public byte[] Key;
    }
}
