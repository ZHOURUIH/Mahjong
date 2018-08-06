using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WindowShaderBlurMaskVertical : WindowShader
{
	protected float mSampleInterval = 1.5f;
	public WindowShaderBlurMaskVertical()
	{}
	public void setSampleInterval(float sampleInterval) { mSampleInterval = sampleInterval; }
	public override void applyShader(Material mat)
	{
		base.applyShader(mat);
		if (mat != null && mat.shader != null)
		{
			string shaderName = mat.shader.name;
			if (shaderName == "BlurMaskVertical")
			{
				mat.SetFloat("_SampleInterval", mSampleInterval);
			}
		}
	}
}