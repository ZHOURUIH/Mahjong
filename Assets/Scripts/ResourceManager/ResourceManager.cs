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
	public int mLoadSource;
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
		if (mLoadSource == 1)
		{
			mAssetBundleLoader.unload(name, unloadAllLoadedObjects);
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
			UnityUtility.logError("can not find resource : " + name);
		}
		return res;
	}
	public List<string> getFileOrBundleList(string path)
	{
		if (mLoadSource == 0)
		{
			return mResourceLoader.getFileList(path);
		}
		else if (mLoadSource == 1)
		{
			return mAssetBundleLoader.getBundleNameList(path.ToLower());
		}
		return null;
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
			res = mResourceLoader.loadResource(name) as T;
		}
		else if (mLoadSource == 1)
		{
			res = mAssetBundleLoader.loadAsset<T>(name);
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
		if (mLoadSource == 0)
		{
			ret = mResourceLoader.loadResourcesAsync<T>(name, doneCallback);
		}
		else if (mLoadSource == 1)
		{
			ret = mAssetBundleLoader.loadAssetAsync<T>(name, doneCallback);
		}
		if (!ret && errorIfNull)
		{
			UnityUtility.logError("can not find resource : " + name);
		}
		return ret;
	}
}