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

public class AssetBundleInfo : GameBase
{
	public bool mLoaded;
	public string mBundleName;					// 资源所在的AssetBundle名,相对于StreamingAsset,不含后缀
	public AssetBundle mAssetBundle;
	public Dictionary<string, AssetBundleInfo> mParents;	// 依赖的AssetBundle列表
	public Dictionary<string, AssetBundleInfo> mChildren;	// 依赖自己的AssetBundle列表
	public Dictionary<string, AssetInfo> mAssetList;			// 资源包中已加载的所有资源
	public Dictionary<string, AsyncLoadInfo> mLoadAsyncList;	// 需要异步加载的资源列表
	public AssetBundleInfo(string bundleName)
	{
		mBundleName = bundleName;
		mLoaded = false;
		mParents = new Dictionary<string, AssetBundleInfo>();
		mChildren = new Dictionary<string, AssetBundleInfo>();
		mLoadAsyncList = new Dictionary<string, AsyncLoadInfo>();
		mAssetList = new Dictionary<string, AssetInfo>();
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
				if (!dep.Value.mLoaded || !dep.Value.isAllDependenceDone())
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
				if (!dep.Value.mLoaded || !dep.Value.isAllChildrenLoadedDone())
				{
					return false;
				}
			}
		}
		return true;
	}
	// 同步加载资源
	public T loadAsset<T>(ref string fileNameWithSuffix) where T : UnityEngine.Object
	{
		if (!mAssetList.ContainsKey(fileNameWithSuffix))
		{
			return null;
		}
		// 如果AssetBundle还没有加载,则先加载AssetBundle
		if (!mLoaded)
		{
			loadAssetBundle();
		}
		return mAssetList[fileNameWithSuffix].mAssetObject as T;
	}
	// 异步加载资源
	public bool loadAssetAsync(ref string fileNameWithSuffix, AssetLoadDoneCallback callback)
	{
		if (!mAssetList.ContainsKey(fileNameWithSuffix))
		{
			return false;
		}
		// 如果当前资源包还未加载,则需要等待资源包加载完以后才能加载资源
		if (!mLoaded)
		{
			// 记录下需要异步加载的资源名
			if (!mLoadAsyncList.ContainsKey(fileNameWithSuffix))
			{
				AsyncLoadInfo loadInfo = new AsyncLoadInfo();
				loadInfo.mAssetInfo = mAssetList[fileNameWithSuffix];
				loadInfo.mCallback = callback;
				mLoadAsyncList.Add(fileNameWithSuffix, loadInfo);
			}
			else
			{
				UnityUtility.logError("asset is loading, can not load asset async again! name : " + fileNameWithSuffix);
				return false;
			}
			loadAssetBundleAsync();
		}
		// 如果资源包已经加载,则可以直接异步加载资源
		else
		{
			callback(mAssetList[fileNameWithSuffix].mAssetObject);
		}
		return true;
	}
	public void notifyAssetBundleAsyncLoadedDone(AssetBundle assetBundle)
	{
		mLoaded = true;
		mAssetBundle = assetBundle;
		// 异步加载其中所有的资源
		foreach (var assetInfo in mLoadAsyncList)
		{
			assetInfo.Value.mAssetInfo.mLoadDoneCallback = assetInfo.Value.mCallback;
			// 通知AssetBundleLoader请求异步加载Asset
			mResourceManager.mAssetBundleLoader.requestLoadAsset(assetInfo.Value.mAssetInfo, mAssetBundle);
		}
		mLoadAsyncList.Clear();
	}
	//-----------------------------------------------------------------------------
	// 同步加载资源包
	protected void loadAssetBundle()
	{
		if (mLoaded)
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
		List<string> assetNameList = new List<string>(mAssetList.Keys);
		int assetCount = assetNameList.Count;
		for (int i = 0; i < assetCount; ++i)
		{
			UnityEngine.Object obj = mAssetBundle.LoadAsset(CommonDefine.P_RESOURCE_PATH + assetNameList[i]);
			mAssetList[assetNameList[i]].mAssetObject = obj;
		}
		mLoaded = true;
		notifyAssetBundleLoaded(this);
	}
	static protected void notifyAssetBundleLoaded(AssetBundleInfo info)
	{
		// 如果所有子节点已经加载完毕,则可以卸载资源镜像
		if (info.isAllChildrenLoadedDone())
		{
			info.unload(false);
		}
	}
	// 异步加载资源包
	protected void loadAssetBundleAsync()
	{
		if (mLoaded)
		{
			return;
		}
		// 先确保所有依赖项已经加载
		foreach (var info in mParents)
		{
			info.Value.loadAssetBundleAsync();
		}
		// 通知AssetBundleLoader请求异步加载AssetBundle
		mResourceManager.mAssetBundleLoader.requestLoadAssetBundle(this);
	}
}