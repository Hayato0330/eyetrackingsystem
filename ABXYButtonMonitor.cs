using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ABXYButtonMonitor : MonoBehaviour
{
    [SerializeField] GameObject objA;
    [SerializeField] GameObject objB;
    [SerializeField] GameObject objX;
    [SerializeField] GameObject objY;

    bool isEnabledA = false;
    bool isEnabledB = false;
    bool isEnabledX = false;
    bool isEnabledY = false;

    // Start is called before the first frame update
    void Start()
    {
        objA.SetActive(isEnabledA);
        objB.SetActive(isEnabledB);
        objX.SetActive(isEnabledX);
        objY.SetActive(isEnabledY);
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.A)){
            isEnabledA = !isEnabledA;
            objA.SetActive(isEnabledA);
        }
        if (OVRInput.GetDown(OVRInput.RawButton.B)){
            isEnabledB = !isEnabledB;
            objB.SetActive(isEnabledB);
        }
        if (OVRInput.GetDown(OVRInput.RawButton.X)){
            isEnabledX = !isEnabledX;
            objX.SetActive(isEnabledX);
        }
        if (OVRInput.GetDown(OVRInput.RawButton.Y)){
            isEnabledY = !isEnabledY;
            objY.SetActive(isEnabledY);
        }
    }
}
