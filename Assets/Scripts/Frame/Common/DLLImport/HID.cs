//using UnityEngine;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Runtime.InteropServices;
//public struct SP_DEVICE_INTERFACE_DATA
//{
//	public uint cbSize;
//	public Guid interfaceClassGuid;
//	public uint flags;
//	public IntPtr reserved;
//}
//[StructLayout(LayoutKind.Sequential)]
//public class SP_DEVINFO_DATA
//{
//	public uint cbSize;
//	public Guid classGuid;
//	public uint devInst = 0;
//	public IntPtr reserved;
//}
//[StructLayout(LayoutKind.Sequential)]
//public struct HIDD_ATTRIBUTES
//{
//	public int Size;
//	public ushort VendorID;
//	public ushort ProductID;
//	public ushort VersionNumber;
//}
//public enum DIGCF
//{
//	DIGCF_DEFAULT = 0x1,
//	DIGCF_PRESENT = 0x2,
//	DIGCF_ALLCLASSES = 0x4,
//	DIGCF_PROFILE = 0x8,
//	DIGCF_DEVICEINTERFACE = 0x10
//}
//public struct HIDP_CAPS
//{
//	public UInt16 Usage;                    // USHORT   
//	public UInt16 UsagePage;                // USHORT   
//	public UInt16 InputReportByteLength;
//	public UInt16 OutputReportByteLength;
//	public UInt16 FeatureReportByteLength;
//	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
//	public UInt16[] Reserved;                // USHORT  Reserved[17];               
//	public UInt16 NumberLinkCollectionNodes;
//	public UInt16 NumberInputButtonCaps;
//	public UInt16 NumberInputValueCaps;
//	public UInt16 NumberInputDataIndices;
//	public UInt16 NumberOutputButtonCaps;
//	public UInt16 NumberOutputValueCaps;
//	public UInt16 NumberOutputDataIndices;
//	public UInt16 NumberFeatureButtonCaps;
//	public UInt16 NumberFeatureValueCaps;
//	public UInt16 NumberFeatureDataIndices;
//}
//[StructLayout(LayoutKind.Sequential, Pack = 2)]
//internal struct SP_DEVICE_INTERFACE_DETAIL_DATA
//{
//	internal int cbSize;
//	internal short devicePath;
//}
//public class HID
//{
//	public const uint GENERIC_READ = 0x80000000;
//	public const uint GENERIC_WRITE = 0x40000000;
//	public const uint FILE_SHARE_READ = 0x00000001;
//	public const uint FILE_SHARE_WRITE = 0x00000002;
//	public const int OPEN_EXISTING = 3;
//	//获得GUID
//	[DllImport("hid.dll")]
//	public static extern void HidD_GetHidGuid(ref Guid HidGuid);
//	//过滤设备，获取需要的设备
//	[DllImport("setupapi.dll", SetLastError = true)]
//	public static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, IntPtr Enumerator, IntPtr HwndParent, DIGCF Flags);
//	//获取设备，true获取到
//	[DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
//	public static extern int SetupDiEnumDeviceInterfaces(IntPtr hDevInfo, IntPtr devInfo, ref Guid interfaceClassGuid, UInt32 memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);
//	// 获取接口的详细信息 必须调用两次 第1次返回长度 第2次获取数据 
//	[DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
//	public static extern int SetupDiGetDeviceInterfaceDetail(IntPtr deviceInfoSet, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, IntPtr deviceInterfaceDetailData,
//		int deviceInterfaceDetailDataSize, ref int requiredSize, SP_DEVINFO_DATA deviceInfoData);
//	//获取设备文件
//	[DllImport("kernel32.dll", SetLastError = true)]
//	public static extern int CreateFile(
//		string lpFileName,                            // file name
//		uint dwDesiredAccess,                        // access mode
//		uint dwShareMode,                            // share mode
//		uint lpSecurityAttributes,                    // SD
//		uint dwCreationDisposition,                    // how to create
//		uint dwFlagsAndAttributes,                    // file attributes
//		uint hTemplateFile                            // handle to template file
//		);
//	//读取设备文件
//	[DllImport("Kernel32.dll", SetLastError = true)]
//	public static extern bool ReadFile(IntPtr hFile,byte[] lpBuffer,uint nNumberOfBytesToRead,ref uint lpNumberOfBytesRead,IntPtr lpOverlapped);
//	//释放设备
//	[DllImport("hid.dll")]
//	public static extern bool HidD_FreePreparsedData(int PreparsedData);
//	[DllImport("hid.dll")]
//	public static extern byte HidD_GetAttributes(int handle, ref HIDD_ATTRIBUTES attribute);
//	//关闭访问设备句柄，结束进程的时候把这个加上保险点
//	[DllImport("kernel32.dll")]
//	public static extern int CloseHandle(int hObject);
//	[DllImport("hid.dll", SetLastError = true)]   
//    public static extern int HidP_GetCaps(int pPHIDP_PREPARSED_DATA, ref HIDP_CAPS myPHIDP_CAPS);
//	[DllImport("hid.dll", SetLastError = true)]
//	public static extern int HidD_GetPreparsedData(int hObject, ref int pPHIDP_PREPARSED_DATA);
//}

using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

[StructLayout(LayoutKind.Sequential)]
public struct HIDD_ATTRIBUTES
{
	public Int32 Size;
	public Int16 VendorID;
	public Int16 ProductID;
	public Int16 VersionNumber;
}
[StructLayout(LayoutKind.Sequential)]
public struct SP_DEVINFO_DATA
{
	public uint cbSize;
	public Guid ClassGuid;
	public uint DevInst;
	public IntPtr Reserved;
}
[StructLayout(LayoutKind.Sequential)]
public struct HIDP_CAPS
{
	public System.UInt16 Usage;                 // USHORT
	public System.UInt16 UsagePage;             // USHORT
	public System.UInt16 InputReportByteLength;
	public System.UInt16 OutputReportByteLength;
	public System.UInt16 FeatureReportByteLength;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
	public System.UInt16[] Reserved;                // USHORT  Reserved[17];			
	public System.UInt16 NumberLinkCollectionNodes;
	public System.UInt16 NumberInputButtonCaps;
	public System.UInt16 NumberInputValueCaps;
	public System.UInt16 NumberInputDataIndices;
	public System.UInt16 NumberOutputButtonCaps;
	public System.UInt16 NumberOutputValueCaps;
	public System.UInt16 NumberOutputDataIndices;
	public System.UInt16 NumberFeatureButtonCaps;
	public System.UInt16 NumberFeatureValueCaps;
	public System.UInt16 NumberFeatureDataIndices;
}

public class HID
{
	public const string HID_DLL = "hid.dll";
	[DllImport(HID_DLL, SetLastError = true)]
	public static extern void HidD_GetHidGuid(ref Guid hidGuid);
	[DllImport(HID_DLL, SetLastError = true)]
	public static extern bool HidD_GetPreparsedData(
	SafeFileHandle hObject,
	ref IntPtr PreparsedData);

	[DllImport(HID_DLL, SetLastError = true)]
	public static extern Boolean HidD_FreePreparsedData(ref IntPtr PreparsedData);

	[DllImport(HID_DLL, SetLastError = true)]
	public static extern int HidP_GetCaps(
		IntPtr pPHIDP_PREPARSED_DATA,                   // IN PHIDP_PREPARSED_DATA  PreparsedData,
		ref HIDP_CAPS myPHIDP_CAPS);                // OUT PHIDP_CAPS  Capabilities

	[DllImport(HID_DLL, SetLastError = true)]
	public static extern Boolean HidD_GetAttributes(SafeFileHandle hObject, ref HIDD_ATTRIBUTES Attributes);

	[DllImport(HID_DLL, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
	public static extern bool HidD_GetFeature(
		IntPtr hDevice,
		IntPtr hReportBuffer,
		uint ReportBufferLength);

	[DllImport(HID_DLL, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
	public static extern bool HidD_SetFeature(
		IntPtr hDevice,
		IntPtr ReportBuffer,
		uint ReportBufferLength);

	[DllImport(HID_DLL, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
	public static extern bool HidD_GetProductString(
		SafeFileHandle hDevice,
		IntPtr Buffer,
		uint BufferLength);

	[DllImport(HID_DLL, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
	public static extern bool HidD_GetSerialNumberString(
		SafeFileHandle hDevice,
		IntPtr Buffer,
		uint BufferLength);

	[DllImport(HID_DLL, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
	public static extern Boolean HidD_GetManufacturerString(
		SafeFileHandle hDevice,
		IntPtr Buffer,
		uint BufferLength);
}