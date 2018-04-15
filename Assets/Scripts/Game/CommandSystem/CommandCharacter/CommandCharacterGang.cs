using UnityEngine;
using System.Collections;

public class CommandCharacterGang : Command
{
	public Character mDroppedPlayer;
	public MAHJONG mMahjong;
	public override void init()
	{
		base.init();
		mDroppedPlayer = null;
		mMahjong = MAHJONG.M_MAX;
	}
	public override void execute()
	{
		Character character = (mReceiver) as Character;
		character.gangMahjong(mMahjong, mDroppedPlayer);

		if (character != mDroppedPlayer)
		{
			pushCommand<CommandCharacterTakeDrop>(mDroppedPlayer);
		}

		// 通知布局
		CharacterData data = character.getCharacterData();
		mScriptMahjongHandIn.notifyPengOrGang(data.mPosition, data.mPengGangList);

		// 然后重新排列玩家手里的牌
		pushCommand<CommandCharacterReorderMahjong>(character);
	}
	public override string showDebugInfo()
	{
		return base.showDebugInfo() + " : mahjong : " + mMahjong + ", dropped player : " + mDroppedPlayer.getName();
	}
}