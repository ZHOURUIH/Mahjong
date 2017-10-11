using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct MEMORY_STATUS_EX
{
	public uint dwLength;
	public uint dwMemoryLoad;
	public ulong dwTotalPhys;
	public ulong dwAvailPhys;
	public ulong dwTotalPageFile;
	public ulong dwAvailPageFile;
	public ulong dwTotalVirtual;
	public ulong dwAvailVirtual;
	public ulong dwAvailExtendedVirtual;
}

public class Kernel32
{
	public const string KERNEL32_DLL = "kernel32.dll";
	[DllImport(KERNEL32_DLL)]
	public extern static IntPtr LoadLibrary(string path);
	[DllImport(KERNEL32_DLL)]
	public extern static IntPtr GetProcAddress(IntPtr lib, String funcName);
	[DllImport(KERNEL32_DLL)]
	public extern static bool FreeLibrary(IntPtr lib);
	[DllImport(KERNEL32_DLL)]
	public static extern void GlobalMemoryStatusEx(ref MEMORY_STATUS_EX meminfo);
}