using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AssetBundleLoader : GameBase
{
	protected Dictionary<string, AssetBundleInfo> mAssetBundleInfoList;
	protected Dictionary<string, AssetInfo> mAssetToBundleInfo;
	protected static Dictionary<Type, List<string>> mTypeSuffixList;        // 资源类型对应的后缀名
	protected List<AssetBundleInfo> mRequestBundleList;
	protected int mAssetBundleCoroutineCount = 0;
	protected int MAX_ASSET_BUNDLE_COROUTINE = 8;
	public AssetBundleLoader()
	{
		mAssetBundleInfoList = new Dictionary<string, AssetBundleInfo>();
		mAssetToBundleInfo = new Dictionary<string, AssetInfo>();
		mRequestBundleList = new List<AssetBundleInfo>();
		if (mTypeSuffixList == null)
		{
			mTypeSuffixList = new Dictionary<Type, List<string>>();
			registeSuffix(typeof(Texture), ".png");
			registeSuffix(typeof(Texture2D), ".png");
			registeSuffix(typeof(GameObject), ".prefab");
			registeSuffix(typeof(GameObject), ".fbx");
			registeSuffix(typeof(Material), ".mat");
			registeSuffix(typeof(Shader), ".shader");
			registeSuffix(typeof(AudioClip), ".wav");
			registeSuffix(typeof(AudioClip), ".mp3");
			registeSuffix(typeof(TextAsset), ".txt");
			registeSuffix(typeof(UIAtlas), ".prefab");
			registeSuffix(typeof(RuntimeAnimatorController), ".controller");
		}
	}
	public bool init()
	{
		string path = CommonDefine.F_STREAMING_ASSETS_PATH + "StreamingAssets.xml";
		if (!isFileExist(path))
		{
			return false;
		}
		byte[] fileBuffer = null;
		openFile(path, ref fileBuffer);
		MemoryStream stream = new MemoryStream(fileBuffer);
		XmlDocument doc = new XmlDocument();
		doc.Load(stream);
		XmlNodeList nodeList = doc.SelectSingleNode("files").ChildNodes;
		foreach (XmlElement xe in nodeList)
		{
			string bundleName = xe.SelectSingleNode("bundleName").InnerText;        // 资源所在的AssetBundle名
			bundleName = getFileNameNoSuffix(bundleName);                           // 移除后缀名
			string assetName = xe.SelectSingleNode("assetName").InnerText;          // 相对于Resources的资源文件名,带后缀,没有重复,
			if (!mAssetBundleInfoList.ContainsKey(bundleName))
			{
				mAssetBundleInfoList.Add(bundleName, new AssetBundleInfo(bundleName));
			}
			AssetBundleInfo bundleInfo = mAssetBundleInfoList[bundleName];
			bundleInfo.addAssetName(assetName);
			// 为AssetBundle添加依赖项
			XmlNode deps = xe.SelectSingleNode("deps");
			if (null != deps)
			{
				XmlNodeList depList = deps.ChildNodes;
				foreach (XmlElement _xe in depList)
				{
					// _xe.InnerText已经是相对于StreamingAssets的相对路径了,只需要去除后缀名
					string depName = _xe.InnerText;
					depName = getFileNameNoSuffix(depName);
					bundleInfo.addParent(depName);
				}
			}
			mAssetToBundleInfo.Add(assetName, bundleInfo.getAssetInfo(assetName));
		}
		// 配置清单解析完毕后,为每个AssetBundleInfo查找对应的依赖项
		foreach (var info in mAssetBundleInfoList)
		{
			info.Value.findAllDependence();
		}
		return true;
	}
	public void update(float elapsedTime)
	{
		if(mRequestBundleList.Count > 0 && mAssetBundleCoroutineCount < MAX_ASSET_BUNDLE_COROUTINE)
		{
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_IPHONE || UNITY_IOS || UNITY_STANDALONE_LINUX
			bool loadFromWWW = false;
#elif UNITY_ANDROID
			bool loadFromWWW = true;
#endif
			// 找到第一个依赖项已经加载完毕的资源
			int count = mRequestBundleList.Count;
			for (int i = 0; i < count; ++i)
			{
				if(mRequestBundleList[i].isAllParentLoaded())
				{
					mGameFramework.StartCoroutine(loadAssetBundleCoroutine(mRequestBundleList[i], loadFromWWW));
					mRequestBundleList.RemoveAt(i);
					break;
				}
			}
		}
	}
	public void destroy()
	{
		foreach (var item in mAssetBundleInfoList)
		{
			item.Value.unload();
		}
	}
	public void unload(string name)
	{
		AssetBundleInfo assetBundle = getAssetBundleInfo(name);
		if(assetBundle == null)
		{
			return;
		}
		assetBundle.unload();
	}
	public AssetBundleInfo getAssetBundleInfo(string name)
	{
		if (mAssetBundleInfoList.ContainsKey(name))
		{
			return mAssetBundleInfoList[name];
		}
		return null;
	}
	// 得到文件夹中的所有文件,如果文件夹被打包成一个AssetBundle,则返回AssetBundle中的所有资源名
	// 如果文件夹中包含多个AssetBundle(一般一个AssetBundle代表一个预设),则返回所有预设的名字
	public List<string> getFileList(string path)
	{
		if(path.EndsWith("/"))
		{
			path = path.Substring(0, path.Length - 1);
		}
		List<string> fileList = new List<string>();
		// 该文件夹被打包成一个AssetBundle
		if(mAssetBundleInfoList.ContainsKey(path))
		{
			foreach (var item in mAssetBundleInfoList[path].mAssetList)
			{
				fileList.Add(getFileNameNoSuffix(item.Key, true));
			}
		}
		// 判断文件夹中是否包含预设(因为预设在列表中也是跟文件夹一样的打成一个AssetBundle,所以需要另外判断)
		foreach (var item in mAssetBundleInfoList)
		{
			if (item.Key.StartsWith(path) && item.Key != path)
			{
				string fileName = getFileNameNoSuffix(item.Key, true);
				if(!fileList.Contains(fileName))
				{
					fileList.Add(fileName);
				}
			}
		}
		return fileList;
	}
	// 资源是否已经加载
	public bool isAssetLoaded<T>(string fileName) where T : UnityEngine.Object
	{
		List<string> fileNameList = adjustResourceName<T>(fileName);
		// 只返回第一个找到的资源
		int count = fileNameList.Count;
		for (int i = 0; i < count; ++i)
		{
			string fileNameWithSuffix = fileNameList[i];
			// 找不到资源则直接返回
			if (mAssetToBundleInfo.ContainsKey(fileNameWithSuffix))
			{
				AssetBundleInfo bundleInfo = mAssetToBundleInfo[fileNameWithSuffix].mParentAssetBundle;
				if (bundleInfo.mLoaded != LOAD_STATE.LS_LOADED)
				{
					return false;
				}
				else
				{
					return bundleInfo.mAssetList[fileNameWithSuffix].mAssetObject != null;
				}
			}
		}
		return false;
	}
	// 获得资源,如果资源包未加载,则返回空
	public T getAsset<T>(string fileName) where T : UnityEngine.Object
	{
		List<string> fileNameList = adjustResourceName<T>(fileName);
		// 只返回第一个找到的资源
		int count = fileNameList.Count;
		for(int i = 0; i < count; ++i)
		{
			string fileNameWithSuffix = fileNameList[i];
			if (mAssetToBundleInfo.ContainsKey(fileNameWithSuffix))
			{
				AssetBundleInfo bundleInfo = mAssetToBundleInfo[fileNameWithSuffix].mParentAssetBundle;
				return bundleInfo.getAsset<T>(fileNameWithSuffix);
			}
		}
		return null;
	}
	// 同步加载资源包
	public List<UnityEngine.Object> loadAssetBundle(string bundleName)
	{
		if (mAssetBundleInfoList.ContainsKey(bundleName))
		{
			AssetBundleInfo bundleInfo = mAssetBundleInfoList[bundleName];
			if (bundleInfo.mLoaded == LOAD_STATE.LS_LOADING)
			{
				logError("asset bundle is loading, can not load again! name : " + bundleName);
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
				foreach (var item in bundleInfo.mAssetList)
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
			logError("asset bundle is loading or loaded, can not load again! name : " + bundleName);
			return false;
		}
		// 加载资源包
		bundleInfo.loadAssetBundleAsync(doneCallback);
		return true;
	}
	// 同步加载资源,文件名称,不带后缀,Resources下的相对路径
	public T loadAsset<T>(string fileName) where T : UnityEngine.Object
	{
		List<string> fileNameList = adjustResourceName<T>(fileName);
		// 只加载第一个找到的资源,所以不允许有重名的同类资源
		T res = null;
		int count = fileNameList.Count;
		for (int i = 0; i < count; ++i)
		{
			string fileNameWithSuffix = fileNameList[i];
			if (mAssetToBundleInfo.ContainsKey(fileNameWithSuffix))
			{
				AssetBundleInfo bundleInfo = mAssetToBundleInfo[fileNameWithSuffix].mParentAssetBundle;
				res = bundleInfo.loadAsset<T>(ref fileNameWithSuffix);
				if(res != null)
				{
					break;
				}
			}
		}
		return res;
	}
	// 异步加载资源
	public bool loadAssetAsync<T>(string fileName, AssetLoadDoneCallback doneCallback, object userData) where T : UnityEngine.Object
	{
		List<string> fileNameList = adjustResourceName<T>(fileName);
		// 只加载第一个找到的资源,所以不允许有重名的同类资源
		int loadedCount = 0;
		int count = fileNameList.Count;
		for(int i = 0; i < count; ++i)
		{
			string fileNameWithSuffix = fileNameList[i];
			if (mAssetToBundleInfo.ContainsKey(fileNameWithSuffix))
			{
				AssetBundleInfo bundleInfo = mAssetToBundleInfo[fileNameWithSuffix].mParentAssetBundle;
				bool ret = bundleInfo.loadAssetAsync(ref fileNameWithSuffix, doneCallback, userData);
				if (ret)
				{
					++loadedCount;
					break;
				}
			}
		}
		return loadedCount != 0;
	}
	// 请求异步加载资源包
	public void requestLoadAssetBundle(AssetBundleInfo bundleInfo)
	{
		mRequestBundleList.Add(bundleInfo);
	}
	//-------------------------------------------------------------------------------------------------------------------------------------
	protected IEnumerator loadAssetBundleCoroutine(AssetBundleInfo bundleInfo, bool loadFromWWW)
	{
		++mAssetBundleCoroutineCount;
		logInfo(bundleInfo.mBundleName + " start load bundle", LOG_LEVEL.LL_NORMAL);
		// 先确保依赖项全部已经加载完成,才能开始加载当前请求的资源包
		while (!bundleInfo.isAllParentLoaded())
		{
			yield return null;
		}

		AssetBundle assetBundle = null;
		// 通过www加载
		if (loadFromWWW)
		{
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_IPHONE || UNITY_IOS || UNITY_STANDALONE_LINUX
			string path = "file:\\" + CommonDefine.F_STREAMING_ASSETS_PATH + bundleInfo.mBundleName + CommonDefine.ASSET_BUNDLE_SUFFIX;
#elif UNITY_ANDROID
			string path = CommonDefine.F_STREAMING_ASSETS_PATH + bundleInfo.mBundleName + CommonDefine.ASSET_BUNDLE_SUFFIX;
#endif
			WWW www = new WWW(path);
			yield return www;
			assetBundle = www.assetBundle;
			www.Dispose();
			www = null;
		}
		// 直接从文件加载
		else
		{
			AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(CommonDefine.F_STREAMING_ASSETS_PATH + bundleInfo.mBundleName + CommonDefine.ASSET_BUNDLE_SUFFIX);
			if (request == null)
			{
				logError("can not load asset bundle async : " + bundleInfo.mBundleName);
				--mAssetBundleCoroutineCount;
				yield break;
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
				logError("can not load asset async : " + item.Value.mAssetName);
				--mAssetBundleCoroutineCount;
				yield break;
			}
			yield return assetRequest;
			item.Value.mAssetObject = assetRequest.asset;
		}
		logInfo(bundleInfo.mBundleName + " load bundle done", LOG_LEVEL.LL_NORMAL);

		yield return new WaitForEndOfFrame();
		// 通知AssetBundleInfo
		bundleInfo.notifyAssetBundleAsyncLoadedDone(assetBundle);
		--mAssetBundleCoroutineCount;
	}
	protected List<string> adjustResourceName<T>(string fileName) where T : UnityEngine.Object
	{
		// 将\\转为/,加上后缀名,转为小写
		rightToLeft(ref fileName);
		List<string> fileNameList = addSuffix(fileName, typeof(T));
		int count = fileNameList.Count;
		for (int i = 0; i < count; ++i)
		{
			fileNameList[i] = fileNameList[i].ToLower();
		}
		return fileNameList;
	}
	// 为资源名加上对应的后缀名
	public static List<string> addSuffix(string fileName, Type type)
	{
		List<string> fileNameWithSuffix = new List<string>();
		if (mTypeSuffixList.ContainsKey(type))
		{
			int suffixCount = mTypeSuffixList[type].Count;
			for(int i = 0; i < suffixCount; ++i)
			{
				fileNameWithSuffix.Add(fileName + mTypeSuffixList[type][i]);
			}
			return fileNameWithSuffix;
		}
		else
		{
			logError("resource type : " + type.ToString() + " is not registered!");
		}
		return fileNameWithSuffix;
	}
	protected void registeSuffix(Type t, string suffix)
	{
		if (mTypeSuffixList.ContainsKey(t))
		{
			mTypeSuffixList[t].Add(suffix);
		}
		else
		{
			List<string> list = new List<string>();
			list.Add(suffix);
			mTypeSuffixList.Add(t, list);
		}
	}
}