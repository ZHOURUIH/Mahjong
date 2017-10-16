using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class WindowComponentKeyFrameMove : ComponentKeyFrame
{
	public Vector3 mStartPos;	// 移动开始时的位置
	public Vector3 mTargetPos;
	public WindowComponentKeyFrameMove(Type type, string name)
		:
		base(type, name)
	{}
	public void setTargetPos(Vector3 pos) { mTargetPos = pos; }
	public void setStartPos(Vector3 pos) { mStartPos = pos; }
	//-------------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type) { return base.isType(type) || type == typeof(WindowComponentKeyFrameMove); }
	protected override void applyTrembling(float value)
	{
		txUIObject uiObj = mComponentOwner as txUIObject;
		Vector3 curPos = mStartPos + (mTargetPos - mStartPos) * value;
		uiObj.setLocalPosition(curPos);
	}
}