using System;
using System.Collections;
using System.Collections.Generic;

public class SCStartGame : SocketPacket
{
	public byte[] mDice;
	public SCStartGame(PACKET_TYPE type)
		:
		base(type)
	{
		mDice = new byte[2];
	}
	public override void read(byte[] data)
	{
		int index = 0;
		BinaryUtility.readBytes(data, ref index, -1, mDice, -1, -1);
	}
	public override void write(byte[] data)
	{
		int index = 0;
		BinaryUtility.writeBytes(data, ref index, -1, mDice, -1, -1);
	}
	public override int getSize()
	{
		return sizeof(byte) * mDice.Length;
	}
	public override void execute()
	{
		GameScene gameScene = mGameSceneManager.getCurScene();
		if(gameScene.getType() != GAME_SCENE_TYPE.GST_MAHJONG)
		{
			return;
		}
		CommandGameSceneChangeProcedure cmd = new CommandGameSceneChangeProcedure();
		cmd.mProcedure = PROCEDURE_TYPE.PT_MAHJONG_RUNNING_DICE;
		mCommandSystem.pushCommand(cmd, mGameSceneManager.getCurScene());

		CommandMahjongSceneNotifyDice cmdDice = new CommandMahjongSceneNotifyDice();
		cmdDice.mDice = mDice;
		mCommandSystem.pushCommand(cmdDice, gameScene);
	}
}