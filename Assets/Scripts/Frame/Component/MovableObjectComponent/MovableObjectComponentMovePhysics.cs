using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MovableObjectComponentMovePhysics : ComponentKeyFramePhysics
{
	public Vector3 mStartPos;	// 移动开始时的位置
	public Vector3 mTargetPos;
	public MovableObjectComponentMovePhysics(Type type, string name)
		:
		base(type, name)
	{}
	public void setTargetPos(Vector3 pos) { mTargetPos = pos; }
	public void setStartPos(Vector3 pos) { mStartPos = pos; }
	//-------------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type) { return base.isType(type) || type == typeof(MovableObjectComponentMovePhysics); }
	protected override void applyTrembling(float value)
	{
		MovableObject obj = mComponentOwner as MovableObject;
		Vector3 curPos = mStartPos + (mTargetPos - mStartPos) * value;
		obj.setPosition(curPos);
	}
}