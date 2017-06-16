using System;
using System.Collections;
using System.Collections.Generic;

public class SCStartGame : SocketPacket
{
	protected BYTES mDice = new BYTES(2);
	public SCStartGame(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	protected override void fillParams()
	{
		pushParam(mDice);
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
		cmdDice.mDice = mDice.mValue;
		mCommandSystem.pushCommand(cmdDice, gameScene);
	}
}