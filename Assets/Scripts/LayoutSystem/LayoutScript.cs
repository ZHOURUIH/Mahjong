using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AssignInfo
{
	public txUIObject mObject;
	public string     mName;
	public txUIObject mParent;
	public int		  mActive;
	public string	  mAttachedPrefab;	// 该物体下挂接的预设
}

public class WindowInfo
{
	public GameObject mGameObject;
	public string	  mName;
	public GameObject mParent;
}

public abstract class LayoutScript : CommandReceiver
{
	protected Dictionary<Command, float>		   mDelayCmdList;	// 布局显示和隐藏时的延迟命令列表,当命令执行时,会从列表中移除该命令
	protected GameLayout						   mLayout;
	protected txUIObject						   mRoot;
	protected LAYOUT_TYPE						   mType;
	protected Dictionary<string, List<WindowInfo>> mAllWindowList;
	public LayoutScript(LAYOUT_TYPE type, string name, GameLayout layout)
		:
		base(name)
	{
		mType = type;
		mLayout = layout;
		mDelayCmdList = new Dictionary<Command, float>();
		mAllWindowList = new Dictionary<string, List<WindowInfo>>();
	}
	public LAYOUT_TYPE getType() { return mType; }
	public GameLayout getLayout() { return mLayout; }
	public void setRoot(txUIObject root) { mRoot = root; }
	public txUIObject getRoot() { return mRoot; }
	public void findAllWindow()
	{
		findWindow(null, mRoot.mObject, ref mAllWindowList);
	}
	public void findWindow(GameObject parent, GameObject gameObject, ref Dictionary<string, List<WindowInfo>> windowList)
	{
		// 将自己加入列表
		string name = gameObject.name;
		WindowInfo info = new WindowInfo();
		info.mGameObject = gameObject;
		info.mName = name;
		info.mParent = parent;
		if(windowList.ContainsKey(name))
		{
			windowList[name].Add(info);
		}
		else
		{
			List<WindowInfo> infoList = new List<WindowInfo>();
			infoList.Add(info);
			windowList.Add(name, infoList);
		}
		// 再将所有子窗口加入列表
		Transform transform = gameObject.transform;
		int childCount = transform.childCount;
		for(int i = 0; i < childCount; ++i)
		{
			findWindow(gameObject, transform.GetChild(i).gameObject, ref windowList);
		}
	}
	public abstract void assignWindow();
	public abstract void init();
	public virtual void update(float elapsedTime) { }
	// 通知脚本开始显示或隐藏,中断全部命令
	public void notifyStartShowOrHide()
	{
		foreach(var cmd in mDelayCmdList)
		{
			mCommandSystem.interruptCommand(cmd.Key);
		}
		mDelayCmdList.Clear();
	}
	// 在开始显示之前,需要将所有的状态都重置到加载时的状态
	public virtual void onReset(){}
	public abstract void onShow(bool immediately, string param);
	// immediately表示是否需要立即隐藏,即便有隐藏的动画也需要立即执行完
	public abstract void onHide(bool immediately, string param);
	public void addDelayCmd(Command cmd)
	{
		mDelayCmdList.Add(cmd, 0.0f);
		cmd.addStartCommandCallback(onCmdStarted, this);
	}
	// 创建txUIObject,并且新建GameObject,分配到txUIObject中
	public T createObject<T>(txUIObject parent, string name, bool active) where T : txUIObject, new()
	{
		T obj = new T();
		GameObject go = new GameObject(name);
		if(parent != null && parent.mObject != null)
		{
			go.transform.parent = parent.mObject.transform;
		}
		go.transform.localScale = Vector3.one;
		go.layer = parent.mObject.layer;
		obj.init(mLayout, go);
		obj.setActive(active);
		return obj;
	}
	// 创建txUIObject,并且在布局中查找GameObject分配到txUIObject
	// active为-1则表示不设置active,0表示false,1表示true
	public T newObject<T>(txUIObject parent, string name, int active = -1) where T : txUIObject, new()
	{
		GameObject parentObj = (parent != null) ? parent.mObject : null;
		GameObject gameObject = getObjectFromList(parentObj, name);
		// 先在全部子窗口列表中查找,查找不到,再去场景中查找
		if (gameObject == null)
		{
			gameObject = UnityUtility.getGameObject(parentObj, name);
		}
		if (gameObject == null)
		{
			UnityUtility.logError("object is null, name : " + name);
			return null;
		}
		T obj = new T();
		obj.init(mLayout, gameObject);
		if (active == 0)
		{
			obj.setActive(false);
		}
		else if (active == 1)
		{
			obj.setActive(true);
		}
		return obj;
	}
	public T newObject<T>(string name, int active = -1) where T : txUIObject, new()
	{
		return newObject<T>(mLayout.getRoot(), name, active);
	}
	public void instantiateObject(txUIObject parent, string name)
	{
		GameObject gameObject = UnityUtility.instantiatePrefab(parent.mObject, CommonDefine.R_LAYOUT_PREFAB_PATH + name);
		findWindow(parent.mObject, gameObject, ref mAllWindowList);
	}
	//----------------------------------------------------------------------------------------------------
	protected void onCmdStarted(object userdata, Command cmd)
	{
		mDelayCmdList.Remove(cmd);
	}
	protected GameObject getObjectFromList(GameObject parent, string name)
	{
		if(!mAllWindowList.ContainsKey(name))
		{
			return null;
		}
		List<WindowInfo> infoList = mAllWindowList[name];
		int count = infoList.Count;
		for(int i = 0; i < count; ++i)
		{
			if(infoList[i].mParent == parent)
			{
				return infoList[i].mGameObject;
			}
		}
		return null;
	}
}