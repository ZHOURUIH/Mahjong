using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AsyncLoadInfo
{
	public string mName;
	public AssetInfo mAssetInfo;
	public AssetLoadDoneCallback mCallback;
}

public enum LOAD_STATE
{
	LS_UNLOAD,
	LS_LOADING,
	LS_LOADED,
}

public class AssetBundleInfo : GameBase
{
	public LOAD_STATE mLoaded;
	public string mBundleName;					// 资源所在的AssetBundle名,相对于StreamingAsset,不含后缀
	public AssetBundle mAssetBundle;
	public Dictionary<string, AssetBundleInfo> mParents;	// 依赖的AssetBundle列表
	public Dictionary<string, AssetBundleInfo> mChildren;	// 依赖自己的AssetBundle列表
	public Dictionary<string, AssetInfo> mAssetList;			// 资源包中已加载的所有资源
	public int mLoadedCount;
	public AssetBundleLoadDoneCallback mAssetBundleLoadCallback;
	public AssetBundleInfo(string bundleName)
	{
		mBundleName = bundleName;
		mLoaded = LOAD_STATE.LS_UNLOAD;
		mParents = new Dictionary<string, AssetBundleInfo>();
		mChildren = new Dictionary<string, AssetBundleInfo>();
		mAssetList = new Dictionary<string, AssetInfo>();
		mLoadedCount = 0;
	}
	public void unload(bool unloadAllLoadedObjects)
	{
		mAssetBundle.Unload(unloadAllLoadedObjects);
		mAssetBundle = null;
	}
	public void addAssetName(string fileNameWithSuffix)
	{
		if (!mAssetList.ContainsKey(fileNameWithSuffix))
		{
			mAssetList.Add(fileNameWithSuffix, new AssetInfo(this, fileNameWithSuffix));
		}
		else
		{
			UnityUtility.logError("there is asset in asset bundle, asset : " + fileNameWithSuffix + ", asset bundle : " + mBundleName);
		}
	}
	public AssetInfo getAssetInfo(string fileNameWithSuffix)
	{
		if (mAssetList.ContainsKey(fileNameWithSuffix))
		{
			return mAssetList[fileNameWithSuffix];
		}
		return null;
	}
	// 添加依赖项
	public void addParent(string dep)
	{
		if (!mParents.ContainsKey(dep))
		{
			mParents.Add(dep, null);
		}
	}
	// 通知有其他的AssetBundle依赖了自己
	public void notifyChild(AssetBundleInfo other)
	{
		if (!mChildren.ContainsKey(other.mBundleName))
		{
			mChildren.Add(other.mBundleName, other);
		}
	}
	// 查找所有依赖项
	public void findAllDependence()
	{
		List<string> keys = new List<string>(mParents.Keys);
		foreach (var depName in keys)
		{
			// 找到自己的父节点
			mParents[depName] = mResourceManager.mAssetBundleLoader.getAssetBundleInfo(depName);
			// 并且通知父节点添加自己为子节点
			mParents[depName].notifyChild(this);
		}
	}
	// 所有依赖项是否都已经加载完成
	public bool isAllDependenceDone()
	{
		// 如果没有依赖项,则直接返回依赖项已加载
		// 有依赖项,则判断依赖项是否已经加载以及依赖项的依赖项是否已经加载
		if (mParents.Count > 0)
		{
			foreach (var dep in mParents)
			{
				if (dep.Value.mLoaded != LOAD_STATE.LS_LOADED || !dep.Value.isAllDependenceDone())
				{
					return false;
				}
			}
		}
		return true;
	}
	// 依赖于自己的AssetBundle是否已经全部加载完毕
	public bool isAllChildrenLoadedDone()
	{
		// 如果有依赖自己的AssetBundle
		if (mChildren.Count > 0)
		{
			foreach (var dep in mChildren)
			{
				if (dep.Value.mLoaded != LOAD_STATE.LS_LOADED || !dep.Value.isAllChildrenLoadedDone())
				{
					return false;
				}
			}
		}
		return true;
	}
	// 获得资源,只能在加载后才能获得
	public T getAsset<T>(string fileNameWithSuffix) where T : UnityEngine.Object
	{
		if (!mAssetList.ContainsKey(fileNameWithSuffix))
		{
			return null;
		}
		if (mLoaded != LOAD_STATE.LS_LOADED)
		{
			return null;
		}
		return mAssetList[fileNameWithSuffix].mAssetObject as T;
	}
	// 同步加载资源包
	public void loadAssetBundle()
	{
		if (mLoaded != LOAD_STATE.LS_UNLOAD)
		{
			return;
		}
		// 先确保所有依赖项已经加载
		foreach (var info in mParents)
		{
			info.Value.loadAssetBundle();
		}
		// 然后加载AssetBundle
		mAssetBundle = AssetBundle.LoadFromFile(CommonDefine.F_STREAMING_ASSETS_PATH + mBundleName + CommonDefine.ASSET_BUNDLE_SUFFIX);
		if (mAssetBundle == null)
		{
			UnityUtility.logError("can not load asset bundle : " + mBundleName);
			return;
		}
		// 加载其中的所有资源
		loadAllAsset();
		mLoaded = LOAD_STATE.LS_LOADED;
	}
	// 异步加载资源包
	public void loadAssetBundleAsync(AssetBundleLoadDoneCallback doneCallback)
	{
		if (mLoaded != LOAD_STATE.LS_UNLOAD)
		{
			return;
		}
		mLoaded = LOAD_STATE.LS_LOADING;
		// 通知AssetBundleLoader请求异步加载AssetBundle,在协程中会判断依赖项的加载
		mResourceManager.mAssetBundleLoader.requestLoadAssetBundle(this);
		mAssetBundleLoadCallback = doneCallback;
	}
	// 通知资源包已经异步加载完成
	public void notifyAssetBundleAsyncLoadedDone(AssetBundle assetBundle)
	{
		mLoaded = LOAD_STATE.LS_LOADED;
		mAssetBundle = assetBundle;
		List<UnityEngine.Object> resList = new List<UnityEngine.Object>();
		foreach(var item in mAssetList)
		{
			resList.Add(item.Value.mAssetObject);
		}
		if (mAssetBundleLoadCallback != null)
		{
			mAssetBundleLoadCallback(resList);
		}
	}
	//-----------------------------------------------------------------------------
	protected void loadAllAsset()
	{
		foreach (var item in mAssetList)
		{
			UnityEngine.Object obj = mAssetBundle.LoadAsset(CommonDefine.P_RESOURCE_PATH + item.Key);
			mAssetList[item.Key].mAssetObject = obj;
		}
	}
}