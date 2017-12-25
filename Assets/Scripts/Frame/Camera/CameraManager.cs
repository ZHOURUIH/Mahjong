using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CameraManager : FrameComponent
{
	protected Dictionary<string, GameCamera> mCameraList;
	protected GameCamera mMainCamera;
	protected GameCamera mUICamera;
	protected GameCamera mUIForeEffectCamera;
	protected GameCamera mUIBackEffectCamera;
	protected GameCamera mUIBlurCamera;
	public CameraManager(string name)
		:base(name)
	{
		mCameraList = new Dictionary<string, GameCamera>();
	}
	public override void init()
	{
		findMainCamera();
		mUICamera = getCamera("UICamera", "UI Root");
		mUIForeEffectCamera = getCamera("UIForeEffectCamera", "UI Root");
		mUIBackEffectCamera = getCamera("UIBackEffectCamera", "UI Root");
		mUIBlurCamera = getCamera("UIBlurCamera", "UI Root");
	}
	public override void destroy()
	{
		base.destroy();
	}
	public override void update(float elapsedTime)
	{
		foreach(var camera in mCameraList)
		{
			camera.Value.update(elapsedTime);
		}
	}
	// 获得摄像机,名字是场景中摄像机的名字
	public GameCamera getCamera(string name, string parentName = "", bool createIfNull = true)
	{
		if(mCameraList.ContainsKey(name))
		{
			return mCameraList[name];
		}
		else if(createIfNull)
		{
			return createCamera(name, parentName);
		}
		return null;
	}
	public GameCamera createCamera(string name, string parentName = "")
	{
		GameObject parent = UnityUtility.getGameObject(null, parentName);
		GameCamera camera = null;
		GameObject obj = UnityUtility.getGameObject(parent, name);
		if (obj != null)
		{
			camera = new GameCamera(name);
			camera.init();
			camera.setObject(obj);
			mCameraList.Add(camera.getName(), camera);
		}
		return camera;
	}
	public void findMainCamera()
	{
		string name = "MainCamera";
		if(getCamera(name, "", false) != null)
		{
			destroyCamera(name);
		}
		mMainCamera = createCamera(name);
	}
	public GameCamera getMainCamera(){return mMainCamera;}
	public GameCamera getUICamera(){return mUICamera;}
	public GameCamera getUIForeEffectCamera(){return mUIForeEffectCamera;}
	public GameCamera getUIBackEffectCamera() {return mUIBackEffectCamera;}
	public GameCamera getUIBlurCamera() { return mUIBlurCamera; }
	public void destroyCamera(string name)
	{
		GameCamera camera = getCamera(name);
		if(camera == null)
		{
			return;
		}
		camera.destroy();
		mCameraList.Remove(name);
		if (camera == mMainCamera)
		{
			mMainCamera = null;
		}
		else if(camera == mUICamera)
		{
			mUICamera = null;
		}
		else if(camera == mUIForeEffectCamera)
		{
			mUIForeEffectCamera = null;
		}
		else if(camera == mUIBackEffectCamera)
		{
			mUIBackEffectCamera = null;
		}
		else if(camera == mUIBlurCamera)
		{
			mUIBlurCamera = null;
		}
	}
}