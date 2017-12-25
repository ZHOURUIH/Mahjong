using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CameraLinkerSwitchAroundTarget : CameraLinkerSwitch
{
	float mTotalAngle;
	float mRotatedAngle;
	bool mClockwise;
	float mDistanceDelta;
	float mDistanceCurrent;
	public CameraLinkerSwitchAroundTarget(CAMERA_LINKER_SWITCH type, CameraLinker parentLinker)
		:
		base(type, parentLinker)
	{
		mTotalAngle = 0.0f;
		mRotatedAngle = 0.0f;
		mClockwise = true;
		mDistanceDelta = 0.0f;
		mDistanceCurrent = 0.0f;
		mSpeed = Mathf.PI / 2.0f;
	}
	public override void init(Vector3 origin, Vector3 target, float speed)
	{
		base.init(origin, target, speed);
		if (mClockwise)
		{
			mTotalAngle = MathUtility.getAngleFromVector(mTargetRelative) - MathUtility.getAngleFromVector(mOriginRelative);
			MathUtility.adjustRadian360(ref mTotalAngle);
			mSpeed = Mathf.Abs(mSpeed);
		}
		else
		{
			mTotalAngle = MathUtility.getAngleFromVector(mOriginRelative) - MathUtility.getAngleFromVector(mTargetRelative);
			MathUtility.adjustRadian360(ref mTotalAngle);
			mSpeed = -Mathf.Abs(mSpeed);
		}
		mDistanceDelta = MathUtility.getLength(mTargetRelative) - MathUtility.getLength(mOriginRelative);
		mDistanceCurrent = 0.0f;
		mRotatedAngle = 0.0f;
	}
	public override void update(float elapsedTime)
	{
		if (mParentLinker == null)
		{
			return;
		}
		mRotatedAngle += mSpeed * elapsedTime;
		// 计算速度
		float time = mTotalAngle / mSpeed;
		float distanceSpeed = mDistanceDelta / time;
		mDistanceCurrent += distanceSpeed * elapsedTime;

		// 顺时针
		if (mClockwise)
		{
			if (mRotatedAngle >= mTotalAngle)
			{
				mRotatedAngle = mTotalAngle;
				mParentLinker.setRelativePosition(mTargetRelative);
				mParentLinker.notifyFinishSwitching(this);
				mDistanceCurrent = 0.0f;
				mRotatedAngle = 0.0f;
			}
			else
			{
				// z方向上旋转后的轴
				Vector3 rotateAxis = MathUtility.rotateVector3(mOriginRelative, mRotatedAngle);
				// 距离变化
				Vector3 projectVec = rotateAxis;
				projectVec.y = 0;
				projectVec = MathUtility.normalize(projectVec);
				projectVec = projectVec * (MathUtility.getLength(mOriginRelative) + mDistanceCurrent);
				// 高度变化
				rotateAxis.y = (mTargetRelative.y - mOriginRelative.y) * (mRotatedAngle / mTotalAngle) + mOriginRelative.y;
				//最终值
				rotateAxis.x = projectVec.x;
				rotateAxis.z = projectVec.z;
				mParentLinker.setRelativePosition(rotateAxis);
			}
		}
		// 逆时针
		else
		{
			if (mRotatedAngle <= mTotalAngle)
			{
				mRotatedAngle = mTotalAngle;
				mParentLinker.setRelativePosition(mTargetRelative);
				mParentLinker.notifyFinishSwitching(this);
				mDistanceCurrent = 0.0f;
				mRotatedAngle = 0.0f;
			}
			else
			{
				Vector3 rotateAxis = MathUtility.rotateVector3(mOriginRelative, mRotatedAngle);
				Vector3 projectVec = rotateAxis;
				projectVec.y = 0;
				projectVec = MathUtility.normalize(projectVec);
				projectVec = projectVec * (MathUtility.getLength(mOriginRelative) + mDistanceCurrent);

				rotateAxis.y = (mTargetRelative.y - mOriginRelative.y) * (mRotatedAngle / mTotalAngle) + mOriginRelative.y;
				rotateAxis.x = projectVec.x;
				rotateAxis.z = projectVec.z;
				mParentLinker.setRelativePosition(rotateAxis);
			}
		}
	}
	public override void destroy()
	{
		base.destroy();
	}
}