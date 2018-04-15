using UnityEngine;
using System.Collections;

public class CommandCharacterPeng : Command
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
		character.pengMahjong(mMahjong);
		
		// 从已经打出的牌中拿走已经碰的那张牌
		pushCommand<CommandCharacterTakeDrop>(mDroppedPlayer);

		CharacterData data = character.getCharacterData();
		// 通知布局
		mScriptMahjongHandIn.notifyPengOrGang(data.mPosition, data.mPengGangList);

		// 然后重新排列玩家手里的牌
		pushCommand<CommandCharacterReorderMahjong>(character);
	}
	public override string showDebugInfo()
	{
		return base.showDebugInfo() + " : mahjong : " + mMahjong + ", dropped player : " + mDroppedPlayer.getName();
	}
}