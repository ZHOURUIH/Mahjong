using UnityEngine;
using System.Collections.Generic;

public class ObjectInfo
{
	public GameObject mObject;
	public string mFileWithPath;
	protected bool mUsing;
	public bool isUsing() { return mUsing; }
	public void setUsing(bool value) { mUsing = value; }
}

public class PrefabInfo
{
	public GameObject mPrefab;
	public string mFileWithPath;
	public LOAD_STATE mState;
}

public class ObjectPool : FrameBase
{
	protected Dictionary<string, Dictionary<GameObject, ObjectInfo>> mInstanceFileList;
	protected Dictionary<string, PrefabInfo> mPrefabList;
	protected Dictionary<GameObject, ObjectInfo> mInstanceList;
	protected static int mNameIDSeed;
	public ObjectPool()
	{
		mInstanceFileList = new Dictionary<string, Dictionary<GameObject, ObjectInfo>>();
		mPrefabList = new Dictionary<string, PrefabInfo>();
		mInstanceList = new Dictionary<GameObject, ObjectInfo>();
	}
	public void destroy()
	{
		foreach (var item in mInstanceList)
		{
			UnityUtility.destroyGameObject(item.Value.mObject);
		}
		mPrefabList.Clear();
		mInstanceList.Clear();
	}
	public void update(float elapsedTime)
	{
		foreach(var item in mInstanceList)
		{
			if(item.Key == null)
			{
				UnityUtility.logError("Object can not be destroy outside of ObjectManager!");
			}
		}
	}
	public GameObject createObject(string fileWithPath)
	{
		ObjectInfo obj = getUnusedObject(fileWithPath);
		if (obj == null)
		{
			obj = new ObjectInfo();
			GameObject prefab = getPrefab(fileWithPath);
			if (prefab == null)
			{
				UnityUtility.logError("can not find prefab : " + fileWithPath);
				return null;
			}
			obj.mObject = UnityUtility.instantiatePrefab(null, prefab);
			obj.mObject.name = "object";
			obj.mFileWithPath = fileWithPath;
			addObject(obj);
		}
		obj.setUsing(true);
		return obj.mObject;
	}
	public void destroyObject(GameObject obj)
	{
		if(mInstanceList.ContainsKey(obj))
		{
			mInstanceList[obj].setUsing(false);
		}
	}
	public GameObject loadPrefab(string fileWithPath, bool async = false)
	{
		if(mPrefabList.ContainsKey(fileWithPath))
		{
			return mPrefabList[fileWithPath].mPrefab;
		}
		PrefabInfo info = new PrefabInfo();
		info.mFileWithPath = fileWithPath;
		mPrefabList.Add(fileWithPath, info);
		if (async)
		{
			info.mPrefab = null;
			info.mState = LOAD_STATE.LS_LOADING;
			mResourceManager.loadResourceAsync<GameObject>(fileWithPath, onPrefabLoaded, fileWithPath, false);
		}
		else
		{
			GameObject prefab = mResourceManager.loadResource<GameObject>(fileWithPath, false);
			if (prefab == null)
			{
				UnityUtility.logInfo("can not load prefab : " + fileWithPath);
				return null;
			}
			info.mPrefab = prefab;
			info.mState = LOAD_STATE.LS_LOADED;
			return prefab;
		}
		return null;
	}
	public string createUniqueName(string preName)
	{
		return preName + StringUtility.intToString(mNameIDSeed++);
	}
	//-------------------------------------------------------------------------------------------------------------------------
	protected ObjectInfo getUnusedObject(string fileWithPath)
	{
		if(mInstanceFileList.ContainsKey(fileWithPath))
		{
			foreach (var item in mInstanceFileList[fileWithPath])
			{
				if (!item.Value.isUsing())
				{
					return item.Value;
				}
			}
		}
		return null;
	}
	protected void addObject(ObjectInfo objInfo)
	{
		if(!mInstanceFileList.ContainsKey(objInfo.mFileWithPath))
		{
			mInstanceFileList.Add(objInfo.mFileWithPath, new Dictionary<GameObject, ObjectInfo>());
		}
		mInstanceFileList[objInfo.mFileWithPath].Add(objInfo.mObject, objInfo);
		if(!mInstanceList.ContainsKey(objInfo.mObject))
		{
			mInstanceList.Add(objInfo.mObject, objInfo);
		}
		else
		{
			return;
		}
	}
	protected void removeObject(ObjectInfo objInfo)
	{
		if(mInstanceFileList.ContainsKey(objInfo.mFileWithPath))
		{
			mInstanceFileList[objInfo.mFileWithPath].Remove(objInfo.mObject);
		}
		if(mInstanceList.ContainsKey(objInfo.mObject))
		{
			mInstanceList.Remove(objInfo.mObject);
		}
	}
	protected GameObject getPrefab(string fileWithPath, bool loadIfNull = true)
	{
		// 已加载或正在加载
		if(mPrefabList.ContainsKey(fileWithPath))
		{
			if(mPrefabList[fileWithPath].mState == LOAD_STATE.LS_LOADED)
			{
				return mPrefabList[fileWithPath].mPrefab;
			}
		}
		// 未加载
		else if (loadIfNull)
		{
			return loadPrefab(fileWithPath);
		}
		return null;
	}
	protected void onPrefabLoaded(Object res, object userData)
	{
		string fileWithPath = userData as string;
		PrefabInfo info = mPrefabList[fileWithPath];
		info.mPrefab = res as GameObject;
		info.mState = LOAD_STATE.LS_LOADED;
	}
}

public class ObjectManager : FrameComponent
{
	protected GameObject mManagerObject;
	protected ObjectPool mObjectPool;
	public ObjectManager(string name)
		:base(name)
	{
		mObjectPool = new ObjectPool();
	}
	public override void init()
	{
		mManagerObject = UnityUtility.getGameObject(mGameFramework.getGameFrameObject(), "ObjectManager");
	}
	public override void destroy()
	{
		mObjectPool.destroy();
		mObjectPool = null;
		base.destroy();
	}
	public override void update(float elapsedTime)
	{
		base.update(elapsedTime);
		mObjectPool.update(elapsedTime);
	}
	// resources下的相对路径,不带后缀名
	public void loadPrefab(string fileWithPath)
	{
		mObjectPool.loadPrefab(fileWithPath, true);
	}
	// 获取和创建使用同一接口
	public GameObject createObject(string fileWithPath)
	{
		GameObject obj = mObjectPool.createObject(fileWithPath);
		// 返回前先确保物体是挂接到预设管理器下的
		if (obj.transform.parent != mManagerObject.transform)
		{
			obj.transform.SetParent(mManagerObject.transform);
		}
		// 显示物体
		obj.SetActive(true);
		return obj;
	}
	public GameObject createObject(GameObject parent, string fileWithPath)
	{
		GameObject obj = createObject(fileWithPath);
		obj.transform.SetParent(parent.transform);
		obj.transform.localPosition = Vector3.zero;
		obj.transform.localEulerAngles = Vector3.zero;
		obj.transform.localScale = Vector3.one;
		return obj;
	}
	public void destroyObject(GameObject obj)
	{
		// 隐藏物体,并且将物体重新挂接到预设管理器下,重置物体变换
		obj.transform.SetParent(mManagerObject.transform);
		obj.SetActive(false);
		obj.transform.localPosition = Vector3.zero;
		obj.transform.localEulerAngles = Vector3.zero;
		obj.transform.localScale = Vector3.one;
		mObjectPool.destroyObject(obj);
	}
}