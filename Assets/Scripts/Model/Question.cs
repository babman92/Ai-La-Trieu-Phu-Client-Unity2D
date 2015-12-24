using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Model
{
    public class Question
    {
        public static Ultis ultis = new Ultis();
        public string Level { get; set; }

        public string question;
        public string QuestionStr
        {
            get { return question; }
            set
            {
                question = ultis.DecodeQuestion(value);
            }
        }
        public string Id { get; set; }

        string casea;
        string caseb;
        string casec;
        string cased;
        public string CaseADecode { get; set; }
        public string CaseBDecode { get; set; }
        public string CaseCDecode { get; set; }
        public string CaseDDecode { get; set; }

        int rightAnswerIndex;
        public int RightAnswerIndex {
            get { return rightAnswerIndex; }
            set { rightAnswerIndex = value;
            switch (rightAnswerIndex)
            {
                case 0:
                    this.RightAnswer = Answer.A;
                    break;
                case 1:
                    this.RightAnswer = Answer.B;
                    break;
                case 2:
                    this.RightAnswer = Answer.C;
                    break;
                case 3:
                    this.RightAnswer = Answer.D;
                    break;
                default:
                    break;
            }
            }
        }
        public Answer RightAnswer { get; set; }

        public string CaseA
        {
            get { return casea; }
            set
            {
                casea = value;
                CaseADecode = ultis.Base64Decode(ultis.DecodeData(casea));
            }
        }

        public string CaseB
        {
            get { return caseb; }
            set
            {
                caseb = value;
                CaseBDecode = ultis.Base64Decode(ultis.DecodeData(caseb));
            }
        }

        public string CaseC
        {
            get { return casec; }
            set
            {
                casec = value;
                CaseCDecode = ultis.Base64Decode(ultis.DecodeData(casec));
            }
        }

        public string CaseD
        {
            get { return cased; }
            set
            {
                cased = value;
                CaseDDecode = ultis.Base64Decode(ultis.DecodeData(cased));
            }
        }

        public Question()
        {
            Level = string.Empty;
            QuestionStr = string.Empty;
            Id = string.Empty;
            CaseA = string.Empty;
            CaseB = string.Empty;
            CaseC = string.Empty;
            CaseD = string.Empty;
            RightAnswer = Answer.NONE;
        }
    }
}
