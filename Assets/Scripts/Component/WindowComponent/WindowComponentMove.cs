using UnityEngine;
using System;
using System.Collections;

public class WindowComponentMove : ComponentMove
{
	public WindowComponentMove(Type type, string name)
		:
		base(type, name)
	{ }
	public override bool isType(Type type)
	{
		return base.isType(type) || type == typeof(WindowComponentMove);
	}
	public override void applyMove(Vector3 position, bool done = false)
	{
		txUIObject window = mComponentOwner as txUIObject;
		window.setLocalPosition(position);
	}
}