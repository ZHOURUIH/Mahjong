using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using UnityEngine.SceneManagement;

public class SceneSystem : MonoBehaviour
{
	protected Dictionary<string, SceneInstance> mSceneList;
	protected Dictionary<string, Type> mSceneRegisteList;
	public SceneSystem()
	{
		mSceneList = new Dictionary<string, SceneInstance>();
		mSceneRegisteList = new Dictionary<string, Type>();
	}
	public void init()
	{
		;
	}
	public void destroy()
	{
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
		scene.mOperation = SceneManager.LoadSceneAsync(name, mode);
		scene.mOperation.allowSceneActivation = true;
		mSceneList.Add(name, scene);
		StartCoroutine(loadSceneCoroutine(scene, active, callback, userData));
	}
	public void unloadScene(string name)
	{
		if (!mSceneList.ContainsKey(name))
		{
			return;
		}
		mSceneList[name].destroy();
		SceneManager.UnloadScene(name);
		mSceneList.Remove(name);
	}
	//------------------------------------------------------------------------------------------------------------------------------
	protected IEnumerator loadSceneCoroutine(SceneInstance scene, bool active, SceneLoadCallback callback, object userData)
	{
		string sceneName = scene.mName;
		while(true)
		{
			if (callback != null)
			{
				callback(scene.mOperation, false, userData);
			}
			// 当allowSceneActivation为true时,加载到progress为1时停止,并且isDone为true
			// 当allowSceneActivation为false时,加载到progress为0.9时就停止,并且isDone为false,当场景被激活时isDone变为true,progress也为1
			if (scene.mOperation.isDone && scene.mOperation.progress >= 1.0f)
			{
				break;
			}
			yield return null;
		}
		scene.mRoot = UnityUtility.getGameObject(null, sceneName + "_Root");
		if(scene.mRoot == null)
		{
			UnityUtility.logError(sceneName + " scene must have a root node with name : " + sceneName + "_Root");
		}
		activeScene(sceneName, active);
		scene.mState = LOAD_STATE.LS_LOADED;
		scene.mScene = SceneManager.GetSceneByName(sceneName);
		if (callback != null)
		{
			callback(scene.mOperation, true, userData);
		}
	}
	protected SceneInstance createScene(string sceneName)
	{
		return UnityUtility.createInstance<SceneInstance>(mSceneRegisteList[sceneName], sceneName);
	}
}