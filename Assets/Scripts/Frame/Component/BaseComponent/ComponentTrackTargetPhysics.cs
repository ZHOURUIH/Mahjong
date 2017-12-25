using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ComponentTrackTargetPhysics : ComponentTrackTargetBase
{
	public ComponentTrackTargetPhysics(Type type, string name)
		:
		base(type, name)
	{}
	public override void fixedUpdate(float elapsedTime)
	{
		if(mTarget != null)
		{
			Vector3 targetPos = getTargetPosition();
			Vector3 curPos = getPosition();
			float remainDis = (targetPos - curPos).magnitude;
			Vector3 newPos;
			bool done = false;
			if (remainDis > mSpeed * elapsedTime)
			{
				newPos = Vector3.Normalize(targetPos - curPos) * mSpeed * elapsedTime + curPos;
			}
			else
			{
				newPos = targetPos;
				done = true;
			}
			setPosition(newPos);
			if (done && mDoneCallback != null)
			{
				mDoneCallback(this);
			}
		}
		base.fixedUpdate(elapsedTime);
	}
	//-----------------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type) { return base.isType(type) || type == typeof(ComponentTrackTargetPhysics); }
}