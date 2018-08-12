using UnityEngine;
using System.Collections;

public class AndroidAssetLoader : FrameComponent
{
	protected static AndroidJavaClass mUnityPlayer;
	protected static AndroidJavaObject mCurrentActivity;
	public AndroidAssetLoader(string name)
		:base(name)
	{
		mUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		mCurrentActivity = mUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
	}
	public override void destroy()
	{
		mUnityPlayer.Dispose();
		mCurrentActivity.Dispose();
		base.destroy();
	}
	// 相对于StreamingAssets的路径
	public static byte[] loadFile(string path)
	{
		byte[] buffer = mCurrentActivity.Call<byte[]>("loadAB", path);
		return buffer;
	}
	public static string loadTextFile(string path)
	{
		byte[] buffer = mCurrentActivity.Call<byte[]>("loadAB", path);
		string str = BinaryUtility.bytesToString(buffer);
		return str;
	}
}
