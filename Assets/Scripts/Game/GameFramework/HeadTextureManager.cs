using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeadTextureManager : GameBase
{
	protected Dictionary<string, Texture> mHeadTextureList;
	public HeadTextureManager()
	{
		mHeadTextureList = new Dictionary<string, Texture>();
	}
	public virtual void init()
	{
		;
	}
	public void destroy()
	{
		foreach (var item in mHeadTextureList)
		{
			GameObject.Destroy(item.Value);
		}
		mHeadTextureList.Clear();
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
	protected void onLoadWeChatHead(UnityEngine.Object tex, object userData)
	{
		string openID = userData as string;
		mHeadTextureList[openID] = tex as Texture;
		notifyTextureLoaded(tex as Texture, openID);
	}
	protected void notifyTextureLoaded(Texture tex, string openID)
	{
		//CommandMatchSystemNotifyWeChatHeadLoaded cmd = mCommandSystem.newCmd<CommandMatchSystemNotifyWeChatHeadLoaded>();
		//cmd.mHead = tex as Texture;
		//cmd.mID = openID;
		//mCommandSystem.pushCommand(cmd, mCurMatchSystem);
	}
}
