using System;
using System.Collections;
using System.Collections.Generic;

public class SCNotifyMahjongEnd : SocketPacket
{
	public SCNotifyMahjongEnd(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	protected override void fillParams(){}
	public override void execute()
	{
		GameScene gameScene = mGameSceneManager.getCurScene();
		if (gameScene.getType() != GAME_SCENE_TYPE.GST_MAHJONG)
		{
			return;
		}
		MahjongScene mahjongScene = gameScene as MahjongScene;
		Room room = mahjongScene.getRoom();
		CommandRoomEnd cmdEnd = new CommandRoomEnd();
		mCommandSystem.pushCommand(cmdEnd, room);
	}
}