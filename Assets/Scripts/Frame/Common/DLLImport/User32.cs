using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class User32
{
	public const string USER32_DLL = "user32.dll";
	[DllImport(USER32_DLL)]
	public extern static IntPtr SetWindowLong(IntPtr hwnd, int _nIndex, uint dwNewLong);
	[DllImport(USER32_DLL)]
	public extern static bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
	[DllImport(USER32_DLL)]
	public extern static IntPtr GetForegroundWindow();
}