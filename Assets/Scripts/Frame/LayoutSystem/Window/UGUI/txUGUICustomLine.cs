using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class txUGUICustomLine : txUIObject
{
	public CustomLine mCustomLine;
	public WindowShaderCurve mCurveShader;
	public txUGUICustomLine()
	{
		mCustomLine = new CustomLine();
	}
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
		mCustomLine.init(mObject);
		mCurveShader = new WindowShaderCurve();
	}
	public override void destroy()
	{
		mCustomLine.destroy();
		base.destroy();
	}
	public override void update(float elapsedTime)
	{
		base.update(elapsedTime);
		if(mCurveShader != null)
		{
			mCurveShader.applyShader(mCustomLine.getMaterial());
		}
	}
	public WindowShaderCurve getWindowShader() { return mCurveShader; }
	public void setPointList(List<Vector3> pointList)
	{
		mCustomLine.setPointList(pointList);
	}
	public void setPointList(Vector3[] pointList)
	{
		mCustomLine.setPointList(pointList);
	}
	public void setPointListBezier(List<Vector3> pointList, int bezierDetail = 10)
	{
		setPointList(getBezierPoints(pointList, false, bezierDetail));
	}
	public void setPointListBezier(Vector3[] pointList, int bezierDetail = 10)
	{
		setPointList(getBezierPoints(pointList, false, bezierDetail));
	}
	public void setPointListSmooth(List<Vector3> pointList, int bezierDetail = 10)
	{
		setPointList(getCurvePoints(pointList, false, bezierDetail));
	}
	public void setPointListSmooth(Vector3[] pointList, int bezierDetail = 10)
	{
		setPointList(getCurvePoints(pointList, false, bezierDetail));
	}
	public override void setAlpha(float alpha)
	{
		mCurveShader.mAlpha = alpha;
	}
	public override float getAlpha()
	{
		return mCurveShader.mAlpha;
	}
}

