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

    // デバッグ用
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
        任意のタイミングで this.videoPlayer.Play(); を一度呼ぶだけで動画再生ができるように，こうした．
        this.videoPlayer.Prepare(); を実行すれば Prepared 状態にできる，とdocには書いてあったが，Play を二度実行しないと再生できず不自然だったので，あえてこうする．
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
        少し、lengthからひかないと、nullになってしまい、if文が動かない
        デバッグでは0.01では、ダメ。0.05はok。念のため、0.1にしておく
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
