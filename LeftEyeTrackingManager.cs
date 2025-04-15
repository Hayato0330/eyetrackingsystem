using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.XR;
using System.Linq;
using System.Text;
using TMPro;


public class LeftEyeTrackingManager : MonoBehaviour
{
	[SerializeField] public Transform _videoObject; // videoのオブジェクト
	[SerializeField] public Transform _rayObject; // rayのオブジェクト
	[SerializeField] public Transform _hitObject; // hitの点のオブジェクト
	[SerializeField] public MeshFilter _screenQuadMeshFilter; // QuadのMesh
	[SerializeField] public MeshCollider _screenQuadMeshCollider; // quadのMeshCollider

	[SerializeField] private GameObject videoObj1;
    [SerializeField] private GameObject videoObj2;
    [SerializeField] private GameObject videoObj3;

	// デバッグ用
	[SerializeField] private TextMeshProUGUI TestText;

	public List<float> aoiTimePerMinute; //LeftEyeTrakingManager.csにてデータ追加
	public List<DateTime> aoiVectorTimeStamp;
	public List<Vector3> aoiVector3List; //LeftEyeTrackingManager.csにてデータ追加
	private float aoiTimer;
	private float interval = 60; 
	private float aoiVectorInteval = 0.2f;
	private float timer;
	private float aoiVectorTimer;
	private bool isMeasure = false;
	private bool isPreMeasure = false;
	
	// デバッグ用
	private string aoiTimeString = "t";

	private float _rayCastDistance = 100f;
	
	private Ray rayLeft;
	private Plane plane;

	private List<Vector3> _meshVertices; // Meshの４頂点をワールド座標に変換して格納するためのリスト
	private float _meshWidth, _meshHeight; // Quadの幅と高さ

	OVREyeGaze eyeGaze;

	private VideoController video1;
    private VideoController video2;
    private VideoController video3;

    private bool isPlay1;
    private bool isPlay2;
    private bool isPlay3;

	private int videoCounter = 0;

	void Start()
	{
		video1 = videoObj1.GetComponent<VideoController>();
        video2 = videoObj2.GetComponent<VideoController>();
        video3 = videoObj3.GetComponent<VideoController>();
    
        isPlay1 = video1.isPlay;
        isPlay2 = video2.isPlay;
        isPlay3 = video3.isPlay;

		eyeGaze = GetComponent<OVREyeGaze>();
		rayLeft = new Ray(_rayObject.position, _rayObject.forward);
		plane = new Plane(_videoObject.up, _videoObject.position);

		aoiTimer = 0;
		timer = 0;
		aoiVectorTimer = 0;

        TestText.SetText("Start");


		// 頂点を取得しワールド座標に変換
		_meshVertices = _screenQuadMeshFilter.mesh.vertices.Select(pos => _screenQuadMeshFilter.transform.TransformPoint(pos)).ToList();
        _meshWidth = _meshVertices[1].x - _meshVertices[0].x;
        _meshHeight = _meshVertices[2].y - _meshVertices[0].y;

		aoiTimePerMinute = new List<float>();
		aoiVectorTimeStamp = new List<DateTime>();
		aoiVector3List = new List<Vector3>();
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

        if(!isPreMeasure && isMeasure)
        {
            aoiTimer = 0;
			timer = 0;
			aoiVectorTimer = 0;
			//TestText.SetText("1");

			aoiTimePerMinute = new List<float>();
			aoiVectorTimeStamp = new List<DateTime>();
			aoiVector3List = new List<Vector3>();
        }
		if(isMeasure)
		{
			//TestText.SetText("2");
			timer += Time.deltaTime;
			aoiVectorTimer += Time.deltaTime;

			if(eyeGaze == null)
			{
				return;
			}

			if(eyeGaze.EyeTrackingEnabled)
			{
				//TestText.SetText("3");

				_rayObject.rotation = eyeGaze.transform.rotation;
				_rayObject.position = eyeGaze.transform.position;
				rayLeft = new Ray(_rayObject.position, _rayObject.forward);

				
				aoiTimeString = aoiTimer.ToString() +":"+ aoiVector3List.Count.ToString() +":"+ aoiTimePerMinute.Count.ToString();
				TestText.SetText(aoiTimeString);
				

				// rayとplaneとの当たり判定

				var isHit = plane.Raycast(rayLeft, out var enter); // ヒット場合はenterに平面までの距離が格納

				_hitObject.gameObject.SetActive(isHit); // hitした場合のみオブジェクトを表示

				if(isHit)
				{
					_hitObject.position = rayLeft.GetPoint(enter);
				}
				
				if(aoiVectorTimer >= aoiVectorInteval)
				{
				// TestText.SetText("4");
				aoiVector3Save();
				aoiVectorTimer = 0;
				// TestText.SetText("5");
				}

				HitQuad();

				// 60sごと記録
				if(timer >= interval)
				{
					aoiTimePerMinute.Add(aoiTimer);
					aoiTimer = 0;
					timer = 0;
				}
			}
		}
	}

	private void HitQuad()
	{
		if(_screenQuadMeshCollider.Raycast(rayLeft, out var screenHitInfo, _rayCastDistance))
		{
			var p = screenHitInfo.point;
			aoiTimer += Time.deltaTime;
		}
	}

	private void VideoEnd()
    {
        isPlay1 = video1.isPlay;
        isPlay2 = video2.isPlay;
        isPlay3 = video3.isPlay;
        if(videoCounter == 0)
        {
            isPreMeasure = isMeasure;
            isMeasure = isPlay1;
        }
        else if(videoCounter == 1)
        {
            isPreMeasure = isMeasure;
            isMeasure = isPlay2;
        }
        else
        {
            isPreMeasure = isMeasure;
            isMeasure = isPlay3;
        }
    }

	private void aoiVector3Save()
	{
		aoiVectorTimeStamp.Add(DateTime.Now);
		aoiVector3List.Add(_hitObject.position);
	}
}
