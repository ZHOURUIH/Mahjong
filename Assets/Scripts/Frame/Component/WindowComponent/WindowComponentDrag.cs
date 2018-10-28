using UnityEngine;
using System;
using System.Collections;

public class WindowComponentDrag : ComponentDrag
{
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
		window.setLocalPosition(UnityUtility.screenPosToWindowPos(screenPos, window.getParent()) - 
			new Vector2(Screen.currentResolution.width / 2.0f, Screen.currentResolution.height / 2.0f));
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
		return (mComponentOwner as txUIObject).getBoxCollider().Raycast(UnityUtility.getRay(mousePosition), out hit, 10000.0f);
	}
}