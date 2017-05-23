using UnityEngine;
using System.Collections;
using System.Net;


public class HttpPacket : GameBase
{
	protected HTTP_PACKET	mPacketType;
	protected string		mAddress;
	public HttpPacket(HTTP_PACKET type, string address)
	{
		mPacketType = type;
		mAddress = address;
	}
	public HTTP_PACKET getPacketType() { return mPacketType; }
	public string getAddress() { return mAddress; }
	public virtual void handlePacket(ref HttpListenerResponse response, ref HttpListenerRequest request) { }
	public static void sendReply(ref HttpListenerResponse response, string resultString)
	{
		response.StatusCode = 200;
		byte[] buffer = System.Text.Encoding.UTF8.GetBytes(resultString);
		response.ContentLength64 = buffer.Length;
		System.IO.Stream output = response.OutputStream;
		output.Write(buffer, 0, buffer.Length);
		output.Close();
	}

	IEnumerator SendPost(string url, WWWForm wForm)
	{
		WWW postData = new WWW(url, wForm);
		yield return postData;
		if (postData.error != null)
		{
			UnityUtility.logInfo(postData.error);
		}
		else
		{
			UnityUtility.logInfo(postData.text);
		}
	}
}