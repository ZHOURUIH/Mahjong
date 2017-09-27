using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class Dll
{
	protected Dictionary<string, Delegate> mFunctionList;
	protected IntPtr mHandle;
	protected string mLibraryName;
	public void init(string name)
	{
		mLibraryName = name;
		mFunctionList = new Dictionary<string, Delegate>();
		mHandle = DllImportExtern.LoadLibrary(mLibraryName);
	}
	public void destroy()
	{
		DllImportExtern.FreeLibrary(mHandle);
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
			IntPtr api = DllImportExtern.GetProcAddress(mHandle, funcName);
			if(api == IntPtr.Zero)
			{
				UnityUtility.logError("can not find function, name : " + funcName);
				return null;
			}
			Delegate dele = Marshal.GetDelegateForFunctionPointer(api, t);
			mFunctionList.Add(funcName, dele);
		}
		return mFunctionList[funcName];
	}
}

public class DllImportExtern
{
	public const string KERNEL32_DLL = "kernel32.dll";
	public const string WINMM_DLL = "winmm.dll";
	public const string USER32_DLL = "user32.dll";
	protected static Dictionary<string, Dll> mDllLibraryList = new Dictionary<string,Dll>();

	[DllImport(KERNEL32_DLL)]
	public extern static IntPtr LoadLibrary(string path);
	[DllImport(KERNEL32_DLL)]
	public extern static IntPtr GetProcAddress(IntPtr lib, String funcName);
	[DllImport(KERNEL32_DLL)]
	public extern static bool FreeLibrary(IntPtr lib);

	[DllImport(USER32_DLL)]
	public extern static IntPtr SetWindowLong(IntPtr hwnd, int _nIndex, uint dwNewLong);
	[DllImport(USER32_DLL)]
	public extern static bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
	[DllImport(USER32_DLL)]
	public extern static IntPtr GetForegroundWindow();
	//将要执行的函数转换为委托
	public static Delegate Invoke(string library, string funcName, Type t)
	{
		if (mDllLibraryList.ContainsKey(library))
		{
			return mDllLibraryList[library].getFunction(funcName, t);
		}
		return null;
	}
	public void init()
	{
		registerDLL(WINMM_DLL);
	}
	public void destroy()
	{
		foreach (var library in mDllLibraryList)
		{
			library.Value.destroy();
		}
		mDllLibraryList.Clear();
	}
	protected void registerDLL(string name)
	{
		Dll dll = new Dll();
		dll.init(name);
		mDllLibraryList.Add(dll.getName(), dll);
	}
}