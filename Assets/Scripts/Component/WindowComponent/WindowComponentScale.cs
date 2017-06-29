using UnityEngine;
using System;
using System.Collections;

public class WindowComponentScale : ComponentScale
{
	public WindowComponentScale(Type type, string name)
		:
	base(type, name)
	{}
	//-----------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type){return base.isType(type) || type == typeof(WindowComponentScale);}
	protected override void applyScale(Vector3 scale, bool refreshNow, bool done)
	{
		txUIObject window = (mComponentOwner) as txUIObject;
		window.setLocalScale(new Vector3(scale.x, scale.y, 0.0f));
	}
}