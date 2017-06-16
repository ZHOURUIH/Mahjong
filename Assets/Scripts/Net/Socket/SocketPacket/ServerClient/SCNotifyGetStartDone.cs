using System;
using System.Collections;
using System.Collections.Generic;

public class SCNotifyGetStartDone : SocketPacket
{
	public SCNotifyGetStartDone(PACKET_TYPE type)
		:
		base(type)
	{
		;
	}
	public override void read(byte[] data)
	{}
	public override void write(byte[] data)
	{}
	public override int getSize()
	{
		return 0;
	}
	public override void execute()
	{
		GameScene gameScene = mGameSceneManager.getCurScene();
		if (gameScene.getType() != GAME_SCENE_TYPE.GST_MAHJONG)
		{
			return;
		}
		CommandMahjongSceneNotifyStartDone cmd = new CommandMahjongSceneNotifyStartDone();
		mCommandSystem.pushCommand(cmd, gameScene);
	}
}