using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoControllerManager : MonoBehaviour
{
    [SerializeField] GameObject videoPlayerObjA;
    [SerializeField] GameObject videoPlayerObjB;
    [SerializeField] GameObject videoPlayerObjC;

    bool isEnabledA = true;
    bool isEnabledB = false;
    bool isEnabledC = false;

    int videoCounter = 0; // 0:A, 1:B, 2:C
    

    // Start is called before the first frame update
    void Start()
    {
        videoPlayerObjA.SetActive(isEnabledA);
        videoPlayerObjB.SetActive(isEnabledB);
        videoPlayerObjC.SetActive(isEnabledC);
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.X))
        {
            videoCounter++;
            if(videoCounter > 2){videoCounter = 0;}
            if(videoCounter == 0)
            {
                isEnabledA = true;
                isEnabledB = false;
                isEnabledC = false;
                videoPlayerObjA.SetActive(isEnabledA);
                videoPlayerObjB.SetActive(isEnabledB);
                videoPlayerObjC.SetActive(isEnabledC);
            }
            else if(videoCounter == 1)
            {
                isEnabledA = false;
                isEnabledB = true;
                isEnabledC = false;
                videoPlayerObjA.SetActive(isEnabledA);
                videoPlayerObjB.SetActive(isEnabledB);
                videoPlayerObjC.SetActive(isEnabledC);
            }
            else
            {
                isEnabledA = false;
                isEnabledB = false;
                isEnabledC = true;
                videoPlayerObjA.SetActive(isEnabledA);
                videoPlayerObjB.SetActive(isEnabledB);
                videoPlayerObjC.SetActive(isEnabledC);
            }
        }
    }
}
