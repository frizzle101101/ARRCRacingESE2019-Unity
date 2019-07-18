/*
 
    -----------------------
    UDP-Receive (send to)
    -----------------------
    // [url]http://msdn.microsoft.com/de-de/library/bb979228.aspx#ID0E3BAC[/url]
   
   
    // > receive
    // 127.0.0.1 : 8051
   
    // send
    // nc -u 127.0.0.1 8051
 
*/
using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPReceive : MonoBehaviour
{
    Thread receiveThread;

    UdpClient client;

    private string IP = "192.168.2.10";
    private int port = 8051;

    private float xpos = 0;
    private float ypos = 0;
    private float ori = 0;

    private float fieldX = 10;
    private float fieldY = 10;

    private string lastReceivedUDPPacket = "";


    private static void Main()
    {
        UDPReceive receiveObj = new UDPReceive();
        receiveObj.UDPInitilize();

        string text = "";
        do
        {
            text = Console.ReadLine();
        }
        while (!text.Equals("EXIT"));
    }
    public void Start()
    {
        UDPInitilize();
        print("UDPInitilize() complete");
    }

    private void Update()
    {
        GameObject maincamera = GameObject.FindGameObjectWithTag("MainCamera");
        maincamera.transform.position = new Vector3(xpos, 0, ypos);
        maincamera.transform.rotation = new Quaternion(0, ori, 0, 0);
    }

    void OnGUI()
    {
        Rect rectObj = new Rect(40, 10, 200, 400);
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = 50;
        GUI.Box(rectObj, "# UDPReceive\n"+ IP +" " + port + " \n" + "\nLast Packet: \n" + lastReceivedUDPPacket, style);
    }

    private void UDPInitilize()
    {
        print("UDPReceive.init()");

        port = 8051;

        print("Receiving " + IP + port);

        receiveThread = new Thread(new ThreadStart(ReceiveUDP));
        receiveThread.IsBackground = true;
        receiveThread.Start();

    }

    private void ReceiveUDP()
    {
        client = new UdpClient(port);
        IPEndPoint anyIP = new IPEndPoint(IPAddress.Parse(IP), port);
        bool initilized = false;
        string text = "";
        do
        {
            try
            {
                
                byte[] data = client.Receive(ref anyIP);

                text = Encoding.ASCII.GetString(data);

                if(initilized)
                {
                    GetData(text);
                }
                else
                {
                    initilized = GetInit(text);
                }

                print(">> " + text);

                lastReceivedUDPPacket = text;
            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        } while (text != "EXIT");
    }

    private bool GetInit(string text)
    {
        string[] textsplit = text.Split(':', ' ');
        if (textsplit[0] == "INIT")
        {
            fieldX = Int32.Parse(textsplit[2]) / (float)10.0;
            fieldY = Int32.Parse(textsplit[4]) / (float)10.0;

            return true;
        }
        return false;
    }

    private void GetData(string text)
    {
        string[] textsplit = text.Split(':', ' ');
        if (textsplit[0] == "DATA")
        {
            xpos = Int32.Parse(textsplit[1]) / (float)10.0;
            ypos = Int32.Parse(textsplit[3]) / (float)10.0;
            ori = Int32.Parse(textsplit[5]) / (float)10.0;
        }
        else if (textsplit[0] == "INIT")
        {
            GetInit(text);
        }
    }
}
