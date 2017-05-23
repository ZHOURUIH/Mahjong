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
	protected Dictionary<AssetBundleInfo, bool> mBundleRequestList;	// 已经请求过异步加载的资源包列表,value表示是否已经加载完毕
	protected Dictionary<AssetInfo, bool> mAssetRequestList;		// 已经请求过一部加载的资源列表,value表示是否已经加载完毕

	public AssetBundleLoader()
	{
		mAssetBundleInfoList = new Dictionary<string, AssetBundleInfo>();
		mAssetToBundleInfo = new Dictionary<string, AssetInfo>();
		mBundleRequestList = new Dictionary<AssetBundleInfo, bool>();
		mAssetRequestList = new Dictionary<AssetInfo, bool>();
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
			bundleName = StringUtility.getFileNameNoSuffix(ref bundleName);			// 移除后缀名
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
					depName = StringUtility.getFileNameNoSuffix(ref depName);
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
	// 同步加载动更资源,文件名称,不带后缀,Resources下的相对路径
	public T loadAsset<T>(string fileName) where T : UnityEngine.Object
	{
		string fileNameWithSuffix = fileName;
		adjustResourceName<T>(ref fileNameWithSuffix);
		// 找不到资源则直接返回
		if (!mAssetToBundleInfo.ContainsKey(fileNameWithSuffix))
		{
			return null;
		}
		AssetBundleInfo bundleInfo = mAssetToBundleInfo[fileNameWithSuffix].mParentAssetBundle;
		T res = bundleInfo.loadAsset<T>(ref fileNameWithSuffix);
		return res;
	}
	public bool loadAssetAsync<T>(string fileName, AssetLoadDoneCallback doneCallback) where T : UnityEngine.Object
	{
		string fileNameWithSuffix = fileName;
		adjustResourceName<T>(ref fileNameWithSuffix);
		// 找不到资源则直接返回
		if (!mAssetToBundleInfo.ContainsKey(fileNameWithSuffix))
		{
			return false;
		}
		AssetBundleInfo bundleInfo = mAssetToBundleInfo[fileNameWithSuffix].mParentAssetBundle;
		bool ret = bundleInfo.loadAssetAsync(ref fileNameWithSuffix, doneCallback);
		return ret;
	}
	public void requestLoadAssetBundle(AssetBundleInfo bundleInfo)
	{
		if (mBundleRequestList.ContainsKey(bundleInfo))
		{
			return;
		}
		mBundleRequestList.Add(bundleInfo, false);
		StartCoroutine(loadAssetBundleCoroutine(bundleInfo));
	}
	public void requestLoadAsset(AssetInfo assetInfo, AssetBundle assetBundle)
	{
		if (mAssetRequestList.ContainsKey(assetInfo))
		{
			return;
		}
		mAssetRequestList.Add(assetInfo, false);
		StartCoroutine(assetLoadCoroutine(assetInfo, assetBundle));
	}
	//-----------------------------------------------------------------------------------------------
	protected IEnumerator loadAssetBundleCoroutine(AssetBundleInfo bundleInfo)
	{
		// 先确保依赖项全部已经加载完成,才能开始加载当前请求的资源包
		while (!bundleInfo.isAllDependenceDone())
		{
			yield return null;
		}
		// 然后加载AssetBundle
		AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(CommonDefine.F_STREAMING_ASSETS_PATH + bundleInfo.mBundleName);
		if (request == null)
		{
			UnityUtility.logError("can not load asset bundle async : " + bundleInfo.mBundleName);
		}
		while (!request.isDone)
		{
			yield return null;
		}
		// 资源包加载完成后记录下来
		mBundleRequestList[bundleInfo] = true;
		bundleInfo.notifyAssetBundleAsyncLoadedDone(request.assetBundle);
	}
	protected IEnumerator assetLoadCoroutine(AssetInfo assetInfo, AssetBundle assetBundle)
	{
		AssetBundleRequest request = assetBundle.LoadAssetAsync(CommonDefine.P_RESOURCE_PATH + assetInfo.mAssetName);
		if (request == null)
		{
			UnityUtility.logError("can not load asset async : " + assetInfo.mAssetName);
		}
		while (!request.isDone)
		{
			yield return null;
		}
		assetInfo.mAssetObject = request.asset;
		mAssetRequestList[assetInfo] = true;
		if (assetInfo.mLoadDoneCallback != null)
		{
			assetInfo.mLoadDoneCallback(assetInfo.mAssetObject);
			assetInfo.mLoadDoneCallback = null;
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