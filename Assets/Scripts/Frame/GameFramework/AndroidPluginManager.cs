using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AndroidPluginManager : FrameComponent
{
	protected static AndroidJavaClass mUnityPlayer;
	protected static AndroidJavaObject mMainActivity;
	public AndroidPluginManager(string name)
		:base(name)
	{
#if !UNITY_EDITOR && UNITY_ANDROID
		mUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		mMainActivity = mUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
#endif
	}
	public override void init()
	{
		;
	}
	public override void destroy()
	{
#if !UNITY_EDITOR && UNITY_ANDROID
		mUnityPlayer.Dispose();
		mMainActivity.Dispose();
#endif
		base.destroy();
	}
	public static AndroidJavaObject getMainActivity() { return mMainActivity; }
}