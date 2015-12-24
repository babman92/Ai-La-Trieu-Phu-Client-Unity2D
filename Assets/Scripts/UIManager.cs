using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public void OpenTrainScreen()
    {
        Application.LoadLevel("train");
    }

    public void OpenHomeScreen()
    {
        Application.LoadLevel("home");
    }

    public void OpenSettingScreen()
    {
        Application.LoadLevel("setting");
    }

    public void OpenInfoScreen()
    {
        Application.LoadLevel("info");
    }

    public void OpenGuideScreen()
    {
        Application.LoadLevel("guide");
    }

    public void OpenListRoomScreen()
    {
        Application.LoadLevel("list_room");
    }

    public void OpenRecordScreen()
    {
        Application.LoadLevel("record");
    }

    public void OpenChallengeScreen()
    {
        Application.LoadLevel("challenge");
    }
}
