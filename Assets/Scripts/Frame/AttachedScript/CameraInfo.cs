using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CameraInfo : MonoBehaviour
{
	protected GameCamera mGameCamera;
	public string mCurLinkerName;
	public string mLinkedObjectName;
	public GameObject mLinkedObject;
	public Vector3 mRelative;
	public Vector3 mCurRelative;
	public void Awake()
	{
		;
	}
	public void Update()
	{
		updateInfo();
	}
	private void OnDisable()
	{
		updateInfo();
	}
	public void setCamera(GameCamera camera)
	{
		mGameCamera = camera;
	}
	//-------------------------------------------------------------------------------------------------------
	protected void updateInfo()
	{
		if(mGameCamera == null)
		{
			return;
		}
		CameraLinker linker = mGameCamera.getCurLinker();
		if (linker != null)
		{
			mCurLinkerName = linker.getName();
			mLinkedObject = linker.getLinkObject().getObject();
			mLinkedObjectName = linker.getLinkObject().getName();
			mRelative = linker.getRelativePosition();
			if(mLinkedObject != null)
			{
				mCurRelative = mGameCamera.getWorldPosition() - mLinkedObject.transform.position;
			}
			else
			{
				mCurRelative = Vector3.zero;
			}
		}
		else
		{
			mCurLinkerName = "";
			mLinkedObjectName = "";
			mLinkedObject = null;
			mRelative = Vector3.zero;
			mCurRelative = Vector3.zero;
		}
	}
}