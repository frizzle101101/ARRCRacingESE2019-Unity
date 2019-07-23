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
    private Thread receiveThread;

    private UdpClient client;

    private string IP = "192.168.1.124";
    private int port = 8049;

    private float xpos = 0;
    private float ypos = 0;
    private float ori = 0;

    private float fieldX = 254.3f;
    private float fieldY = 278.286f;
    private float scale = 0.275f;

    public float cameraHeight = 12.23f;

    private float fieldBorderThickness = 0.001f;
    public float borderWidth = 10;

    public float cameraXAngleAdjust = 6.92f;
    public float cameraZAngleAdjust = 1.62f;

    public float cubeTest = 65f;


    private bool initilized = false;
    private bool ackFlag = false;

    private string lastReceivedUDPPacket = "";

    private GameObject maincamera;
    private GameObject field;
    private GameObject fieldXaxis;
    private GameObject fieldYaxis;
    private GameObject fieldXplus;
    private GameObject fieldYplus;
    private GameObject postmax;
    private GameObject cubetest;


    public void Start()
    {
        UDPInitilize();
        print("UDPInitilize() complete");

        maincamera = GameObject.FindGameObjectWithTag("MainCamera");
        field = GameObject.FindGameObjectWithTag("Field");
        fieldXaxis = GameObject.FindGameObjectWithTag("FieldXaxis");
        fieldYaxis = GameObject.FindGameObjectWithTag("FieldYaxis");
        fieldXplus = GameObject.FindGameObjectWithTag("FieldXplus");
        fieldYplus = GameObject.FindGameObjectWithTag("FieldYplus");

        postmax = GameObject.FindGameObjectWithTag("LEDPostMax");
        cubetest = GameObject.FindGameObjectWithTag("CubeTest");
    }

    private void Update()
    {
        

        //if (initilized)
        {
            maincamera.transform.position = new Vector3(scale * xpos, scale * cameraHeight, scale * ypos);
            maincamera.transform.localEulerAngles = new Vector3(cameraXAngleAdjust, ori, cameraZAngleAdjust);
            cubetest.transform.position = new Vector3(scale * (fieldX / 2), scale * (cubeTest / 2), scale * (fieldY / 2));
            cubetest.transform.localScale = new Vector3(scale * cubeTest , scale * cubeTest , scale * cubeTest );
        }
        //else
        {
            Vector3 scaleVector = new Vector3(scale, scale, scale);

            //field.transform.localScale = scaleVector;
            fieldXaxis.transform.localScale = new Vector3(scale * fieldX, scale * fieldBorderThickness, scale * borderWidth);
            fieldYaxis.transform.localScale = new Vector3(scale * borderWidth, scale * fieldBorderThickness, scale * fieldY);
            fieldXplus.transform.localScale = new Vector3(scale * fieldX, scale * fieldBorderThickness, scale * borderWidth);
            fieldYplus.transform.localScale = new Vector3(scale * borderWidth, scale * fieldBorderThickness, scale * fieldY);
            
            fieldXaxis.transform.localPosition = new Vector3(scale * (fieldX / 2), 0, scale * (-borderWidth / 2));
            fieldYaxis.transform.localPosition = new Vector3(scale * (-borderWidth / 2), 0, scale * (fieldY / 2));
            fieldXplus.transform.localPosition = new Vector3(scale * (fieldX / 2), 0, scale * (fieldY + borderWidth / 2));
            fieldYplus.transform.localPosition = new Vector3(scale * (fieldX + borderWidth / 2), 0, scale * (fieldY / 2));

            postmax.transform.localPosition = new Vector3(scale * fieldX, scale * 0.25f, scale * fieldY);

            

            initilized = true;
        }

    }

    void OnGUI()
    {
        Rect rectObj = new Rect(40, 10, 200, 400);
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = 50;
        GUI.Box(rectObj, "# UDPReceive\n"+ IP +" " + port + " \n" + "\nLast Packet: \n" + lastReceivedUDPPacket, style);
    }

    void OnApplicationQuit()
    {
        SendUDP("SHUTDOWN");
        //ReceiveACK();
        print("Application ending sending SHUTDOWN, disconecting...");
        //client.Close();
    }

    private void UDPInitilize()
    {
        print("UDPReceive.init()");
        
        print("Receiving " + IP + port);

        receiveThread = new Thread(new ThreadStart(ReceiveUDP));
        receiveThread.IsBackground = true;
        receiveThread.Start();

    }

    private void ReceiveACK()
    {
        while (!ackFlag) { }
        ackFlag = false;
    }

    private void SendUDP(string command)
    {
        byte[] bCommand = Encoding.ASCII.GetBytes(command);
        client.Send(bCommand, bCommand.Length);
    }

    private void ReceiveUDP()
    {
        client = new UdpClient(port);
        IPEndPoint ipendpoint = new IPEndPoint(IPAddress.Parse(IP), port);
        client.Connect(ipendpoint);
        string text = "";
        do
        {
            try
            {

                byte[] data = client.Receive(ref ipendpoint);

                text = Encoding.ASCII.GetString(data);

                print(">> " + text);

                lastReceivedUDPPacket = text;

                GetData(text);

                
            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        } while (text != "EXIT");
    }
   

    private void GetData(string text)
    {
        //"DATA X:000 Y:000 O:000"
        text.Replace('.', ',');
        string[] textsplit = text.Split(':', ' ');
        if (textsplit[0] == "DATA")
        {
            xpos = float.Parse(textsplit[2]);
            ypos = float.Parse(textsplit[4]);
            ori = float.Parse(textsplit[6]);
        }
        else if (textsplit[0] == "INIT")
        {
            fieldX = float.Parse(textsplit[2]);
            fieldY = float.Parse(textsplit[4]);
            scale = float.Parse(textsplit[6])/5.0f;
        }
        else if (textsplit[0] == "SHUTDOWN")
        {
            print("SHUTDOWN Command detected, Sending ACK");
            SendUDP("ACK");
        }
        else if (textsplit[0] == "SHUTDOWN!")
        {
            print("SHUTDOWN! Command detected");
        }
    }
}
