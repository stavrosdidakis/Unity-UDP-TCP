using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class TCPClient : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;
    private byte[] buffer = new byte[4096];

    private void Start()
    {
        StartClient();
    }

    private void StartClient()
    {
        client = new TcpClient();
        client.BeginConnect("127.0.0.1", 8888, ClientConnected, null);
    }

    private void ClientConnected(IAsyncResult result)
    {
        client.EndConnect(result);
        Debug.Log("Connected to server.");

        stream = client.GetStream();
        stream.BeginRead(buffer, 0, buffer.Length, DataReceived, null);
    }

    private void DataReceived(IAsyncResult result)
    {
        int bytesRead = stream.EndRead(result);
        if (bytesRead <= 0)
        {
            Debug.Log("Disconnected from server.");
            return;
        }

        string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
        Debug.Log("Received from server: " + message);

        stream.BeginRead(buffer, 0, buffer.Length, DataReceived, null);
    }

    public void SendData(string message)
    {
        byte[] data = Encoding.ASCII.GetBytes(message);
        stream.Write(data, 0, data.Length);
    }

    private void OnApplicationQuit()
    {
        client.Close();
    }
}
