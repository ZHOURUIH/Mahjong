using System;
using System.Collections;
using System.Collections.Generic;

public class SCContinueGameRet : SocketPacket
{
	public BOOL mBanker = new BOOL();
	public SCContinueGameRet(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	protected override void fillParams()
	{
		pushParam(mBanker);
	}
	public override void execute()
	{
		GameScene gameScene = mGameSceneManager.getCurScene();
		if(gameScene.getSceneType() != GAME_SCENE_TYPE.GST_MAHJONG)
		{
			return;
		}
		MahjongScene mahjongScene = gameScene as MahjongScene;

		// 跳转到等待流程
		CommandGameSceneChangeProcedure cmdProcedure = newCmd(out cmdProcedure);
		cmdProcedure.mProcedure = PROCEDURE_TYPE.PT_MAHJONG_WAITING;
		pushCommand(cmdProcedure, mahjongScene);

		// 将自己加入房间
		CharacterMyself myself = mCharacterManager.getMyself();
		CommandRoomJoin cmdJoin = newCmd(out cmdJoin);
		cmdJoin.mCharacter = myself;
		pushCommand(cmdJoin, mahjongScene.getRoom());

		CommandCharacterNotifyBanker cmdBanker = newCmd(out cmdBanker);
		cmdBanker.mBanker = mBanker.mValue;
		pushCommand(cmdBanker, myself);
	}
}