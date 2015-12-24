using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SocketIO;
using Assets.Scripts;
using System.Collections.Generic;
using System;
using System.Text;

public class HomeManager : MonoBehaviour
{
    SocketIOComponent socket;
    Ultis ultis;

    public GameObject goGroupButtonController;
    public GameObject goPlTrain;
    public GameObject goPlHome;

    public GameObject goLogo;
    public GameObject goRecord;
    public GameObject goListRoom;

    public GameObject goPlRooms;
    public GameObject goPlRoom;
    public GameObject goMoneyMoldy;
    public GameObject goLoading;

    public GameObject goRecordConatainer;
    public GameObject goPlayer1;
    public GameObject goPlayer2;
    public GameObject goPlayer3;
    public GameObject goPlayerNormal;

    public GameObject goPlListRoom;
    public Text lblMessage;
    //----
    public Animator animInfo;
    public Animator animGuide;
    public Animator animSetting;
    //-----
    public Animator animNotify;
    public Text lblNotify;

    void EnableLoading(bool isEnable)
    {
        goLoading.active = isEnable;
    }

    public void OpenInfoDialog()
    {
        goGroupButtonController.SetActive(false);
        animInfo.enabled = true;
        animInfo.SetBool("isShowInfo", true);
    }

    public void CloseInfoDialog()
    {
        goGroupButtonController.SetActive(true);
        animInfo.SetBool("isShowInfo", false);
    }

    public void OpenGuideDialog()
    {
        goGroupButtonController.SetActive(false);
        animGuide.enabled = true;
        animGuide.SetBool("isShowInfo", true);
    }

    public void CloseGuideDialog()
    {
        goGroupButtonController.SetActive(true);
        animGuide.SetBool("isShowInfo", false);
    }

    public void OpenSettingDialog()
    {
        goGroupButtonController.SetActive(false);
        animSetting.enabled = true;
        animSetting.SetBool("isShowInfo", true);
    }

    public void CloseSettingDialog()
    {
        goGroupButtonController.SetActive(true);
        animSetting.SetBool("isShowInfo", false);
    }

    public void OpenTrain(PlayMode mode = PlayMode.TRAIN)
    {
        goLogo.SetActive(false);
        goPlHome.SetActive(false);
        goPlTrain.SetActive(true);
        Ultis.Mode = mode;
        switch (mode)
        {
            case PlayMode.TRAIN:
                var plHelps = goPlTrain.transform.Find("pl_helps");
                var plMessage = plHelps.FindChild("pl_message").gameObject;
                plMessage.SetActive(false);
                var plHelpChild = plHelps.transform.Find("pl_help_child").gameObject;
                plHelpChild.SetActive(true);
                var plHeplMoneyMoldy = goMoneyMoldy.transform.FindChild("pl_helps").gameObject;
                plHeplMoneyMoldy.SetActive(false);
                break;
            case PlayMode.CHALLENGE:
                break;
            case PlayMode.NONE:
                break;
            default:
                break;
        }
    }

    public void OpenRecord()
    {
        if (goRecordConatainer.transform.childCount == 0)
        {
            GetRecord();
        }
        ChangeLogo();
        goRecord.SetActive(true);
        goPlHome.SetActive(false);
    }

    public void CloseRecord()
    {
        ChangeLogo();
        //goLogo.enabled = true;
        goRecord.SetActive(false);
        goPlHome.SetActive(true);
    }

    public void OpenListRoom()
    {
        EnableLoading(true);
        ChangeLogo();
        //goLogo.enabled = false;
        goPlHome.SetActive(false);
        goListRoom.SetActive(true);
        GetListRoom();
    }

    public void CloseListRoom()
    {
        goLogo.SetActive(true);
        //ChangeLogo();
        //goLogo.enabled = true;
        goListRoom.SetActive(false);
        goPlHome.SetActive(true);
    }

    Sprite spr;
    bool isLogoMain = true;
    public void ChangeLogo()
    {
        if (goLogo.active)
        {
            goLogo.SetActive(false);
        }
        else
        {
            goLogo.SetActive(true);
        }
    }

    // Use this for initialization
    void Start()
    {
        GetSocketIO();
        ultis = new Ultis();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GetSocketIO()
    {
        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();
        socket.On(Command.server_send_listroom, OnReceiveListRoom);
        socket.On(Command.server_send_client_join_room, OnClientJoinRoom);
        socket.On(Command.server_send_record, OnGetRecord);
    }

    private void OnGetRecord(SocketIOEvent obj)
    {
        var data = obj.data;
        var records = data["records"].list;
        if (records.Count > 1)
        {
            var r1 = Instantiate(goPlayer1, new Vector3(0, 0), Quaternion.identity) as GameObject;
            SetRecordsData(0, records[0], r1);
        }
        if (records.Count > 2)
        {
            var r2 = Instantiate(goPlayer2, new Vector3(0, 0), Quaternion.identity) as GameObject;
            SetRecordsData(1, records[1], r2);
        }
        if (records.Count > 3)
        {
            var r3 = Instantiate(goPlayer3, new Vector3(0, 0), Quaternion.identity) as GameObject;
            SetRecordsData(2, records[2], r3);
        }
        if (records.Count > 3)
            for (int i = 3; i < records.Count; i++)
            {
                var recordData = records[i];
                var r = Instantiate(goPlayerNormal, new Vector3(0, 0), Quaternion.identity) as GameObject;
                SetRecordsData(i, recordData, r);
            }
    }

    private void SetRecordsData(int i, JSONObject recordData, GameObject r)
    {
        try
        {
            var pos = r.transform.Find("lbl_pos").gameObject.GetComponent<Text>();
            pos.text = (i + 1).ToString();
            var name = r.transform.Find("lbl_name").gameObject.GetComponent<Text>();
            name.text = ultis.TrimStartEndChar(recordData["username"].ToString());
            var money = r.transform.Find("lbl_money").gameObject.GetComponent<Text>();
            money.text = ultis.TrimStartEndChar(recordData["money"].ToString());
            r.transform.SetParent(goRecordConatainer.transform);
            r.transform.localScale = Vector3.one;
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    private void OnClientJoinRoom(SocketIOEvent obj)
    {
        try
        {
            Debug.Log(obj.data);
            if (bool.Parse(obj.data["status"].ToString()))
            {
                if (Ultis.Username.Equals(obj.data["username_new_client"].ToString()))
                {
                    Ultis.Mode = PlayMode.CHALLENGE;
                    goPlListRoom.SetActive(false);

                    OpenTrain(Ultis.Mode);

                    var trainManager = goMoneyMoldy.GetComponent<TrainManager>() as TrainManager;
                    trainManager.OpenChallenge();

                    trainManager.goPlMaskTrain.SetActive(false);
                    trainManager.EnableAllButtonAnswer(false);

                    trainManager.OpenReadyConfirm();
                    var plHeplMoneyMoldy = goMoneyMoldy.transform.FindChild("pl_helps").gameObject;
                    plHeplMoneyMoldy.SetActive(false);

                    Ultis.RoomIdSelected = obj.data["room_id"].ToString();

                    Ultis.CurrenPlayerInRoom = Convert.ToInt32(obj.data["number_player"]);
                    var mess = obj.data["message"].ToString();
                }
            }
            else
            {
                OpenNotification(obj.data["message"].ToString());
            }
            #region old

            //if (Ultis.CurrenPlayerInRoom == 1)
            //{
            //    ShowMessage(mess + ", waiting for a new player");
            //}
            //else if (Ultis.CurrenPlayerInRoom == 2)
            //{
            //    ShowMessage(mess + ", starting game");
            //} 
            #endregion
        }
        catch (Exception ex)
        {
            print(ex.Message);
        }
    }

    public void OpenNotification(string notify)
    {
        animNotify.enabled = true;
        lblNotify.text = notify;
        animNotify.SetBool("isShowInfo", true);
    }

    private void OnReceiveListRoom(SocketIOEvent obj)
    {
        try
        {
            foreach (Transform child in goPlRooms.transform)
            {
                Destroy(child.gameObject);
            }
            EnableLoading(false);
            var data = obj.data;
            var listRoom = data["listroom"];
            for (int i = 0; i < listRoom.Count; i++)
            {
                var room = listRoom.list[i];
                var x = Instantiate(goPlRoom, new Vector3(0, 0), Quaternion.identity) as GameObject;
                var lblMaster = x.transform.FindChild("lbl_room_master");
                var lblRoomName = x.transform.FindChild("lbl_room_name");
                var btn = x.transform.FindChild("btn_room").GetComponent<Button>();
                btn.onClick.AddListener(delegate() { OnChooseRoom(room["RoomId"].ToString()); });
                lblMaster.GetComponent<Text>().text = room["Master"].ToString();
                lblRoomName.GetComponent<Text>().text = room["Name"].ToString();
                x.transform.SetParent(goPlRooms.transform);
                x.transform.localScale = Vector3.one;
            }
        }
        catch (System.Exception ex)
        {
            print(ex.Message);
        }
    }

    private void OnChooseRoom(string roomId)
    {
        var data = new Dictionary<string, string>();
        data.Add("command", Command.client_choose_room);
        data.Add("roomId", roomId);
        ultis.SendData(socket, Command.client_message, data);
    }

    void GetListRoom()
    {
        var data = new Dictionary<string, string>();
        data.Add("command", Command.client_get_listroom);
        ultis.SendData(socket, Command.client_message, data);
    }

    void ShowMessage(string message)
    {
        lblMessage.text = message;
    }

    void GetRecord()
    {
        var data = new Dictionary<string, string>();
        data.Add("command", Command.client_get_record);
        ultis.SendData(socket, Command.client_message, data);
    }
}

public enum PlayMode
{
    TRAIN, CHALLENGE, NONE
}
