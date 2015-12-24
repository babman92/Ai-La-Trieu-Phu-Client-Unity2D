using UnityEngine;
using System.Collections;
using SocketIO;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine.UI;
using System;
using Assets.Scripts.Model;

public class ConnectManager : MonoBehaviour
{
    SocketIOComponent socket;
    Ultis ultis;
    // Use this for initialization
    public GameObject goLoading;
    public GameObject goRegister;
    public GameObject goTrainScript;

    public GameObject goPlHome;
    public GameObject goPlTrain;
    public GameObject goPlMoldy;
    public GameObject goLogo;
    public GameObject goSplash;

    public Animator animRegister;
    public Animator animNotify;
    public Animator animConfirmReconnect;
    public Animator animLoseConnect;
    public Animator animSplash;

    public Text lblNotify;

    public Text nofityText;
    const string fileUserConfig = "userconfig.txt";

    void EnableLoading(bool isEnable)
    {
        goLoading.active = isEnable;
    }

    TrainManager trainManager;
    void Start()
    {
        ultis = new Ultis();
        trainManager = goTrainScript.GetComponent<TrainManager>();
        Ultis.CurrentQuestionId = PlayerPrefs.GetString("CurrentQuestionId", "1");
        Ultis.CurrentMoneyText = PlayerPrefs.GetString("CurrentMoneyText", "0");
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.SetString("CurrentQuestionId", Ultis.CurrentQuestionId);
        PlayerPrefs.SetString("CurrentMoneyText", Ultis.CurrentMoneyText);
        PlayerPrefs.Save();
        socket.Close();
    }

    public void ConnectToServer()
    {
        EnableLoading(true);
        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();
        socket.url = "ws://glacial-ridge-3998.herokuapp.com:80/socket.io/?EIO=3&transport=websocket";

        socket.On("message", TestEvent);
        socket.On("open", OnSocketOpen);
        socket.On("disconnect", OnDisconnect);
        socket.On("error", OnError);
        socket.On("close", OnClose);
        socket.On("connect", OnConnect);

        socket.On(Command.server_send_confirm_signup, OnSignUp);
        socket.On(Command.server_confirm_login, OnLogin);
        socket.On(Command.server_send_reconnect, OnReconnect);
        socket.Connect();
    }

    private void OnReconnect(SocketIOEvent obj)
    {
        print(obj.data);
        var data = obj.data;
        var status = bool.Parse(data["status"].ToString());
        if (status)
        {
            ActiveTrainSreen(obj.data["question"]);
        }
        else
        {
            OpenNotification(data["message"].ToString());
        }
        EnableLoading(false);
    }

    public void OpenNotification(string notify)
    {
        animNotify.enabled = true;
        lblNotify.text = notify;
        animNotify.SetBool("isShowInfo", true);
    }

    public void CloseNotification()
    {
        animNotify.SetBool("isShowInfo", false);
    }

    private void OnLogin(SocketIOEvent obj)
    {
        var data = obj.data;
        bool status = bool.Parse(data["status"].ToString());
        if (status)
        {
            EnableLoading(false);
            PrepareStartGame();
        }
        else
        {
            string username = ultis.GetUsername();
            Login(username, string.Empty);
        }
        EnableLoading(false);
    }

    bool checkConfigFileExist(string fileName)
    {
        if (ultis.ReadFile(fileName).Equals(string.Empty))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void OnSignUp(SocketIOEvent obj)
    {
        var data = obj.data;
        var mess = data["message"].ToString();
        var name = data["username"].ToString();
        nofityText.text = mess;
        ShowNofify();
        if (bool.Parse(data["status"].ToString()))
        {
            PlayerPrefs.SetString("username", name);
            //ultis.WriteFile(fileUserConfig, name);
            animRegister.SetBool("isShowInfo", false);
        }

        PrepareStartGame();
        EnableLoading(false);
    }

    void ShowNofify()
    {
        animNotify.enabled = true;
        animNotify.SetBool("isShowInfo", true);
    }

    private void OnConnect(SocketIOEvent obj)
    {
        ActiveHomeScreen(true);
        isLoseConnect = false;
        animLoseConnect.SetBool("isShowInfo", false);
        EnableLoading(false);
        Ultis.Username = ultis.GetUsername();
        if (Ultis.Username.Equals(string.Empty))
        {
            OpenRegister();
        }
        else
        {
            EnableLoading(true);
            Login(Ultis.Username, string.Empty);
        }
    }

    private void PrepareStartGame()
    {
        if (Ultis.IsLoseConnect)
        {
            OpenConfirmReconnect();
            Ultis.IsLoseConnect = false;
        }
    }

    public void ConfirmReconnect()
    {
        string username = ultis.GetUsername();
        int level = PlayerPrefs.GetInt("level");
        string roomId = Ultis.RoomIdSelected;
        Reconnect(level, username, roomId, Ultis.CurrentQuestionId);
        animConfirmReconnect.SetBool("isShowInfo", false);
    }

    private void OnClose(SocketIOEvent obj)
    {
        isLoseConnect = Ultis.IsLoseConnect = true;
        Debug.Log("Client is closed...");
    }

    private void OnError(SocketIOEvent obj)
    {
        //if (!isLoseConnect)
        //return;
        isLoseConnect = Ultis.IsLoseConnect = true;
        Debug.Log("Connect Error - " + obj.data);
    }

    private void OnDisconnect(SocketIOEvent obj)
    {
        isLoseConnect = Ultis.IsLoseConnect = true;
        Debug.Log("Server disconnect...");
    }

    bool isLoseConnect = false;

    void Update()
    {
        OpenLoseConnect();
    }

    float countTimeHideSplash = 2;
    void FixedUpdate()
    {
        if (animSplash.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animSplash.IsInTransition(0))
        {
            countTimeHideSplash -= Time.deltaTime;
            if (countTimeHideSplash <= 0)
            {
                goSplash.SetActive(false);
                ConnectToServer();
                EnableLoading(false);
            }
        }
    }

    void OpenLoseConnect()
    {
        try
        {
            if (isLoseConnect)
            {
                EnableLoading(false);
                animLoseConnect.enabled = true;
                if (animLoseConnect.GetBool("isShowInfo"))
                    return;
                animLoseConnect.SetBool("isShowInfo", true);
                isLoseConnect = false;
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public void Reconnect()
    {
        ActiveHomeScreen(true);
        EnableLoading(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ActiveHomeScreen(bool active)
    {
        trainManager.ActiveHomeScreen(true, false, Ultis.CurrentLevel);
    }

    void ActiveTrainSreen(JSONObject j)
    {
        trainManager.OpenTrainReconnect(j);
    }

    private void OnSocketOpen(SocketIOEvent obj)
    {
        Debug.Log("Socket opened - " + socket.sid);
    }

    private void TestEvent(SocketIOEvent obj)
    {
        Debug.Log("OnMessage: " + obj.data);
    }

    void OpenRegister()
    {
        goRegister.SetActive(true);
        animRegister.enabled = true;
        animRegister.SetBool("isShowInfo", true);
    }

    public void SubmitName(InputField input)
    {
        EnableLoading(true);
        string name = input.text;
        var data = new Dictionary<string, string>();
        data.Add("command", Command.client_signup_name);
        data.Add("username", name);
        ultis.SendData(socket, Command.client_message, data);
    }

    void Reconnect(int level, string name, string roomId, string questionId)
    {
        EnableLoading(true);
        var data = new Dictionary<string, string>();
        data.Add("command", Command.client_reconnect);
        data.Add("level", level.ToString());
        data.Add("questionId", questionId);
        data.Add("username", name);
        data.Add("roomId", roomId);
        ultis.SendData(socket, data);
    }

    void Login(string name, string pass)
    {
        EnableLoading(true);
        var data = new Dictionary<string, string>();
        data.Add("command", Command.client_login);
        data.Add("username", name);
        data.Add("password", pass);
        ultis.SendData(socket, data);
    }

    public void OpenConfirmReconnect()
    {
        animConfirmReconnect.enabled = true;
        animConfirmReconnect.SetBool("isShowInfo", true);
    }

    public void CloseConfirmReconnect()
    {
        PlayerPrefs.SetInt("level", 1);
        animConfirmReconnect.SetBool("isShowInfo", false);
    }
}
