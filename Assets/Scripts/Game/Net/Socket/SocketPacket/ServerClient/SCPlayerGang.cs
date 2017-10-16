using System;
using System.Collections;
using System.Collections.Generic;

public class SCPlayerGang : SocketPacket
{
	public INT mDroppedPlayerGUID = new INT();
	public BYTE mMahjong = new BYTE();
	public SCPlayerGang(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	protected override void fillParams()
	{
		pushParam(mDroppedPlayerGUID);
		pushParam(mMahjong);
	}
	public override void execute()
	{
		GameScene gameScene = mGameSceneManager.getCurScene();
		if(gameScene.getSceneType() != GAME_SCENE_TYPE.GST_MAHJONG)
		{
			return;
		}
		CommandCharacterGang cmdGang = mCommandSystem.newCmd<CommandCharacterGang>();
		cmdGang.mDroppedPlayer = mCharacterManager.getCharacterByGUID(mDroppedPlayerGUID.mValue);
		cmdGang.mMahjong = (MAHJONG)mMahjong.mValue;
		mCommandSystem.pushCommand(cmdGang, mCharacterManager.getMyself());
	}
}