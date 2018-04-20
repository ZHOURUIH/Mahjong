using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CameraLinkerSmoothFollow : CameraLinker
{
	protected float mSpeedRecover = 0.5f;
	protected float mNormalSpeed = 5.0f;
	protected float mFollowPositionSpeed = 5.0f;
	protected bool mIgnoreY = false;      // 是否忽略Y轴的变化,当Y轴变化时摄像机在Y轴上的位置不会根据时间改变
	protected int mCheckGroundLayer;
	public CameraLinkerSmoothFollow(Type type, string name)
		: base(type, name)
	{
		mCheckGroundLayer = 0;
	}
	public override void update(float elapsedTime)
	{
		base.update(elapsedTime);
		if (mLinkObject == null)
		{
			return;
		}
		if(!MathUtility.isFloatEqual(mNormalSpeed, mFollowPositionSpeed))
		{
			mFollowPositionSpeed = MathUtility.lerp(mFollowPositionSpeed, mNormalSpeed, mSpeedRecover * elapsedTime);
		}
		Vector3 targetPos = mLinkObject.getWorldPosition();   // 获得目标当前坐标
		// 摄像机的目标位置
		Vector3 relative = MathUtility.rotateVector3(mRelativePosition, mLinkObject.getRotation().y * Mathf.Deg2Rad);
		Vector3 nextPos = targetPos + relative;
		// 判断与地面的交点,使摄像机始终位于地面上方
		if (mCheckGroundLayer != 0)
		{
			Ray ray = new Ray(nextPos + Vector3.up, Vector3.down);
			RaycastHit hit;
#if UNITY_EDITOR
			Debug.DrawLine(ray.origin, ray.origin + ray.direction * 10.0f, Color.blue);
#endif
			bool ret = Physics.Raycast(ray, out hit, 1000.0f, 1 << mCheckGroundLayer);
			if (ret && MathUtility.getLength(nextPos - hit.point) < mRelativePosition.y - 0.1f)
			{
				nextPos.y = hit.point.y + mRelativePosition.y - 0.1f;
			}
		}
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
	public void setCheckGroundLayer(int layer) { mCheckGroundLayer = layer; }
	//---------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type) { return base.isType(type) || type == typeof(CameraLinkerSmoothFollow); }
};