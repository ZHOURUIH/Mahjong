using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CameraLinkerFixed : CameraLinker
{
	protected bool mUseTargetYaw;           // 是否使用目标物体的旋转来旋转摄像机的位置
	public CameraLinkerFixed(Type type, string name)
		: base(type, name)
	{
		mUseTargetYaw = true;
	}
	public override void update(float elapsedTime)
	{
		base.update(elapsedTime);
		// 如果使用目标物体的航向角,则对相对位置进行旋转
		Vector3 relative = mRelativePosition;
		if (mUseTargetYaw)
		{
			relative = MathUtility.rotateVector3(relative, mLinkObject.getRotation().y * Mathf.Deg2Rad);
		}
		applyRelativePosition(relative);
	}
	public void setUseTargetYaw(bool use) { mUseTargetYaw = use; }
	public bool getUseTargetYaw() { return mUseTargetYaw; }
	//---------------------------------------------------------------------------------------
	protected override bool isType(Type type) { return base.isType(type) || type == typeof(CameraLinkerFixed); }
};