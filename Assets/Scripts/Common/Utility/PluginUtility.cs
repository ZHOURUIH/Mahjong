using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using LitJson;
using ZXing;
using UnityEngine;
using ZXing.QrCode;

public class PluginUtility : GameBase
{
	public void init() { }
	public void destroy() { }
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
	public static LitJson.JsonData postHttpWebRequest(string url, string method, string param)
	{
		// 转换输入参数的编码类型，获取byte[]数组 
		byte[] byteArray = BinaryUtility.stringToBytes("gamedata=" + param, Encoding.UTF8);
		// 初始化新的webRequst
		// 1． 创建httpWebRequest对象
		HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(url + "/" + method));
		// 2． 初始化HttpWebRequest对象
		webRequest.Method = "POST";
		webRequest.ContentType = "application/x-www-form-urlencoded";
		webRequest.ContentLength = byteArray.Length;
		webRequest.Credentials = CredentialCache.DefaultCredentials;
		webRequest.Timeout = 5000;
		Stream newStream;
		try
		{
			//3． 附加要POST给服务器的数据到HttpWebRequest对象(附加POST数据的过程比较特殊，它并没有提供一个属性给用户存取，需要写入HttpWebRequest对象提供的一个stream里面。)
			newStream = webRequest.GetRequestStream();//创建一个Stream,赋值是写入HttpWebRequest对象提供的一个stream里面
			newStream.Write(byteArray, 0, byteArray.Length);
			newStream.Close();
			//4． 读取服务器的返回信息
			HttpWebResponse response;
			response = (HttpWebResponse)webRequest.GetResponse();
			StreamReader php = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
			string phpend = php.ReadToEnd();
			return JsonMapper.ToObject(phpend);
		}
		catch (Exception)	// 如果上面 连接的时候 有错误 那么直接 跳到二维码界面 返回 连接超时的字符串
		{
			return null;
		}
	}
	static public string generateHttpGET(string url, Dictionary<string, string> get)
	{
		string Parameters = "";
		if (get.Count > 0)
		{
			Parameters = "?";
			//从集合中取出所有参数，设置表单参数（AddField()).  
			foreach (KeyValuePair<string, string> post_arg in get)
			{
				Parameters += post_arg.Key + "=" + post_arg.Value + "&";
				StringUtility.removeLast(ref Parameters, '&');
			}
		}
		return url + Parameters;
	} 
}