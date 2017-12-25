using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using UnityEngine.SceneManagement;

public class SceneInstance
{
	public LOAD_STATE mState;
	public Scene mScene;
	public AsyncOperation mOperation;
	public GameObject mRoot;
}

public class SceneSystem : MonoBehaviour
{
	protected Dictionary<string, SceneInstance> mSceneList;
	public SceneSystem()
	{
		mSceneList = new Dictionary<string, SceneInstance>();
	}
	public void init()
	{
		;
	}
	public void destroy()
	{
		mSceneList.Clear();
	}
	public void unloadScene(string name)
	{
		SceneManager.UnloadScene(name);
		mSceneList.Remove(name);
	}
	public GameObject getSceneRoot(string name)
	{
		if (!mSceneList.ContainsKey(name))
		{
			return null;
		}
		return mSceneList[name].mRoot;
	}
	public void activeScene(string name, bool active = true)
	{
		if (!mSceneList.ContainsKey(name))
		{
			return;
		}
		if (mSceneList[name].mRoot != null && mSceneList[name].mRoot.activeSelf != active)
		{
			mSceneList[name].mRoot.SetActive(active);
		}
	}
	public void loadScene(string name, LoadSceneMode mode)
	{
		SceneManager.LoadScene(name, mode);
		SceneInstance scene = new SceneInstance();
		scene.mState = LOAD_STATE.LS_LOADED;
		scene.mOperation = null;
		scene.mScene = SceneManager.GetSceneByName(name);
		mSceneList.Add(name, scene);
	}
	public void loadSceneAsync(string name, LoadSceneMode mode, bool active, SceneLoadCallback callback, object userData = null)
	{
		StartCoroutine(loadSceneCoroutine(name, mode, active, callback, userData));
	}
	//------------------------------------------------------------------------------------------------------------------------------
	protected IEnumerator loadSceneCoroutine(string name, LoadSceneMode mode, bool active, SceneLoadCallback callback, object userData)
	{
		SceneInstance scene = new SceneInstance();
		scene.mState = LOAD_STATE.LS_LOADING;
		scene.mOperation = SceneManager.LoadSceneAsync(name, mode);
		scene.mOperation.allowSceneActivation = true;
		mSceneList.Add(name, scene);
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
		scene.mRoot = UnityUtility.getGameObject(null, name + "_Root");
		if(scene.mRoot == null)
		{
			UnityUtility.logError(name + " scene must have a root node with name : " + name + "_Root");
		}
		activeScene(name, active);
		scene.mState = LOAD_STATE.LS_LOADED;
		scene.mScene = SceneManager.GetSceneByName(name);
		if (callback != null)
		{
			callback(scene.mOperation, true, userData);
		}
	}
}