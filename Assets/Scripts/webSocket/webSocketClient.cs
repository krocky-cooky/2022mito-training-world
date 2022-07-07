using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


using WebSocketSharp;

public class JsonDataFormat
{
    public float torque;
    public float speed;
    public float position;
    public float rotationAngleFromInitialPosition;
}

public class webSocketClient : MonoBehaviour
{
    // Start is called before the first frame update
    private WebSocket _socket;
    private Queue<string> _messageQueue;
    private JsonDataFormat receivedData;
    private string text = "hello";
    private bool changed = false;
    public bool connected = false;

    [SerializeField]
    private string ESP32PrivateIP = "192.168.128.17";
    [SerializeField]
    private Text viwer;
    [SerializeField]
    private GameObject weight;
    

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
        if(changed)
        {
            changed = false;
            Debug.Log(this.weight);
            this.weight.GetComponent<shortTrainingBar>().changeBarStatusFlag(receivedData);
        }
        
        
    }

    private void changeText(string txt)
    {
        //Debug.Log("change start");
        text = txt;
        //Debug.Log("change end");

    }

    private void Connect()
    {
        _socket.OnOpen += (sender,e) => 
        {
            Debug.Log("WebSocket Open");
            this.changeText("WebSocket Open");
            connected = true;
        };

        _socket.OnMessage += (s,e) => 
        {
            //Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            receivedData = JsonUtility.FromJson<JsonDataFormat>(e.Data);
            this.changeText($"torque: {receivedData.torque}\nspeed: {receivedData.speed}\nposition: {receivedData.position}\nangle: {receivedData.rotationAngleFromInitialPosition}");
            changed = true;
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
