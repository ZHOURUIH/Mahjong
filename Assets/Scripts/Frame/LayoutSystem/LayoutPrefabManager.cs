using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class LayoutPrefabManager : FrameComponent
{
	protected GameObject mManagerObject;
	protected Dictionary<string, GameObject> mPrefabList;
	protected int mLoadedCount;	// 已加载的布局使用的预设数量
	public LayoutPrefabManager(string name)
		:base(name)
	{
		mPrefabList = new Dictionary<string, GameObject>();
		mLoadedCount = 0;
	}
	public override void init()
	{
		mManagerObject = UnityUtility.getGameObject(mGameFramework.getGameFrameObject(), "LayoutPrefabManager", true);
	}
	// 加载所有LayoutPrefab下的预设
	public void loadAll(bool async)
	{
		string path = CommonDefine.R_NGUI_SUB_PREFAB_PATH;
		List<string> fileList = mResourceManager.getFileList(path);
		int fileCount = fileList.Count;
		for (int i = 0; i < fileCount; ++i)
		{
			string fileNameNoSuffix = fileList[i];
			mPrefabList.Add(fileNameNoSuffix.ToLower(), null);
			if (async)
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
	public override void destroy()
	{
		base.destroy();
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
	public float getLoadedPercent()
	{
		if (mPrefabList.Count == 0)
		{
			return 1.0f;
		}
		return (float)mLoadedCount / (float)mPrefabList.Count;
	}
	//---------------------------------------------------------------------------------------------------------
	protected void onLayoutPrefabLoaded(UnityEngine.Object res, object userData)
	{
		GameObject prefab = res as GameObject;
		mPrefabList[prefab.name.ToLower()] = prefab;
		++mLoadedCount;
	}
};