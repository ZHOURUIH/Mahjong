using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColliderCallBack
{
	public txUIButton mButton;
	public BoxCollider mCollider;
	public BoxColliderClickCallback mClickCallback;
	public BoxColliderHoverCallback mHoverCallback;
	public BoxColliderPressCallback mPressCallback;
}

public class GlobalTouchSystem : GameBase
{
	Dictionary<txUIButton, ColliderCallBack> mButtonCallbackList;
	Dictionary<BoxCollider, ColliderCallBack> mBoxColliderCallbackList;
	List<BoxCollider> mBoxColliderList;
	Vector3 mLastMousePosition;
	txUIButton mHoverButton;
	public GlobalTouchSystem()
	{
		mButtonCallbackList = new Dictionary<txUIButton, ColliderCallBack>();
		mBoxColliderCallbackList = new Dictionary<BoxCollider, ColliderCallBack>();
		mBoxColliderList = new List<BoxCollider>();
	}
	public void init()
	{
		;
	}
	public void destroy()
	{
		;
	}
	public txUIButton getHoverButton(Vector3 pos)
	{
		// 返回Layout深度最大的,也就是最靠前的窗口
		List<BoxCollider> boxList = globalRaycast(pos);
		txUIButton forwardButton = null;
		foreach(var box in boxList)
		{
			txUIButton button = mBoxColliderCallbackList[box].mButton;
			GameLayout layout = button.mLayout;
			if (forwardButton == null || (button.getHandleInput() && layout.getRenderOrder() > forwardButton.mLayout.getRenderOrder()))
			{
				forwardButton = button;
			}
		}
		return forwardButton;
	}
	public void update(float elapsedTime)
	{
		// 鼠标移动检测
		if (mLastMousePosition != Input.mousePosition)
		{
			// 计算鼠标当前所在最前端的窗口
			txUIButton newWindow = getHoverButton(Input.mousePosition);
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
			mLastMousePosition = Input.mousePosition;
		}
	}
	// 注册碰撞器,只有注册了的碰撞器才会进行检测
	public void registerBoxCollider(txUIButton button, BoxColliderClickCallback clickCallback, 
		BoxColliderHoverCallback hoverCallback = null, BoxColliderPressCallback pressCallback = null)
	{
		if (!mButtonCallbackList.ContainsKey(button))
		{
			BoxCollider box = button.getBoxCollider();
			ColliderCallBack colliderCallback = new ColliderCallBack();
			colliderCallback.mButton = button;
			colliderCallback.mCollider = box;
			colliderCallback.mClickCallback = clickCallback;
			colliderCallback.mHoverCallback = hoverCallback;
			colliderCallback.mPressCallback = pressCallback;
			mButtonCallbackList.Add(button, colliderCallback);
			mBoxColliderCallbackList.Add(box, colliderCallback);
			mBoxColliderList.Add(box);
		}
	}
	// 注销碰撞器
	public void unregisterBoxCollider(txUIButton button)
	{
		if (mButtonCallbackList.ContainsKey(button))
		{
			mButtonCallbackList.Remove(button);
			mBoxColliderCallbackList.Remove(button.getBoxCollider());
			mBoxColliderList.Remove(button.getBoxCollider());
		}
	}
	public void notifyGlobalPress(bool press)
	{
		List<BoxCollider> raycast = globalRaycast(Input.mousePosition);
		foreach (var box in raycast)
		{
			if (mBoxColliderCallbackList[box].mButton.getHandleInput() 
				&& mBoxColliderCallbackList[box].mCollider.enabled
				&& mBoxColliderCallbackList[box].mPressCallback != null)
			{
				mBoxColliderCallbackList[box].mPressCallback(mBoxColliderCallbackList[box].mButton, press);
			}
		}
		if(!press)
		{
			// 检测所有拣选到的盒子
			foreach (var box in raycast)
			{
				if (mBoxColliderCallbackList[box].mButton.getHandleInput() 
					&& mBoxColliderCallbackList[box].mCollider.enabled
					&& mBoxColliderCallbackList[box].mClickCallback != null)
				{
					mBoxColliderCallbackList[box].mClickCallback(mBoxColliderCallbackList[box].mButton);
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
	protected List<BoxCollider> globalRaycast(Vector3 mousePos)
	{
		Ray ray = UnityUtility.getRay(Input.mousePosition);
		List<BoxCollider> raycastRet = UnityUtility.Raycast(ray, mBoxColliderList);
		return raycastRet;
	}
	// 通知脚本屏幕被激活,也就是屏幕有点击事件
	protected void notifyScreenActived()
	{
        ;
	}
}
