using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class TCPServer : MonoBehaviour
{
    private TcpListener server;
    private List<TcpClient> clients = new List<TcpClient>();
    private List<NetworkStream> clientStreams = new List<NetworkStream>();
    private byte[] buffer = new byte[4096];

    private void Start()
    {
        StartServer();
    }

    private void StartServer()
    {
        server = new TcpListener(IPAddress.Any, 8888);
        server.Start();
        Debug.Log("Server started. Waiting for clients...");

        server.BeginAcceptTcpClient(ClientConnected, null);
    }

    private void ClientConnected(IAsyncResult result)
    {
        TcpClient client = server.EndAcceptTcpClient(result);
        clients.Add(client);

        Debug.Log("Client connected: " + client.Client.RemoteEndPoint);

        NetworkStream stream = client.GetStream();
        clientStreams.Add(stream);

        stream.BeginRead(buffer, 0, buffer.Length, DataReceived, client);
        server.BeginAcceptTcpClient(ClientConnected, null);
    }

    private void DataReceived(IAsyncResult result)
    {
        TcpClient client = (TcpClient)result.AsyncState;
        NetworkStream stream = client.GetStream();

        int bytesRead = stream.EndRead(result);
        if (bytesRead <= 0)
        {
            ClientDisconnected(client);
            return;
        }

        string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
        Debug.Log("Received from " + client.Client.RemoteEndPoint + ": " + message);

        // Broadcast the message to all connected clients, including the sender
        BroadcastMessage(message);

        stream.BeginRead(buffer, 0, buffer.Length, DataReceived, client);
    }

    private void BroadcastMessage(string message)
    {
        byte[] data = Encoding.ASCII.GetBytes(message);
        foreach (var stream in clientStreams)
        {
            stream.Write(data, 0, data.Length);
        }
    }

    private void ClientDisconnected(TcpClient client)
    {
        Debug.Log("Client disconnected: " + client.Client.RemoteEndPoint);
        clientStreams.Remove(client.GetStream());
        clients.Remove(client);
    }

    private void OnApplicationQuit()
    {
        server.Stop();
        foreach (var client in clients)
        {
            client.Close();
        }
    }
}
