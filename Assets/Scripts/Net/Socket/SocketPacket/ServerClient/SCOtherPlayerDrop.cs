using System;
using System.Collections;
using System.Collections.Generic;

public class SCOtherPlayerDrop : SocketPacket
{
	public INT mPlayerGUID = new INT();
	public INT mIndex = new INT();
	public BYTE mMahjong = new BYTE();
	public SCOtherPlayerDrop(PACKET_TYPE type)
		:
		base(type)
	{
		fillParams();
		zeroParams();
	}
	protected override void fillParams()
	{
		pushParam(mPlayerGUID);
		pushParam(mIndex);
		pushParam(mMahjong);
	}
	public override void execute()
	{
		// 清空提示信息
		ScriptMahjongFrame mahjongFrame = mLayoutManager.getScript(LAYOUT_TYPE.LT_MAHJONG_FRAME) as ScriptMahjongFrame;
		mahjongFrame.notifyInfo("");
		CommandCharacterDrop cmd = mCommandSystem.newCmd<CommandCharacterDrop>();
		cmd.mMah = (MAHJONG)mMahjong.mValue;
		cmd.mIndex = mIndex.mValue;
		mCommandSystem.pushCommand(cmd, mCharacterManager.getCharacterByGUID(mPlayerGUID.mValue));
	}
}