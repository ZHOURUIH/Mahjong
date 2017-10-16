using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CameraManager : CommandReceiver
{
	protected Dictionary<string, GameCamera> mCameraList;
	protected GameCamera mMainCamera;
	protected GameCamera mUICamera;
	protected GameCamera mUIForeEffectCamera;
	protected GameCamera mUIBackEffectCamera;
	protected GameCamera mUIBlurCamera;
	public CameraManager()
		:
		base(typeof(CameraManager).ToString())
	{
		mCameraList = new Dictionary<string, GameCamera>();
	}
	public void init()
	{
		mMainCamera = getCamera("MainCamera");
		mUICamera = getCamera("UICamera", "UI Root");
		mUIForeEffectCamera = getCamera("UIForeEffectCamera", "UI Root");
		mUIBackEffectCamera = getCamera("UIBackEffectCamera", "UI Root");
		mUIBlurCamera = getCamera("UIBlurCamera", "UI Root");
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
	public GameCamera getCamera(string name, string parentName = "")
	{
		if(mCameraList.ContainsKey(name))
		{
			return mCameraList[name];
		}
		else
		{
			GameObject parent = UnityUtility.getGameObject(null, parentName);
			GameCamera camera = null;
			GameObject obj = UnityUtility.getGameObject(parent, name);
			if(obj != null)
			{
				camera = new GameCamera(obj);
				camera.init();
				mCameraList.Add(camera.getName(), camera);
			}
			return camera;
		}
	}
	public GameCamera getMainCamera(){return mMainCamera;}
	public GameCamera getUICamera(){return mUICamera;}
	public GameCamera getUIForeEffectCamera(){return mUIForeEffectCamera;}
	public GameCamera getUIBackEffectCamera() {return mUIBackEffectCamera;}
	public GameCamera getUIBlurCamera() { return mUIBlurCamera; }
}