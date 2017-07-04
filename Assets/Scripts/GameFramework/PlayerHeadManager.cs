using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;

public class PlayerHeadManager : GameBase
{
	protected Dictionary<Character, Texture> mPlayerHeadList;
	public PlayerHeadManager()
	{
		mPlayerHeadList = new Dictionary<Character, Texture>();
	}
	public Texture getHead(Character player)
	{
		if (mPlayerHeadList.ContainsKey(player))
		{
			return mPlayerHeadList[player];
		}
		return null;
	}
	public void init()
	{
		;
	}
	// 加载所有KeyFrame下的关键帧
	public void loadHead(Character player)
	{
		// 通过微信接口下载微信头像
		mResourceManager.loadTextureFromUrl("", onHeadLoaded, player);
	}
	public void destroy()
	{
		mPlayerHeadList.Clear();
	}
	//------------------------------------------------------------------------------------------------------------------
	protected void onHeadLoaded(Texture tex, object userData)
	{
		mPlayerHeadList.Add(userData as Character, tex);
	}
}
