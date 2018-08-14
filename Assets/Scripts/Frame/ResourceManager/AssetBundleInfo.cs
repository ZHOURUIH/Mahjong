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
	public object mUserData;
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
	public int mLoadedParentCount;
	public int mLoadedChildCount;
	public Dictionary<string, AssetBundleInfo> mParents;	// 依赖的AssetBundle列表
	public Dictionary<string, AssetBundleInfo> mChildren;	// 依赖自己的AssetBundle列表
	public Dictionary<string, AssetInfo> mAssetList;			// 资源包中已加载的所有资源
	public Dictionary<string, AsyncLoadInfo> mLoadAsyncList;	// 需要异步加载的资源列表
	public AssetBundleLoadDoneCallback mAssetBundleLoadCallback;
	public AssetBundleInfo(string bundleName)
	{
		mBundleName = bundleName;
		mLoadedParentCount = 0;
		mLoadedChildCount = 0;
		mLoaded = LOAD_STATE.LS_UNLOAD;
		mParents = new Dictionary<string, AssetBundleInfo>();
		mChildren = new Dictionary<string, AssetBundleInfo>();
		mLoadAsyncList = new Dictionary<string, AsyncLoadInfo>();
		mAssetList = new Dictionary<string, AssetInfo>();
	}
	public void unload()
	{
		if(mAssetBundle != null)
		{
			logInfo("unload completely AssetBundle : " + mBundleName);
			mAssetBundle.Unload(true);
			mAssetBundle = null;
		}
	}
	public void addAssetName(string fileNameWithSuffix)
	{
		if (!mAssetList.ContainsKey(fileNameWithSuffix))
		{
			mAssetList.Add(fileNameWithSuffix, new AssetInfo(this, fileNameWithSuffix));
		}
		else
		{
			logError("there is asset in asset bundle, asset : " + fileNameWithSuffix + ", asset bundle : " + mBundleName);
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
	public void addChild(AssetBundleInfo other)
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
			mParents[depName].addChild(this);
		}
	}
	// 所有依赖项是否都已经加载完成
	public bool isAllParentLoaded()
	{
		return mLoadedParentCount == mParents.Count;
	}
	public bool isAllChildLoaded()
	{
		return mLoadedChildCount == mChildren.Count;
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
	// 同步加载资源
	public T loadAsset<T>(ref string fileNameWithSuffix) where T : UnityEngine.Object
	{
		if (!mAssetList.ContainsKey(fileNameWithSuffix))
		{
			return null;
		}
		// 如果AssetBundle还没有加载,则先加载AssetBundle
		if (mLoaded != LOAD_STATE.LS_LOADED)
		{
			loadAssetBundle();
		}
		return mAssetList[fileNameWithSuffix].mAssetObject as T;
	}
	// 异步加载资源
	public bool loadAssetAsync(ref string fileNameWithSuffix, AssetLoadDoneCallback callback, object userData)
	{
		if (!mAssetList.ContainsKey(fileNameWithSuffix))
		{
			return false;
		}
		// 如果当前资源包还未加载完毕,则需要等待资源包加载完以后才能加载资源
		if (mLoaded == LOAD_STATE.LS_UNLOAD || mLoaded == LOAD_STATE.LS_LOADING)
		{
			// 记录下需要异步加载的资源名
			if (!mLoadAsyncList.ContainsKey(fileNameWithSuffix))
			{
				AsyncLoadInfo loadInfo = new AsyncLoadInfo();
				loadInfo.mAssetInfo = mAssetList[fileNameWithSuffix];
				loadInfo.mCallback = callback;
				loadInfo.mUserData = userData;
				mLoadAsyncList.Add(fileNameWithSuffix, loadInfo);
			}
			else
			{
				logError("asset is loading, can not load asset async again! name : " + fileNameWithSuffix);
				return false;
			}
			if(mLoaded == LOAD_STATE.LS_UNLOAD)
			{
				loadAssetBundleAsync(null);
			}
		}
		// 如果资源包已经加载,则可以直接异步加载资源
		else
		{
			callback(mAssetList[fileNameWithSuffix].mAssetObject, userData);
		}
		return true;
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
#if UNITY_ANDROID && UNITY_EDITOR
		byte[] assetBundleBuffer = AndroidAssetLoader.loadAsset(mBundleName + CommonDefine.ASSET_BUNDLE_SUFFIX);
		mAssetBundle = AssetBundle.LoadFromMemory(assetBundleBuffer);
#else
		mAssetBundle = AssetBundle.LoadFromFile(CommonDefine.F_STREAMING_ASSETS_PATH + mBundleName + CommonDefine.ASSET_BUNDLE_SUFFIX);
#endif

		if (mAssetBundle == null)
		{
			logError("can not load asset bundle : " + mBundleName);
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
		mLoaded = LOAD_STATE.LS_LOADED;
		afterLoaded();
	}
	// 异步加载资源包
	public void loadAssetBundleAsync(AssetBundleLoadDoneCallback callback)
	{
		if (mLoaded != LOAD_STATE.LS_UNLOAD)
		{
			return;
		}
		// 先确保所有依赖项已经加载
		foreach (var info in mParents)
		{
			info.Value.loadAssetBundleAsync(null);
		}
		// 通知AssetBundleLoader请求异步加载AssetBundle
		mLoaded = LOAD_STATE.LS_LOADING;
		mResourceManager.mAssetBundleLoader.requestLoadAssetBundle(this);
		mAssetBundleLoadCallback = callback;
	}
	public void notifyAssetBundleAsyncLoadedDone(AssetBundle assetBundle)
	{
		mLoaded = LOAD_STATE.LS_LOADED;
		mAssetBundle = assetBundle;

		// 通知请求的资源回调
		foreach (var assetInfo in mLoadAsyncList)
		{
			if (assetInfo.Value.mCallback != null)
			{
				assetInfo.Value.mCallback(assetInfo.Value.mAssetInfo.mAssetObject, assetInfo.Value.mUserData);
			}
		}
		mLoadAsyncList.Clear();
		afterLoaded();
	}
	//-----------------------------------------------------------------------------------------------------------------------------
	protected void afterLoaded()
	{
		// 通知自己的所有子节点,自己已经加载完了
		foreach (var item in mChildren)
		{
			item.Value.notifyParentLoaded(this);
		}
		// 通知自己的所有父节点,自己已经加载完了
		foreach (var item in mParents)
		{
			item.Value.notifyChildLoaded(this);
		}
		if (mLoadedChildCount == mChildren.Count)
		{
			notifyAllChildLoaded();
		}
	}
	// 通知自己有一个父节点已经加载完毕
	protected void notifyParentLoaded(AssetBundleInfo parent)
	{
		++mLoadedParentCount;
	}
	protected void notifyChildLoaded(AssetBundleInfo child)
	{
		++mLoadedChildCount;
		if(mLoadedChildCount == mChildren.Count)
		{
			notifyAllChildLoaded();
		}
	}
	protected void notifyAllChildLoaded()
	{
		// 不做任何处理,不卸载资源包镜像,仅在确认卸载资源包以及程序退出时使用unload(true)
	}
}