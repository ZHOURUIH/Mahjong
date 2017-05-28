using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using UnityEngine;

public class AssetBundleLoader : MonoBehaviour
{
	protected Dictionary<string, AssetBundleInfo> mAssetBundleInfoList;
	protected Dictionary<string, AssetInfo> mAssetToBundleInfo;
	protected static Dictionary<Type, string> mTypeSuffixList;		// 资源类型对应的后缀名
	protected Dictionary<AssetBundleInfo, bool> mBundleRequestList; // 已经请求过异步加载的资源包列表,value表示是否已经加载完毕
	protected Dictionary<string, AssetLoadDoneCallback> mAssetLoadCallback;

	public AssetBundleLoader()
	{
		mAssetBundleInfoList = new Dictionary<string, AssetBundleInfo>();
		mAssetToBundleInfo = new Dictionary<string, AssetInfo>();
		mBundleRequestList = new Dictionary<AssetBundleInfo, bool>();
		mAssetLoadCallback = new Dictionary<string, AssetLoadDoneCallback>();
		if (mTypeSuffixList == null)
		{
			mTypeSuffixList = new Dictionary<Type, string>();
			mTypeSuffixList.Add(typeof(Texture), ".png");
			mTypeSuffixList.Add(typeof(GameObject), ".prefab");
			mTypeSuffixList.Add(typeof(Material), ".mat");
			mTypeSuffixList.Add(typeof(Shader), ".shader");
			mTypeSuffixList.Add(typeof(AudioClip), ".wav");
		}
	}
	public bool init()
	{
		string path = CommonDefine.F_STREAMING_ASSETS_PATH + "StreamingAssets.xml";
		if (!File.Exists(path))
		{
			return false;
		}
		XmlDocument doc = new XmlDocument();
		doc.Load(path);
		XmlNodeList nodeList = doc.SelectSingleNode("files").ChildNodes;
		foreach (XmlElement xe in nodeList)
		{
			AssetBundleInfo bundleInfo = null;
			string bundleName = xe.SelectSingleNode("bundleName").InnerText;		// 资源所在的AssetBundle名
			bundleName = StringUtility.getFileNameNoSuffix(bundleName);			// 移除后缀名
			string assetName = xe.SelectSingleNode("assetName").InnerText;			// 资源文件名,带后缀,没有重复
			string fileNameWithSuffix = assetName.Substring(CommonDefine.P_RESOURCE_PATH.Length);	// 需要移除前缀
			if (mAssetBundleInfoList.ContainsKey(bundleName))
			{
				bundleInfo = mAssetBundleInfoList[bundleName];
			}
			else
			{
				bundleInfo = new AssetBundleInfo(bundleName);
				mAssetBundleInfoList.Add(bundleName, bundleInfo);
			}
			
			bundleInfo.addAssetName(fileNameWithSuffix);
			// 为AssetBundle添加依赖项
			XmlNode deps = xe.SelectSingleNode("deps");
			if (null != deps)
			{
				XmlNodeList depList = deps.ChildNodes;
				foreach (XmlElement _xe in depList)
				{
					// _xe.InnerText已经是相对于StreamingAssets的相对路径了,只需要去除后缀名
					string depName = _xe.InnerText;
					depName = StringUtility.getFileNameNoSuffix(depName);
					bundleInfo.addParent(depName);
				}
			}
			mAssetToBundleInfo.Add(fileNameWithSuffix, bundleInfo.getAssetInfo(fileNameWithSuffix));
		}
		// 配置清单解析完毕后,为每个AssetBundleInfo查找对应的依赖项
		foreach (var info in mAssetBundleInfoList)
		{
			info.Value.findAllDependence();
		}
		// 然后或者所有的资源
		return true;
	}
	public void update(float elapsedTime)
	{
		;
	}
	public void destroy()
	{
		;
	}
	public void unload(string name, bool unloadAllLoadedObjects)
	{
		AssetBundleInfo assetBundle = getAssetBundleInfo(name);
		if(assetBundle == null)
		{
			return;
		}
		assetBundle.unload(unloadAllLoadedObjects);
	}
	public AssetBundleInfo getAssetBundleInfo(string name)
	{
		if (mAssetBundleInfoList.ContainsKey(name))
		{
			return mAssetBundleInfoList[name];
		}
		return null;
	}
	public List<string> getBundleNameList(string path)
	{
		List<string> bundleNameList = new List<string>();
		foreach(var item in mAssetBundleInfoList)
		{
			if(item.Key.StartsWith(path))
			{
				bundleNameList.Add(item.Key);
			}
		}
		return bundleNameList;
	}
	// 资源是否已经加载
	public bool isAssetLoaded<T>(string fileName) where T : UnityEngine.Object
	{
		string fileNameWithSuffix = fileName;
		adjustResourceName<T>(ref fileNameWithSuffix);
		// 找不到资源则直接返回
		if (!mAssetToBundleInfo.ContainsKey(fileNameWithSuffix))
		{
			return false;
		}
		AssetBundleInfo bundleInfo = mAssetToBundleInfo[fileNameWithSuffix].mParentAssetBundle;
		if(bundleInfo.mLoaded != LOAD_STATE.LS_LOADED)
		{
			return false;
		}
		else
		{
			return bundleInfo.mAssetList[fileNameWithSuffix].mAssetObject != null;
		}
	}
	// 获得资源,如果资源包未加载,则返回空
	public T getAsset<T>(string fileName) where T : UnityEngine.Object
	{
		string fileNameWithSuffix = fileName;
		adjustResourceName<T>(ref fileNameWithSuffix);
		// 找不到资源则直接返回
		if (!mAssetToBundleInfo.ContainsKey(fileNameWithSuffix))
		{
			return null;
		}
		AssetBundleInfo bundleInfo = mAssetToBundleInfo[fileNameWithSuffix].mParentAssetBundle;
		return bundleInfo.getAsset<T>(fileNameWithSuffix);
	}
	// 同步加载资源包
	public List<UnityEngine.Object> loadAssetBundle(string bundleName)
	{
		if(mAssetBundleInfoList.ContainsKey(bundleName))
		{
			AssetBundleInfo bundleInfo = mAssetBundleInfoList[bundleName];
			if (bundleInfo.mLoaded == LOAD_STATE.LS_LOADING)
			{
				UnityUtility.logError("asset bundle is loading, can not load again! name : " + bundleName);
				return null;
			}
			// 如果还未加载,则加载资源包
			if (bundleInfo.mLoaded == LOAD_STATE.LS_UNLOAD)
			{
				bundleInfo.loadAssetBundle();
			}
			// 加载完毕,返回资源列表
			if (bundleInfo.mLoaded == LOAD_STATE.LS_LOADED)
			{
				List<UnityEngine.Object> assetList = new List<UnityEngine.Object>();
				foreach(var item in bundleInfo.mAssetList)
				{
					assetList.Add(item.Value.mAssetObject);
				}
				return assetList;
			}
		}
		return null;
	}
	// 异步加载资源包
	public bool loadAssetBundleAsync(string bundleName, AssetBundleLoadDoneCallback doneCallback)
	{
		if (!mAssetBundleInfoList.ContainsKey(bundleName))
		{
			return false;
		}
		AssetBundleInfo bundleInfo = mAssetBundleInfoList[bundleName];
		if (bundleInfo.mLoaded != LOAD_STATE.LS_UNLOAD)
		{
			UnityUtility.logError("asset bundle is loading or loaded, can not load again! name : " + bundleName);
			return false;
		}
		// 加载资源包
		bundleInfo.loadAssetBundleAsync(doneCallback);
		return true;
	}
	// 同步加载动更资源,文件名称,不带后缀,Resources下的相对路径
	public T loadAsset<T>(string fileName) where T : UnityEngine.Object
	{
		string fileNameWithSuffix = fileName;
		adjustResourceName<T>(ref fileNameWithSuffix);
		if (!mAssetToBundleInfo.ContainsKey(fileNameWithSuffix))
		{
			return null;
		}
		AssetBundleInfo bundleInfo = mAssetToBundleInfo[fileNameWithSuffix].mParentAssetBundle;
		if (bundleInfo.mLoaded == LOAD_STATE.LS_LOADING)
		{
			UnityUtility.logError("asset bundle is loading! can not load asset! asset name : " + fileName);
			return null;
		}
		// 如果资源包还未加载,则先加载资源包
		if (bundleInfo.mLoaded == LOAD_STATE.LS_UNLOAD)
		{
			loadAssetBundle(bundleInfo.mBundleName);
		}
		return bundleInfo.getAsset<T>(fileNameWithSuffix);
	}
	// 异步加载资源
	public bool loadAssetAsync<T>(string fileName, AssetLoadDoneCallback callback) where T : UnityEngine.Object
	{
		string fileNameWithSuffix = fileName;
		adjustResourceName<T>(ref fileNameWithSuffix);
		if (!mAssetToBundleInfo.ContainsKey(fileNameWithSuffix))
		{
			return false;
		}
		AssetBundleInfo bundleInfo = mAssetToBundleInfo[fileNameWithSuffix].mParentAssetBundle;
		if (bundleInfo.mLoaded == LOAD_STATE.LS_LOADING)
		{
			UnityUtility.logError("asset bundle is loading! can not load asset! asset name : " + fileName);
			return false;
		}
		// 如果资源包还未加载,则先加载资源包
		if (bundleInfo.mLoaded == LOAD_STATE.LS_UNLOAD)
		{
			mAssetLoadCallback.Add(fileNameWithSuffix, callback);
			loadAssetBundleAsync(bundleInfo.mBundleName, null);
		}
		// 如果已经加载,则直接回调
		else if(bundleInfo.mLoaded == LOAD_STATE.LS_LOADED)
		{
			callback(bundleInfo.mAssetList[fileNameWithSuffix].mAssetObject);
		}
		return true;
	}
	// 请求异步加载资源包
	public void requestLoadAssetBundle(AssetBundleInfo bundleInfo)
	{
		if (mBundleRequestList.ContainsKey(bundleInfo))
		{
			return;
		}
		mBundleRequestList.Add(bundleInfo, false);
		StartCoroutine(loadAssetBundleCoroutine(bundleInfo));
	}
	//-----------------------------------------------------------------------------------------------
	protected IEnumerator loadAssetBundleCoroutine(AssetBundleInfo bundleInfo, bool loadFromWWW = false)
	{
		// 先确保依赖项全部已经加载完成,才能开始加载当前请求的资源包
		// 异步加载所有未加载的依赖项
		foreach (var item in bundleInfo.mParents)
		{
			if (item.Value.mLoaded == LOAD_STATE.LS_UNLOAD)
			{
				item.Value.mLoaded = LOAD_STATE.LS_LOADING;
				yield return StartCoroutine(loadAssetBundleCoroutine(item.Value, loadFromWWW));
			}
		}
		// 为了避免异步加载的对象依赖相同的依赖项时造成错误(因为上面只是加载了未加载的依赖项,而正在加载的依赖项没有判断)
		// 所以此处仍然需要等待所有依赖项都加载完毕
		while (!bundleInfo.isAllDependenceDone())
		{
			yield return null;
		}
		AssetBundle assetBundle = null;
		// 通过www加载
		if (loadFromWWW)
		{
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_IPHONE || UNITY_IOS
			string path = "file:\\" + CommonDefine.F_STREAMING_ASSETS_PATH + bundleInfo.mBundleName + CommonDefine.ASSET_BUNDLE_SUFFIX;
#elif UNITY_ANDROID
			string path = "file:\\" + Application.dataPath + "!/assets" + bundleInfo.mBundleName + CommonDefine.ASSET_BUNDLE_SUFFIX;
#endif
			WWW www = new WWW(path);
			yield return www;
			assetBundle = www.assetBundle;
		}
		// 直接从文件加载
		else
		{
			AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(CommonDefine.F_STREAMING_ASSETS_PATH + bundleInfo.mBundleName + CommonDefine.ASSET_BUNDLE_SUFFIX);
			if (request == null)
			{
				UnityUtility.logError("can not load asset bundle async : " + bundleInfo.mBundleName);
			}
			yield return request;
			assetBundle = request.assetBundle;
		}
		// 异步加载其中所有的资源
		foreach (var item in bundleInfo.mAssetList)
		{
			AssetBundleRequest assetRequest = assetBundle.LoadAssetAsync(CommonDefine.P_RESOURCE_PATH + item.Value.mAssetName);
			if (assetRequest == null)
			{
				UnityUtility.logError("can not load asset async : " + item.Value.mAssetName);
			}
			yield return assetRequest;
			item.Value.mAssetObject = assetRequest.asset;
		}
		// 加载完成后进行通知
		mBundleRequestList[bundleInfo] = true;
		bundleInfo.notifyAssetBundleAsyncLoadedDone(assetBundle);

		// 然后通知所有的资源回调
		foreach(var item in bundleInfo.mAssetList)
		{
			if(mAssetLoadCallback.ContainsKey(item.Key))
			{
				mAssetLoadCallback[item.Key](item.Value.mAssetObject);
				mAssetLoadCallback.Remove(item.Key);
			}
		}
	}
	protected void adjustResourceName<T>(ref string fileName) where T : UnityEngine.Object
	{
		// 将\\转为/,加上后缀名,转为小写
		StringUtility.rightToLeft(ref fileName);
		addSuffix(ref fileName, typeof(T));
		fileName = fileName.ToLower();
	}
	// 为资源名加上对应的后缀名
	public static void addSuffix(ref string fileName, Type type)
	{
		if (mTypeSuffixList.ContainsKey(type))
		{
			string suffix = mTypeSuffixList[type];
			fileName += suffix;
		}
		else
		{
			UnityUtility.logError("resource type : " + type.ToString() + " is not registered!");
		}
	}
}