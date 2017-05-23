using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ResourceLoader : MonoBehaviour
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
	// name为Resources下的相对路径,不带后缀
	public UnityEngine.Object loadResources(string name)
	{
		string path = StringUtility.getFilePath(name);
		// 如果文件夹还未加载,则先加载文件夹
		if (!mLoadedPath.ContainsKey(path))
		{
			Dictionary<string, UnityEngine.Object> loadedResource = new Dictionary<string,UnityEngine.Object>();
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
		// 如果加载完以后仍然不存在,则返回null
		if (!mLoadedPath[path].ContainsKey(name))
		{
			return null;
		}
		else
		{
			return mLoadedPath[path][name];
		}
	}
	// name为Resources下的相对路径,不带后缀
	public bool loadResourcesAsync<T>(string name, AssetLoadDoneCallback doneCallback) where T : UnityEngine.Object
	{
		string path = StringUtility.getFilePath(name);
		// 如果文件夹还未加载,则先加载文件夹
		if (!mLoadedPath.ContainsKey(path))
		{
			Dictionary<string, UnityEngine.Object> loadedResource = new Dictionary<string, UnityEngine.Object>();
			mLoadedPath.Add(path, loadedResource);
		}
		// 已经加载,则返回true
		if (mLoadedPath[path].ContainsKey(name))
		{
			doneCallback(mLoadedPath[path][name]);
		}
		else
		{
			StartCoroutine(loadResourceAsyncCoroutine<T>(name, doneCallback));
		}
		return true;
	}
	//---------------------------------------------------------------------------------------------------------------------------------------
	protected IEnumerator loadResourceAsyncCoroutine<T>(string name, AssetLoadDoneCallback doneCallback) where T : UnityEngine.Object
	{
		ResourceRequest request = Resources.LoadAsync<T>(name);
		while (!request.isDone)
		{
			yield return null;
		}
		string path = StringUtility.getFilePath(name);
		mLoadedPath[path].Add(name, request.asset);
		doneCallback(request.asset);
	}
}