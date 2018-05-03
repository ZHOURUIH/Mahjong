using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ResourceLoader : GameBase
{
	protected Dictionary<string, Dictionary<string, UnityEngine.Object>> mLoadedPath;
	public ResourceLoader()
	{
		mLoadedPath = new Dictionary<string, Dictionary<string, UnityEngine.Object>>();
	}
	public void init()
	{
		;
	}
	public void update(float elapsedTime)
	{
		;
	}
	public void destroy()
	{
		;
	}
	public List<string> getFileList(string path)
	{
		List<string> fileList = new List<string>();
		FileUtility.findFiles(CommonDefine.RESOURCES + "/" + path, ref fileList);
		// 去除meta文件
		List<string> newFileList = new List<string>();
		int fileCount = fileList.Count;
		for(int i = 0; i < fileCount; ++i)
		{
			if(!fileList[i].EndsWith(".meta"))
			{
				newFileList.Add(StringUtility.getFileNameNoSuffix(fileList[i], true));
			}
		}
		return newFileList;
	}
	public bool isResourceLoaded(string name)
	{
		string path = StringUtility.getFilePath(name);
		if (!mLoadedPath.ContainsKey(path))
		{
			return mLoadedPath[path].ContainsKey(name);
		}
		return false;
	}
	public UnityEngine.Object getResource(string name)
	{
		string path = StringUtility.getFilePath(name);
		if (!mLoadedPath.ContainsKey(path))
		{
			return null;
		}
		if (!mLoadedPath[path].ContainsKey(name))
		{
			return null;
		}
		else
		{
			return mLoadedPath[path][name];
		}
	}
	// 同步加载资源,name为Resources下的相对路径,不带后缀
	public UnityEngine.Object loadResource(string name)
	{
		string path = StringUtility.getFilePath(name);
		// 如果文件夹还未加载,则先加载文件夹
		if (!mLoadedPath.ContainsKey(path))
		{
			mLoadedPath.Add(path, new Dictionary<string, UnityEngine.Object>());
		}
		// 已经加载,则返回true
		if (!mLoadedPath[path].ContainsKey(name))
		{
			mLoadedPath[path][name] = Resources.Load(name);
		}
		return mLoadedPath[path][name];
	}
	// 异步加载资源,name为Resources下的相对路径,不带后缀
	public bool loadResourcesAsync<T>(string name, AssetLoadDoneCallback doneCallback, object userData) where T : UnityEngine.Object
	{
		string path = StringUtility.getFilePath(name);
		// 如果文件夹还未加载,则先加载文件夹
		if (!mLoadedPath.ContainsKey(path))
		{
			mLoadedPath.Add(path, new Dictionary<string, UnityEngine.Object>());
		}
		// 已经加载,则返回true
		if (mLoadedPath[path].ContainsKey(name))
		{
			// 如果已经加载完毕,则返回,否则继续等待
			if(mLoadedPath[path][name] != null)
			{
				doneCallback(mLoadedPath[path][name], null);
			}
		}
		else
		{
			mLoadedPath[path].Add(name, null);
			mGameFramework.StartCoroutine(loadResourceCoroutine<T>(name, doneCallback, userData));
		}
		return true;
	}
	// 同步加载整个文件夹
	public List<UnityEngine.Object> loadPath(string path)
	{
		if (!mLoadedPath.ContainsKey(path))
		{
			Dictionary<string, UnityEngine.Object> loadedResource = new Dictionary<string, UnityEngine.Object>();
			mLoadedPath.Add(path, loadedResource);
			UnityEngine.Object[] resList = Resources.LoadAll(path);
			string tempPath = path;
			tempPath += "/";
			int resCount = resList.Length;
			for (int i = 0; i < resCount; ++i)
			{
				string fullName = tempPath + resList[i].name;
				if (loadedResource.ContainsKey(fullName))
				{
					UnityUtility.logError("folder is loaded,but there is more than one resource named : " + resList[i].name);
					break;
				}
				else
				{
					loadedResource.Add(fullName, resList[i]);
				}
			}
		}
		return new List<UnityEngine.Object>(mLoadedPath[path].Values);
	}
	// 异步加载整个文件夹
	public void loadPathAsync(string path, AssetBundleLoadDoneCallback callback)
	{
		if (mLoadedPath.ContainsKey(path))
		{
			callback(new List<UnityEngine.Object>(mLoadedPath[path].Values));
		}
		else
		{
			mLoadedPath.Add(path, new Dictionary<string, UnityEngine.Object>());
			mGameFramework.StartCoroutine(loadPathCoroutine(path, callback));
		}
	}
	//---------------------------------------------------------------------------------------------------------------------------------------
	protected IEnumerator loadResourceCoroutine<T>(string resName, AssetLoadDoneCallback doneCallback, object userData) where T : UnityEngine.Object
	{
		UnityUtility.logInfo(resName + " start load!", LOG_LEVEL.LL_NORMAL);
		ResourceRequest request = Resources.LoadAsync<T>(resName);
		yield return request;
		string path = StringUtility.getFilePath(resName);
		mLoadedPath[path][resName] = request.asset;
		doneCallback(request.asset, userData);
		UnityUtility.logInfo(resName + " load done!", LOG_LEVEL.LL_NORMAL);
	}
	protected IEnumerator loadPathCoroutine(string path, AssetBundleLoadDoneCallback callback)
	{
		// 查找文件夹
		List<string> fileList = new List<string>();
		FileUtility.findFiles(path, ref fileList);
		int fileCount = fileList.Count;
		List<UnityEngine.Object> resList = new List<UnityEngine.Object>();
		for (int i = 0; i < fileCount; ++i)
		{
			if(fileList[i].EndsWith(".meta"))
			{
				continue;
			}
			ResourceRequest assetRequest = Resources.LoadAsync(fileList[i]);
			yield return assetRequest;
			mLoadedPath[path].Add(fileList[i], assetRequest.asset);
		}
		callback(resList);
	}
}