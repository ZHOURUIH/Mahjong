using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CameraLinkerFixed : CameraLinker
{
	public CameraLinkerFixed(Type type, string name)
		: base(type, name)
	{
		;
	}
	public override void update(float elapsedTime)
	{
		base.update(elapsedTime);
		applyRelativePosition(mRelativePosition);
	}
	//---------------------------------------------------------------------------------------
	protected override bool isType(Type type) { return base.isType(type) || type == typeof(CameraLinkerFixed); }
};