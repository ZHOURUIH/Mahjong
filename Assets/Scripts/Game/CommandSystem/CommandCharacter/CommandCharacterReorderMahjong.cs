using UnityEngine;
using System.Collections;

public class CommandCharacterReorderMahjong : Command
{
	public override void init()
	{
		base.init();
	}
	public override void execute()
	{
		Character character = (mReceiver) as Character;
		CharacterNPC npc = character as CharacterNPC;
		npc.reorderMahjong();
		// 通知布局麻将被重新排列
		CharacterData data = npc.getCharacterData();
		ScriptMahjongHandIn handIn = mLayoutManager.getScript(LAYOUT_TYPE.LT_MAHJONG_HAND_IN) as ScriptMahjongHandIn;
		handIn.notifyReorder(data.mPosition, data.mHandIn);
	}
}