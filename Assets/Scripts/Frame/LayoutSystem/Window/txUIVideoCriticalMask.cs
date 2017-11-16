using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using RenderHeads.Media.AVProVideo;

public class txUIVideoCriticalMask : txUIVideo
{
	protected float mCriticalValue = 1.0f;
	protected bool mInverseVertical = false;
	public txUIVideoCriticalMask()
	{
		;
	}
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
	}
	public override void update(float elapsedTime)
	{
		base.update(elapsedTime);
	}
	public void setCriticalValue(float critical) { mCriticalValue = critical; }
	public void setInverseVertical(bool inverse) { mInverseVertical = inverse; }
	//---------------------------------------------------------------------------------------------------------------------------------------
	protected override void applyShader(Material mat)
	{
		base.applyShader(mat);
		if (mat != null && mat.shader != null)
		{
			string shaderName = mat.shader.name;
			if (shaderName == "CriticalMask")
			{
				mat.SetFloat("_CriticalValue", mCriticalValue);
				mat.SetInt("_InverseVertical", mInverseVertical ? 1 : 0);
			}
		}
	}
}