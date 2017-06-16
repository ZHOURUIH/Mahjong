using UnityEngine;
using System.Collections;

public class CommandMahjongSceneNotifyDiceDone : Command
{
	public CommandMahjongSceneNotifyDiceDone(bool showInfo = true, bool delay = false)
		:
		base(showInfo, delay)
	{ }
	public override void execute()
	{
		GameScene gameScene = (mReceiver) as GameScene;
		if(!gameScene.atProcedure(PROCEDURE_TYPE.PT_MAHJONG_RUNNING_DICE))
		{
			return;
		}
		// 发消息通知服务器掷骰子完毕
		CSDiceDone diceDone = mSocketNetManager.createPacket(PACKET_TYPE.PT_CS_DICE_DONE) as CSDiceDone;
		mSocketNetManager.sendMessage(diceDone);
	}
}