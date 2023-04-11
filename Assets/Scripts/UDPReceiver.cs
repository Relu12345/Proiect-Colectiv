using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class UDPReceiver : MonoBehaviour
{
    public static string msg = "";

    string SubString = "06-04-00-00-00-";
    UdpClient udpClient;
    int port = 4000;

    void Start()
    {
        udpClient = new UdpClient(port);
        udpClient.BeginReceive(ReceiveCallback, null);
    }

    void ReceiveCallback(IAsyncResult ar)
    {
        IPEndPoint ip = new IPEndPoint(IPAddress.Any, port);
        byte[] receivedBytes = udpClient.EndReceive(ar, ref ip);

        string msg_bytes = BitConverter.ToString(receivedBytes);
        msg_bytes = msg_bytes.Substring(msg_bytes.Length - 110);
        msg_bytes = msg_bytes.Substring(0, 38);
      
        int startIndex = msg_bytes.IndexOf(SubString);
        msg_bytes = msg_bytes.Substring(startIndex + SubString.Length);

        if (msg_bytes.Equals("02-55-70"))
        {
            Debug.Log("Up");
            msg = "Up";
        }
        if (msg_bytes.Equals("04-44-6F-77-6E"))
        {
            Debug.Log("Down");
            msg = "Down";
        }
        if (msg_bytes.Equals("04-4C-65-66-74"))
        {
            Debug.Log("Left");
            msg = "Left";
        }
        if (msg_bytes.Equals("05-52-69-67-68-74"))
        {
            Debug.Log("Right");
            msg = "Right";
        }
        if (msg_bytes.Equals("06-42-75-74-74-6F-6E"))
        {
            Debug.Log("Button");
            msg = "Button";
        }

        udpClient.BeginReceive(ReceiveCallback, null);
    }

}
