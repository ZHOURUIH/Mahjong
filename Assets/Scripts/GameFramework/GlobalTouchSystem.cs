using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ColliderCallBack
{
	public txUIButton mButton;
	public BoxColliderClickCallback mClickCallback;
	public BoxColliderHoverCallback mHoverCallback;
	public BoxColliderPressCallback mPressCallback;
}

public class CompareFunc : IComparer<int>
{
	public int Compare(int a, int b)
	{
		if(a > b)
		{
			return -1;
		}
		else if(a < b)
		{
			return 1;
		}
		else
		{
			return 0;
		}
	}
}

public class GlobalTouchSystem : GameBase
{
	protected Dictionary<txUIButton, ColliderCallBack> mButtonCallbackList;
	protected SortedDictionary<int, List<txUIButton>> mButtonOrderList;	// 深度由大到小的列表
	protected Vector3 mLastMousePosition;
	protected txUIButton mHoverButton;
	protected bool mUseHover = false;		// 是否判断鼠标悬停在某个窗口
	public GlobalTouchSystem()
	{
		mButtonCallbackList = new Dictionary<txUIButton, ColliderCallBack>();
		mButtonOrderList = new SortedDictionary<int, List<txUIButton>>(new CompareFunc());
	}
	public void init()
	{
		;
	}
	public void destroy()
	{
		mButtonCallbackList.Clear();
		mButtonOrderList.Clear();
	}
	public Vector3 getCurMousePosition()
	{
		return Input.mousePosition;
	}
	public txUIButton getHoverButton(Vector3 pos)
	{
		// 返回Layout深度最大的,也就是最靠前的窗口
		List<txUIButton> boxList = globalRaycast(pos);
		txUIButton forwardButton = null;
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
	public void update(float elapsedTime)
	{
		if (!mUseHover)
		{
			return;
		}
		// 鼠标移动检测
		Vector3 curMousePosition = getCurMousePosition();
		if (mLastMousePosition != curMousePosition)
		{
			// 计算鼠标当前所在最前端的窗口
			txUIButton newWindow = getHoverButton(curMousePosition);
			// 判断鼠标是否还在当前窗口内
			if (mHoverButton != null)
			{
				// 鼠标已经移动到了其他窗口中,发送鼠标离开的事件
				if (newWindow != mHoverButton)
				{
					// 不过也许此时悬停窗口已经不接收输入事件了或者碰撞盒子被禁用了,需要判断一下
					if (mHoverButton.getHandleInput() && mHoverButton.getBoxCollider().enabled)
					{
						if (mButtonCallbackList[mHoverButton].mHoverCallback != null)
						{
							mButtonCallbackList[mHoverButton].mHoverCallback(mHoverButton, false);
						}
					}
					// 找到鼠标所在的新的窗口,给该窗口发送鼠标进入的事件
					if (newWindow != null)
					{
						if (mButtonCallbackList[newWindow].mHoverCallback != null)
						{
							mButtonCallbackList[newWindow].mHoverCallback(newWindow, true);
						}	
					}
				}
			}
			// 如果上一帧鼠标没有在任何窗口内,则计算这一帧鼠标所在的窗口
			else
			{
				// 发送鼠标进入的事件
				if (newWindow != null)
				{
					if (mButtonCallbackList[newWindow].mHoverCallback != null)
					{
						mButtonCallbackList[newWindow].mHoverCallback(newWindow, true);
					}
				}
			}
			mHoverButton = newWindow;
			mLastMousePosition = curMousePosition;
		}
	}
	// 注册碰撞器,只有注册了的碰撞器才会进行检测
	public void registerBoxCollider(txUIButton button, BoxColliderClickCallback clickCallback, 
		BoxColliderHoverCallback hoverCallback = null, BoxColliderPressCallback pressCallback = null)
	{
		if (!mButtonCallbackList.ContainsKey(button))
		{
			ColliderCallBack colliderCallback = new ColliderCallBack();
			colliderCallback.mButton = button;
			colliderCallback.mClickCallback = clickCallback;
			colliderCallback.mHoverCallback = hoverCallback;
			colliderCallback.mPressCallback = pressCallback;
			mButtonCallbackList.Add(button, colliderCallback);
			if(!mButtonOrderList.ContainsKey(button.getDepth()))
			{
				mButtonOrderList.Add(button.getDepth(), new List<txUIButton>());
			}
			mButtonOrderList[button.getDepth()].Add(button);
		}
	}
	// 注销碰撞器
	public void unregisterBoxCollider(txUIButton button)
	{
		if (mButtonCallbackList.ContainsKey(button))
		{
			mButtonCallbackList.Remove(button);
			mButtonOrderList[button.getDepth()].Remove(button);
		}
	}
	public void notifyButtonDepthChanged(txUIButton button, int lastDepth)
	{
		// 移除旧的按钮
		mButtonOrderList[lastDepth].Remove(button);
		// 添加新的按钮
		if (!mButtonOrderList.ContainsKey(button.getDepth()))
		{
			mButtonOrderList.Add(button.getDepth(), new List<txUIButton>());
		}
		mButtonOrderList[button.getDepth()].Add(button);
	}
	public void notifyGlobalPress(bool press)
	{
		List<txUIButton> raycast = globalRaycast(getCurMousePosition());
		foreach (var button in raycast)
		{
			if (mButtonCallbackList[button].mPressCallback != null)
			{
				mButtonCallbackList[button].mPressCallback(button, press);
			}
		}
		if(!press)
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
				notifyScreenActived();
			}
		}
	}
	//--------------------------------------------------------------------------------------------------------------------------
	// 全局射线检测
	protected List<txUIButton> globalRaycast(Vector3 mousePos)
	{
		Ray ray = UnityUtility.getRay(mousePos);
		List<txUIButton> raycastRet = UnityUtility.raycast(ray, mButtonOrderList);
		return raycastRet;
	}
	// 通知脚本屏幕被激活,也就是屏幕有点击事件
	protected void notifyScreenActived()
	{
        ;
	}
}
