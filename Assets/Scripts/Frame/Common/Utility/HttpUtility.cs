using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Threading;
using LitJson;

public delegate void OnHttpWebRequestCallback(LitJson.JsonData data, object userData);
public class RequestThreadParam
{
	public HttpWebRequest mRequest;
	public byte[] mByteArray;
	public OnHttpWebRequestCallback mCallback;
	public object mUserData;
	public Thread mThread;
	public string mFullURL;
}

public class HttpUtility : FrameComponent
{
	protected static List<Thread> mHttpThreadList;
	public HttpUtility(string name)
		:base(name)
	{
		mHttpThreadList = new List<Thread>();
	}
	public override void destroy()
	{
		int count = mHttpThreadList.Count;
		for(int i = 0; i < count; ++i)
		{
			mHttpThreadList[i].Abort();
			mHttpThreadList[i] = null;
		}
		mHttpThreadList.Clear();
		base.destroy();
	}
	public static JsonData httpWebRequestPost(string url, string param, OnHttpWebRequestCallback callback = null, object callbakcUserData = null)
	{
		// 转换输入参数的编码类型，获取byte[]数组 
		byte[] byteArray = BinaryUtility.stringToBytes(param, Encoding.UTF8);
		// 初始化新的webRequst
		// 1． 创建httpWebRequest对象
		HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(url));
		// 2． 初始化HttpWebRequest对象
		webRequest.Method = "POST";
		webRequest.ContentType = "application/x-www-form-urlencoded";
		webRequest.ContentLength = byteArray.Length;
		webRequest.Credentials = CredentialCache.DefaultCredentials;
		webRequest.Timeout = 10000;
		// 异步
		if(callback != null)
		{
			RequestThreadParam threadParam = new RequestThreadParam();
			threadParam.mRequest = webRequest;
			threadParam.mByteArray = byteArray;
			threadParam.mCallback = callback;
			threadParam.mUserData = callbakcUserData;
			threadParam.mFullURL = url + param;
			Thread httpThread = new Thread(waitPostHttpWebRequest);
			threadParam.mThread = httpThread;
			httpThread.Start(threadParam);
			httpThread.IsBackground = true;
			mHttpThreadList.Add(httpThread);
			return null;
		}
		// 同步
		else
		{
			try
			{
				// 3． 附加要POST给服务器的数据到HttpWebRequest对象(附加POST数据的过程比较特殊，它并没有提供一个属性给用户存取，需要写入HttpWebRequest对象提供的一个stream里面。)
				Stream newStream = webRequest.GetRequestStream();//创建一个Stream,赋值是写入HttpWebRequest对象提供的一个stream里面
				newStream.Write(byteArray, 0, byteArray.Length);
				newStream.Close();
				// 4． 读取服务器的返回信息
				HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
				StreamReader php = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
				string phpend = php.ReadToEnd();
				php.Close();
                response.Close();
                return JsonMapper.ToObject(phpend);
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
	static public string generateHttpGet(string url, Dictionary<string, string> get)
	{
		string Parameters = "";
		if (get.Count > 0)
		{
			Parameters = "?";
			//从集合中取出所有参数，设置表单参数（AddField()).  
			foreach (KeyValuePair<string, string> post_arg in get)
			{
				Parameters += post_arg.Key + "=" + post_arg.Value + "&";
			}
			StringUtility.removeLast(ref Parameters, '&');
		}
		return url + Parameters;
	}
	static public JsonData httpWebRequestGet(string urlString, OnHttpWebRequestCallback callback = null)
	{
		HttpWebRequest httprequest = (HttpWebRequest)WebRequest.Create(new Uri(urlString));//根据url地址创建HTTpWebRequest对象
		httprequest.Method = "GET";
		httprequest.KeepAlive = false;//持久连接设置为false
		httprequest.ProtocolVersion = HttpVersion.Version11;// 网络协议的版本
		httprequest.ContentType = "application/x-www-form-urlencoded";//http 头
		httprequest.AllowAutoRedirect = true;
		httprequest.MaximumAutomaticRedirections = 2;
		httprequest.Timeout = 10000;//设定超时10秒（毫秒）
		// 异步
		if (callback != null)
		{
			RequestThreadParam threadParam = new RequestThreadParam();
			threadParam.mRequest = httprequest;
			threadParam.mByteArray = null;
			threadParam.mCallback = callback;
			threadParam.mFullURL = urlString;
			Thread httpThread = new Thread(waitGetHttpWebRequest);
			threadParam.mThread = httpThread;
			httpThread.Start(threadParam);
			httpThread.IsBackground = true;
			mHttpThreadList.Add(httpThread);
			return null;
		}
		// 同步
		else
		{
			try
			{
				HttpWebResponse response = (HttpWebResponse)httprequest.GetResponse();
				Stream steam = response.GetResponseStream();
				StreamReader reader = new StreamReader(steam, Encoding.UTF8);
				string pageStr = reader.ReadToEnd();
				reader.Close();
				response.Close();
				httprequest.Abort();
				reader = null;
				response = null;
				httprequest = null;
				return JsonMapper.ToObject(pageStr);
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
	//--------------------------------------------------------------------------------------------------------------------------------------------------------------
	static protected void waitPostHttpWebRequest(object param)
	{
		RequestThreadParam threadParam = param as RequestThreadParam;
		try
		{
			//3． 附加要POST给服务器的数据到HttpWebRequest对象(附加POST数据的过程比较特殊，它并没有提供一个属性给用户存取，需要写入HttpWebRequest对象提供的一个stream里面。)
			Stream newStream = threadParam.mRequest.GetRequestStream();//创建一个Stream,赋值是写入HttpWebRequest对象提供的一个stream里面
			newStream.Write(threadParam.mByteArray, 0, threadParam.mByteArray.Length);
			newStream.Close();
			//4． 读取服务器的返回信息
			HttpWebResponse response = (HttpWebResponse)threadParam.mRequest.GetResponse();
			StreamReader php = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
			string phpend = php.ReadToEnd();
			php.Close();
			response.Close();
			threadParam.mCallback(JsonMapper.ToObject(phpend), threadParam.mUserData);
		}
		catch (Exception e)
		{
			threadParam.mCallback(null, threadParam.mUserData);
			string info = "http post result exception : " + e.Message + ", url : " + threadParam.mFullURL;
			UnityUtility.logInfo(info);
			if (mFrameLogSystem != null)
			{
				mFrameLogSystem.logHttpOverTime(info);
			}
		}
		finally
		{
			mHttpThreadList.Remove(threadParam.mThread);
		}
	}
	static protected void waitGetHttpWebRequest(object param)
	{
		RequestThreadParam threadParam = param as RequestThreadParam;
		try
		{
			HttpWebResponse response = (HttpWebResponse)threadParam.mRequest.GetResponse();
			Stream steam = response.GetResponseStream();
			StreamReader reader = new StreamReader(steam, Encoding.UTF8);
			string pageStr = reader.ReadToEnd();
			reader.Close();
			response.Close();
			reader = null;
			response = null;
			threadParam.mRequest.Abort();
			threadParam.mRequest = null;
			threadParam.mCallback(JsonMapper.ToObject(pageStr), threadParam.mUserData);
		}
		catch (Exception e)
		{
			threadParam.mCallback(null, threadParam.mUserData);
			string info = "http get result exception : " + e.Message + ", url : " + threadParam.mFullURL;
			UnityUtility.logInfo(info);
			if (mFrameLogSystem != null)
			{
				mFrameLogSystem.logHttpOverTime(info);
			}
		}
		finally
		{
			mHttpThreadList.Remove(threadParam.mThread);
		}
	}
	//public static int OpenUSBDevice(ushort VID, ushort PID)
	//{
	//	int HidHandle = -1;
	//	Guid guidHID = new Guid();
	//	HID.HidD_GetHidGuid(ref guidHID);
	//	IntPtr hDevInfo = HID.SetupDiGetClassDevs(ref guidHID, IntPtr.Zero, IntPtr.Zero, DIGCF.DIGCF_PRESENT | DIGCF.DIGCF_DEVICEINTERFACE);
	//	int bufferSize = 0;
	//	//获取设备，true获取到
	//	SP_DEVICE_INTERFACE_DATA DeviceInterfaceData = new SP_DEVICE_INTERFACE_DATA();
	//	DeviceInterfaceData.cbSize = (uint)Marshal.SizeOf(DeviceInterfaceData);
	//	int index = 0;
	//	do
	//	{
	//		int result = HID.SetupDiEnumDeviceInterfaces(hDevInfo, IntPtr.Zero, ref guidHID, (UInt32)index++, ref DeviceInterfaceData);
	//		if (result == 0)
	//		{
	//			int error = Kernel32.GetLastError();
	//			break;
	//		}
	//		//第一次调用出错，但可以返回正确的Size 
	//		SP_DEVINFO_DATA strtInterfaceData = new SP_DEVINFO_DATA();
	//		result = HID.SetupDiGetDeviceInterfaceDetail(hDevInfo, ref DeviceInterfaceData, IntPtr.Zero, 0, ref bufferSize, strtInterfaceData);
	//		//第二次调用传递返回值，调用即可成功
	//		IntPtr detailDataBuffer = Marshal.AllocHGlobal(bufferSize);
	//		SP_DEVICE_INTERFACE_DETAIL_DATA detailData = new SP_DEVICE_INTERFACE_DETAIL_DATA();
	//		detailData.cbSize = Marshal.SizeOf(typeof(SP_DEVICE_INTERFACE_DETAIL_DATA));
	//		Marshal.StructureToPtr(detailData, detailDataBuffer, false);
	//		result = HID.SetupDiGetDeviceInterfaceDetail(hDevInfo, ref DeviceInterfaceData, detailDataBuffer, bufferSize, ref bufferSize, strtInterfaceData);
	//		//获取设备路径访
	//		IntPtr pdevicePathName = (IntPtr)((int)detailDataBuffer + 4);
	//		string devicePathName = Marshal.PtrToStringAuto(pdevicePathName);

	//		//连接设备文件
	//		HidHandle = HID.CreateFile(devicePathName, HID.GENERIC_READ | HID.GENERIC_WRITE, HID.FILE_SHARE_READ | HID.FILE_SHARE_WRITE, 0, HID.OPEN_EXISTING, 0, 0);

	//		HIDD_ATTRIBUTES Attributes = new HIDD_ATTRIBUTES();
	//		Attributes.Size = Marshal.SizeOf(typeof(HIDD_ATTRIBUTES));
	//		result = HID.HidD_GetAttributes(HidHandle, ref Attributes);
	//		if (result != 0 && (Attributes.VendorID == VID) && (Attributes.ProductID == PID))
	//		{
	//			int PreparsedData = 0;
	//			//获取USB设备的预解析数据
	//			int ret = HID.HidD_GetPreparsedData(HidHandle, ref PreparsedData);
	//			if (ret != 0)
	//			{
	//				//printf("无法获取USB设备的预解析数据!");
	//				return -1;
	//			}
	//			HIDP_CAPS caps = new HIDP_CAPS();
	//			int status = HID.HidP_GetCaps(PreparsedData, ref caps);
	//			HID.HidD_FreePreparsedData(PreparsedData);
	//		}
	//	} while (true);
	//	return HidHandle;
	//}
}