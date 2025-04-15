using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{

    [SerializeField] VideoPlayer videoPlayer;
    /*
    [SerializeField] private GameObject FaceObj;
    [SerializeField] private GameObject SaveObj;

    private BlinkMeasure blink;
    private SaveToCSV save;
    */

    public bool isPlay = false;

    // �f�o�b�O�p
    public string shuryostr = "start";

    void Start() {
        /*
        blink = FaceObj.GetComponent<BlinkMeasure>();
        save = SaveObj.GetComponent<SaveToCSV>();
        */

        isPlay = false;

        shuryostr = "start";
    }

    void OnEnable() {
        /*
        NOTE:
        �C�ӂ̃^�C�~���O�� this.videoPlayer.Play(); ����x�ĂԂ����œ���Đ����ł���悤�ɁC���������D
        this.videoPlayer.Prepare(); �����s����� Prepared ��Ԃɂł���C��doc�ɂ͏����Ă��������CPlay ���x���s���Ȃ��ƍĐ��ł����s���R�������̂ŁC�����Ă�������D
        */
        this.videoPlayer.Play();
        this.videoPlayer.Pause();
        this.videoPlayer.time = 0.0;
        isPlay = false;
    }

    // Update is called once per frame
    void Update() {
        if ( OVRInput.GetDown(OVRInput.RawButton.A) ) {
            if (this.videoPlayer.isPaused) {
                this.videoPlayer.Play();
                shuryostr = "play";
                isPlay = true;
            } else {
                this.videoPlayer.Pause();
                shuryostr = "pause";
                isPlay = false;
            }
        }

        if ( OVRInput.GetDown(OVRInput.RawButton.B) ) {
            this.videoPlayer.time = 0.0;
        }

        /*
        �����Alength����Ђ��Ȃ��ƁAnull�ɂȂ��Ă��܂��Aif���������Ȃ�
        �f�o�b�O�ł�0.01�ł́A�_���B0.05��ok�B�O�̂��߁A0.1�ɂ��Ă���
        */

        if(this.videoPlayer.time >= this.videoPlayer.length - 0.1) 
        {
            shuryostr = "end";
            isPlay = false;
            /*
            blink.isMeasure = !blink.isMeasure;
            save.isSave = !save.isSave;
            */
        }
    }

    static async void Delay100(){await Task.Delay(100);} 
}
