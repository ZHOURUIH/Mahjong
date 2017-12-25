using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 被动拖尾是设置拖尾的相对移动,模拟物体移动效果,适用于静止的物体
public class RibbonTrailStatic : RibbonTrail
{
	protected float mSpeed = 1.0f;
	public override void update(float elapsedTime)
	{
		if(mObject == null)
		{
			return;
		}
		// 更新所有片段的位置
		Vector3 dir = mTransform.parent.localToWorldMatrix.MultiplyVector(Vector3.back);
		Vector3 moveDelta = dir * mSpeed * elapsedTime;
		int count = mSectionList.Count;
		for (int i = 0; i < count; ++i)
		{
			mSectionList[i].move(moveDelta);
		}
		// 如果最新的一个点距离当前已经超过了最小距离,则添加一个点
		Transform transform = mObject.transform;
		if(mSectionList.Count == 0 || (mSectionList[0].mStart - transform.localPosition).sqrMagnitude > mMinDistance * mMinDistance)
		{
			addSection(transform.position, transform.position + transform.localToWorldMatrix.MultiplyVector(Vector3.up * mTrailHeight));
		}
		base.update(elapsedTime);
	}
	public void setSpeed(float speed)
	{
		mSpeed = speed;
	}
}