using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class LayoutSubPrefabManager : FrameComponent
{
	protected GameObject mManagerObject;
	protected Dictionary<string, GameObject> mPrefabList;
	protected int mLoadedCount; // 已加载的布局使用的预设数量
	public LayoutSubPrefabManager(string name)
		: base(name)
	{
		mPrefabList = new Dictionary<string, GameObject>();
		mLoadedCount = 0;
	}
	public override void init()
	{
		mManagerObject = getGameObject(mGameFramework.getGameFrameObject(), "LayoutSubPrefabManager", true);
	}
	// 加载所有LayoutPrefab下的预设
	public void loadAll(bool async)
	{
		loadSubPrefabFolder(CommonDefine.R_NGUI_SUB_PREFAB_PATH, async);
		loadSubPrefabFolder(CommonDefine.R_UGUI_SUB_PREFAB_PATH, async);
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
				logError("instantiate object's name can not be empty!");
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
	protected void loadSubPrefabFolder(string path, bool async)
	{
		List<string> fileList = mResourceManager.getFileList(path);
		int fileCount = fileList.Count;
		for (int i = 0; i < fileCount; ++i)
		{
			string fileNameNoSuffix = fileList[i];
			mPrefabList.Add(fileNameNoSuffix.ToLower(), null);
			if (async)
			{
				mResourceManager.loadResourceAsync<GameObject>(path + fileNameNoSuffix, onLayoutPrefabLoaded, null, true);
			}
			else
			{
				GameObject go = mResourceManager.loadResource<GameObject>(path + fileNameNoSuffix, true);
				mPrefabList[go.name.ToLower()] = go;
				++mLoadedCount;
			}
		}
	}
};