using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CameraManager : CommandReceiver
{
	protected Dictionary<string, GameCamera> mCameraList;
	protected GameCamera mMainCamera;
	protected GameCamera mUIEffectCamera;
	protected GameCamera mUICamera;
	public CameraManager()
		:
		base(typeof(CameraManager).ToString())
	{
		mCameraList = new Dictionary<string,GameCamera>();
	}
	public void init()
	{
		// 默认获取主摄像机
		mMainCamera = getCamera("Main Camera");
		mUIEffectCamera = getCamera("EffectCamera");
		mUICamera = getCamera("UICamera");
	}
	public override void destroy()
	{
		base.destroy();
	}
	public void update(float elapsedTime)
	{
		foreach(var camera in mCameraList)
		{
			camera.Value.update(elapsedTime);
		}
	}
	// 获得摄像机,名字是场景中摄像机的名字
	public GameCamera getCamera(string name)
	{
		if(mCameraList.ContainsKey(name))
		{
			return mCameraList[name];
		}
		else
		{
			GameCamera camera = null;
			GameObject obj = UnityUtility.getGameObject(null, name);
			if(obj != null)
			{
				camera = new GameCamera(obj);
				camera.init();
				mCameraList.Add(camera.getName(), camera);
			}
			return camera;
		}
	}
	public GameCamera getMainCamera()
	{
		return mMainCamera;
	}
	public GameCamera getUIEffectCamera()
	{
		return mUIEffectCamera;
	}
	public GameCamera getUICamera()
	{
		return mUICamera;
	}
}