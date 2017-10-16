using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Threading;
using LitJson;
using ZXing;
using UnityEngine;
using ZXing.QrCode;

public delegate void OnHttpWebRequestCallback(LitJson.JsonData data, object userData);
public class RequestThreadParam
{
	public HttpWebRequest mRequest;
	public byte[] mByteArray;
	public OnHttpWebRequestCallback mCallback;
	public object mUserData;
	public Thread mThread;
}

public class PluginUtility : GameBase
{
	static List<Thread> mHttpThreadList;
	public void init() 
	{
		if (mHttpThreadList == null)
		{
			mHttpThreadList = new List<Thread>();
		}
	}
	public void destroy()
	{
		int count = mHttpThreadList.Count;
		for(int i = 0; i < count; ++i)
		{
			mHttpThreadList[i].Abort();
			mHttpThreadList[i] = null;
		}
		mHttpThreadList.Clear();
	}
	public static Color32[] stringToBinaryCodeColour(string textForEncoding, int width, int height)
	{
		var writer = new BarcodeWriter
		{
			Format = BarcodeFormat.QR_CODE,
			Options = new QrCodeEncodingOptions
			{
				// 设置二维码的显示宽度 编码方式 和 边缘宽度
				Height = height,
				Width = width,
				CharacterSet = "UTF-8",
				Margin = 1,
			}
		};
		return writer.Write(textForEncoding);
	}

	// 一个生成二维码的方法 需要一个字符串
	public static Texture2D stringToBinaryCode(string md5Str)
	{
		Texture2D encoded = new Texture2D(256, 256);
		var textForEncoding = md5Str;
		if (textForEncoding != null)
		{
			var color32 = stringToBinaryCodeColour(textForEncoding, encoded.width, encoded.height);
			encoded.SetPixels32(color32);
			encoded.Apply();
		}
		return encoded;
	}
	public static LitJson.JsonData httpWebRequestPost(string url, string param, OnHttpWebRequestCallback callback, object callbakcUserData = null)
	{
		// 转换输入参数的编码类型，获取byte[]数组 
		byte[] byteArray = BinaryUtility.stringToBytes("gamedata=" + param, Encoding.UTF8);
		// 初始化新的webRequst
		// 1． 创建httpWebRequest对象
		HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(url));
		// 2． 初始化HttpWebRequest对象
		webRequest.Method = "POST";
		webRequest.ContentType = "application/x-www-form-urlencoded";
		webRequest.ContentLength = byteArray.Length;
		webRequest.Credentials = CredentialCache.DefaultCredentials;
		webRequest.Timeout = 5000;
		// 异步
		if(callback != null)
		{
			RequestThreadParam threadParam = new RequestThreadParam();
			threadParam.mRequest = webRequest;
			threadParam.mByteArray = byteArray;
			threadParam.mCallback = callback;
			threadParam.mUserData = callbakcUserData;
			Thread httpThread = new Thread(waitPostHttpWebRequest);
			threadParam.mThread = httpThread;
			httpThread.Start(threadParam);
			mHttpThreadList.Add(httpThread);
			return null;
		}
		// 同步
		else
		{
			try
			{
				//3． 附加要POST给服务器的数据到HttpWebRequest对象(附加POST数据的过程比较特殊，它并没有提供一个属性给用户存取，需要写入HttpWebRequest对象提供的一个stream里面。)
				Stream newStream = webRequest.GetRequestStream();//创建一个Stream,赋值是写入HttpWebRequest对象提供的一个stream里面
				newStream.Write(byteArray, 0, byteArray.Length);
				newStream.Close();
				//4． 读取服务器的返回信息
				HttpWebResponse response;
				response = (HttpWebResponse)webRequest.GetResponse();
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
			HttpWebResponse response;
			response = (HttpWebResponse)threadParam.mRequest.GetResponse();
			StreamReader php = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
			string phpend = php.ReadToEnd();
			php.Close();
            response.Close();
            threadParam.mCallback(JsonMapper.ToObject(phpend), threadParam.mUserData);
		}
		catch (Exception)
		{
			threadParam.mCallback(null, threadParam.mUserData);
		}
		finally
		{
			mHttpThreadList.Remove(threadParam.mThread);
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
	static public LitJson.JsonData httpWebRequestGet(string urlString, OnHttpWebRequestCallback callback)
	{
		System.GC.Collect();
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
			Thread httpThread = new Thread(waitGetHttpWebRequest);
			threadParam.mThread = httpThread;
			httpThread.Start(threadParam);
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
	static protected void waitGetHttpWebRequest(object param)
	{
		RequestThreadParam threadParam = param as RequestThreadParam;
		try
		{
			HttpWebResponse response = (HttpWebResponse)threadParam.mRequest.GetResponse();
			if (response.StatusCode != HttpStatusCode.OK)
			{
				UnityUtility.logInfo("接受超时");
			}
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
		catch (Exception)
		{
			threadParam.mCallback(null, threadParam.mUserData);
		}
		finally
		{
			mHttpThreadList.Remove(threadParam.mThread);
		}
	}
}