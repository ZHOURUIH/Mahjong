using UnityEngine;
using System.Collections;

public class CommandSocketConnectNetState : Command
{
	public NET_STATE mNetState;
	public override void init()
	{
		base.init();
		mNetState = NET_STATE.NS_SERVER_CLOSE;
	}
	public override void execute()
	{
		if(mNetState == NET_STATE.NS_NET_CLOSE)
		{
			UnityUtility.logInfo("网络已断开");
		}
		else if(mNetState == NET_STATE.NS_SERVER_CLOSE)
		{
			UnityUtility.logInfo("服务器关闭");
		}
	}
	public override string showDebugInfo()
	{
		return base.showDebugInfo() + " : state : " + mNetState;
	}
}