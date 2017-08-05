using System;
using System.Collections;
using System.Collections.Generic;

public class SCOtherPlayerGang : SocketPacket
{
	public INT mOtherPlayerGUID = new INT();
	public INT mDroppedPlayerGUID = new INT();
	public BYTE mMahjong = new BYTE();
	public SCOtherPlayerGang(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	protected override void fillParams()
	{
		pushParam(mOtherPlayerGUID);
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
		// 清空提示信息
		ScriptMahjongFrame mahjongFrame = mLayoutManager.getScript(LAYOUT_TYPE.LT_MAHJONG_FRAME) as ScriptMahjongFrame;
		mahjongFrame.notifyInfo("");
		CommandCharacterGang cmdGang = mCommandSystem.newCmd<CommandCharacterGang>();
		cmdGang.mDroppedPlayer = mCharacterManager.getCharacterByGUID(mDroppedPlayerGUID.mValue);
		cmdGang.mMahjong = (MAHJONG)mMahjong.mValue;
		mCommandSystem.pushCommand(cmdGang, mCharacterManager.getCharacterByGUID(mOtherPlayerGUID.mValue));
	}
}