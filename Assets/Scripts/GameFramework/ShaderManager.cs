using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ShaderManager
{
	protected Dictionary<string, Shader> mShaderList = new Dictionary<string, Shader>();
	public ShaderManager()
	{
		;
	}
	public void init()
	{
		;
	}
	public void destroy()
	{
		;
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