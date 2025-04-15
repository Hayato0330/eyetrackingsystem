using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System.IO;
using System.Text;
using System;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Threading.Tasks;

//namespace BlinkMeasure
//{
    public class BlinkMeasure : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI TestText;
        [SerializeField] private GameObject contextObj;
        [SerializeField] private GameObject termsObj;
        [SerializeField] private GameObject blinkObj;
        [SerializeField] private GameObject videoObj1;
        [SerializeField] private GameObject videoObj2;
        [SerializeField] private GameObject videoObj3;

        private OVRFaceExpressions faceExpressions;
        private bool isMeasure = false; //start or stop
        private bool isPreMeasure = false;
        private bool isEyeClose = false; // eye t:close f:open
        private bool isPreEyeClose = false; //pre

        private float blinktimer = 0; // 0-60 range time
        private float interval = 60; // 1m time
        private int timeCounter = 0; // minute count

        private int blinkCounter = 0; 
        
        private float sleepTime = 0;
        private float sleepInterval = 5;
        private bool isSleep = false; // t:sleep
        private bool isPreSleep = false; // t:sleep
        private int sleepCounter = 0;

        public List<DateTime> timeList; //BlinkMeasure.csにてデータ追加
        public List<int> blinkCounterPerMinute; //BlinkMeasure.csにてデータ追加
        public List<int> sleepCounterPerMinute; //BlinkMeasure.csにてデータ追加
        public List<DateTime> blinkTimeStamp; //BlinkMeasure.csにてデータ追加

        private int terms = 0; //0:明, 1:集中時暗, 2:集中時明

        private List<int> preMeasureList; // 事前計測用のList
        private float preAverage = 0; // 事前計測の平均
        
        private int isConcentrate = 0;// -1:down, 0:normal 1:up
        private float lightChangeInterval = 30; 
        private float v = 1; // 現在の明度
        private bool BorW = false; // t:black, f:white
        private int lightch = 0; // 1:blight 0:keep -1:dark

        private Color contextColor = UnityEngine.Color.HSVToRGB(0,0,1);
        private Renderer contextRenderer;

        private Color termsColor = new Color(1.0f,0.0f,0.0f,1.0f);
        private Renderer termsRenderer;

        private VideoController video1;
        private VideoController video2;
        private VideoController video3;

        private bool isPlay1;
        private bool isPlay2;
        private bool isPlay3;

        private int videoCounter = 0;

        // デバッグ用
        /*
        private string left = "l"; // left eye 1:close
        private string right = "r";// right eye 1:close
        private string blinkCountString = "b"; 
        private string timeString = "t";
        private string sleepString = "s";
        private string result = "re";
        private string video1str = "1";
        private string video2str = "2";
        private string video3str = "3";
        private string videoStr = "0";
        private bool isEnd = false;
        */
        void Start()
        {
            faceExpressions = GetComponent<OVRFaceExpressions>();
            contextRenderer = contextObj.GetComponent<Renderer>();
            contextRenderer.material.color = contextColor;

            termsRenderer = termsObj.GetComponent<Renderer>();
            termsRenderer.material.color = new Color(1.0f,0.0f,0.0f,1.0f);

            video1 = videoObj1.GetComponent<VideoController>();
            video2 = videoObj2.GetComponent<VideoController>();
            video3 = videoObj3.GetComponent<VideoController>();
           
            isPlay1 = video1.isPlay;
            isPlay2 = video2.isPlay;
            isPlay3 = video3.isPlay;

            timeList = new List<DateTime>();
            blinkCounterPerMinute = new List<int>();
            sleepCounterPerMinute = new List<int>();
            preMeasureList = new List<int>();
            blinkTimeStamp = new List<DateTime>();

            //デバッグ用
            /*
            TestText.SetText("Start");
            video1str = video1.shuryostr;
            video2str = video2.shuryostr;
            video3str = video3.shuryostr;
            */
        }

        // Update is called once per frame
        void Update()
        {
            VideoEnd();
            if(!isPreMeasure && !isMeasure) // face確認用
            {
                if(faceExpressions.FaceTrackingEnabled && faceExpressions.ValidExpressions)
                {
                    if(faceExpressions[OVRFaceExpressions.FaceExpression.EyesClosedL] > 0.9 && faceExpressions[OVRFaceExpressions.FaceExpression.EyesClosedR] > 0.9)
                    {
                        blinkObj.SetActive(true);
                    }
                    else
                    {
                        blinkObj.SetActive(false);
                    }
                }
            }

            if(!isPreMeasure && isMeasure) // 計測開始時初期化
            {
                isConcentrate = 0;
                blinkCounter = 0;
                timeCounter = 0;
                blinktimer = 0;
                sleepCounter = 0;
                sleepTime = 0;
                preAverage = 0;
                v = 1;
                lightch  = 0;
                BorW = false;
                contextRenderer.material.color = UnityEngine.Color.HSVToRGB(0,0,1);
                timeList = new List<DateTime>();
                blinkCounterPerMinute = new List<int>();
                sleepCounterPerMinute = new List<int>();
                preMeasureList = new List<int>();
                blinkTimeStamp = new List<DateTime>();
                termsObj.SetActive(false);
                blinkObj.SetActive(false);

                /*
                isEnd = false;
                DebugText();
                */
            }

            if(OVRInput.GetDown(OVRInput.RawButton.Y)) //0:明, 1:集中時暗, 2:集中時明
            {
                terms ++;
                if(terms > 2)
                {
                    terms = 0;
                }
                if(terms == 0)
                {
                    termsColor = new Color(1.0f,0.0f,0.0f,1.0f); //red
                    termsRenderer.material.color = termsColor;
                }
                else if(terms == 1)
                {
                    termsColor = new Color(0.0f,1.0f,0.0f,1.0f); //blue
                    termsRenderer.material.color = termsColor;
                }
                else
                {
                    termsColor = new Color(0.0f,0.0f,1.0f,1.0f); //green
                    termsRenderer.material.color = termsColor;
                }
            }

            if(OVRInput.GetDown(OVRInput.RawButton.X))
            {
                videoCounter ++;
                if(videoCounter > 2)
                {
                    videoCounter = 0;
                }
            }

            if(isMeasure)
            {
                // termsObj.SetActive(false);

                blinktimer += Time.deltaTime;

                // デバッグ用
                //DebugText();

                BlinkCount();
                SleepCount();

                LightChange();

                // 60sごと記録
                if(blinktimer >= interval)
                {
                    timeList.Add(DateTime.Now);
                    blinkCounterPerMinute.Add(blinkCounter);
                    sleepCounterPerMinute.Add(sleepCounter);
                    timeCounter ++;

                    if(timeCounter <= 5)
                    {
                        preMeasureList.Add(blinkCounter);
                    }

                    if(timeCounter == 5)
                    {
                        preAverage = (float)preMeasureList.Average();


                        //DebugText(); //デバッグ用
                    }

                    if (timeCounter >= 5)
                    {
                        UporDown();
                        LightChangeOrNot();
                    }

                    blinkCounter = 0;
                    blinktimer = 0;
                    sleepCounter = 0;
                }
            }
            else
            {
                termsObj.SetActive(true);
            }
            if(isPreMeasure && !isMeasure)
            {
                contextRenderer.material.color = UnityEngine.Color.HSVToRGB(0,0,1);
                BorW = false;
                // デバッグ用
                /*
                isEnd = true;
                DebugText();
                */
            }
        }

        private void BlinkCount()
        {
            // フェイストラッキングの有効時

            if(faceExpressions.FaceTrackingEnabled && faceExpressions.ValidExpressions)
            {
                if(faceExpressions[OVRFaceExpressions.FaceExpression.EyesClosedL] > 0.9 && faceExpressions[OVRFaceExpressions.FaceExpression.EyesClosedR] > 0.9)
                {
                    isEyeClose = true;
                }
                else
                {
                    isEyeClose = false;
                }

                if(isPreEyeClose && !isEyeClose)
                {
                    blinkCounter ++;
                    blinkTimeStamp.Add(DateTime.Now);
                }

                isPreEyeClose = isEyeClose;
            }
        }

        private void SleepCount()
        {
            // 5秒目をつぶると寝ている判定

            if(isPreEyeClose && isEyeClose)
            {
                sleepTime += Time.deltaTime;
            }
            else
            {
                sleepTime = 0;
                isPreSleep = isSleep;
                isSleep = false;
            }

            if(sleepTime >= sleepInterval)
            {
                isPreSleep = isSleep;
                isSleep = true;
            }

            if(isPreSleep && !isSleep)
            {
                sleepCounter ++;
            }
        }

        private void UporDown()
        {
            if(sleepCounter > 0)
            {
                isConcentrate = -1;
            }
            else
            {
                if(blinkCounter > preAverage)
                {
                    isConcentrate = -1; //down
                }
                else if(blinkCounter < preAverage)
                {
                    isConcentrate = 1; //up
                }
                else
                {
                    isConcentrate = 0; //normal
                }
            }
        }

        private void ChangeToBlight()
        {
            var x = blinktimer / lightChangeInterval;
            if(x <= 1)
            {
                v = x;
                contextColor = UnityEngine.Color.HSVToRGB(0,0,v);
                contextRenderer.material.color = contextColor;
                BorW = false;
            }
        }

        private void ChangeToDark()
        {
            var x = blinktimer / lightChangeInterval;
            if(x <= 1)
            {
                v = 1 - x;
                contextColor = UnityEngine.Color.HSVToRGB(0,0,v);
                contextRenderer.material.color = contextColor;
                BorW = true;
            }
        }


        private void LightChange()
        {
            if(timeCounter >= 5)
            {
                if(lightch == 1) // 1:blight 0:keep -1:dark
                {
                    ChangeToBlight();
                }
                else if(lightch == -1)
                {
                    ChangeToDark();
                }
                else
                {
                    // keep
                }
            }
        }

        private void LightChangeOrNot()
        {
            if(terms == 1) //0:明, 1:集中時暗, 2:集中時明
            {
                if(!BorW && isConcentrate == 1) // white & up
                {
                    lightch = -1; // 1:blight 0:keep -1:dark
                }
                else if(BorW && isConcentrate == -1) // black & down
                {
                    lightch = 1;
                }
                else
                {
                    lightch = 0;
                }
            }
            else if(terms == 2)
            {
                if(BorW && isConcentrate == 1) // black & up
                {
                    lightch = 1;
                }
                else if(!BorW && isConcentrate == -1) // white & down
                {
                    lightch = -1;
                }
                else
                {
                    lightch = 0;
                }
            }
        }

        private void VideoEnd()
        {
            //Delay10();
            /*
            video1str = video1.isPlay.ToString();
            video2str = video2.isPlay.ToString();
            video3str = video3.isPlay.ToString();
            */

            isPlay1 = video1.isPlay;
            isPlay2 = video2.isPlay;
            isPlay3 = video3.isPlay;
            if(videoCounter == 0)
            {
                isPreMeasure = isMeasure;
                isMeasure = isPlay1;
                // videoStr = "1";
            }
            else if(videoCounter == 1)
            {
                isPreMeasure = isMeasure;
                isMeasure = isPlay2;
                // videoStr = "2";
            }
            else
            {
                isPreMeasure = isMeasure;
                isMeasure = isPlay3;
                // videoStr  = "3";
            }
        }
        /*
        private void DebugText()
        {
            video1str = video1.isPlay.ToString();
            video2str = video2.isPlay.ToString();
            video3str = video3.isPlay.ToString();

            left = faceExpressions[OVRFaceExpressions.FaceExpression.EyesClosedL].ToString();
            right = faceExpressions[OVRFaceExpressions.FaceExpression.EyesClosedR].ToString();
            blinkCountString = blinkCounter.ToString();
            timeString = blinktimer.ToString();
            sleepString = sleepCounter.ToString();
            result =  "Left:" + left + Environment.NewLine 
                    + "Right:" + right + Environment.NewLine 
                    + "BlinkCounter:" + blinkCountString + Environment.NewLine 
                    + "SleepCount:" + sleepString + Environment.NewLine 
                    + "Time:" + timeString + Environment.NewLine
                    + "video1:" + video1str + Environment.NewLine
                    + "video2:" + video2str + Environment.NewLine
                    + "video3:" + video3str + Environment.NewLine
                    + "video:"  + videoStr  + Environment.NewLine
                    + "isEnd:" + isEnd.ToString();

            if(timeCounter >= 5)
            {
                result = result + Environment.NewLine
                        + "pre" + preMeasureList[0].ToString() 
                        + ":"   + preMeasureList[1].ToString()
                        + ":"   + preMeasureList[2].ToString()
                        + ":"   + preMeasureList[3].ToString()
                        + ":"   + preMeasureList[4].ToString() + Environment.NewLine
                        + "preave" + preAverage.ToString();
            }
            TestText.SetText(result);
        }
        */
        static async void Delay10(){await Task.Delay(10);} 
    }
// }
