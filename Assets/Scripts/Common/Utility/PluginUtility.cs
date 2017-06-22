using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ZXing;
using UnityEngine;
using ZXing.QrCode;

public class PluginUtility : GameBase
{
	public void init() { }
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
}