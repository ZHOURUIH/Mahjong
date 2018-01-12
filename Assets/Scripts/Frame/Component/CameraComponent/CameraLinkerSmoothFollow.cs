using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CameraLinkerSmoothFollow : CameraLinker
{
	protected float mFollowPositionSpeed = 5.0f;
	protected bool mIgnoreY = false;      // 是否忽略Y轴的变化,当Y轴变化时摄像机在Y轴上的位置不会根据时间改变
	public CameraLinkerSmoothFollow(Type type, string name)
		: base(type, name)
	{
		;
	}
	public override void update(float elapsedTime)
	{
		base.update(elapsedTime);
		if (mLinkObject == null)
		{
			return;
		}
		Vector3 targetPos = mLinkObject.getWorldPosition();   // 获得目标当前坐标
		// 摄像机的目标位置
		Vector3 relative = mRelativePosition;
		relative = MathUtility.rotateVector3(relative, mLinkObject.getRotation().y * Mathf.Deg2Rad);
		Vector3 nextPos = targetPos + relative;
		// 得到摄像机当前位置
		Vector3 cameraCurPos = mCamera.getPosition();
		Vector3 cameraNewPos;
		// 如果已经很接近目标位置了,则直接设置为目标位置
		if (MathUtility.getLength(cameraCurPos - nextPos) <= 0.01f)
		{
			cameraNewPos = nextPos;
		}
		else
		{
			cameraNewPos = MathUtility.lerp(cameraCurPos, nextPos, mFollowPositionSpeed * elapsedTime);
		}
		applyRelativePosition(cameraNewPos - targetPos);
	}
	public void setFollowPositionSpeed(float speed) { mFollowPositionSpeed = speed; }
	public void setIgnoreY(bool ignore) { mIgnoreY = ignore; }
	public bool getIgnoreY() { return mIgnoreY; }
	//---------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type) { return base.isType(type) || type == typeof(CameraLinkerSmoothFollow); }
};