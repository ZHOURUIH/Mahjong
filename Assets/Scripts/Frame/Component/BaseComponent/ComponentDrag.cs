using UnityEngine;
using System;
using System.Collections;

public class ComponentDrag : GameComponent
{
	protected bool mDrag = false;
	public ComponentDrag(Type type, string name)
		:
		base(type, name)
	{}
	public override void update(float elapsedTime)
	{
		// 左键按下时,鼠标悬停在物体上,则开始拖动
		Vector3 mousePosition = mGlobalTouchSystem.getCurMousePosition();
		if(Input.GetMouseButtonDown(0) && mouseHovered(mousePosition))
		{
			onMouseDown(mousePosition);
		}
		if(mDrag && Input.GetMouseButtonUp(0))
		{
			onMouseUp(mousePosition);
		}
		if (mDrag)
		{
			onMouseMove(mousePosition);
		}
	}
	protected void onMouseDown(Vector3 mousePosition)
	{
		mDrag = true;
	}
	protected void onMouseUp(Vector3 mousePosition)
	{
		mDrag = false;
		onDragEnd();
	}
	protected void onMouseMove(Vector3 mousePosition)
	{
		applyScreenPosition(mousePosition);
		onDraging();
	}
	//--------------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type) { return type == typeof(ComponentDrag); }
	protected override void setBaseType(){mBaseType = typeof(ComponentDrag);}
	protected virtual void applyScreenPosition(Vector3 screenPos) { }
	protected virtual Vector3 getScreenPosition() { return Vector3.zero; }
	protected virtual bool mouseHovered(Vector3 mousePosition) { return false; }
	protected virtual void onDragEnd() { }
	protected virtual void onDraging() { }
}