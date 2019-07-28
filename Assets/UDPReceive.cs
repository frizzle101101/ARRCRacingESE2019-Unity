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

using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPReceive : MonoBehaviour
{
    private Thread receiveThread;

    private UdpClient client;

    private static string IP = "192.168.1.124";
    private static int port = 8049;

    private static float xpos = 0;
    private static float ypos = 0;
    private static float ori = 0;

    private static float fieldX = 254.3f;
    private static float fieldY = 278.286f;
    private static float scale = 0.275f;
    private static float fieldBorderThickness = 0.001f;

    private static float cubePosX = fieldX / 2;
    private static float cubePosY = fieldY / 2;

    public float cameraHeight = 12.23f;    
    public float borderWidth = 10;
    public float cameraXAngleAdjust = 6.92f;
    public float cameraZAngleAdjust = 1.62f;
    public float cubeSize = 23.49f;
    public bool freeCamera = false;
    public float gamePlayTime = 30;

    private static bool ackFlag = false;

    private static bool gameStarted = false;
    private static float gameStartTime = 0;
    private static float timeLeftInGame = 0;

    private static string lastReceivedUDPPacket = "";

    private static int score = 0;

    private static GameObject maincamera;
    private static GameObject field;
    private static GameObject fieldXaxis;
    private static GameObject fieldYaxis;
    private static GameObject fieldXplus;
    private static GameObject fieldYplus;
    private static GameObject postmax;
    private static GameObject cubeObj;


    private void Start()
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
        cubeObj = GameObject.FindGameObjectWithTag("CubeTest");
    }

    private void Update()
    {
        if (freeCamera == false)
        {
            maincamera.transform.position = new Vector3(scale * xpos, scale * cameraHeight, scale * ypos);
            maincamera.transform.localEulerAngles = new Vector3(cameraXAngleAdjust, ori, cameraZAngleAdjust);
        }
        cubeObj.transform.position = new Vector3(scale * cubePosX, scale * (cubeSize / 2), scale * cubePosY);
        cubeObj.transform.localScale = new Vector3(scale * cubeSize , scale * cubeSize , scale * cubeSize );
        fieldXaxis.transform.localScale = new Vector3(scale * fieldX, scale * fieldBorderThickness, scale * borderWidth);
        fieldYaxis.transform.localScale = new Vector3(scale * borderWidth, scale * fieldBorderThickness, scale * fieldY);
        fieldXplus.transform.localScale = new Vector3(scale * fieldX, scale * fieldBorderThickness, scale * borderWidth);
        fieldYplus.transform.localScale = new Vector3(scale * borderWidth, scale * fieldBorderThickness, scale * fieldY);
            
        fieldXaxis.transform.localPosition = new Vector3(scale * (fieldX / 2), 0, scale * (-borderWidth / 2));
        fieldYaxis.transform.localPosition = new Vector3(scale * (-borderWidth / 2), 0, scale * (fieldY / 2));
        fieldXplus.transform.localPosition = new Vector3(scale * (fieldX / 2), 0, scale * (fieldY + borderWidth / 2));
        fieldYplus.transform.localPosition = new Vector3(scale * (fieldX + borderWidth / 2), 0, scale * (fieldY / 2));

        postmax.transform.localPosition = new Vector3(scale * fieldX, scale * 0.25f, scale * fieldY);

        if(gameStarted)
        {
            timeLeftInGame = (gamePlayTime - (Time.time - gameStartTime));
            if (timeLeftInGame <= 0)
                gameStarted = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameStarted)
        {
            score++;
            Random.InitState((int)Time.time);
            do
            {
                cubePosX = Random.Range((cubeSize / 2), fieldX - (cubeSize / 2));
            } while (!((maincamera.transform.position.x / scale) + (fieldX / 3) < cubePosX) && !((maincamera.transform.position.x / scale) - (fieldX / 3) > cubePosX));
            do
            {
                cubePosY = Random.Range((cubeSize / 2), fieldY - (cubeSize / 2));
            } while (!((maincamera.transform.position.y / scale) + (fieldY / 3) < cubePosY) && !((maincamera.transform.position.y / scale) - (fieldY / 3) > cubePosY));
        }
    }

    private void OnGUI()
    {
        Rect startStopButton = new Rect(40, 10, 50, 20);
        Rect timeCounter = new Rect(100, 5, 50, 20);
        Rect scoreBox = new Rect(480, 5, 50, 20);
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = 27;
        if (gameStarted)
        {
            
            if (GUI.Button(startStopButton, "Stop"))
            {
                gameStarted = false;
            }
            GUI.Box(timeCounter, timeLeftInGame.ToString("0.0 s"), style);
        }
        else
        {
            cubeObj.SetActive(false);
            if (GUI.Button(startStopButton, "Start"))
            {
                gameStarted = true;
                gameStartTime = Time.time;
                score = 0;
                cubeObj.SetActive(true);
                cubePosX = fieldX / 2;
                cubePosY = fieldY / 2;
            }
            GUI.Box(timeCounter, gamePlayTime.ToString("0.0 s"), style);
        }

        GUI.Box(scoreBox, score.ToString("Score: 0"), style);








    }

    private void OnApplicationQuit()
    {
        //SendUDP("SHUTDOWN");
        //ReceiveACK();
        print("Application ending sending SHUTDOWN, disconecting...");
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
                GetData(text);
                print(">> " + text);
                lastReceivedUDPPacket = text;
            }
            catch (System.Exception err)
            {
                print(err.ToString());
            }
        } while (true);
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
