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

public abstract class LayoutScript : CommandReceiver
{
	protected List<int> mDelayCmdList;  // 布局显示和隐藏时的延迟命令列表,当命令执行时,会从列表中移除该命令
	protected GameLayout mLayout;
	protected txUIObject mRoot;
	protected LAYOUT_TYPE mType;
	public LayoutScript(string name, GameLayout layout)
		:
		base(name)
	{
		mLayout = layout;
		mType = mLayout.getType();
		mDelayCmdList = new List<int>();
		LayoutRegister.onScriptChanged(this);
	}
	public override void destroy()
	{
		LayoutRegister.onScriptChanged(this, false);
		base.destroy();
	}
	public LAYOUT_TYPE getType() { return mType; }
	public GameLayout getLayout() { return mLayout; }
	public void setRoot(txUIObject root) { mRoot = root; }
	public txUIObject getRoot() { return mRoot; }
	// 用于接收GlobalTouchSystem处理的输入事件
	public void registeBoxCollider(txUIObject obj, BoxColliderClickCallback clickCallback = null,
		BoxColliderHoverCallback hoverCallback = null, BoxColliderPressCallback pressCallback = null)
	{
		mGlobalTouchSystem.registeBoxCollider(obj, clickCallback, pressCallback, hoverCallback);
	}
	public void registeBoxCollider(txUIObject obj, bool passRay)
	{
		mGlobalTouchSystem.registeBoxCollider(obj, null, null, null);
		obj.setPassRay(passRay);
	}
	// 用于接收NGUI处理的输入事件
	public void registeBoxColliderNGUI(txUIObject obj, UIEventListener.VoidDelegate clickCallback,
		UIEventListener.BoolDelegate pressCallback = null, UIEventListener.BoolDelegate hoverCallback = null)
	{
		mGlobalTouchSystem.registeBoxColliderNGUI(obj, clickCallback, pressCallback, hoverCallback);
	}
	public void unregisteBoxCollider(txUIObject obj)
	{
		mGlobalTouchSystem.unregisteBoxCollider(obj);
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
		if(parent == null)
		{
			parent = mRoot;
		}
		GameObject gameObject = getGameObject(parent.mObject, name);
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
		GameObject go = UnityUtility.createObject(name);
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
	public T createObject<T>(string name, bool active = true) where T : txUIObject, new()
	{
		return createObject<T>(null, name, active);
	}
	// 创建txUIObject,并且在布局中查找GameObject分配到txUIObject
	// active为-1则表示不设置active,0表示false,1表示true
	public T newObject<T>(out T obj, txUIObject parent, string name, int active = -1) where T : txUIObject, new()
	{
		obj = null;
		GameObject parentObj = (parent != null) ? parent.mObject : null;
		GameObject gameObject = getGameObject(parentObj, name);
		if (gameObject == null)
		{
			logError("object is null, name : " + name);
			return obj;
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
		return obj;
	}
	public T newObject<T>(out T obj, string name, int active = -1) where T : txUIObject, new()
	{
		return newObject<T>(out obj, mRoot, name, active);
	}
	public static T newUIObject<T>(string name, txUIObject parent, GameLayout layout, GameObject gameObj) where T : txUIObject, new()
	{
		T obj = new T();
		obj.init(layout, gameObj, parent);
		return obj;
	}
	public void instantiateObject(txUIObject parent, string prefabName, string name)
	{
		GameObject gameObject = mLayoutSubPrefabManager.instantiate(prefabName, parent.mObject, name);
		gameObject.SetActive(false);
	}
	public void instantiateObject(txUIObject parent, string name)
	{
		GameObject gameObject = mLayoutSubPrefabManager.instantiate(name, parent.mObject, name);
		gameObject.SetActive(false);
	}
	public void destroyObject(txUIObject obj, bool immediately = false)
	{
		// 查找该节点下的所有窗口,从布局中注销
		List<GameObject> children = new List<GameObject>();
		findAllChild(obj.mObject, children);
		int count = children.Count;
		for(int i = 0; i < count; ++i)
		{
			mLayout.unregisterUIObject(mLayout.getUIObject(children[i]));
		}
		UnityUtility.destroyGameObject(obj.mObject, immediately);
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
			logError("命令执行后移除命令失败!");
		}
	}
	protected void findAllChild(GameObject obj, List<GameObject> children)
	{
		// 先把自己添加进列表
		children.Add(obj);
		// 然后把自己所有的子节点添加进列表
		Transform trans = obj.transform;
		int count = trans.childCount;
		for(int i = 0; i < count; ++i)
		{
			findAllChild(trans.GetChild(i).gameObject, children);
		}
	}
}