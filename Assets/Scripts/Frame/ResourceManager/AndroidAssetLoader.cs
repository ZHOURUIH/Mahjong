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
	public static byte[] loadAsset(string path)
	{
		byte[] buffer = mCurrentActivity.Call<byte[]>("loadAsset", path);
		return buffer;
	}
	public static string loadTxtAsset(string path)
	{
		string str = mCurrentActivity.Call<string>("loadTxtAsset", path);
		return str;
	}
	public static byte[] loadFile(string path)
	{
		byte[] buffer = mCurrentActivity.Call<byte[]>("loadFile", path);
		return buffer;
	}
	public static string loadTxtFile(string path)
	{
		string str = mCurrentActivity.Call<string>("loadTxtFile", path);
		return str;
	}
}
