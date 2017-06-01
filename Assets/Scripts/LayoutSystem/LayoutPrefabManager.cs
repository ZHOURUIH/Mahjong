using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class LayoutPrefabManager : GameBase
{
	protected GameObject mManagerObject;
	protected Dictionary<string, GameObject> mPrefabList;
	protected int mLoadedCount;	// 已加载的布局使用的预设数量
	public LayoutPrefabManager()
	{
		mPrefabList = new Dictionary<string, GameObject>();
		mLoadedCount = 0;
	}
	public void init()
	{
		mManagerObject = UnityUtility.getGameObject(mGameFramework.getGameFrameObject(), "LayoutPrefabManager");
		if (mManagerObject == null)
		{
			UnityUtility.logError("error: can not find LayoutPrefabManager!");
			return;
		}
		bool async = true;
		string path = CommonDefine.R_LAYOUT_PREFAB_PATH;
		List<string> fileList = mResourceManager.getFileOrBundleList(path);
		int fileCount = fileList.Count;
		for (int i = 0; i < fileCount; ++i)
		{
			string fileNameNoSuffix = StringUtility.getFileNameNoSuffix(fileList[i], true);
			mPrefabList.Add(fileNameNoSuffix.ToLower(), null);
			if(async)
			{
				mResourceManager.loadResourceAsync<GameObject>(path + fileNameNoSuffix, onLayoutPrefabLoaded, true);
			}
			else
			{
				GameObject go = mResourceManager.loadResource<GameObject>(path + fileNameNoSuffix, true);
				mPrefabList[go.name.ToLower()] = go;
				++mLoadedCount;
			}
		}
	}
	public void destroy()
	{
		;
	}
	public UnityEngine.Object getPrefab(string name)
	{
		name = name.ToLower();
		if (mPrefabList.ContainsKey(name))
		{
			return mPrefabList[name];
		}
		return null;
	}
	public GameObject instantiate(string prefabName, GameObject parent, string objectName)
	{
		prefabName = prefabName.ToLower();
		if (mPrefabList.ContainsKey(prefabName))
		{
			if (objectName == "")
			{
				UnityUtility.logError("instantiate object's name can not be empty!");
				return null;
			}
			return UnityUtility.instantiatePrefab(parent, mPrefabList[prefabName], objectName, Vector3.one, Vector3.zero, Vector3.zero);
		}
		return null;
	}
	public bool isLoadDone()
	{
		return mLoadedCount == mPrefabList.Count;
	}
	//---------------------------------------------------------------------------------------------------------
	protected void onLayoutPrefabLoaded(UnityEngine.Object res)
	{
		GameObject prefab = res as GameObject;
		mPrefabList[prefab.name.ToLower()] = prefab;
		++mLoadedCount;
	}
};