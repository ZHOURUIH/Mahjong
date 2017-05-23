using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

public class HttpServerManager
{
	#region
	private string			mWebHomeDir;
	private HttpListener	mListener;
	private Thread			mListenThread;
	HttpPacketFactory		mHttpFactory;

	public string mHost;
	public string mPort;

	public string WebHomeDir
	{
		get { return this.mWebHomeDir; }
		set
		{
			if (!Directory.Exists(value))
				throw new Exception("http服务器设置的根目录不存在!");
			this.mWebHomeDir = value;
		}
	}

	/// 服务器是否在运行    
	public bool IsRunning
	{
		get { return (mListener == null) ? false : mListener.IsListening; }
	}
	#endregion

	#region

	public bool AddPrefixes(string uriPrefix)
	{
		string uri = "http://" + uriPrefix + ":" + mPort + "/";
		if (mListener.Prefixes.Contains(uri)) return false;
		mListener.Prefixes.Add(uri);
		return true;
	}

	/// <summary>    
	/// 启动服务    
	/// </summary>    
	public void init(string host, string port, string webHomeDir)
	{
		mHost = host;
		mPort = port;
		mWebHomeDir = webHomeDir;
		if (mHost == "")
		{
			return;
		}
		mListener = new HttpListener();
		mHttpFactory = new HttpPacketFactory();
		mHttpFactory.init();
		if (mListener.IsListening)
			return;

		mListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;//指定身份验证 Anonymous匿名访问
		if (!string.IsNullOrEmpty(mHost) && mHost.Length > 0)
			mListener.Prefixes.Add("http://" + mHost + ":" + mPort + "/");
		else if (mListener.Prefixes == null || mListener.Prefixes.Count == 0)
			mListener.Prefixes.Add("http://localhost:" + mPort + "/");

		mListener.Start();
		mListenThread = new Thread(AcceptClient);
		mListenThread.Name = "httpserver";
		mListenThread.Start();
	}
	public void destroy() { }
	/// 停止服务      
	public void Stop()
	{
		try
		{
			if (mListener != null)
			{
				mListener.Stop();

			}
		}
		catch (Exception ex)
		{
			UnityUtility.logInfo(typeof(HttpServerManager) + ex.Message);
		}
	}


	/// /接受客户端请求    
	void AcceptClient()
	{
		//int maxThreadNum, portThreadNum;  
		////线程池  
		//int minThreadNum;  
		//ThreadPool.GetMaxThreads(out maxThreadNum, out portThreadNum);  
		//ThreadPool.GetMinThreads(out minThreadNum, out portThreadNum);  
		//Console.WriteLine("最大线程数：{0}", maxThreadNum);  
		//Console.WriteLine("最小空闲线程数：{0}", minThreadNum);  

		while (mListener.IsListening)
		{
			try
			{
				HttpListenerContext context = mListener.GetContext();
				//new Thread(HandleRequest).Start(context);  
				ThreadPool.QueueUserWorkItem(new WaitCallback(HandleRequest), context);
			}
			catch
			{

			}
		}

	}
	#endregion

	#region HandleRequest
	//处理客户端请求    
	private void HandleRequest(object ctx)
	{
		HttpListenerContext context = ctx as HttpListenerContext;
		HttpListenerResponse response = context.Response;
		HttpListenerRequest request = context.Request;
		try
		{
			string rawUrl = request.RawUrl;
			int paramStartIndex = rawUrl.IndexOf('?');
			if (paramStartIndex > 0)
				rawUrl = rawUrl.Substring(0, paramStartIndex);
			else if (paramStartIndex == 0)
				rawUrl = "";


			#region 网页请求
			//string InputStream = "";
			using (var streamReader = new StreamReader(request.InputStream))
			{
				//InputStream = streamReader.ReadToEnd();
			}
			string filePath = "";
			if (string.IsNullOrEmpty(rawUrl) || rawUrl.Length == 0 || rawUrl == "/")
				filePath = WebHomeDir + "/index.html";// + directorySeparatorChar + "Index.html";
			else
				filePath = WebHomeDir + rawUrl;//.Replace("/", directorySeparatorChar);
			if (!File.Exists(filePath))
			{
				HttpPacket packet = HttpPacketFactory.createPacket(rawUrl);
				if (null != packet)
				{
					packet.handlePacket(ref response, ref request);
				}
				else
				{
					response.ContentLength64 = 0;
					response.StatusCode = 404;
					response.Abort();
				}
			}
			else
			{
				response.StatusCode = 200;
				string exeName = Path.GetExtension(filePath);
				response.ContentType = GetContentType(exeName);
				FileStream fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read, FileShare.ReadWrite);
				int byteLength = (int)fileStream.Length;
				byte[] fileBytes = new byte[byteLength];
				fileStream.Read(fileBytes, 0, byteLength);
				fileStream.Flush();
				fileStream.Dispose();
				response.ContentLength64 = byteLength;
				response.OutputStream.Write(fileBytes, 0, byteLength);
				response.OutputStream.Flush();
			}
			#endregion

		}
		catch (Exception ex)
		{
			UnityUtility.logInfo(typeof(HttpServerManager) + ex.Message);
			response.StatusCode = 200;
			response.ContentType = "text/plain";
			using (StreamWriter writer = new StreamWriter(response.OutputStream, Encoding.UTF8))
			{
				writer.WriteLine("接收完成！");
			}
		}
		try
		{
			response.Close();
		}
		catch (Exception ex)
		{
			UnityUtility.logInfo(typeof(HttpServerManager) + ex.Message);
		}
	}
	#endregion

	#region GetContentType
	/// <summary>    
	/// 获取文件对应MIME类型    
	/// </summary>    
	/// <param name="fileExtention">文件扩展名,如.jpg</param>    
	/// <returns></returns>    
	protected string GetContentType(string fileExtention)
	{
		if (string.Compare(fileExtention, ".html", true) == 0
					|| string.Compare(fileExtention, ".htm", true) == 0)
			return "text/html;charset=utf-8";
		else if (string.Compare(fileExtention, ".js", true) == 0)
			return "application/javascript";
		else if (string.Compare(fileExtention, ".css", true) == 0)
			return "text/css";
		else if (string.Compare(fileExtention, ".png", true) == 0)
			return "image/png";
		else if (string.Compare(fileExtention, ".jpg", true) == 0 || string.Compare(fileExtention, ".jpeg", true) == 0)
			return "image/jpeg";
		else if (string.Compare(fileExtention, ".gif", true) == 0)
			return "image/gif";
		else if (string.Compare(fileExtention, ".swf", true) == 0)
			return "application/x-shockwave-flash";
		else
			return "";//application/octet-stream  
	}
	#endregion

	#region WriteStreamToFile
	//const int ChunkSize = 1024 * 1024;  
	private void WriteStreamToFile(BinaryReader br, string fileName, long length)
	{
		byte[] fileContents = new byte[] { };
		var bytes = new byte[length];
		int i = 0;
		while ((i = br.Read(bytes, 0, (int)length)) != 0)
		{
			byte[] arr = new byte[fileContents.LongLength + i];
			fileContents.CopyTo(arr, 0);
			Array.Copy(bytes, 0, arr, fileContents.Length, i);
			fileContents = arr;
		}

		using (var fs = new FileStream(fileName, FileMode.Create))
		{
			using (var bw = new BinaryWriter(fs))
			{
				bw.Write(fileContents);
			}
		}
	}
	#endregion
}