using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class UDPClient : MonoBehaviour
{
    private UdpClient client;
    private IPEndPoint serverEndPoint;
    private byte[] buffer = new byte[4096];

    private void Start()
    {
        StartClient();
    }

    private void StartClient()
    {
        try
        {
            client = new UdpClient();
            serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888);
        }
        catch (Exception e)
        {
            Debug.LogError("Error starting client: " + e.Message);
        }
    }

    private void Update()
    {
        // Simulate sending a message on a key press
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SendData("This is a hello from your client!");
        }
    }

    private void SendData(string message)
    {
        try
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            client.Send(data, data.Length, serverEndPoint);
        }
        catch (Exception e)
        {
            Debug.LogError("Error sending data: " + e.Message);
        }
    }

    private void OnApplicationQuit()
    {
        client.Close();
    }
}
