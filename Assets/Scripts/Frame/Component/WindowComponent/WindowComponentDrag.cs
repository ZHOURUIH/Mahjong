using UnityEngine;
using System;
using System.Collections;

public class WindowComponentDrag : ComponentDrag
{
	protected txUIObject mDragHoverWindow;
	public WindowComponentDrag(Type type, string name)
		:
		base(type, name)
	{}
	public override void update(float elapsedTime)
	{
		base.update(elapsedTime);
	}
	//--------------------------------------------------------------------------------------------------------------
	public override void setActive(bool active)
	{
		if (active && !LT.checkStaticPanel(mComponentOwner as txUIObject))
		{
			return;
		}
		base.setActive(active);
	}
	protected override bool isType(Type type) { return base.isType(type) || type == typeof(WindowComponentDrag); }
	protected override void applyScreenPosition(Vector3 screenPos)
	{
		txUIObject window = mComponentOwner as txUIObject;
		Vector2 rootSize = WidgetUtility.getRootSize();
		window.setLocalPosition(UnityUtility.screenPosToWindowPos(screenPos, window.getParent()) - rootSize / 2);
	}
	protected override Vector3 getScreenPosition()
	{
		Camera camera = mCameraManager.getUICamera().getCamera();
		return camera.WorldToScreenPoint((mComponentOwner as txUIObject).getWorldPosition());
	}
	protected override bool mouseHovered(Vector3 mousePosition)
	{
		// 使用当前鼠标位置判断是否悬停,暂时忽略被其他窗口覆盖的情况
		RaycastHit hit;
		BoxCollider collider = (mComponentOwner as txUIObject).getBoxCollider();
		if(collider == null)
		{
			logError("not find collider, can not drag!");
			return false;
		}
		return collider.Raycast(UnityUtility.getRay(mousePosition), out hit, 10000.0f);
	}
	protected override void onDragEnd()
	{
		// 判断当前鼠标所在位置是否有窗口
		Vector3 curMousePosition = mGlobalTouchSystem.getCurMousePosition();
		txUIObject receiveWindow = mGlobalTouchSystem.getHoverWindow(curMousePosition, mComponentOwner as txUIObject);
		if(receiveWindow != null)
		{
			receiveWindow.onReceiveDrag(mComponentOwner as txUIObject);
		}
		mDragHoverWindow = null;
	}
	protected override void onDraging()
	{
		Vector3 curMousePosition = mGlobalTouchSystem.getCurMousePosition();
		txUIObject curHover = mGlobalTouchSystem.getHoverWindow(curMousePosition, mComponentOwner as txUIObject);
		// 悬停的窗口改变了
		if (curHover != mDragHoverWindow)
		{
			if(mDragHoverWindow != null)
			{
				mDragHoverWindow.onDragHoverd(mComponentOwner as txUIObject, false);
			}
			mDragHoverWindow = curHover;
			if (mDragHoverWindow != null)
			{
				mDragHoverWindow.onDragHoverd(mComponentOwner as txUIObject, true);
			}
		}
	}
}