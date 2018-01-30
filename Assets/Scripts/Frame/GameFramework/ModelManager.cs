using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Net;

public class ModelManager : FrameComponent
{
	protected Dictionary<string, GameObject> mModelPrefabList;
	protected Dictionary<string, GameObject> mModelInstanceList;
	public ModelManager(string name)
		:base(name)
	{
		mModelPrefabList = new Dictionary<string, GameObject>();
		mModelInstanceList = new Dictionary<string, GameObject>();
	}
	public override void init()
	{
		;
	}
	public override void destroy()
	{
		foreach (var item in mModelInstanceList)
		{
			UnityUtility.destroyGameObject(item.Value);
		}
#if !UNITY_EDITOR
		foreach (var item in mModelPrefabList)
		{
			UnityUtility.destroyGameObject(item.Value, true, true);
		}
#endif
		mModelPrefabList.Clear();
		mModelInstanceList.Clear();
		base.destroy();
	}
	public GameObject createModel(string fileWithPath, string modelName)
	{
		if(mModelInstanceList.ContainsKey(modelName))
		{
			return mModelInstanceList[modelName];
		}
		GameObject prefab = getModelPrefab(fileWithPath);
		if(prefab == null)
		{
			UnityUtility.logError("can not find model : " + fileWithPath);
			return null;
		}
		GameObject model = UnityUtility.instantiatePrefab(null, prefab);
		model.name = modelName;
		mModelInstanceList.Add(modelName, model);
		return model;
	}
	public GameObject getModel(string modelName)
	{
		if (mModelInstanceList.ContainsKey(modelName))
		{
			return mModelInstanceList[modelName];
		}
		return null;
	}
	public void destroyModel(string modelName)
	{
		if (mModelInstanceList.ContainsKey(modelName))
		{
			UnityUtility.destroyGameObject(mModelInstanceList[modelName]);
			mModelInstanceList.Remove(modelName);
		}
	}
	public void destroyModelPrefab(string fileWithPath)
	{
		if (mModelPrefabList.ContainsKey(fileWithPath))
		{
			UnityUtility.destroyGameObject(mModelPrefabList[fileWithPath]);
			mModelPrefabList.Remove(fileWithPath);
		}
	}
	//----------------------------------------------------------------------------------------------------------------------------------------
	protected GameObject getModelPrefab(string fileWithPath, bool loadIfNull = true)
	{
		if(mModelPrefabList.ContainsKey(fileWithPath))
		{
			return mModelPrefabList[fileWithPath];
		}
		else if(loadIfNull)
		{
			return loadPrefab(fileWithPath);
		}
		return null;
	}
	protected GameObject loadPrefab(string fileWithPath)
	{
		GameObject prefab = mResourceManager.loadResource<GameObject>(fileWithPath, false);
		if(prefab == null)
		{
			UnityUtility.logInfo("can not find model : " + fileWithPath);
			return null;
		}
		mModelPrefabList.Add(fileWithPath, prefab);
		return prefab;
	}
}