using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : FrameComponent
{
	public AssetBundleLoader mAssetBundleLoader;
	public ResourceLoader mResourceLoader;
	public int mLoadSource;
	public ResourceManager(string name)
		:base(name)
	{
		mAssetBundleLoader = new AssetBundleLoader();
		mResourceLoader = new ResourceLoader();
	}
	public override void init()
	{
		mAssetBundleLoader.init();
		mResourceLoader.init();
#if UNITY_EDITOR
		mLoadSource = (int)mFrameConfig.getFloatParam(GAME_DEFINE_FLOAT.GDF_LOAD_RESOURCES);
#else
		mLoadSource = 1;
#endif
	}
	public override void update(float elapsedTime)
	{
		mAssetBundleLoader.update(elapsedTime);
		mResourceLoader.update(elapsedTime);
	}
	public override void destroy()
	{
		if (mAssetBundleLoader != null)
		{
			mAssetBundleLoader.destroy();
		}
		if (mResourceLoader != null)
		{
			mResourceLoader.destroy();
		}
		base.destroy();
	}
	public void unload(string name)
	{
		// 只能用AssetBundleLoader卸载
		if (mLoadSource == 1)
		{
			mAssetBundleLoader.unload(name);
		}
	}
	// 指定资源是否已经加载
	public bool isResourceLoaded<T>(string name) where T : UnityEngine.Object
	{
		bool ret = false;
		if (mLoadSource == 0)
		{
			ret = mResourceLoader.isResourceLoaded(name);
		}
		else if (mLoadSource == 1)
		{
			ret = mAssetBundleLoader.isAssetLoaded<T>(name);
		}
		return ret;
	}
	// 获得资源
	public T getResource<T>(string name, bool errorIfNull) where T : UnityEngine.Object
	{
		T res = null;
		if (mLoadSource == 0)
		{
			res = mResourceLoader.getResource(name) as T;
		}
		else if (mLoadSource == 1)
		{
			res = mAssetBundleLoader.getAsset<T>(name);
		}
		if (res == null && errorIfNull)
		{
			logError("can not find resource : " + name);
		}
		return res;
	}
	// path为resources下相对路径,返回的列表中文件名不带路径不带后缀
	// 如果从Resource中加载,则区分大小写,如果从AssetBundle中加载,传入的路径不区分大小写,返回的文件列表全部为小写
	// lower表示是否将返回列表中的字符串全部转为小写
	public List<string> getFileList(string path, bool lower = false)
	{
		List<string> fileList = null;
		if (mLoadSource == 0)
		{
			fileList = mResourceLoader.getFileList(path);
			if(lower)
			{
				int count = fileList.Count;
				for (int i = 0; i < count; ++i)
				{
					fileList[i] = fileList[i].ToLower();
				}
			}
		}
		else if (mLoadSource == 1)
		{
			fileList = mAssetBundleLoader.getFileList(path.ToLower());
		}
		return fileList;
	}
	// 异步加载指定的资源包
	public void loadPathOrBundleAsync(string path, AssetBundleLoadDoneCallback callback)
	{
		if (mLoadSource == 0)
		{
			mResourceLoader.loadPathAsync(path, callback);
		}
		else if (mLoadSource == 1)
		{
			mAssetBundleLoader.loadAssetBundleAsync(path, callback);
		}
	}
	// name是Resources下的相对路径,errorIfNull表示当找不到资源时是否报错提示
	public T loadResource<T>(string name, bool errorIfNull) where T : UnityEngine.Object
	{
		T res = null;
		if (mLoadSource == 0)
		{
			res = mResourceLoader.loadResource<T>(name);
		}
		else if (mLoadSource == 1)
		{
			res = mAssetBundleLoader.loadAsset<T>(name);
		}
		if (res == null && errorIfNull)
		{
			logError("can not find resource : " + name);
		}
		return res;
	}
	// name是Resources下的相对路径,errorIfNull表示当找不到资源时是否报错提示
	public bool loadResourceAsync<T>(string name, AssetLoadDoneCallback doneCallback, object userData, bool errorIfNull) where T : UnityEngine.Object
	{
		bool ret = false;
		if (mLoadSource == 0)
		{
			ret = mResourceLoader.loadResourcesAsync<T>(name, doneCallback, userData);
		}
		else if (mLoadSource == 1)
		{
			ret = mAssetBundleLoader.loadAssetAsync<T>(name, doneCallback, userData);
		}
		if (!ret && errorIfNull)
		{
			logError("can not find resource : " + name);
		}
		return ret;
	}
	public void loadAssetsFromUrl<T>(string url, AssetLoadDoneCallback callback, object userData) where T : UnityEngine.Object
	{
		mGameFramework.StartCoroutine(loadAssetsUrl(url, typeof(T), callback, userData));
	}
	public void loadAssetsFromUrl(string url, AssetLoadDoneCallback callback, object userData)
	{
		mGameFramework.StartCoroutine(loadAssetsUrl(url, null, callback, userData));
	}
	protected IEnumerator loadAssetsUrl(string url, Type assetsType, AssetLoadDoneCallback callback, object userData)
	{
		WWW www = new WWW(url);
		yield return www;
		if (www.error != null)
		{
			// 下载失败
			logInfo("下载失败 : " + url + ", info : " + www.error, LOG_LEVEL.LL_FORCE);
			callback(null, null, userData);
		}
		else
		{
			UnityEngine.Object obj = null;
			if (assetsType == typeof(AudioClip))
			{
#if UNITY_5_3_5
				obj = www.audioClip;
#elif UNITY_2018_2 || UNITY_2018_1
				obj = www.GetAudioClip();
#else
				obj = WWW.GetAudioClip(www);
#endif
			}
			else if (assetsType == typeof(Texture2D) || assetsType == typeof(Texture))
			{
				obj = www.texture;
			}
			else if (assetsType == typeof(AssetBundle))
			{
				obj = www.assetBundle;
			}
			if (obj != null)
			{
				obj.name = url;
			}
			callback(obj, www.bytes, userData);
		}
		www.Dispose();
		www = null;
	}
}