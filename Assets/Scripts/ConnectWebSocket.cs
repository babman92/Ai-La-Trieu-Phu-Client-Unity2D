using UnityEngine;
using System.Collections;
using WebSocketSharp;

public class ConnectWebSocket : MonoBehaviour
{
    WebSocket ws;
    // Use this for initialization
    void Start()
    {
        ws = new WebSocket("ws://glacial-ridge-3998.herokuapp.com:80");

        ws.OnOpen += ws_OnOpen;
        ws.OnMessage += ws_OnMessage;
        ws.OnError += ws_OnError;
        ws.OnClose += ws_OnClose;
        ws.Connect();
    }

    void ws_OnClose(object sender, CloseEventArgs e)
    {
        print(e.Reason);
    }

    void ws_OnError(object sender, ErrorEventArgs e)
    {
        print(e.Message);
    }

    void ws_OnMessage(object sender, MessageEventArgs e)
    {
        print(e.Data);
    }

    void ws_OnOpen(object sender, System.EventArgs e)
    {
        print("connected to server");
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnApplicationQuit()
    {
        ws.CloseAsync();
    }
}
