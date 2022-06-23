using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


using WebSocketSharp;

public class JsonDataFormat
{
    public float torque;
    public float position;
    public float count;
}

public class webSocketClient : MonoBehaviour
{
    // Start is called before the first frame update
    private WebSocket _socket;
    private Queue<string> _messageQueue;
    private JsonDataFormat receivedData;
    private string text = "hello";
    public bool connected = false;

    [SerializeField]
    private string ESP32PrivateIP = "192.168.1.1";
    [SerializeField]
    private Text viwer;
    

    void Awake()
    {
        _socket = new WebSocket($"ws://{ESP32PrivateIP}/");
    }
    
    void Start()
    {
        Connect();
        viwer.text = text;
        
        
    }

    // Update is called once per frame
    void Update()
    {
        viwer.text = text;
        
        
    }

    private void changeText(string txt)
    {
        Debug.Log("change start");
        text = txt;
        Debug.Log("change end");

    }

    private void Connect()
    {
        _socket.OnOpen += (sender,e) => 
        {
            Debug.Log("WebSocket Open");
            connected = true;
        };

        _socket.OnMessage += (s,e) => 
        {
            Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            receivedData = JsonUtility.FromJson<JsonDataFormat>(e.Data);
            this.changeText($"torque: {receivedData.torque}\nposition: {receivedData.position}\ncount: {receivedData.count}");
        };
        
        _socket.OnClose += (sender, e) =>
        {
            Debug.Log("WebSocket Close");
            connected = false;
        };

        _socket.Connect();
    }

    private void OnDestroy()
    {
        _socket.Close();
        _socket = null;
    }

    public JsonDataFormat getReceivedData()
    {
        if(receivedData == null)return null;
        else return receivedData;
    }
}
