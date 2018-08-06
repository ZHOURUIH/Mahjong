using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class Dll : GameBase
{
	protected Dictionary<string, Delegate> mFunctionList;
	protected IntPtr mHandle;
	protected string mLibraryName;
	public void init(string name)
	{
		mLibraryName = name;
		mFunctionList = new Dictionary<string, Delegate>();
		mHandle = Kernel32.LoadLibrary(mLibraryName);
	}
	public void destroy()
	{
		Kernel32.FreeLibrary(mHandle);
		mFunctionList = null;
	}
	public string getName()
	{
		return mLibraryName;
	}
	public Delegate getFunction(string funcName, Type t)
	{
		if (!mFunctionList.ContainsKey(funcName))
		{
			IntPtr api = Kernel32.GetProcAddress(mHandle, funcName);
			if(api == IntPtr.Zero)
			{
				logError("can not find function, name : " + funcName);
				return null;
			}
			Delegate dele = Marshal.GetDelegateForFunctionPointer(api, t);
			mFunctionList.Add(funcName, dele);
		}
		return mFunctionList[funcName];
	}
}