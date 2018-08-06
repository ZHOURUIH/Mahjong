using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Net;

public class FrameComponent : CommandReceiver
{
	public FrameComponent(string name)
		:base(name)
	{}
	public virtual void init() { }
	public override void destroy()
	{
		base.destroy();
	}
	public virtual void update(float elapsedTime) { }
	public virtual void fixedUpdate(float elapsedTime) { }
	public virtual void lateUpdate(float elapsedTime) { }
}