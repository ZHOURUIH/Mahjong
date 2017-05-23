using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ResourceManager : GameBase
{
	protected GameObject mManagerObject;
	public AssetBundleLoader mAssetBundleLoader;
	public ResourceLoader mResourceLoader;
	protected int mLoadSource;
	public ResourceManager()
	{
		;
	}
	public void init()
	{
		mManagerObject = UnityUtility.getGameObject(mGameFramework.getGameFrameObject(), "ResourceManager");
		GameObject abLoaderObject = UnityUtility.getGameObject(mManagerObject, "AssetBundleLoader");
		GameObject resLoaderObject = UnityUtility.getGameObject(mManagerObject, "ResourceLoader");
		mAssetBundleLoader = abLoaderObject.GetComponent<AssetBundleLoader>();
		mResourceLoader = resLoaderObject.GetComponent<ResourceLoader>();
		mAssetBundleLoader.init();
		mResourceLoader.init();
#if UNITY_EDITOR
		mLoadSource = (int)mGameConfig.getFloatParam(GAME_DEFINE_FLOAT.GDF_LOAD_RESOURCES);
#else
		mLoadSource = 1;
#endif
	}
	public void update(float elapsedTime)
	{
		mAssetBundleLoader.update(elapsedTime);
		mResourceLoader.update(elapsedTime);
	}
	public void destroy()
	{
		mAssetBundleLoader.destroy();
		mResourceLoader.destroy();
	}
	public void unload(string name, bool unloadAllLoadedObjects)
	{
		// 只能用AssetBundleLoader卸载
		if (mLoadSource == -1 || mLoadSource == 1)
		{
			if (mAssetBundleLoader != null)
			{
				mAssetBundleLoader.unload(name, unloadAllLoadedObjects);
			}
		}
	}
	// name是Resources下的相对路径,errorIfNull表示当找不到资源时是否报错提示
	public T loadResource<T>(string name, bool errorIfNull) where T : UnityEngine.Object
	{
		T res = null;
		// 先从AssetBundle中加载
		if (mLoadSource == -1)
		{
			if (mAssetBundleLoader != null)
			{
				res = mAssetBundleLoader.loadAsset<T>(name);
			}
			else
			{
				res = mResourceLoader.loadResources(name) as T;
			}
		}
		else if (mLoadSource == 0)
		{
			res = mResourceLoader.loadResources(name) as T;
		}
		else if (mLoadSource == 1)
		{
			if (mAssetBundleLoader != null)
			{
				res = mAssetBundleLoader.loadAsset<T>(name);
			}
		}
		if (res == null && errorIfNull)
		{
			UnityUtility.logError("can not find resource : " + name);
		}
		return res;
	}
	// name是Resources下的相对路径,errorIfNull表示当找不到资源时是否报错提示
	public bool loadResourceAsync<T>(string name, AssetLoadDoneCallback doneCallback, bool errorIfNull) where T : UnityEngine.Object
	{
		bool ret = false;
		// 先从AssetBundle中加载
		if (mLoadSource == -1)
		{
			if (mAssetBundleLoader != null)
			{
				ret = mAssetBundleLoader.loadAssetAsync<T>(name, doneCallback);
			}
			else
			{
				ret = mResourceLoader.loadResourcesAsync<T>(name, doneCallback);
			}
		}
		else if (mLoadSource == 0)
		{
			ret = mResourceLoader.loadResourcesAsync<T>(name, doneCallback);
		}
		else if (mLoadSource == 1)
		{
			if (mAssetBundleLoader != null)
			{
				ret = mAssetBundleLoader.loadAssetAsync<T>(name, doneCallback);
			}
		}
		if (!ret && errorIfNull)
		{
			UnityUtility.logError("can not find resource : " + name);
		}
		return ret;
	}
}