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
	public override void update(float elapsedTime)
	{
		base.update(elapsedTime);
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
		if(active && SceneManager.GetActiveScene().name != name)
		{
			SceneManager.SetActiveScene(mSceneList[name].mScene);
		}
	}
	// 目前只支持异步加载,因为SceneManager.LoadScene并不是真正地同步加载
	// 该方法只能保证在这一帧结束后场景能加载完毕,但是函数返回后场景并没有加载完毕
	public void loadSceneAsync(string name, bool active, SceneLoadCallback callback, object userData = null)
	{
		// 如果场景已经加载,则直接返回
		if (mSceneList.ContainsKey(name))
		{
			if(callback != null)
			{
				callback(1.0f, true, userData);
			}
			return;
		}
		SceneInstance scene = createScene(name);
		scene.mState = LOAD_STATE.LS_LOADING;
		mSceneList.Add(scene.mName, scene);
		mGameFramework.StartCoroutine(loadSceneCoroutine(scene, active, callback, userData));
	}
	public void unloadScene(string name)
	{
		if (!mSceneList.ContainsKey(name))
		{
			return;
		}
		mSceneList[name].destroy();
		SceneManager.UnloadSceneAsync(name);
		mSceneList.Remove(name);
	}
	// 卸载除了dontUnloadSceneName以外的其他场景,初始默认场景除外
	public void unloadOtherScene(string dontUnloadSceneName)
	{
		Dictionary<string, SceneInstance> tempList = new Dictionary<string, SceneInstance>(mSceneList);
		foreach(var item in tempList)
		{
			if(item.Key != dontUnloadSceneName)
			{
				unloadScene(item.Key);
			}
		}
	}
	//------------------------------------------------------------------------------------------------------------------------------
	protected IEnumerator loadSceneCoroutine(SceneInstance scene, bool active, SceneLoadCallback callback, object userData = null)
	{
		// 所有场景都只能使用叠加的方式来加载,方便场景管理器来管理所有场景的加载和卸载
		scene.mOperation = SceneManager.LoadSceneAsync(scene.mName, LoadSceneMode.Additive);
		// allowSceneActivation指定了加载场景时是否需要调用场景中所有脚本的Awake和Start,以及贴图材质的引用等等
		scene.mOperation.allowSceneActivation = true;
		while(true)
		{
			if (callback != null)
			{
				callback(scene.mOperation.progress, false, userData);
			}
			// 当allowSceneActivation为true时,加载到progress为1时停止,并且isDone为true,scene.isLoaded为true
			// 当allowSceneActivation为false时,加载到progress为0.9时就停止,并且isDone为false, scene.isLoaded为false
			// 当场景被激活时isDone变为true,progress也为1,scene.isLoaded为true
			if (scene.mOperation.isDone || scene.mOperation.progress >= 1.0f)
			{
				break;
			}

			yield return null;
		}
		// 首先获得场景
		scene.mScene = SceneManager.GetSceneByName(scene.mName);
		// 获得了场景根节点才能使场景显示或隐藏
		scene.mRoot = UnityUtility.getGameObject(null, scene.mName + "_Root", true);
		activeScene(scene.mName, active);
		scene.mState = LOAD_STATE.LS_LOADED;
		if (callback != null)
		{
			callback(1.0f, true, userData);
		}
	}
	protected SceneInstance createScene(string sceneName)
	{
		return UnityUtility.createInstance<SceneInstance>(mSceneRegisteList[sceneName], sceneName);
	}
}