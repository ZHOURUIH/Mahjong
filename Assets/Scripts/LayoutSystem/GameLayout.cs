using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameLayout : GameBase
{
	protected LAYOUT_TYPE	mType;
	protected LayoutScript	mScript;
	protected string		mName;
	protected txUIPanel		mLayoutPanel;
	protected txUIObject	mRoot;
	protected int			mRenderOrder;		// 渲染顺序,越大则渲染优先级越高
	protected bool			mScriptInited;		// 脚本是否已经初始化
	protected bool			mScriptControlHide;	// 是否由脚本来控制隐藏
	protected Dictionary<int, txUIObject> mObjectList;
	protected Dictionary<GameObject, txUIObject> mGameObjectSearchList;
	public GameLayout()
	{
		mObjectList = new Dictionary<int, txUIObject>();
		mGameObjectSearchList = new Dictionary<GameObject, txUIObject>();
		mScriptInited = false;
		mScriptControlHide = false;
	}
	public void setRenderOrder(int renderOrder)
	{
		mRenderOrder = renderOrder;
		mLayoutPanel.setDepth(mRenderOrder);
	}
	public int getRenderOrder()
	{
		return mRenderOrder;
	}
	public txUIObject getUIObject(GameObject go)
	{
		if(mGameObjectSearchList.ContainsKey(go))
		{
			return mGameObjectSearchList[go];
		}
		return null;
	}
	public LayoutScript createLayoutScript()
	{
		LayoutScript script = mLayoutManager.createScript(mType, mName, this);
		if (script == null)
		{
			UnityUtility.logError("can not create layout script! type : " + mType);
		}
		return script;
	}
	public void init(LAYOUT_TYPE type, string name, int renderOrder)
	{	
		mName = name;
		mType = type;
		mScript = createLayoutScript();
		// 初始化布局脚本
		mLayoutPanel = mScript.newObject<txUIPanel>(mLayoutManager.getUIRoot(), mName);
		setRenderOrder(renderOrder);
		mRoot = mScript.newObject<txUIObject>(mLayoutPanel, "Root");
		mScript.setRoot(mRoot);
		mScript.findAllWindow();
		mScript.assignWindow();
		mScript.init();
		mScriptInited = true;
		// 加载完布局后强制隐藏
		setVisibleForce(false);
	}
	public void update(float elapsedTime)
	{
		if (isVisible() && mScript != null && mScriptInited)
		{
			// 先更新所有的UI物体
			foreach (var obj in mObjectList)
			{
				if (obj.Value.isActive())
				{
					obj.Value.update(elapsedTime);
				}
			}
			mScript.update(elapsedTime);
		}
	}
	public void destroy()
	{
		mLayoutPanel.destroy();
		mLayoutPanel = null;
		mScript.destroy();
		mScript = null;
	}
	public List<BoxCollider> getAllBoxCollider()
	{
		List<BoxCollider> boxList = new List<BoxCollider>();
		foreach(var obj in mObjectList)
		{
			BoxCollider collider = obj.Value.mObject.GetComponent<BoxCollider>();
			if(collider != null)
			{
				boxList.Add(collider);
			}
		}
		return boxList;
	}
	public txUIObject getRoot(){ return mRoot; }
	// 设置是否会立即隐藏,应该由布局脚本调用
	public void setScriptControlHide(bool control) { mScriptControlHide = control; }
	public void setVisible(bool visible, bool immediately, string param)
	{
		if (mScript == null || !mScriptInited)
		{
			return;
		}
		// 设置布局显示或者隐藏时需要先通知脚本开始显示或隐藏
		mScript.notifyStartShowOrHide();
		// 显示布局时立即显示
		if (visible)
		{
			mLayoutPanel.setActive(visible);
			mScript.onReset();
			mScript.onGameState();
			mScript.onShow(immediately, param);
		}
		// 隐藏布局时需要判断
		else
		{
			if (!mScriptControlHide)
			{
				mLayoutPanel.setActive(visible);
			}
			mScript.onHide(immediately, param);
		}
	}
	public void setVisibleForce(bool visible)
	{
		if (mScript == null || !mScriptInited)
		{
			return;
		}
		// 直接设置布局显示或隐藏
		mLayoutPanel.setActive(visible);
	}
	public bool isVisible()
	{
		if (mLayoutPanel != null)
		{
			return mLayoutPanel.isActive();
		}
		return false;
	}
	public LayoutScript getScript() { return mScript; }
	public LAYOUT_TYPE getType() { return mType; }
	public string getName() { return mName; }
	public void registerUIObject(txUIObject uiObj)
	{
		mObjectList.Add(uiObj.mID, uiObj);
		mGameObjectSearchList.Add(uiObj.mObject, uiObj);
	}
	public void unregisterUIObject(txUIObject uiObj)
	{
		if(mObjectList.ContainsKey(uiObj.mID))
		{
			mObjectList.Remove(uiObj.mID);
		}
		if(mGameObjectSearchList.ContainsKey(uiObj.mObject))
		{
			mGameObjectSearchList.Remove(uiObj.mObject);
		}
	}
	public void setLayer(string layer)
	{
		UnityUtility.setGameObjectLayer(mLayoutPanel, layer);
	}
}