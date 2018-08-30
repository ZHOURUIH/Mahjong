using UnityEngine;
using System.Collections;

// 向服务器请求一页房间列表
public class CommandMainSceneAutoRequestTimeChanged : Command
{
	public int mTime;
	public override void init()
	{
		base.init();
		mTime = 0;
	}
	public override void execute()
	{
		GameScene gameScene = mReceiver as GameScene;
		mScriptRoomList.setRemainRequestTime(mTime);
	}
	public override string showDebugInfo()
	{
		return base.showDebugInfo() + " : mTime : " + mTime;
	}
}