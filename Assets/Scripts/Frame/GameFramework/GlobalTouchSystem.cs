using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ColliderCallBack
{
	public txUIObject mButton;
	public BoxColliderClickCallback mClickCallback;
	public BoxColliderHoverCallback mHoverCallback;
	public BoxColliderPressCallback mPressCallback;
}

public class UIDepth
{
	public int mPanelDepth;
	public int mWindowDepth;
	public UIDepth(int panelDepth, int windowDepth)
	{
		mPanelDepth = panelDepth;
		mWindowDepth = windowDepth;
	}
}

public class CompareFunc : IComparer<UIDepth>
{
	public int Compare(UIDepth a, UIDepth b)
	{
		// 优先比较布局的深度
		if(a.mPanelDepth > b.mPanelDepth)
		{
			return -1;
		}
		else if(a.mPanelDepth < b.mPanelDepth)
		{
			return 1;
		}
		// 如果布局深度相同,再比较窗口深度
		else
		{
			if (a.mWindowDepth > b.mWindowDepth)
			{
				return -1;
			}
			else if (a.mWindowDepth < b.mWindowDepth)
			{
				return 1;
			}
			else
			{
				return 0;
			}
		}
	}
}

public class GlobalTouchSystem : FrameComponent
{
	protected Dictionary<txUIObject, ColliderCallBack> mButtonCallbackList;
	protected SortedDictionary<UIDepth, List<txUIObject>> mButtonOrderList;	// 深度由大到小的列表
	protected Vector3 mLastMousePosition;
	protected txUIObject mHoverButton;
	protected bool mUseHover = true;        // 是否判断鼠标悬停在某个窗口
	protected bool mUseGlobalTouch = true;  // 是否使用全局触摸检测来进行界面的输入检测
	protected float mCurStayTime;
	protected float mStayTime = 0.15f;
	protected Vector3 mPressMousePosition;
	protected float mSquaredClickThreshhold = 15 * 15;  // 点击阈值的平方
	protected bool mSimulateTouch = true;		// 是否模拟触摸屏
	protected Vector2 mCurTouchPosition;        // 在模拟触摸屏的条件下,当前触摸点
	protected bool mMousePressed = false;		// 在模拟触摸屏的条件下,屏幕是否被按下
	public GlobalTouchSystem(string name)
		:base(name)
	{
		mButtonCallbackList = new Dictionary<txUIObject, ColliderCallBack>();
		mButtonOrderList = new SortedDictionary<UIDepth, List<txUIObject>>(new CompareFunc());
		mCurStayTime = -1.0f;
	}
	public override void init()
	{
		;
	}
	public override void destroy()
	{
		mButtonCallbackList.Clear();
		mButtonOrderList.Clear();
		base.destroy();
	}
	public void setUseGlobalTouch(bool use) { mUseGlobalTouch = use; }
	public Vector3 getCurMousePosition()
	{
		if(mSimulateTouch)
		{
			return mCurTouchPosition;
		}
		else
		{
			return Input.mousePosition;
		}
	}
	public txUIObject getHoverButton(Vector3 pos)
	{
		// 返回Layout深度最大的,也就是最靠前的窗口
		List<txUIObject> boxList = globalRaycast(pos);
		txUIObject forwardButton = null;
		foreach(var button in boxList)
		{
			GameLayout layout = button.mLayout;
			if (forwardButton == null || layout.getRenderOrder() > forwardButton.mLayout.getRenderOrder())
			{
				forwardButton = button;
			}
		}
		return forwardButton;
	}
	public override void update(float elapsedTime)
	{
		if (!mUseHover || !mUseGlobalTouch)
		{
			return;
		}
		// 模拟触摸时,更新触摸点
		if(mSimulateTouch && mMousePressed)
		{
			mCurTouchPosition = Input.mousePosition;
		}
		// 鼠标移动检测
		Vector3 curMousePosition = getCurMousePosition();
		if (mLastMousePosition != curMousePosition)
		{
			// 计算鼠标当前所在最前端的窗口
			txUIObject newWindow = getHoverButton(curMousePosition);
			// 判断鼠标是否还在当前窗口内
			if (mHoverButton != null)
			{
				// 鼠标已经移动到了其他窗口中,发送鼠标离开的事件
				if (newWindow != mHoverButton)
				{
					// 不过也许此时悬停窗口已经不接收输入事件了或者碰撞盒子被禁用了,需要判断一下
					if (mHoverButton.getHandleInput() && mHoverButton.getBoxCollider().enabled)
					{
						hoverWindow(mHoverButton, false);
					}
					// 找到鼠标所在的新的窗口,给该窗口发送鼠标进入的事件
					hoverWindow(newWindow, true);
				}
			}
			// 如果上一帧鼠标没有在任何窗口内,则计算这一帧鼠标所在的窗口
			else
			{
				// 发送鼠标进入的事件
				hoverWindow(newWindow, true);
			}
			mHoverButton = newWindow;
			if(mHoverButton != null)
			{
				Vector2 moveDelta = curMousePosition - mLastMousePosition;
				mHoverButton.onMouseMove(curMousePosition, moveDelta, getLength(moveDelta) / elapsedTime);
			}
			mLastMousePosition = curMousePosition;
			mCurStayTime = 0.0f;
		}
		else
		{
			if(mCurStayTime >= 0.0f)
			{
				mCurStayTime += elapsedTime;
				if (mCurStayTime >= mStayTime)
				{
					mCurStayTime = -1.0f;
					if (mHoverButton != null)
					{
						mHoverButton.onMouseStay(curMousePosition);
					}
				}
			}
		}
	}
	// 用于接收NGUI处理的输入事件,不经过GlobalTouchSystem
	public void registeBoxColliderNGUI(txUIObject button, UIEventListener.VoidDelegate clickCallback,
		UIEventListener.BoolDelegate pressCallback = null, UIEventListener.BoolDelegate hoverCallback = null)
	{
		button.setClickCallback(clickCallback);
		button.setPressCallback(pressCallback);
		button.setHoverCallback(hoverCallback);
	}
	// 注册碰撞器,只有注册了的碰撞器才会进行检测
	public void registeBoxCollider(txUIObject button, BoxColliderClickCallback clickCallback = null, 
		BoxColliderPressCallback pressCallback = null, BoxColliderHoverCallback hoverCallback = null)
	{
		if(mUseGlobalTouch)
		{
			if (!mButtonCallbackList.ContainsKey(button))
			{
				ColliderCallBack colliderCallback = new ColliderCallBack();
				colliderCallback.mButton = button;
				colliderCallback.mClickCallback = clickCallback;
				colliderCallback.mHoverCallback = hoverCallback;
				colliderCallback.mPressCallback = pressCallback;
				mButtonCallbackList.Add(button, colliderCallback);
				UIDepth depth = new UIDepth(button.mLayout.getRenderOrder(), button.getDepth());
				if (!mButtonOrderList.ContainsKey(depth))
				{
					mButtonOrderList.Add(depth, new List<txUIObject>());
				}
				mButtonOrderList[depth].Add(button);
			}
		}
		// 如果不使用
		else
		{
			logError("Not Active Global Touch! use public void registeBoxCollider(txUIObject button, " + 
				"UIEventListener.VoidDelegate clickCallback = null,UIEventListener.BoolDelegate pressCallback = null, " + 
				"UIEventListener.BoolDelegate hoverCallback = null) instead");
		}
	}
	// 注销碰撞器
	public void unregisteBoxCollider(txUIObject button)
	{
		if (mButtonCallbackList.ContainsKey(button))
		{
			mButtonCallbackList.Remove(button);
			UIDepth depth = new UIDepth(button.mLayout.getRenderOrder(), button.getDepth());
			mButtonOrderList[depth].Remove(button);
		}
	}
	public void notifyButtonDepthChanged(txUIObject button, int lastDepth)
	{
		// 如果之前没有记录过,则不做判断
		UIDepth oldDepth = new UIDepth(button.mLayout.getRenderOrder(), lastDepth);
		if (!mButtonOrderList.ContainsKey(oldDepth) || !mButtonOrderList[oldDepth].Contains(button))
		{
			return;
		}
		// 移除旧的按钮
		mButtonOrderList[oldDepth].Remove(button);
		// 添加新的按钮
		UIDepth newDepth = new UIDepth(button.mLayout.getRenderOrder(), button.getDepth());
		if (!mButtonOrderList.ContainsKey(newDepth))
		{
			mButtonOrderList.Add(newDepth, new List<txUIObject>());
		}
		mButtonOrderList[newDepth].Add(button);
	}
	public void notifyGlobalPress(bool press)
	{
		// 开始触摸时记录触摸状态,同步上一次的触摸位置
		if (mSimulateTouch)
		{
			mMousePressed = press;
			if(mMousePressed)
			{
				mCurTouchPosition = Input.mousePosition;
				mLastMousePosition = mCurTouchPosition;
			}
		}
		Vector3 mousePosition = getCurMousePosition();
		var raycast = globalRaycast(mousePosition);
		foreach (var button in raycast)
		{
			if (mButtonCallbackList[button].mPressCallback != null)
			{
				mButtonCallbackList[button].mPressCallback(button, press);
			}
			if (press)
			{
				button.onMouseDown(mousePosition);
			}
			else
			{
				button.onMouseUp(mousePosition);
			}
		}
		if(!press)
		{
			if(getSquaredLength(mPressMousePosition - mousePosition) <= mSquaredClickThreshhold)
			{
				// 检测所有拣选到的盒子
				foreach (var button in raycast)
				{
					if (mButtonCallbackList[button].mClickCallback != null)
					{
						mButtonCallbackList[button].mClickCallback(button);
					}
				}
				if (raycast.Count == 0)
				{
					GameScene gameScene = mGameSceneManager.getCurScene();
					gameScene.notifyScreenActived();
				}
			}
		}
		else
		{
			mPressMousePosition = mousePosition;
		}
	}
	//--------------------------------------------------------------------------------------------------------------------------
	// 全局射线检测
	protected List<txUIObject> globalRaycast(Vector3 mousePos)
	{
		Ray ray = UnityUtility.getRay(mousePos);
		var raycastRet = UnityUtility.raycast(ray, mButtonOrderList);
		return raycastRet;
	}
	protected void hoverWindow(txUIObject window, bool hover)
	{
		if (window != null)
		{
			window.setMouseHovered(hover);
			if (mButtonCallbackList[window].mHoverCallback != null)
			{
				mButtonCallbackList[window].mHoverCallback(window, hover);
			}
			if(hover)
			{
				window.onMouseEnter();
			}
			else
			{
				window.onMouseLeave();
			}
		}
	}
}
