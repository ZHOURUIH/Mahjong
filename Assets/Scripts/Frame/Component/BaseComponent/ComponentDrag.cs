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
		// 左键在窗口中按下
		Vector3 mousePosition = Input.mousePosition;
		if (mouseHovered(mousePosition) && Input.GetMouseButtonDown(0))
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
	}
	protected void onMouseMove(Vector3 mousePosition)
	{
		applyScreenPosition(mousePosition);
	}
	//--------------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type) { return type == typeof(ComponentDrag); }
	protected override void setBaseType(){mBaseType = typeof(ComponentDrag);}
	protected virtual void applyScreenPosition(Vector3 screenPos) { }
	protected virtual Vector3 getScreenPosition() { return Vector3.zero; }
	protected virtual bool mouseHovered(Vector3 mousePosition) { return false; }
}