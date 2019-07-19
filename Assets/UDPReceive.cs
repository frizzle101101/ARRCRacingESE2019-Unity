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

    public string IP = "192.168.2.10";
    private int port = 8051;

    public float xpos = 0;
    public float ypos = 0;
    public float ori = 0;

    public float fieldX = 10;
    public float fieldY = 10;
    public float scale = 1;

    public float cameraHeight = 0.2f;

    private float fieldBorderThickness = 0.001f;
    public float borderWidth = 1;

    private bool initilized = false;

    private string lastReceivedUDPPacket = "";

    private GameObject maincamera;
    private GameObject field;
    private GameObject fieldXaxis;
    private GameObject fieldYaxis;
    private GameObject fieldXplus;
    private GameObject fieldYplus;
    private GameObject postmax;


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

        maincamera = GameObject.FindGameObjectWithTag("MainCamera");
        field = GameObject.FindGameObjectWithTag("Field");
        fieldXaxis = GameObject.FindGameObjectWithTag("FieldXaxis");
        fieldYaxis = GameObject.FindGameObjectWithTag("FieldYaxis");
        fieldXplus = GameObject.FindGameObjectWithTag("FieldXplus");
        fieldYplus = GameObject.FindGameObjectWithTag("FieldYplus");

        postmax = GameObject.FindGameObjectWithTag("LEDPostMax");
    }

    private void Update()
    {
        if (initilized)
        {
            maincamera.transform.position = new Vector3(xpos, cameraHeight, ypos);
            maincamera.transform.rotation = new Quaternion(0, ori, 0, 0);
        }
        else
        {
            field.transform.localScale = new Vector3(scale, scale, scale);
            fieldXaxis.transform.localScale = new Vector3(fieldX, fieldBorderThickness, borderWidth);
            fieldYaxis.transform.localScale = new Vector3(borderWidth, fieldBorderThickness, fieldY);
            fieldXplus.transform.localScale = new Vector3(fieldX, fieldBorderThickness, borderWidth);
            fieldYplus.transform.localScale = new Vector3(borderWidth, fieldBorderThickness, fieldY);

            fieldXaxis.transform.position = new Vector3(scale * (fieldX / 2), 0, scale * (-borderWidth / 2));
            fieldYaxis.transform.position = new Vector3(scale * (-borderWidth / 2), 0, scale * (fieldY / 2));
            fieldXplus.transform.position = new Vector3(scale * (fieldX / 2), 0, scale * (fieldX + borderWidth / 2));
            fieldYplus.transform.position = new Vector3(scale * (fieldY + borderWidth / 2), 0, scale * (fieldY / 2));

            postmax.transform.position = new Vector3(scale * fieldX, 0.25f, scale * fieldY);
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
                    GetInit(text);
                    initilized = true;
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

    private void GetInit(string text)
    {
        //"INIT X:000 Y:000 S:000"
        string[] textsplit = text.Split(':', ' ');
        if (textsplit[0] == "INIT")
        {
            fieldX = Int32.Parse(textsplit[2]);
            fieldY = Int32.Parse(textsplit[4]);
            scale = Int32.Parse(textsplit[6]);
        }
    }

    private void GetData(string text)
    {
        //"DATA X:000 Y:000 O:000"
        string[] textsplit = text.Split(':', ' ');
        if (textsplit[0] == "DATA")
        {
            xpos = Int32.Parse(textsplit[1]);
            ypos = Int32.Parse(textsplit[3]);
            ori = Int32.Parse(textsplit[5]);
        }
        else if (textsplit[0] == "INIT")
        {
            GetInit(text);
        }
    }
}
