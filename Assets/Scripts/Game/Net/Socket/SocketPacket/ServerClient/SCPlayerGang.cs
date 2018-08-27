using System;
using System.Collections;
using System.Collections.Generic;

public class SCPlayerGang : SocketPacket
{
	public INT mDroppedPlayerGUID = new INT();
	public BYTE mMahjong = new BYTE();
	public SCPlayerGang(PACKET_TYPE type)
		: base(type) { }
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
		CommandCharacterGang cmdGang = newCmd(out cmdGang);
		cmdGang.mDroppedPlayer = mCharacterManager.getCharacter(mDroppedPlayerGUID.mValue);
		cmdGang.mMahjong = (MAHJONG)mMahjong.mValue;
		pushCommand(cmdGang, mCharacterManager.getMyself());
	}
}