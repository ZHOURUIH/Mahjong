using System;
using System.Collections;
using System.Collections.Generic;

public class SCMahjongPoolSize : SocketPacket
{
	public BYTE mCount = new BYTE();
	public SCMahjongPoolSize(PACKET_TYPE type)
		: base(type) { }
	protected override void fillParams()
	{
		pushParam(mCount);
	}
	public override void execute()
	{
		GameScene gameScene = mGameSceneManager.getCurScene();
		if (gameScene.getSceneType() != GAME_SCENE_TYPE.GST_MAHJONG)
		{
			return;
		}
		MahjongScene mahjongScene = gameScene as MahjongScene;
		Room room = mahjongScene.getRoom();
		CommandRoomMahjongPoolSize cmd = newCmd(out cmd);
		cmd.mCount = mCount.mValue;
		pushCommand(cmd, room);
	}
}