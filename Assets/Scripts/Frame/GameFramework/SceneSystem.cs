using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using UnityEngine.SceneManagement;

public class SceneSystem : FrameComponent
{
	protected Dictionary<string, SceneInstance> mSceneList;
	protected Dictionary<string, Type> mSceneRegisteList;
	public SceneSystem(string name)
		:base(name)
	{
		mSceneList = new Dictionary<string, SceneInstance>();
		mSceneRegisteList = new Dictionary<string, Type>();
	}
	public override void init()
	{
		base.init();
	}
	public override void destroy()
	{
		base.destroy();
		Dictionary<string, SceneInstance> sceneList = new Dictionary<string, SceneInstance>(mSceneList);
		foreach (var item in sceneList)
		{
			unloadScene(item.Key);
		}
	}
	public void initScene(string name)
	{
		if (!mSceneList.ContainsKey(name) || mSceneList[name].mInited)
		{
			return;
		}
		mSceneList[name].init();
	}
	public void registeScene<T>(string name) where T : SceneInstance
	{
		mSceneRegisteList.Add(name, typeof(T));
	}
	public T getScene<T>(string name) where T : SceneInstance
	{
		if (!mSceneList.ContainsKey(name))
		{
			return null;
		}
		return mSceneList[name] as T;
	}
	public void activeScene(string name, bool active = true)
	{
		if (!mSceneList.ContainsKey(name))
		{
			return;
		}
		mSceneList[name].setActive(active);
	}
	public void loadScene(string name, LoadSceneMode mode)
	{
		SceneManager.LoadScene(name, mode);
		SceneInstance scene = createScene(name);
		scene.mState = LOAD_STATE.LS_LOADED;
		scene.mOperation = null;
		scene.mScene = SceneManager.GetSceneByName(name);
		mSceneList.Add(name, scene);
	}
	public void loadSceneAsync(string name, LoadSceneMode mode, bool active, SceneLoadCallback callback, object userData = null)
	{
		SceneInstance scene = createScene(name);
		scene.mState = LOAD_STATE.LS_LOADING;
		scene.mLoadCallback = callback;
		scene.mUserData = userData;
		scene.mLoadMode = mode;
		mSceneList.Add(scene.mName, scene);
		GameBase.mMonoUtility.StartCoroutine(loadSceneCoroutine(scene, active));
	}
	public void unloadScene(string name)
	{
		if (!mSceneList.ContainsKey(name))
		{
			return;
		}
		mSceneList[name].destroy();
		// 只有叠加的场景可以卸载,其他的只能通过加载其他场景来自动卸载
		if(mSceneList[name].mLoadMode == LoadSceneMode.Additive)
		{
			SceneManager.UnloadScene(name);
		}
		mSceneList.Remove(name);
	}
	//------------------------------------------------------------------------------------------------------------------------------
	protected IEnumerator loadSceneCoroutine(SceneInstance scene, bool active)
	{
		scene.mOperation = SceneManager.LoadSceneAsync(scene.mName, scene.mLoadMode);
		scene.mOperation.allowSceneActivation = true;
		while(true)
		{
			if (scene.mLoadCallback != null)
			{
				scene.mLoadCallback(scene.mOperation, false, scene.mUserData);
			}
			// 当allowSceneActivation为true时,加载到progress为1时停止,并且isDone为true
			// 当allowSceneActivation为false时,加载到progress为0.9时就停止,并且isDone为false,当场景被激活时isDone变为true,progress也为1
			if (scene.mOperation.isDone && scene.mOperation.progress >= 1.0f)
			{
				break;
			}
			yield return null;
		}
		scene.mRoot = UnityUtility.getGameObject(null, scene.mName + "_Root", true);
		activeScene(scene.mName, active);
		scene.mState = LOAD_STATE.LS_LOADED;
		scene.mScene = SceneManager.GetSceneByName(scene.mName);
		if (scene.mLoadCallback != null)
		{
			scene.mLoadCallback(scene.mOperation, true, scene.mUserData);
			scene.mLoadCallback = null;
			scene.mUserData = null;
		}
	}
	protected SceneInstance createScene(string sceneName)
	{
		return UnityUtility.createInstance<SceneInstance>(mSceneRegisteList[sceneName], sceneName);
	}
}