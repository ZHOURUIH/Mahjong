using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// 相对位置固定,但是旋转会平滑过渡
public class CameraLinkerSmoothRotate : CameraLinker
{
	protected float mSmoothRotateSpeed = 5.0f;
	protected Vector3 mCurRelative;
	public CameraLinkerSmoothRotate(Type type, string name)
		: base(type, name)
	{
		;
	}
	//---------------------------------------------------------------------------------------
	protected override bool isType(Type type) { return base.isType(type) || type == typeof(CameraLinkerSmoothRotate); }
	protected override void updateLinker(float elapsedTime)
	{
		// 如果使用目标物体的航向角,则对相对位置进行旋转
		float targetRadianYaw = mLinkObject.getRotation().y * Mathf.Deg2Rad;
		// 使用摄像机自身的航向角计算相对位置的航向角
		float curYaw = MathUtility.getVectorYaw(-mCurRelative);
		MathUtility.adjustRadian360(ref targetRadianYaw);
		MathUtility.adjustRadian360(ref curYaw);
		// 调整角度范围
		if (Mathf.Abs(curYaw - targetRadianYaw) > Mathf.PI)
		{
			MathUtility.adjustRadian180(ref targetRadianYaw);
			MathUtility.adjustRadian180(ref curYaw);
		}
		curYaw = MathUtility.lerp(curYaw, targetRadianYaw, elapsedTime * mSmoothRotateSpeed, 0.01f);
		MathUtility.adjustRadian360(ref targetRadianYaw);
		MathUtility.adjustRadian360(ref curYaw);
		float curPitch = MathUtility.getVectorPitch(-mRelativePosition);
		Vector3 newRelative = -MathUtility.getDirectionFromRadianYawPitch(curYaw, curPitch) * MathUtility.getLength(mRelativePosition);
		applyRelativePosition(newRelative);
	}
	public override void applyRelativePosition(Vector3 relative)
	{
		base.applyRelativePosition(relative);
		mCurRelative = relative;
	}
};