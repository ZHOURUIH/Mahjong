using UnityEngine;
using System.Collections;

public class CommandMahjongSceneNotifyDiceDone : Command
{
	public override void init()
	{
		base.init();
	}
	public override void execute()
	{
		GameScene gameScene = (mReceiver) as GameScene;
		if(!gameScene.atProcedure(PROCEDURE_TYPE.PT_MAHJONG_RUNNING_DICE))
		{
			return;
		}
		// 发消息通知服务器掷骰子完毕
		mSocketNetManager.sendMessage<CSDiceDone>();
	}
}