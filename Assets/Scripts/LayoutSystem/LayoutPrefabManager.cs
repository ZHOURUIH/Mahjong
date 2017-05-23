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
		List<string> fileList = new List<string>();
		List<string> patterns = new List<string>();
		patterns.Add(".prefab");
		patterns.Add(CommonDefine.ASSET_BUNDLE_SUFFIX);
		string filePath = "";
		string prefabPath = "";
		// 如果在Resources文件夹中找不到,则需要到StreamingAssets中找
		if (FileUtility.isDirExist(CommonDefine.F_ASSETS_PATH + CommonDefine.A_LAYOUT_PREFAB_PATH))
		{
			filePath = CommonDefine.A_LAYOUT_PREFAB_PATH;
			prefabPath = CommonDefine.R_LAYOUT_PREFAB_PATH;
		}
		else
		{
			filePath = CommonDefine.A_BUNDLE_LAYOUT_PREFAB_PATH;
			prefabPath = CommonDefine.R_LAYOUT_PREFAB_PATH.ToLower();
		}
		FileUtility.findFiles(filePath, ref fileList, patterns);
		int fileCount = fileList.Count;
		for (int i = 0; i < fileCount; ++i)
		{
			string fileName = fileList[i];
			string prefabName = StringUtility.getFileNameNoSuffix(ref fileName);
			GameObject prefabObject = mResourceManager.loadResource<GameObject>(prefabPath + prefabName, true);
			mPrefabList.Add(prefabName, prefabObject);
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
			GameObject obj = UnityUtility.instantiatePrefab(mPrefabList[prefabName]);
			obj.name = objectName != "" ? objectName : prefabName;
			obj.transform.parent = parent.transform;
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localEulerAngles = Vector3.zero;
			obj.transform.localScale = Vector3.one;
			return obj;
		}
		return null;
	}
};