using System;
using System.Collections;
using System.Collections.Generic;

public class SCNotifyGetStartDone : SocketPacket
{
	public SCNotifyGetStartDone(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	protected override void fillParams(){ }
	public override void execute()
	{
		GameScene gameScene = mGameSceneManager.getCurScene();
		if (gameScene.getSceneType() != GAME_SCENE_TYPE.GST_MAHJONG)
		{
			return;
		}
		pushCommand<CommandMahjongSceneNotifyStartDone>(gameScene);
	}
}