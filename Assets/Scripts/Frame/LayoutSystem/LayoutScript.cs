using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AssignInfo
{
	public txUIObject mObject;
	public string mName;
	public txUIObject mParent;
	public int mActive;
	public string mAttachedPrefab;  // 该物体下挂接的预设
}

public class WindowInfo
{
	public GameObject mGameObject;
	public string mName;
	public GameObject mParent;
}

public abstract class LayoutScript : CommandReceiver
{
	protected List<int> mDelayCmdList;  // 布局显示和隐藏时的延迟命令列表,当命令执行时,会从列表中移除该命令
	protected GameLayout mLayout;
	protected txUIObject mRoot;
	protected LAYOUT_TYPE mType;
	protected Dictionary<string, List<WindowInfo>> mAllWindowList;
	protected static LayoutRegister mLayoutRegister;
	public LayoutScript(string name, GameLayout layout)
		:
		base(name)
	{
		mLayout = layout;
		mType = mLayout.getType();
		mDelayCmdList = new List<int>();
		mAllWindowList = new Dictionary<string, List<WindowInfo>>();
		if (mLayoutRegister == null)
		{
			mLayoutRegister = new LayoutRegister();
		}
		mLayoutRegister.onScriptChanged(this);
	}
	public override void destroy()
	{
		mLayoutRegister.onScriptChanged(this, false);
		base.destroy();
	}
	public LAYOUT_TYPE getType() { return mType; }
	public GameLayout getLayout() { return mLayout; }
	public void setRoot(txUIObject root) { mRoot = root; }
	public txUIObject getRoot() { return mRoot; }
	public void findAllWindow()
	{
		findWindow(null, mRoot.mObject, ref mAllWindowList);
	}
	public void registeBoxCollider(txUIObject obj, BoxColliderClickCallback clickCallback = null,
		BoxColliderHoverCallback hoverCallback = null, BoxColliderPressCallback pressCallback = null)
	{
		mGlobalTouchSystem.registeBoxCollider(obj, clickCallback, hoverCallback, pressCallback);
	}
	public void unregisteBoxCollider(txUIObject obj)
	{
		mGlobalTouchSystem.unregisteBoxCollider(obj);
	}
	public void findWindow(GameObject parent, GameObject gameObject, ref Dictionary<string, List<WindowInfo>> windowList)
	{
		// 将自己加入列表
		string name = gameObject.name;
		WindowInfo info = new WindowInfo();
		info.mGameObject = gameObject;
		info.mName = name;
		info.mParent = parent;
		if (windowList.ContainsKey(name))
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
		for (int i = 0; i < childCount; ++i)
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
		int count = mDelayCmdList.Count;
		for (int i = 0; i < count; ++i)
		{
			mCommandSystem.interruptCommand(mDelayCmdList[i]);
		}
		mDelayCmdList.Clear();
	}
	// 在开始显示之前,需要将所有的状态都重置到加载时的状态
	public virtual void onReset() { }
	// 重置布局状态后,再根据当前游戏状态设置布局显示前的状态
	public virtual void onGameState() { }
	public abstract void onShow(bool immediately, string param);
	// immediately表示是否需要立即隐藏,即便有隐藏的动画也需要立即执行完
	public abstract void onHide(bool immediately, string param);
	public void addDelayCmd(Command cmd)
	{
		mDelayCmdList.Add(cmd.mAssignID);
		cmd.addStartCommandCallback(onCmdStarted, this);
	}
	public bool hasObject(txUIObject parent, string name)
	{
		GameObject gameObject = getObjectFromList(parent.mObject, name);
		return gameObject != null;
	}
	public T cloneObject<T>(txUIObject parent, txUIObject oriObj, string name, bool active = true) where T : txUIObject, new()
	{
		if (parent == null)
		{
			parent = mRoot;
		}
		GameObject obj = UnityUtility.cloneObject(oriObj.mObject, name);
		T window = newUIObject<T>(name, parent, mLayout, obj);
		window.setActive(active);
		obj.transform.localPosition = oriObj.mObject.transform.localPosition;
		obj.transform.localEulerAngles = oriObj.mObject.transform.localEulerAngles;
		obj.transform.localScale = oriObj.mObject.transform.localScale;
		return window;
	}
	// 创建txUIObject,并且新建GameObject,分配到txUIObject中
	public T createObject<T>(txUIObject parent, string name, bool active = true) where T : txUIObject, new()
	{
		GameObject go = new GameObject(name);
		if (parent == null)
		{
			parent = mRoot;
		}
		go.layer = parent.mObject.layer;
		T obj = newUIObject<T>(name, parent, mLayout, go);
		obj.setActive(active);
		go.transform.localScale = Vector3.one;
		go.transform.localEulerAngles = Vector3.zero;
		go.transform.localPosition = Vector3.zero;
		return obj;
	}
	// 创建txUIObject,并且在布局中查找GameObject分配到txUIObject
	// active为-1则表示不设置active,0表示false,1表示true
	public void newObject<T>(ref T obj, txUIObject parent, string name, int active = -1) where T : txUIObject, new()
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
			return;
		}
		obj = newUIObject<T>(name, parent, mLayout, gameObject);
		if (active == 0)
		{
			obj.setActive(false);
		}
		else if (active == 1)
		{
			obj.setActive(true);
		}
	}
	public void newObject<T>(ref T obj, string name, int active = -1) where T : txUIObject, new()
	{
		newObject<T>(ref obj, mRoot, name, active);
	}
	public static T newUIObject<T>(string name, txUIObject parent, GameLayout layout, GameObject gameObj) where T : txUIObject, new()
	{
		T obj = new T();
		obj.setParent(parent);
		if (parent != null)
		{
			parent.addChild(obj);
			if (gameObj.transform.parent != parent.mObject.transform)
			{
				gameObj.transform.parent = parent.mObject.transform;
			}
		}
		obj.init(layout, gameObj, parent);
		return obj;
	}
	public void instantiateObject(txUIObject parent, string name)
	{
		GameObject gameObject = mLayoutPrefabManager.instantiate(name, parent.mObject, name);
		gameObject.SetActive(false);
		findWindow(parent.mObject, gameObject, ref mAllWindowList);
	}
	public void interruptCommand(int assignID)
	{
		if (mDelayCmdList.Contains(assignID))
		{
			mDelayCmdList.Remove(assignID);
			mCommandSystem.interruptCommand(assignID);
		}
	}
	//----------------------------------------------------------------------------------------------------
	protected void onCmdStarted(object userdata, Command cmd)
	{
		if (!mDelayCmdList.Remove(cmd.mAssignID))
		{
			UnityUtility.logError("命令执行后移除命令失败!");
		}
	}
	protected GameObject getObjectFromList(GameObject parent, string name)
	{
		if (!mAllWindowList.ContainsKey(name))
		{
			return null;
		}
		List<WindowInfo> infoList = mAllWindowList[name];
		int count = infoList.Count;
		for (int i = 0; i < count; ++i)
		{
			if (infoList[i].mParent == parent)
			{
				return infoList[i].mGameObject;
			}
		}
		return null;
	}
}