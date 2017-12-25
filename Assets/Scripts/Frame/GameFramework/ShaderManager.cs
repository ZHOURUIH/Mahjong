using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ShaderManager : FrameComponent
{
	protected Dictionary<string, Shader> mShaderList;
	public ShaderManager(string name)
		:base(name)
	{
		mShaderList = new Dictionary<string, Shader>();
	}
	public override void init()
	{
		;
	}
	public override void destroy()
	{
		// shader不主动卸载
		mShaderList.Clear();
		base.destroy();
	}
	public Shader getShader(string name)
	{
		if(mShaderList.ContainsKey(name))
		{
			return mShaderList[name];
		}
		Shader shader = Shader.Find(name);
		if(shader != null)
		{
			mShaderList.Add(name, shader);
		}
		else
		{
			UnityUtility.logError("can not find shader : " + name);
		}
		return shader;
	}
}