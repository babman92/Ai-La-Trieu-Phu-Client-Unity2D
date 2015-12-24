using SocketIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class Ultis
    {
        public static PlayMode Mode = PlayMode.NONE;
        public static string Username = string.Empty;
        public static int CurrenPlayerInRoom = 0;
        public static string RoomIdSelected = "0";
        public static string CurrentQuestionId = "1";
        public static bool IsLoseConnect = false;
        public static string CurrentMoneyText = string.Empty;
        public static int CurrentLevel = 1;

        public void SendData(SocketIOComponent socket, string command, Dictionary<string, string> data)
        {
            socket.Emit(command, new JSONObject(data));
            Debug.Log(data["command"]);
        }

        public void SendData(SocketIOComponent socket, Dictionary<string, string> data)
        {
            SendData(socket, Command.client_message, data);
        }

        public string DecodeData(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return data;
            }
            data = TrimStartEndChar(data);
            return data.Remove(2, 2);
        }

        public static string TrimStartEndCharStatic(string data)
        {
            if (data[0].Equals('"'))
                data = data.Remove(0, 1);
            if (data[data.Length - 1].Equals('"'))
                data = data.Remove(data.Length - 1, 1);
            return data;
        }

        public string TrimStartEndChar(string data)
        {
            if (data[0].Equals('"'))
                data = data.Remove(0, 1);
            if (data[data.Length - 1].Equals('"'))
                data = data.Remove(data.Length - 1, 1);
            return data;
        }

        public string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public string Base64Decode(string base64EncodedData)
        {
            if (string.IsNullOrEmpty(base64EncodedData))
            {
                return base64EncodedData;
            }
            string decodedText = string.Empty;
            try
            {
                byte[] decodedBytes = Convert.FromBase64String(base64EncodedData);
                decodedText = Encoding.UTF8.GetString(decodedBytes);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
            return decodedText;
        }

        public string DecodeQuestion(string questionEncode)
        {
            string result = string.Empty;
            questionEncode = DecodeData(questionEncode);
            result = Base64Decode(questionEncode);
            return result;
        }

        public int[] GeneratePercents()
        {
            int[] res = new int[4];
            int a = UnityEngine.Random.Range(40, 100);
            int b = UnityEngine.Random.Range(0, 100 - a);
            int c = UnityEngine.Random.Range(0, 100 - a - b);
            int d = 100 - a - b - c;
            res[0] = a;
            res[1] = b;
            res[2] = c;
            res[3] = d;
            return res;
        }

        public void WriteFile(string fileName, string content)
        {
            try
            {
                StreamWriter sr;
                if (File.Exists(fileName))
                    sr = new StreamWriter(File.OpenWrite(fileName));
                else
                    sr = File.CreateText(fileName);
                sr.WriteLine(content);
                sr.Close();
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }

        public string ReadFile(string fileName)
        {
            string data = string.Empty;
            try
            {
                if (File.Exists(fileName))
                {
                    var sr = File.OpenText(fileName);
                    data = sr.ReadToEnd();
                }
                else
                {
                    WriteFile(fileName, string.Empty);
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
            return data;
        }

        public string GetUsername()
        {
            return PlayerPrefs.GetString("username", string.Empty).Trim('\"');
        }
    }
}
