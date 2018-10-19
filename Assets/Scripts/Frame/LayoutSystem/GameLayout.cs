using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameLayout : GameBase
{
	protected LAYOUT_TYPE	mType;
	protected LayoutScript	mScript;
	protected string		mName;
	protected txNGUIPanel	mLayoutPanel;
	protected txUGUICanvas	mLayoutCanvas;
	protected txUIObject	mRoot;
	protected int			mRenderOrder;		// 渲染顺序,越大则渲染优先级越高
	protected bool			mScriptInited;		// 脚本是否已经初始化
	protected bool			mScriptControlHide;	// 是否由脚本来控制隐藏
	protected bool			mIsNGUI;			// 是否为NGUI布局,true为NGUI,false为UGUI
	protected bool			mCheckBoxAnchor;	// 是否检查布局中所有带碰撞盒的窗口是否自适应分辨率
	protected Dictionary<int, txUIObject> mObjectList;
	protected Dictionary<GameObject, txUIObject> mGameObjectSearchList;
	public GameLayout()
	{
		mObjectList = new Dictionary<int, txUIObject>();
		mGameObjectSearchList = new Dictionary<GameObject, txUIObject>();
		mScriptInited = false;
		mScriptControlHide = false;
		mCheckBoxAnchor = true;
	}
	public void setRenderOrder(int renderOrder)
	{
		mRenderOrder = renderOrder;
		getLayoutPanel().setDepth(mRenderOrder);
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
	public void init(LAYOUT_TYPE type, string name, int renderOrder, bool isNGUI)
	{	
		mName = name;
		mType = type;
		mIsNGUI = isNGUI;
		mScript = mLayoutManager.createScript(mName, this);
		if (mScript == null)
		{
			logError("can not create layout script! type : " + mType);
		}
		// 初始化布局脚本
		if (mIsNGUI)
		{
			mScript.newObject(out mLayoutPanel, mLayoutManager.getNGUIRoot(), mName);
		}
		else
		{
			mScript.newObject(out mLayoutCanvas, mLayoutManager.getUGUIRoot(), mName);
		}
		mScript.newObject(out mRoot, getLayoutPanel(), "Root");
		setRenderOrder(renderOrder);
		mScript.setRoot(mRoot);
		mScript.assignWindow();
		// 布局实例化完成,初始化之前,需要调用自适应组件的更新
		ScaleAnchor scaleAnchor = getLayoutPanel().mObject.GetComponent<ScaleAnchor>();
		PaddingAnchor customAnchor = getLayoutPanel().mObject.GetComponent<PaddingAnchor>();
		if (scaleAnchor != null)
		{
			ScaleAnchor.forceUpdateChildren(getLayoutPanel().mObject);
		}
		if (customAnchor != null)
		{
			PaddingAnchor.forceUpdateChildren(getLayoutPanel().mObject);
		}
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
		if(mLayoutPanel != null)
		{
			mLayoutPanel.destroy();
			mLayoutPanel = null;
		}
		if(mLayoutCanvas != null)
		{
			mLayoutCanvas.destroy();
			mLayoutCanvas = null;
		}
		if(mScript != null)
		{
			mScript.destroy();
			mScript = null;
		}
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
			getLayoutPanel().setActive(visible);
			mScript.onReset();
			mScript.onGameState();
			mScript.onShow(immediately, param);
		}
		// 隐藏布局时需要判断
		else
		{
			if (!mScriptControlHide)
			{
				getLayoutPanel().setActive(visible);
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
		getLayoutPanel().setActive(visible);
	}
	public bool isVisible()
	{
		return getLayoutPanel().isActive();
	}
	public txUIObject getRoot() { return mRoot; }
	public LayoutScript getScript() { return mScript; }
	public LAYOUT_TYPE getType() { return mType; }
	public string getName() { return mName; }
	public bool isNGUI() { return mIsNGUI; }
	public void setCheckBoxAnchor(bool check) { mCheckBoxAnchor = check; }
	public bool isCheckBoxAnchor() { return mCheckBoxAnchor; }
	public void registerUIObject(txUIObject uiObj)
	{
		mObjectList.Add(uiObj.mID, uiObj);
		mGameObjectSearchList.Add(uiObj.mObject, uiObj);
	}
	public void unregisterUIObject(txUIObject uiObj)
	{
		if(uiObj == null)
		{
			return;
		}
		mObjectList.Remove(uiObj.mID);
		mGameObjectSearchList.Remove(uiObj.mObject);
	}
	public void setLayer(string layer)
	{
		UnityUtility.setGameObjectLayer(getLayoutPanel(), layer);
	}
	public txUIObject getLayoutPanel()
	{
		if(mIsNGUI)
		{
			return mLayoutPanel;
		}
		else
		{
			return mLayoutCanvas;
		}
	}
}