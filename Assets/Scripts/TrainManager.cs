using UnityEngine;
using System.Collections;
using SocketIO;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine.UI;
using Assets.Scripts.Model;
using System;

public class TrainManager : MonoBehaviour
{
    #region Variables
    public Animator animCallMate;
    public Animator animGroupSupport;
    public Animator animGuestSupport;
    public Animator animReadyConfirm;

    public Animator animConfirmAnswer;
    public Animator animMoneyMoldy;
    public Animator animConfirmStop;
    public Animator animConfirmWrongAnswer;
    public Animator animNotify;
    public Animator animCongrat;
    public Animator animMateHelp;

    public Animator animCaseAFlip;
    public Animator animCaseBFlip;
    public Animator animCaseCFlip;
    public Animator animCaseDFlip;

    public Animator animTimeout;
    public Animator animFinish;
    public Animator animFinishMatch;

    public GameObject goPlHome;
    public GameObject goLogo;
    public GameObject goPlTrain;
    public GameObject goPlMoneyMoldy;
    public GameObject goLoading;
    public GameObject goPlMates;
    public GameObject goPlListRoom;

    bool isFirstPlay = true;

    SocketIOComponent socket;
    Ultis ultis;
    public int level = 0;
    Question questionCurrent;
    Answer answer;

    Sprite sprBackChoosenMoldy;

    public Text lblQuestion;
    public Text lblCaseA;
    public Text lblCaseB;
    public Text lblCaseC;
    public Text lblCaseD;
    public Text lblQuestionNumber;
    public Text lblNotify;
    public Text lblCaseChoosen;
    public Text lblAnswerMateHelp;
    public Text lblTitleMateHelp;
    public Text lblTimeLeft;
    float timeLeft;
    public Text lblMoney;
    public Text lblMessage;

    public Text lblUsername1;
    public Text lblUsername2;
    public Text lblResult1;
    public Text lblResult2;
    public Text lblMoney1;
    public Text lblMoney2;

    public Text lblFinishUsername1;
    public Text lblFinishUsername2;
    public Text lblFinishTime1;
    public Text lblFinishTime2;
    public Text lblFinishMoney1;
    public Text lblFinishMoney2;

    public Button btnCaseA;
    public Button btnCaseB;
    public Button btnCaseC;
    public Button btnCaseD;

    public GameObject goPlPercents;

    public Button btnGroupSupport;
    public Button btnGuesSupport;
    public Button btnStop;
    public Button btnHalfHelp;
    public Button btnCallMate;

    public GameObject goPlMaskTrain;
    bool isStartCountTime = false;

    float timeShowRightAnswer;
    bool isStartCountTimeShowRightAnswer = false;
    bool resultAnswer = false;

    string[] arrMoney = new string[] {
        "200.000","400.000","600.000","1.000.000","2.000.000",
        "3.000.000","6.000.000","10.000.000","14.000.000","22.000.000",
        "30.000.000","40.000.000","60.000.000","85.000.000","150.000.000"
    };

    string[] arrMoneyReal = new string[] {
        "200000","400000","600000","1000000","2000000",
        "3000000","6000000","10000000","14000000","22000000",
        "30000000","40000000","60000000","85000000","150000000"
    };

    bool checkPlayAudioChooseAnswer;
    bool checkPlayAudioHelpHalf;
    bool checkPlayAudioGuestSupport;

    #endregion

    #region Variables Sound

    AudioSource[] listAudioSource;
    AudioSource audioIntro;
    AudioSource audioStartGame;

    AudioSource audioChooseCaseA;
    AudioSource audioChooseCaseB;
    AudioSource audioChooseCaseC;
    AudioSource audioChooseCaseD;

    AudioSource audioRightCaseA;
    AudioSource audioRightCaseB;
    AudioSource audioRightCaseC;
    AudioSource audioRightCaseD;

    AudioSource audioWrongCaseA;
    AudioSource audioWrongCaseB;
    AudioSource audioWrongCaseC;
    AudioSource audioWrongCaseD;

    AudioSource audioTimeout;
    AudioSource audioThankyou;
    AudioSource audioWaitAnswer;

    AudioSource audioHelpHalf;
    AudioSource audioCallMate;
    AudioSource audioGuestSupport;
    #endregion

    void LoadAudio()
    {
        //----------question------------------
        listAudioSource = new AudioSource[15];
        for (int i = 0; i < 15; i++)
        {
            listAudioSource[i] = gameObject.AddComponent<AudioSource>();
            listAudioSource[i].clip = Resources.Load(string.Format("Sounds/q{0}", (i + 1))) as AudioClip;
        }

        audioIntro = LoadAudio(audioIntro, "Sounds/Rule");
        audioStartGame = LoadAudio(audioStartGame, "Sounds/WeStart");
        audioChooseCaseA = LoadAudio(audioChooseCaseA, "Sounds/op1");
        audioChooseCaseB = LoadAudio(audioChooseCaseB, "Sounds/op2");
        audioChooseCaseC = LoadAudio(audioChooseCaseC, "Sounds/op3");
        audioChooseCaseD = LoadAudio(audioChooseCaseD, "Sounds/op4");
        audioRightCaseA = LoadAudio(audioRightCaseA, "Sounds/true1");
        audioRightCaseB = LoadAudio(audioRightCaseB, "Sounds/true2");
        audioRightCaseC = LoadAudio(audioRightCaseC, "Sounds/true3");
        audioRightCaseD = LoadAudio(audioRightCaseD, "Sounds/true4");
        audioWrongCaseA = LoadAudio(audioWrongCaseA, "Sounds/wrong1");
        audioWrongCaseB = LoadAudio(audioWrongCaseB, "Sounds/wrong2");
        audioWrongCaseC = LoadAudio(audioWrongCaseC, "Sounds/wrong3");
        audioWrongCaseD = LoadAudio(audioWrongCaseD, "Sounds/wrong4");
        audioTimeout = LoadAudio(audioTimeout, "Sounds/OutTime");
        audioThankyou = LoadAudio(audioThankyou, "Sounds/ThankYou");
        audioWaitAnswer = LoadAudio(audioWaitAnswer, "Sounds/WaitAnswer");
        audioHelpHalf = LoadAudio(audioHelpHalf, "Sounds/Help2");
        audioCallMate = LoadAudio(audioCallMate, "Sounds/Help0");
        audioGuestSupport = LoadAudio(audioGuestSupport, "Sounds/Help1");
    }

    AudioSource LoadAudio(AudioSource source, string path)
    {
        source = gameObject.AddComponent<AudioSource>();
        source.clip = Resources.Load<AudioClip>(path);
        return source;
    }

    void PlayAudioQuestion(int level)
    {
        if (level > 0)
            listAudioSource[level - 1].Play();
    }

    #region Work With Server
    public void GetSocketIO()
    {
        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();

        socket.On(Command.server_send_question, OnReceiveQuestion);
        socket.On(Command.server_comfirm_answer, OnServerConfirmAnswer);
        socket.On(Command.server_to_room_confirm_ready, OnServerConfirmReady);
        socket.On(Command.server_to_room_start_game, OnStartGame);
        socket.On(Command.server_to_room_send_question, OnReceiveQuestionToRoom);
        socket.On(Command.server_to_room_send_finish_question, OnFinishQuestion);
        socket.On(Command.server_to_room_client_leave, OnClientLeaveRoom);
        socket.On(Command.server_to_room_send_finish_match, OnFinishMatch);
    }

    private void OnFinishMatch(SocketIOEvent obj)
    {
        print(obj.data);
        var data = obj.data;
        var dataPlayers = data["data_finish"].list;
        for (int i = 0; i < dataPlayers.Count; i++)
        {
            var player = dataPlayers[i];
            var playerName = player["player_name"].ToString();
            var moneyTotal = player["total_money"].ToString();
            var averageTime = string.Format("{0}s / câu", player["average_response_time"].ToString());
            if (playerName.Equals(Ultis.Username))
            {
                lblFinishMoney1.text = moneyTotal;
                lblFinishUsername1.text = playerName;
                lblFinishTime1.text = averageTime;
            }
            else
            {
                lblFinishMoney2.text = moneyTotal;
                lblFinishUsername2.text = playerName;
                lblFinishTime2.text = averageTime;
            }
        }
        animFinish.SetBool("isShowInfo", false);

        animFinishMatch.enabled = true;
        animFinishMatch.SetBool("isShowInfo", true);
        ResetTrain();
    }

    private void OnClientLeaveRoom(SocketIOEvent obj)
    {
        var data = obj.data;
        var playerLeaveName = data["player_leave_name"].ToString();
        if (bool.Parse(data["status"].ToString()))
        {
            ResetTrain();
            if (playerLeaveName.Equals(Ultis.Username))
            {
                Ultis.Mode = PlayMode.TRAIN;
                goPlTrain.SetActive(false);
                goPlListRoom.SetActive(true);
            }
            else
            {
                ShowMessage(string.Format("{0} has leave room", playerLeaveName));
            }
        }
        else
        {
            if (playerLeaveName.Equals(Ultis.Username))
            {
                ShowMessage(data["message"].ToString());
            }
        }
    }

    private void OnReceiveQuestionToRoom(SocketIOEvent obj)
    {
        //StartCountTime();
        ReceiveQuestion(obj.data);
    }

    private void OnStartGame(SocketIOEvent obj)
    {
        ShowMessage(obj.data["message"].ToString());
        EnableLoading(true);
    }

    Color white = new Color(255, 255, 255);
    Color yellow = new Color(255, 255, 0);
    private void OnFinishQuestion(SocketIOEvent obj)
    {
        try
        {
            SetColorFinishText(white, lblUsername1, lblUsername2, lblResult1, lblResult2, lblMoney1, lblMoney2);
            var dataFinish = obj.data["data_finish"];

            for (int i = 0; i < dataFinish.list.Count; i++)
            {
                var user = dataFinish[i];
                bool isWin = bool.Parse(user["isWin"].ToString());
                var username = user["name"].ToString();
                var answer_content = ultis.Base64Decode(ultis.DecodeData(user["answer_content"].ToString()));
                var duration = user["duration"].ToString();
                var money = user["money"].ToString();
                var moneyTotal = user["total_money"].ToString();
                if (Ultis.Username.Equals(username))
                {
                    SetMoneyGain(money);
                }
                if (i == 0)
                {
                    if (isWin)
                        SetColorFinishText(yellow, lblUsername1, lblResult1, lblMoney1);
                    lblUsername1.text = username;
                    lblResult1.text = string.Format("{0} - {1}s", answer_content, duration);
                    lblMoney1.text = money;
                }
                else if (i == 1)
                {
                    if (isWin)
                        SetColorFinishText(yellow, lblUsername2, lblResult2, lblMoney2);
                    lblUsername2.text = username;
                    lblResult2.text = string.Format("{0} - {1}s", answer_content, duration);
                    lblMoney2.text = money;
                }
                if (username.Equals(Ultis.Username))
                {
                    SetMoneyGain(moneyTotal);
                }
            }
            OpenFinish();
        }
        catch (Exception ex)
        {
            print(ex.Message);
        }
    }

    void SetColorFinishText(Color color, params Text[] lbl)
    {
        for (int i = 0; i < lbl.Length; i++)
        {
            lbl[i].color = color;
        }
    }

    private void OnServerConfirmReady(SocketIOEvent obj)
    {
        if (bool.Parse(obj.data["status"].ToString()))
        {
            ShowMessage(obj.data["message"].ToString());
            goLoading.SetActive(false);
        }
    }

    private void OnServerConfirmAnswer(SocketIOEvent obj)
    {
        var data = obj.data;
        resultAnswer = bool.Parse(data["result"].ToString());
    }

    private void ShowResultAnswer()
    {
        if (resultAnswer)
        {
            if (level < 15)
            {
                ResetToNewQuestion();
            }
            else if (level == 15)
            {
                OpenCongratulation();
            }
        }
        else
        {
            OpenConfirmWrongAnswer();
        }
    }

    private void ResetToNewQuestion()
    {
        if (Ultis.Mode == PlayMode.TRAIN)
        {
            SetMoneyGain(level);
            SetSelectedMoldy(level);
            level++; // thu tu quan trong
            OpenMoneyMoldyDialog();
        }
        else
        {
            level++;
        }
    }

    private void SetQuestionNumber(int level)
    {
        lblQuestionNumber.text = "Câu " + level;
    }

    private void OnReceiveQuestion(SocketIOEvent obj)
    {
        ReceiveQuestion(obj.data);
    }

    public void SendGetQuestion(int level, bool checkLevel = true)
    {
        if (socket != null)
        {
            //if (checkLevel && level == 1)
            //    return;
            //PlayerPrefs.SetInt("level", level);
            Ultis.CurrentLevel = level;
            print("current level : " + Ultis.CurrentLevel);
            questionCurrent = null;
            EnableLoading(true);
            Debug.Log("send get question - " + level);
            var data = new Dictionary<string, string>();
            data.Add("level", level.ToString());
            data.Add("command", Command.client_get_question);
            ultis.SendData(socket, Command.client_message, data);
        }
    }

    public void SendAnswer(Answer answer)
    {
        var data = new Dictionary<string, string>();
        data.Add("command", Command.client_answer);
        data.Add("questionId", questionCurrent.Id);
        switch (answer)
        {
            case Answer.A:
                data.Add("answer", ultis.TrimStartEndChar(questionCurrent.CaseA));
                break;
            case Answer.B:
                data.Add("answer", ultis.TrimStartEndChar(questionCurrent.CaseB));
                break;
            case Answer.C:
                data.Add("answer", ultis.TrimStartEndChar(questionCurrent.CaseC));
                break;
            case Answer.D:
                data.Add("answer", ultis.TrimStartEndChar(questionCurrent.CaseD));
                break;
            case Answer.NONE:
                break;
            default:
                break;
        }
        ultis.SendData(socket, Command.client_message, data);
        switch (Ultis.Mode)
        {
            case PlayMode.TRAIN:
                StartCountShowRightAnswer();
                break;
            case PlayMode.CHALLENGE:
                break;
            case PlayMode.NONE:
                break;
            default:
                break;
        }
    }

    void StartCountTime()
    {
        timeLeft = 30;
        isStartCountTime = true;
    }

    void ResetCountTime()
    {
        isStartCountTime = false;
    }

    public void ReceiveQuestion(JSONObject data)
    {
        try
        {
            print("Receive Question: " + data.ToString());
            var status = bool.Parse(data[Command.STATUS].ToString());
            if (status)
            {
                questionCurrent = new Question();
                EnableAllButtonAnswer(true);
                Ultis.CurrentQuestionId = questionCurrent.Id = data["id"].ToString();
                questionCurrent.Level = data["level"].ToString();
                questionCurrent.QuestionStr = data["question"].ToString();

                string[] arrAnswer = new string[] {
                    data["casea"].ToString(),
                    data["caseb"].ToString(),
                    data["casec"].ToString(),
                    data["cased"].ToString()
                };

                List<string> listAnswer = new List<string>();
                var randNumber = UnityEngine.Random.Range(1, 4);
                //Debug.Log("rand number: " + randNumber);
                for (int i = randNumber; i < 4 + randNumber; i++)
                {
                    listAnswer.Add(arrAnswer[i % 4]);
                }

                for (int i = 0; i < listAnswer.Count; i++)
                {
                    var rightAns = data["casea"].ToString();
                    if (listAnswer[i].Equals(rightAns))
                    {
                        questionCurrent.RightAnswerIndex = i;
                        break;
                    }
                }

                questionCurrent.CaseA = listAnswer[0];
                questionCurrent.CaseB = listAnswer[1];
                questionCurrent.CaseC = listAnswer[2];
                questionCurrent.CaseD = listAnswer[3];

                if (Ultis.Mode == PlayMode.CHALLENGE)
                    PlayQuestionInfo();
                else
                {
                    if (isFirstPlay)
                    {
                        PlayQuestionInfo();
                        isFirstPlay = false;
                    }
                }

                SetDataQuestion(questionCurrent);

                EnableLoading(false);
                goPlMaskTrain.SetActive(false);
                SetQuestionNumber(level);
            }
            else
            {
                OpenNotification(data[Command.ERROR].ToString());
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    private void SetDataQuestion(Question question)
    {
        if (question != null)
        {
            SetDataQuestion(question.QuestionStr, question.CaseADecode, question.CaseBDecode,
                question.CaseCDecode, question.CaseDDecode);
        }
    }

    private void SetDataQuestion(string question, string casea, string caseb, string casec, string cased)
    {
        //print("set data question");
        lblQuestion.text = question;
        lblCaseA.text = casea;
        lblCaseB.text = caseb;
        lblCaseC.text = casec;
        lblCaseD.text = cased;
    }

    void ClientSendReady()
    {
        var data = new Dictionary<string, string>();
        data.Add("command", Command.client_ready_to_room);
        data.Add("roomId", Ultis.RoomIdSelected);
        data.Add("level", level.ToString());
        print("send ready with level - " + level);
        ultis.SendData(socket, Command.client_message, data);
    }

    void SendAnswerToRoom(Answer answer, string duration)
    {
        var data = new Dictionary<string, string>();
        data.Add("command", Command.client_answer_to_room);
        data.Add("questionId", questionCurrent.Id);
        data.Add("duration", duration);
        data.Add("roomId", Ultis.RoomIdSelected);
        data.Add("answer_case", answer.ToString());
        data.Add("level", level.ToString());
        switch (answer)
        {
            case Answer.A:
                data.Add("answer_content", questionCurrent.CaseA);
                data.Add("answer", ultis.TrimStartEndChar(questionCurrent.CaseA));
                break;
            case Answer.B:
                data.Add("answer_content", questionCurrent.CaseB);
                data.Add("answer", ultis.TrimStartEndChar(questionCurrent.CaseB));
                break;
            case Answer.C:
                data.Add("answer_content", questionCurrent.CaseC);
                data.Add("answer", ultis.TrimStartEndChar(questionCurrent.CaseC));
                break;
            case Answer.D:
                data.Add("answer_content", questionCurrent.CaseD);
                data.Add("answer", ultis.TrimStartEndChar(questionCurrent.CaseD));
                break;
            case Answer.NONE:
                break;
            default:
                break;
        }
        ultis.SendData(socket, Command.client_message, data);
        StartCountShowRightAnswer();
    }

    void SendTimeout()
    {
        var data = new Dictionary<string, string>();
        data.Add("command", Command.client_time_out_to_room);
        data.Add("roomId", Ultis.RoomIdSelected);
        ultis.SendData(socket, data);
    }
    #endregion

    #region Call UI
    public void OpenFinish()
    {
        animFinish.enabled = true;
        animFinish.SetBool("isShowInfo", true);
    }

    public void CloseFinish()
    {
        ResetFlipAnswer();
        animFinish.SetBool("isShowInfo", false);
        ResetToNewQuestion();
        ClientSendReady();
        goPlMaskTrain.SetActive(false);
        EnableAllButtonAnswer(false);
    }

    void ShowMessage(string message)
    {
        lblMessage.text = message;
    }

    public void OpenChallenge()
    {
        level = 1;
        goPlTrain.SetActive(true);
        var plHelps = goPlTrain.transform.Find("pl_helps");
        var plMessage = plHelps.FindChild("pl_message").gameObject;
        plMessage.SetActive(true);
        var plHelpChild = plHelps.transform.Find("pl_help_child").gameObject;
        plHelpChild.SetActive(false);
    }

    void PlayWrongAnswer(Answer ans)
    {
        switch (ans)
        {
            case Answer.A:
                audioWrongCaseA.Play();
                break;
            case Answer.B:
                audioWrongCaseB.Play();
                break;
            case Answer.C:
                audioWrongCaseC.Play();
                break;
            case Answer.D:
                audioWrongCaseD.Play();
                break;
            case Answer.NONE:
                break;
            default:
                break;
        }
    }

    public void OpenTimeout()
    {
        if (Ultis.Mode == PlayMode.TRAIN)
        {
            audioTimeout.Play();
            goPlMaskTrain.SetActive(true);
            animTimeout.enabled = true;
            animTimeout.SetBool("isShowInfo", true);
        }
        else
        {
            answer = Answer.A;
            AnswerQuestion();
        }
    }

    public void CloseTimeout()
    {
        animTimeout.SetBool("isShowInfo", false);
        StopTrain();
    }

    void ResetFlipAnswer()
    {
        animCaseAFlip.SetBool("isFlip", false);
        animCaseBFlip.SetBool("isFlip", false);
        animCaseCFlip.SetBool("isFlip", false);
        animCaseDFlip.SetBool("isFlip", false);
        animCaseAFlip.SetBool("isChoosen", false);
        animCaseBFlip.SetBool("isChoosen", false);
        animCaseCFlip.SetBool("isChoosen", false);
        animCaseDFlip.SetBool("isChoosen", false);
        ResetColorAnswer();
    }

    /// <summary>
    /// nhap nhay ca tra loi
    /// </summary>
    /// <param name="ans"></param>
    void FlipAnswer(Answer ans)
    {
        switch (ans)
        {
            case Answer.A:
                animCaseAFlip.enabled = true;
                animCaseAFlip.SetBool("isFlip", true);
                if (resultAnswer)
                    audioRightCaseA.Play();
                else
                    audioWrongCaseA.Play();
                break;
            case Answer.B:
                animCaseBFlip.enabled = true;
                animCaseBFlip.SetBool("isFlip", true);
                if (resultAnswer)
                    audioRightCaseB.Play();
                else
                    audioWrongCaseB.Play();
                break;
            case Answer.C:
                animCaseCFlip.enabled = true;
                animCaseCFlip.SetBool("isFlip", true);
                if (resultAnswer)
                    audioRightCaseC.Play();
                else
                    audioWrongCaseC.Play();
                break;
            case Answer.D:
                animCaseDFlip.enabled = true;
                animCaseDFlip.SetBool("isFlip", true);
                if (resultAnswer)
                    audioRightCaseD.Play();
                else
                    audioWrongCaseD.Play();
                break;
            case Answer.NONE:
                break;
            default:
                break;
        }
    }

    void SetMoneyGain(int level)
    {
        if (level > 0)
        {
            Ultis.CurrentMoneyText = lblMoney.text = arrMoney[level - 1];
        }
    }

    void SetMoneyGain(string money)
    {
        lblMoney.text = money;
    }

    void EnableLoading(bool isEnable)
    {
        goLoading.SetActive(isEnable);
    }

    public void OpenMateHelp(Button btn)
    {
        btnCallMate.enabled = false;
        var text = btn.transform.FindChild("Text").GetComponent<Text>();
        lblTitleMateHelp.text = text.text + " trợ giúp cho bạn đáp án:";
        lblAnswerMateHelp.text = questionCurrent.RightAnswer.ToString();
        animMateHelp.enabled = true;
        animMateHelp.SetBool("isShowInfo", true);
    }

    public void CloseMateHelp()
    {
        animMateHelp.SetBool("isShowInfo", false);
    }

    public void ShowRightAnswer()
    {
        //nhấp nháy câu trả lời đúng

        FlipAnswer(questionCurrent.RightAnswer);

        #region old
        //switch (questionCurrent.RightAnswer)
        //{
        //    case Answer.A:
        //        btnCaseA.image.color = new Color(255, 0, 0, 255);
        //        break;
        //    case Answer.B:
        //        btnCaseB.image.color = new Color(255, 0, 0, 255);
        //        break;
        //    case Answer.C:
        //        btnCaseC.image.color = new Color(255, 0, 0, 255);
        //        break;
        //    case Answer.D:
        //        btnCaseD.image.color = new Color(255, 0, 0, 255);
        //        break;
        //    case Answer.NONE:
        //        break;
        //    default:
        //        break;
        //} 
        #endregion
    }

    private void ResetColorAnswer()
    {
        // reset màu
        btnCaseA.image.color = new Color(255, 255, 255, 255);
        btnCaseB.image.color = new Color(255, 255, 255, 255);
        btnCaseC.image.color = new Color(255, 255, 255, 255);
        btnCaseD.image.color = new Color(255, 255, 255, 255);
    }

    public void EnableAllButtonAnswer(bool isEnable)
    {
        btnCaseA.enabled = isEnable;
        btnCaseC.enabled = isEnable;
        btnCaseB.enabled = isEnable;
        btnCaseD.enabled = isEnable;
    }

    public void DisableButtonAnswer(Answer ans)
    {
        switch (ans)
        {
            case Answer.A:
                btnCaseA.enabled = false;
                break;
            case Answer.B:
                btnCaseB.enabled = false;
                break;
            case Answer.C:
                btnCaseC.enabled = false;
                break;
            case Answer.D:
                btnCaseD.enabled = false;
                break;
            case Answer.NONE:
                break;
            default:
                break;
        }
    }

    public void OpenConfirmStopDialog()
    {
        goPlMaskTrain.SetActive(true);
        animConfirmStop.enabled = true;
        animConfirmStop.SetBool("isShowInfo", true);
    }

    public void CloseConfirmStopDialog()
    {
        goPlMaskTrain.SetActive(false);
        animConfirmStop.SetBool("isShowInfo", false);
    }

    public void FinishChallenge()
    {
        animFinishMatch.SetBool("isShowInfo", false);

        LeaveRoom(Ultis.RoomIdSelected);
    }

    void ResetTrain()
    {
        level = 1;
        SetQuestionNumber(level);
        SetDataQuestion("...", "...", "...", "...", "...");
        SetMoneyGain("0");
        lblTimeLeft.text = "30";
    }

    public void ResetChallenge()
    {
        level = 1;
        animFinishMatch.SetBool("isShowInfo", false);
        //ConfirmReady();
        CloseFinish();
        ResetTrain();
        goPlMaskTrain.SetActive(false);
        EnableAllButtonAnswer(false);
    }

    public void StopTrain()
    {
        CloseConfirmStopDialog();
        if (Ultis.Mode == PlayMode.CHALLENGE)
        {
            LeaveRoom(Ultis.RoomIdSelected);
        }
        else
        {
            goPlTrain.SetActive(false);
            ActiveHomeScreen(true, true, 1);
            goLogo.SetActive(true);
        }
    }

    /// <summary>
    /// 50 : 50 help
    /// </summary>
    public void HalfHelp()
    {
        btnHalfHelp.enabled = false;
        List<string> listAnswer = new List<string>() { 
            questionCurrent.CaseA,
            questionCurrent.CaseB,
            questionCurrent.CaseC,
            questionCurrent.CaseD
        };

        listAnswer.RemoveAt(questionCurrent.RightAnswerIndex);

        int index1 = UnityEngine.Random.Range(0, 2);
        var ansWrong1 = listAnswer[index1];
        listAnswer.RemoveAt(index1);
        int index2 = UnityEngine.Random.Range(0, 1);
        var ansWrong2 = listAnswer[index2];
        //listAnswer.RemoveAt(index2);

        HideWrongQuestion(ansWrong1);
        HideWrongQuestion(ansWrong2);
    }

    public void PlayAudioHelpHalf()
    {
        audioHelpHalf.Play();
        checkPlayAudioHelpHalf = true;
    }

    public void OpenCallMate()
    {
        audioCallMate.Play();
        goPlMaskTrain.SetActive(true);
        animCallMate.enabled = true;
        animCallMate.SetBool("isShowInfo", true);
        print("open call mate");
    }

    public void CloseCallMate()
    {
        goPlMaskTrain.SetActive(false);
        animCallMate.SetBool("isShowInfo", false);
    }

    public void CallMate(Button btn)
    {
        btnCallMate.enabled = false;
        OpenMateHelp(btn);
        CloseCallMate();
    }

    public void OpenGroupSupport(GameObject goGroupSupport)
    {
        btnGroupSupport.enabled = false;
        goPlMaskTrain.SetActive(true);
        var per1 = goGroupSupport.transform.FindChild("lbl_person_1").GetComponent<Text>();
        var per2 = goGroupSupport.transform.FindChild("lbl_person_2").GetComponent<Text>();
        var per3 = goGroupSupport.transform.FindChild("lbl_person_3").GetComponent<Text>();

        per1.text = string.Format("Người thứ 1 - {0} - {1}%", questionCurrent.RightAnswer, UnityEngine.Random.RandomRange(50, 100));
        per2.text = string.Format("Người thứ 2 - {0} - {1}%", questionCurrent.RightAnswer, UnityEngine.Random.RandomRange(40, 100));
        per3.text = string.Format("Người thứ 3 - {0} - {1}%", questionCurrent.RightAnswer, UnityEngine.Random.RandomRange(30, 100));

        animGroupSupport.enabled = true;
        animGroupSupport.SetBool("isShowInfo", true);
    }

    public void CloseGroupSupport()
    {
        goPlMaskTrain.SetActive(false);
        animGroupSupport.SetBool("isShowInfo", false);
    }

    public void PlayAudioGuestSupport()
    {
        checkPlayAudioGuestSupport = true;
        audioGuestSupport.Play();
    }

    public void OpenGuestSupport(GameObject plMates)
    {
        btnGuesSupport.enabled = false;
        goPlMaskTrain.SetActive(true);
        animGuestSupport.enabled = true;
        animGuestSupport.SetBool("isShowInfo", true);
        int[] percents = ultis.GeneratePercents();
        var plHelpA = plMates.transform.FindChild("pl_help_a");
        var plHelpB = plMates.transform.FindChild("pl_help_b");
        var plHelpC = plMates.transform.FindChild("pl_help_c");
        var plHelpD = plMates.transform.FindChild("pl_help_d");
        var rectA = plHelpA.GetComponent<RectTransform>();
        var rectB = plHelpB.GetComponent<RectTransform>();
        var rectC = plHelpC.GetComponent<RectTransform>();
        var rectD = plHelpD.GetComponent<RectTransform>();

        var lblPercentA = goPlPercents.transform.FindChild("lbl_help_percent_a").GetComponent<Text>();
        var lblPercentB = goPlPercents.transform.FindChild("lbl_help_percent_b").GetComponent<Text>();
        var lblPercentC = goPlPercents.transform.FindChild("lbl_help_percent_c").GetComponent<Text>();
        var lblPercentD = goPlPercents.transform.FindChild("lbl_help_percent_d").GetComponent<Text>();

        plHelpA.GetComponent<Image>().color = new Color(9, 0, 255, 255);
        plHelpB.GetComponent<Image>().color = new Color(9, 0, 255, 255);
        plHelpC.GetComponent<Image>().color = new Color(9, 0, 255, 255);
        plHelpD.GetComponent<Image>().color = new Color(9, 0, 255, 255);

        switch (questionCurrent.RightAnswer)
        {
            case Answer.A:
                plHelpA.GetComponent<Image>().color = new Color(255, 0, 0, 255);
                lblPercentA.text = string.Format("{0}%", percents[0]);
                rectA.sizeDelta = new Vector2(rectA.rect.width, 128 * percents[0] / 100);
                lblPercentA.text = string.Format("{0}%", percents[1]);
                rectB.sizeDelta = new Vector2(rectA.rect.width, 128 * percents[1] / 100);
                lblPercentA.text = string.Format("{0}%", percents[2]);
                rectC.sizeDelta = new Vector2(rectA.rect.width, 128 * percents[2] / 100);
                lblPercentA.text = string.Format("{0}%", percents[3]);
                rectD.sizeDelta = new Vector2(rectA.rect.width, 128 * percents[3] / 100);
                break;
            case Answer.B:
                plHelpB.GetComponent<Image>().color = new Color(255, 0, 0, 255);
                lblPercentB.text = string.Format("{0}%", percents[0]);
                rectB.sizeDelta = new Vector2(rectA.rect.width, 128 * percents[0] / 100);
                lblPercentA.text = string.Format("{0}%", percents[1]);
                rectA.sizeDelta = new Vector2(rectA.rect.width, 128 * percents[1] / 100);
                lblPercentC.text = string.Format("{0}%", percents[2]);
                rectC.sizeDelta = new Vector2(rectA.rect.width, 128 * percents[2] / 100);
                lblPercentD.text = string.Format("{0}%", percents[3]);
                rectD.sizeDelta = new Vector2(rectA.rect.width, 128 * percents[3] / 100);
                break;
            case Answer.C:
                plHelpC.GetComponent<Image>().color = new Color(255, 0, 0, 255);
                lblPercentC.text = string.Format("{0}%", percents[0]);
                rectC.sizeDelta = new Vector2(rectA.rect.width, 128 * percents[0] / 100);
                lblPercentB.text = string.Format("{0}%", percents[1]);
                rectB.sizeDelta = new Vector2(rectA.rect.width, 128 * percents[1] / 100);
                lblPercentA.text = string.Format("{0}%", percents[2]);
                rectA.sizeDelta = new Vector2(rectA.rect.width, 128 * percents[2] / 100);
                lblPercentD.text = string.Format("{0}%", percents[3]);
                rectD.sizeDelta = new Vector2(rectA.rect.width, 128 * percents[3] / 100);
                break;
            case Answer.D:
                plHelpD.GetComponent<Image>().color = new Color(255, 0, 0, 255);
                lblPercentD.text = string.Format("{0}%", percents[0]);
                rectD.sizeDelta = new Vector2(rectA.rect.width, 128 * percents[0] / 100);
                lblPercentB.text = string.Format("{0}%", percents[1]);
                rectB.sizeDelta = new Vector2(rectA.rect.width, 128 * percents[1] / 100);
                lblPercentC.text = string.Format("{0}%", percents[2]);
                rectC.sizeDelta = new Vector2(rectA.rect.width, 128 * percents[2] / 100);
                lblPercentA.text = string.Format("{0}%", percents[3]);
                rectA.sizeDelta = new Vector2(rectA.rect.width, 128 * percents[3] / 100);
                break;
            case Answer.NONE:
                break;
            default:
                break;
        }
    }

    public void CloseGuestSupport()
    {
        goPlMaskTrain.SetActive(false);
        animGuestSupport.SetBool("isShowInfo", false);
    }

    public void OpenConfirmAnswer()
    {
        goPlMaskTrain.SetActive(true);
        animConfirmAnswer.enabled = true;
        animConfirmAnswer.SetBool("isShowInfo", true);
    }

    public void CloseConfirmAnswer()
    {
        goPlMaskTrain.SetActive(false);
        animConfirmAnswer.SetBool("isShowInfo", false);
    }

    public void AnswerQuestion()
    {
        print("send answer on train");
        goPlMaskTrain.SetActive(true);
        ResetCountTime();
        switch (Ultis.Mode)
        {
            case PlayMode.TRAIN:
                CloseConfirmAnswer();
                SendAnswer(answer);
                goPlMaskTrain.SetActive(true);
                break;
            case PlayMode.CHALLENGE:
                SendAnswerToRoom(answer, secondCount.ToString());
                CloseConfirmAnswer();
                EnableAllButtonAnswer(true);
                break;
            case PlayMode.NONE:
                break;
            default:
                break;
        }
        switch (answer)
        {
            case Answer.A:
                animCaseAFlip.SetBool("isChoosen", true);
                break;
            case Answer.B:
                animCaseBFlip.SetBool("isChoosen", true);
                break;
            case Answer.C:
                animCaseCFlip.SetBool("isChoosen", true);
                break;
            case Answer.D:
                animCaseDFlip.SetBool("isChoosen", true);
                break;
            case Answer.NONE:
                break;
            default:
                break;
        }
        audioWaitAnswer.Play();
    }

    public void OpenReadyConfirm()
    {
        if (Ultis.Mode == PlayMode.TRAIN)
            goPlMaskTrain.SetActive(true);
        //SetDataQuestion("...", "...", "...", "...", "...");
        animReadyConfirm.enabled = true;
        animReadyConfirm.SetBool("isShowInfo", true);
    }

    public void ConfirmReady()
    {
        switch (Ultis.Mode)
        {
            case PlayMode.TRAIN:
                SendGetQuestion(level, false);
                goPlMaskTrain.SetActive(false);
                EnableAllButtonAnswer(true);
                CloseMoneyMoldyDialog();
                EnableLoading(true);
                break;
            case PlayMode.CHALLENGE:
                OpenChallenge();
                ClientSendReady();
                break;
            case PlayMode.NONE:
                break;
            default:
                break;
        }
        CloseReadyConfirm();
    }

    public void CloseReadyConfirm()
    {
        animReadyConfirm.SetBool("isShowInfo", false);
    }

    public void OpenTrain()
    {
        Ultis.RoomIdSelected = "0";
        Ultis.Mode = PlayMode.TRAIN;
        OpenMoneyMoldyDialog();
        EnableAllButtonAnswer(false);
    }

    public void OpenTrainReconnect(JSONObject j)
    {
        this.level = Ultis.CurrentLevel;
        SetMoneyGain(level - 1);
        SetQuestionNumber(level);
        goPlHome.SetActive(false);
        goPlTrain.SetActive(true);
        goLogo.SetActive(false);
        Ultis.RoomIdSelected = "0";
        Ultis.Mode = PlayMode.TRAIN;
        EnableAllButtonAnswer(true);
        isFirstPlay = true;
        goPlMaskTrain.SetActive(false);
        ReceiveQuestion(j);
    }

    public void OpenMoneyMoldyDialog()
    {
        animMoneyMoldy.enabled = true;
        animMoneyMoldy.SetBool("isShowMoneyMoldy", true);
        if (isFirstPlay)
        {
            audioIntro.Play();
            level = 1;// PlayerPrefs.GetInt("level");
            OpenReadyConfirm();
        }
        else
        {
            animConfirmAnswer.enabled = false;
            SendGetQuestion(level);
        }
        SetQuestionNumber(level);
        goLogo.SetActive(false);
        goPlTrain.SetActive(false);
        ActiveHomeScreen(false, false, 1);
        SetDataQuestion("...", "...", "...", "...", "...");
        ResetColorAnswer();
    }

    public void ActiveHomeScreen(bool active, bool isPlayWelcomSound, int level)
    {
        if (active)
        {
            goPlMaskTrain.SetActive(false);
            goLogo.SetActive(true);
            this.level = level;
            SetQuestionNumber(level);
            isFirstPlay = active;
            SetDataQuestion("...", "...", "...", "...", "...");

            btnCallMate.enabled = true;
            btnStop.enabled = true;
            btnHalfHelp.enabled = true;
            btnGroupSupport.enabled = true;
            btnGuesSupport.enabled = true;

            ResetMoldy();
            lblMoney.text = "0";
            lblTimeLeft.text = "30";

            ResetCountTime();
            if (isPlayWelcomSound)
                audioThankyou.Play();

            CloseMoneyMoldyDialog();

            goPlHome.SetActive(true);
            goPlTrain.SetActive(false);
            ResetFlipAnswer();
        }
        goPlHome.SetActive(active);
    }

    public void CloseMoneyMoldyDialog()
    {
        goPlTrain.SetActive(true);
        animMoneyMoldy.SetBool("isShowMoneyMoldy", false);
        switch (Ultis.Mode)
        {
            case PlayMode.TRAIN:
                if (isFirstPlay)
                {
                    audioIntro.Stop();
                }
                else
                {
                    PlayQuestionInfo();
                }
                break;
            case PlayMode.NONE:
                break;
            default:
                break;
        }
    }

    void PlayQuestionInfo()
    {
        StartCountTime();
        PlayAudioQuestion(level);
    }

    public void ChooseAnswerA()
    {
        audioChooseCaseA.Play();
        lblCaseChoosen.text = "A";
        answer = Answer.A;
        OpenConfirmAnswer();
    }
    public void ChooseAnswerB()
    {
        audioChooseCaseB.Play();
        lblCaseChoosen.text = "B";
        answer = Answer.B;
        OpenConfirmAnswer();
    }
    public void ChooseAnswerC()
    {
        audioChooseCaseC.Play();
        lblCaseChoosen.text = "C";
        answer = Answer.C;
        OpenConfirmAnswer();
    }
    public void ChooseAnswerD()
    {
        audioChooseCaseD.Play();
        lblCaseChoosen.text = "D";
        answer = Answer.D;
        OpenConfirmAnswer();
    }

    public void OpenConfirmWrongAnswer()
    {
        goPlMaskTrain.SetActive(true);
        ResetFlipAnswer();
        animConfirmWrongAnswer.enabled = true;
        animConfirmWrongAnswer.SetBool("isShowInfo", true);
    }

    public void CloseConfirmWrongAnswer()
    {
        animConfirmWrongAnswer.SetBool("isShowInfo", false);
        StopTrain();
    }

    public void OpenNotification(string notify)
    {
        goPlMaskTrain.SetActive(true);
        animNotify.enabled = true;
        lblNotify.text = notify;
        animNotify.SetBool("isShowInfo", true);
    }

    public void CloseNotification()
    {
        goPlMaskTrain.SetActive(false);
        animNotify.SetBool("isShowInfo", false);
    }

    public void SetSelectedMoldy(int moldy)
    {
        try
        {
            int index = 1;

            ResetMoldy();

            foreach (Transform child in goPlMoneyMoldy.transform)
            {
                if (index == (16 - moldy))
                {
                    var imgBack = child.gameObject.GetComponent<Image>();
                    if (imgBack != null)
                    {
                        //imgBack.sprite = sprBackChoosenMoldy;
                        imgBack.color = new Color(255, 0, 0, 255);
                        break;
                    }
                }
                index++;
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    private void ResetMoldy()
    {
        foreach (Transform child in goPlMoneyMoldy.transform)
        {
            var imgBack = child.gameObject.GetComponent<Image>();
            imgBack.sprite = null;
            imgBack.color = new Color(0, 0, 255, 0);
        }
    }

    public void OpenCongratulation()
    {
        goPlMaskTrain.SetActive(true);
        animCongrat.enabled = true;
        animCongrat.SetBool("isShowInfo", true);
        SaveRecord();
    }

    public void CloseCongrat()
    {
        goPlMaskTrain.SetActive(false);
        animCongrat.SetBool("isShowInfo", false);
        ActiveHomeScreen(true, true, 1);
    }

    private void HideWrongQuestion(string ansWrong1)
    {
        if (ansWrong1.Equals(questionCurrent.CaseA))
        {
            lblCaseA.text = string.Empty;
            DisableButtonAnswer(Answer.A);
        }
        if (ansWrong1.Equals(questionCurrent.CaseB))
        {
            lblCaseB.text = string.Empty;
            DisableButtonAnswer(Answer.B);
        }
        if (ansWrong1.Equals(questionCurrent.CaseC))
        {
            lblCaseC.text = string.Empty;
            DisableButtonAnswer(Answer.C);
        }
        if (ansWrong1.Equals(questionCurrent.CaseD))
        {
            lblCaseD.text = string.Empty;
            DisableButtonAnswer(Answer.D);
        }
    }

    #endregion

    void Start()
    {
        ultis = new Ultis();
        GetSocketIO();
        answer = Answer.NONE;
        sprBackChoosenMoldy = Resources.Load<Sprite>("Textures/buton/cac moc tien");
        EnableAllButtonAnswer(false);
        LoadAudio();
    }

    // Update is called once per frame
    float secondCount = 0;

    void Update()
    {
        if (isStartCountTime)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft > 0)
            {
                secondCount = Mathf.Floor(timeLeft % 60);
                lblTimeLeft.text = secondCount.ToString();
            }
            else
            {
                isStartCountTime = false;
                OpenTimeout();
            }
        }
    }

    bool flagShowRightAnswer = true;

    void StartCountShowRightAnswer()
    {
        flagShowRightAnswer = true;
        isStartCountTimeShowRightAnswer = true;
        timeShowRightAnswer = 9;
    }

    void FixedUpdate()
    {
        if (isStartCountTimeShowRightAnswer)
        {
            timeShowRightAnswer -= Time.deltaTime;
            var t = Mathf.Floor(timeShowRightAnswer % 60);
            if (t == 4 && flagShowRightAnswer)
            {
                flagShowRightAnswer = false;
                print("show right answer...");

                ShowRightAnswer();
            }
            if (timeShowRightAnswer < 0)
            {
                if (Ultis.Mode == PlayMode.TRAIN)
                {
                    print("show result answer...");
                    ShowResultAnswer();
                    isStartCountTimeShowRightAnswer = false;
                }
            }
        }
        CheckHelfAudioComplete();
    }

    void CheckHelfAudioComplete()
    {
        if (checkPlayAudioHelpHalf)
        {
            if (!audioHelpHalf.isPlaying)
            {
                checkPlayAudioHelpHalf = false;
                HalfHelp();
            }
        }

        if (checkPlayAudioGuestSupport)
        {
            if (!audioGuestSupport.isPlaying)
            {
                checkPlayAudioGuestSupport = false;
                OpenGuestSupport(goPlMates);
            }
        }
    }

    public void SaveRecord()
    {
        try
        {
            if (animConfirmWrongAnswer.GetBool("isShowInfo"))
                CloseConfirmWrongAnswer();

            if (level == 1)
                return;
            string money = arrMoneyReal[level - 2];
            int moneyInt = Convert.ToInt32(money) / 1000;
            string name = ultis.GetUsername();// PlayerPrefs.GetString("username").Trim('\"');
            var data = new Dictionary<string, string>();
            data.Add("command", Command.client_save_record);
            data.Add("username", name);
            data.Add("money", moneyInt.ToString());
            ultis.SendData(socket, data);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public void LeaveRoom(string roomId)
    {
        var data = new Dictionary<string, string>();
        data.Add("command", Command.client_leave_room);
        data.Add("roomId", roomId);
        ultis.SendData(socket, data);
    }
}

public enum Answer
{
    A, B, C, D, NONE
}

public enum HelpType
{
    STOP, HALF, CALL_MATE, GUEST_SUPPORT, GROUP_SUPPORT
}
