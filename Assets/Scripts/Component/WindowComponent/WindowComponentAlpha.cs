using UnityEngine;
using System;
using System.Collections;

public class WindowComponentAlpha : ComponentAlpha
{
	public WindowComponentAlpha(Type typeName, string name)
		:
		base(typeName, name)
	{ }
	public override bool isType(Type type)
	{
		return base.isType(type) || type == typeof(WindowComponentAlpha);
	}
	public override void applyAlpha(float alpha, bool done = false)
	{
		txUIObject window = mComponentOwner as txUIObject;
		window.setAlpha(alpha);
	}
}