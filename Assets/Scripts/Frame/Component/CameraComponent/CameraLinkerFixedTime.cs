using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CameraLinkerFixedTime : CameraLinker
{
	protected float mTime;
	protected bool mIgnoreY;      // 是否忽略Y轴的变化,当Y轴变化时摄像机在Y轴上的位置不会根据时间改变
	public CameraLinkerFixedTime(Type type, string name)
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
		GameCamera camera = mComponentOwner as GameCamera;
		Vector3 targetPos = getCenterPosition(); ;   // 获得目标当前坐标

		// 摄像机的目标位置
		Vector3 nextPos = targetPos + mRelativePosition;
		// 得到摄像机当前位置
		Vector3 cameraCurPos = camera.getPosition();
		// 计算总距离
		float totalDistance = MathUtility.getLength(cameraCurPos - nextPos);
		Vector3 cameraNewPos = cameraCurPos;
		if (totalDistance > 0.01f)
		{
			// 计算速度
			float speed = totalDistance / mTime;
			// 这一帧移动的距离
			float moveDis = speed * elapsedTime;
			// 如果距离太大以至于超过了目标点,则直接设置到目标点
			if (moveDis >= totalDistance)
			{
				cameraNewPos = nextPos;
			}
			else
			{
				Vector3 dir = MathUtility.normalize(nextPos - cameraCurPos);
				cameraNewPos = moveDis * dir + cameraCurPos;
				if (mIgnoreY)
				{
					cameraNewPos.y = nextPos.y;
				}
			}
		}
		else
		{
			cameraNewPos = nextPos;
		}
		applyRelativePosition(cameraNewPos - targetPos);
	}
	void setFixedTime(float time) { mTime = time; }
	void setIgnoreY(bool ignore) { mIgnoreY = ignore; }
	bool getIgnoreY() { return mIgnoreY; }
	//---------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type) { return base.isType(type) || type == typeof(CameraLinkerFixedTime); }
};