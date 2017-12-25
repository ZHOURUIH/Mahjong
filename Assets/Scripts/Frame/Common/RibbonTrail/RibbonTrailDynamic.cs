using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 当物体移动时产生的拖尾,适用于移动的物体
public class RibbonTrailDynamic : RibbonTrail
{
	public override void update(float elapsedTime)
	{
		if(mObject == null)
		{
			return;
		}
		// 如果最新的一个点距离当前已经超过了最小距离,则添加一个点
		Transform transform = mObject.transform;
		if(mSectionList.Count == 0 || (mSectionList[0].mStart - transform.localPosition).sqrMagnitude > mMinDistance * mMinDistance)
		{
			addSection(transform.position, transform.position + transform.localToWorldMatrix.MultiplyVector(Vector3.up * mTrailHeight));
		}
		base.update(elapsedTime);
	}
}