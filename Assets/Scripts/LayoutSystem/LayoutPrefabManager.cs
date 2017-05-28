using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class LayoutPrefabManager : GameBase
{
	protected GameObject mManagerObject;
	protected Dictionary<string, GameObject> mPrefabList;
	public LayoutPrefabManager()
	{
		mPrefabList = new Dictionary<string, GameObject>();
	}
	public void init()
	{
		mManagerObject = UnityUtility.getGameObject(mGameFramework.getGameFrameObject(), "LayoutPrefabManager");
		if (mManagerObject == null)
		{
			UnityUtility.logError("error: can not find LayoutPrefabManager!");
			return;
		}
		string path = CommonDefine.R_LAYOUT_PREFAB_PATH;
		if (mResourceManager.mLoadSource == 1)
		{
			path = path.ToLower();
		}
		List<string> fileList = mResourceManager.getFileOrBundleList(path);
		int fileCount = fileList.Count;
		for (int i = 0; i < fileCount; ++i)
		{
			string fileNameNoSuffix = StringUtility.getFileNameNoSuffix(fileList[i]);
			mResourceManager.loadResourceAsync<GameObject>(path + "/" + fileNameNoSuffix, onLayoutPrefabLoaded, true);
		}
	}
	public void destroy()
	{
		;
	}
	public UnityEngine.Object getPrefab(string name)
	{
		if(mPrefabList.ContainsKey(name))
		{
			return mPrefabList[name];
		}
		return null;
	}
	public GameObject instantiate(string prefabName, GameObject parent, string objectName = "")
	{
		if (mPrefabList.ContainsKey(prefabName))
		{
			string name = objectName != "" ? objectName : prefabName;
			return UnityUtility.instantiatePrefab(parent, mPrefabList[prefabName], name, Vector3.one, Vector3.zero, Vector3.zero);
		}
		return null;
	}
	//---------------------------------------------------------------------------------------------------------
	protected void onLayoutPrefabLoaded(UnityEngine.Object res)
	{
		mPrefabList.Add(res.name, res as GameObject);
	}
};