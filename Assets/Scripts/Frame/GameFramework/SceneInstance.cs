using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using UnityEngine.SceneManagement;

public class SceneInstance : GameBase
{
	public string mName;
	public LOAD_STATE mState;
	public Scene mScene;
	public AsyncOperation mOperation;
	public GameObject mRoot;
	public bool mInited;
	// 以下参数仅在加载时使用
	public object mUserData;
	public SceneLoadCallback mLoadCallback;
	public LoadSceneMode mLoadMode;
	public SceneInstance(string name)
	{
		mName = name;
	}
	public virtual void init()
	{
		findGameObject();
		initGameObject();
		mInited = true;
	}
	public virtual void destroy()
	{
		mInited = false;
	}
	public void setActive(bool active)
	{
		if (mRoot != null && mRoot.activeSelf != active)
		{
			mRoot.SetActive(active);
		}
	}
	//-------------------------------------------------------------------------------------------
	protected virtual void findGameObject() { }
	protected virtual void initGameObject() { }
}