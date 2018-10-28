using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CameraLinkerAcceleration : CameraLinker
{
	protected Spring mSpringX;
	protected Spring mSpringY;
	protected Spring mSpringZ;
	protected bool mUseTargetYaw;           // 是否使用目标物体的旋转来旋转摄像机的位置
	public CameraLinkerAcceleration(Type type, string name)
		: base(type, name)
	{
		mSpringX = new Spring();
		mSpringY = new Spring();
		mSpringZ = new Spring();
		mUseTargetYaw = true;
	}
	public override void preUpdate(float elapsedTime)
	{
		if (mLinkObject == null)
		{
			return;
		}
		// 获得加速度
		Vector3 acceleration = mLinkObject.getAcceleration();
		Vector3 curRelative = (mComponentOwner as GameCamera).getPosition() - mLinkObject.getPosition();
		float relativeAngle = getAngleFromVector(curRelative);
		acceleration = rotateVector3(acceleration, relativeAngle) * -1.0f;
		mSpringX.setCurLength(Mathf.Abs(curRelative.x));
		mSpringX.setForce(acceleration.x);
		mSpringY.setCurLength(Mathf.Abs(curRelative.y));
		mSpringY.setForce(acceleration.y);
		mSpringZ.setCurLength(Mathf.Abs(curRelative.z));
		mSpringZ.setForce(acceleration.z);
	}
	public override void setRelativePosition(Vector3 pos, CAMERA_LINKER_SWITCH switchType = CAMERA_LINKER_SWITCH.CLS_NONE, 
		bool useDefaultSwitchSpeed = true, float switchSpeed = 1.0f)
	{
		base.setRelativePosition(pos, switchType, useDefaultSwitchSpeed, switchSpeed);

		mSpringX.setNormaLength(Mathf.Abs(mRelativePosition.x));
		mSpringY.setNormaLength(Mathf.Abs(mRelativePosition.y));
		mSpringZ.setNormaLength(Mathf.Abs(mRelativePosition.z));

		mSpringX.setCurLength(Mathf.Abs(mRelativePosition.x));
		mSpringY.setCurLength(Mathf.Abs(mRelativePosition.y));
		mSpringZ.setCurLength(Mathf.Abs(mRelativePosition.z));

		mSpringX.setForce(0.0f);
		mSpringY.setForce(0.0f);
		mSpringZ.setForce(0.0f);

		mSpringX.setSpeed(0.0f);
		mSpringY.setSpeed(0.0f);
		mSpringZ.setSpeed(0.0f);
		// 改变摄像机位置
		Vector3 targetPos = mLinkObject.getPosition();
		Vector3 curPos = targetPos + mRelativePosition;
		(mComponentOwner as GameCamera).setPosition(curPos);
	}
	public void setUseTargetYaw(bool use) { mUseTargetYaw = use; }
	public bool getUseTargetYaw() { return mUseTargetYaw; }
	//----------------------------------------------------------------------------------------------------------------
	protected override void updateLinker(float elapsedTime)
	{
		mSpringX.update(elapsedTime);
		mSpringY.update(elapsedTime);
		mSpringZ.update(elapsedTime);
		float curX = 0.0f, curY = 0.0f, curZ = 0.0f;
		// 如果使用目标物体的航向角,则对相对位置进行旋转
		Vector3 relative = mRelativePosition;
		if (mUseTargetYaw)
		{
			relative = rotateVector3(relative, mLinkObject.getRotation().y * Mathf.Deg2Rad);
		}
		//判断是否为零
		Vector3 acceleration = mLinkObject.getAcceleration();
		processRelative(mSpringX, relative.x, acceleration.x, ref curX);
		processRelative(mSpringY, relative.y, acceleration.y, ref curY);
		processRelative(mSpringZ, relative.z, acceleration.z, ref curZ);
		// 改变摄像机位置
		applyRelativePosition(new Vector3(curX, curY, curZ));
	}
	protected override bool isType(Type type) { return base.isType(type) || type == typeof(CameraLinkerAcceleration); }
	protected static void processRelative(Spring spring, float relative, float acceleration, ref float curRelative)
	{
		if (!isFloatZero(relative))
		{
			curRelative = spring.getLength() * relative / Mathf.Abs(relative);
		}
		else
		{
			if (!isFloatZero(acceleration))
			{
				curRelative = spring.getLength() * acceleration / Mathf.Abs(acceleration);
			}
		}
	}
};