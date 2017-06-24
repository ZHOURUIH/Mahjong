using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GameCamera : ComponentOwner
{
	protected GameObject mCameraObject;
	protected Transform mTransform;
	protected Camera mCamera;
	public GameCamera(GameObject obj)
		:
		base(obj.name)
	{
		mCameraObject = obj;
		mTransform = mCameraObject.transform;
		mCamera = mCameraObject.GetComponent<Camera>();
	}
	public void init()
	{
		initComponents();
	}
	public override void initComponents()
	{
		addComponent<CameraComponentRotateSpeed>(typeof(CameraComponentRotateSpeed).ToString()).setActive(false);
	}
	public override void destroy()
	{
		base.destroy();
	}
	public void update(float elapsedTime)
	{
		base.updateComponents(elapsedTime);
	}
	// 角度制的欧拉角
	public void setRotation(Vector3 rot)
	{
		mTransform.localEulerAngles = rot;
	}
	public void setPosition(Vector3 pos)
	{
		mTransform.localPosition = pos;
	}
	public Vector3 getRotation()
	{
		return mTransform.localEulerAngles;
	}
	public Vector3 getPosition()
	{
		return mTransform.localPosition;
	}
	public Camera getCamera()
	{
		return mCamera;
	}
	public void setActive(bool active)
	{
		mCameraObject.SetActive(active);
	}
	public string getCameraLayer()
	{
		return LayerMask.LayerToName(mCameraObject.layer);
	}
}