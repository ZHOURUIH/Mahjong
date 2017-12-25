using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using RenderHeads.Media.AVProVideo;

public class txUIVideoMotionBlurCriticalMask : txUIVideoCriticalMask
{
	protected float mMinRange = 300.0f;
	protected int mMaxSample = 30;
	protected float mIncreaseSample = 0.2f;
	protected int mSampleInterval = 3;
	protected Vector2 mSampleCenter;
	public txUIVideoMotionBlurCriticalMask()
	{
		mSampleCenter = new Vector2(0.5f, 0.5f);
	}
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
	}
	public void setMinRange(float minRange) { mMinRange = minRange; }
	public void setMaxSample(int maxSample) { mMaxSample = maxSample; }
	public void setIncreaseSample(float increaseValue) { mIncreaseSample = increaseValue; }
	public void setSampleInterval(int interval) { mSampleInterval = interval; }
	public void setSampleCenter(Vector2 center) { mSampleCenter = center; }
	public void setMotionBlur(bool motionBlur)
	{
		if(motionBlur)
		{
			mMaxSample = 30;
			mSampleInterval = 3;
		}
		else
		{
			mMaxSample = 1;
			mSampleInterval = 1;
		}
	}
	//---------------------------------------------------------------------------------------------------
	protected override void applyShader(Material mat)
	{
		base.applyShader(mat);
		if (mat != null && mat.shader != null)
		{
			string shaderName = mat.shader.name;
			if (shaderName == "MotionBlurCriticalMask")
			{
				mat.SetFloat("_MinRange", mMinRange);
				mat.SetInt("_MaxSample", mMaxSample);
				mat.SetFloat("_IncreaseSample", mIncreaseSample);
				mat.SetInt("_SampleInterval", mSampleInterval);
				mat.SetFloat("_CenterX", mSampleCenter.x);
				mat.SetFloat("_CenterY", mSampleCenter.y);
			}
		}
	}
}