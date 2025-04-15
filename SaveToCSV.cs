using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;
using UnityEngine.UI;
using TMPro;
//using BlinkMeasure;
//using LeftEyeTrackingManager;

public class SaveToCSV : MonoBehaviour
{
    [SerializeField] private GameObject LogObj;
    [SerializeField] private GameObject EyeObj;
    [SerializeField] private GameObject FaceObj;
    [SerializeField] private GameObject videoObj1;
    [SerializeField] private GameObject videoObj2;
    [SerializeField] private GameObject videoObj3;

    [SerializeField] private TextMeshProUGUI TestText;

    public bool isSave = false; // start or stop
    public bool isPreSave = false; // start or stop
    private Log Log;
    private BlinkMeasure blink;
    private LeftEyeTrackingManager aoi;

    private int videoCounter = 0;
    
    private VideoController video1;
    private VideoController video2;
    private VideoController video3;

    private bool isPlay1;
    private bool isPlay2;
    private bool isPlay3;

    private int maxLength;

    void Start()
    {
        blink = FaceObj.GetComponent<BlinkMeasure>();
        aoi = EyeObj.GetComponent<LeftEyeTrackingManager>();
        Log = LogObj.GetComponent<Log>();

        video1 = videoObj1.GetComponent<VideoController>();
        video2 = videoObj2.GetComponent<VideoController>();
        video3 = videoObj3.GetComponent<VideoController>();
           
        isPlay1 = video1.isPlay;
        isPlay2 = video2.isPlay;
        isPlay3 = video3.isPlay;

        videoCounter = 0;

        //TestText.SetText("1");
    }

    void Update()
    {
        if(OVRInput.GetDown(OVRInput.RawButton.X))
        {
            videoCounter ++;
            if(videoCounter > 2)
            {
                videoCounter = 0;
            }
        }

        VideoEnd();

        if(isPreSave && !isSave)
        {
            //TestText.SetText("2");

            var fileNameminute = "honzikkendata_minute_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
            var fileNameblinkstamp = "honzikkendata_blinkstamp_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
            var fileNameaoi = "honzikkendata_aoi_" + DateTime.Now.ToString("yyyyMMddHHmmss")+ ".csv";


            SaveMinute(
                fileNameminute,
                blink.timeList,
                blink.blinkCounterPerMinute, 
                blink.sleepCounterPerMinute, 
                aoi.aoiTimePerMinute
            );

            SaveBlinkStamp(fileNameblinkstamp, blink.blinkTimeStamp);

            SaveAOI(fileNameaoi, aoi.aoiVectorTimeStamp, aoi.aoiVector3List);
        }
    }

    private void SaveMinute(string fn, List<DateTime> tlm, List<int> bc, List<int> sc, List<float> at)
    {
        //TestText.SetText("3");

        Log.Output(fn, ListToCSVMinute(tlm,bc,sc,at));
    }

    private List<string> ListToCSVMinute(List<DateTime> tlm, List<int> bc, List<int> sc, List<float> at)
    {
        //TestText.SetText("4");

        List<string> str = new List<string>(){"time,blinkcount,sleepcount,aoitime"};
        
        //TestText.SetText("5");

        for (int i = 0; i < tlm.Count; i++)
        {
            str.Add(string.Join(",", new List<string>()
            {
                tlm[i].ToString(), 
                bc[i].ToString(), 
                sc[i].ToString(), 
                at[i].ToString()
            }));
        }

        //TestText.SetText("8");

        return str;
    }

    private void SaveBlinkStamp(string fn, List<DateTime> bt)
    {
        Log.Output(fn,ListToCSVBlinkStamp(bt));
    }

    private List<string> ListToCSVBlinkStamp(List<DateTime> bt)
    {
        List<string> str = new List<string>(){"blinktimestamp"};

        for (int i = 0; i < bt.Count; i++)
        {
            str.Add(bt[i].ToString());
        }

        return str;
    }

    private void SaveAOI(string fn, List<DateTime> tlv, List<Vector3> hit)
    {
        Log.Output(fn,ListToCSVAOI(tlv,hit));
    }

    private List<string> ListToCSVAOI(List<DateTime> tlv, List<Vector3> hit)
    {
        List<string> str = new List<string>(){"aoiTime,aoix,aoiy,aoiz"};

        for (int i = 0; i < tlv.Count; i++)
        {
            str.Add(string.Join(",", new List<string>()
            {
                tlv[i].ToString(),
                hit[i].x.ToString(),
                hit[i].y.ToString(),
                hit[i].z.ToString()
            }));
        }

        return str;
    }

    private void VideoEnd()
    {
        isPlay1 = video1.isPlay;
        isPlay2 = video2.isPlay;
        isPlay3 = video3.isPlay;

        if(videoCounter == 0)
        {
            isPreSave = isSave;
            isSave = isPlay1;
        }
        else if(videoCounter == 1)
        {
            isPreSave = isSave;
            isSave = isPlay2;
        }
        else
        {
            isPreSave = isSave;
            isSave = isPlay3;
        }
    }
}
