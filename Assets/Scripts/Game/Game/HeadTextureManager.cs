using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeadTextureManager : FrameComponent
{
	protected Dictionary<string, Texture> mHeadTextureList;
	public HeadTextureManager(string name)
		:base(name)
	{
		mHeadTextureList = new Dictionary<string, Texture>();
	}
	public override void init()
	{
		base.init();
	}
	public override void destroy()
	{
		foreach (var item in mHeadTextureList)
		{
			GameObject.Destroy(item.Value);
		}
		mHeadTextureList.Clear();
		base.destroy();
	}
	public void requestLoadTexture(string url, string openID)
	{
		if (mHeadTextureList.ContainsKey(openID))
		{
			if (mHeadTextureList[openID] != null)
			{
				notifyTextureLoaded(mHeadTextureList[openID], openID);
			}
		}
		else
		{
			mHeadTextureList.Add(openID, null);
			mResourceManager.loadAssetsFromUrl<Texture>(url, onLoadWeChatHead, openID);
		}
	}
	//--------------------------------------------------------------------------------------------------------------------------------------------------
	protected void onLoadWeChatHead(UnityEngine.Object tex, byte[] bytes, object userData)
	{
		string openID = userData as string;
		mHeadTextureList[openID] = tex as Texture;
		notifyTextureLoaded(tex as Texture, openID);
	}
	protected void notifyTextureLoaded(Texture tex, string openID)
	{
		//CommandMatchSystemNotifyWeChatHeadLoaded cmd = newCmd(out cmd);
		//cmd.mHead = tex as Texture;
		//cmd.mID = openID;
		//pushCommand(cmd, mCurMatchSystem);
	}
}
