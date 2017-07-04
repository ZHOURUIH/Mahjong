using UnityEngine;
using System.Collections;

public class CommandCharacterNotifyBanker : Command
{
	public bool mBanker;
	public override void init()
	{
		base.init();
		mBanker = false;
	}
	public override void execute()
	{
		Character character = mReceiver as Character;
		CharacterData data = character.getCharacterData();
		data.mBanker = mBanker;
		ScriptAllCharacterInfo allInfo = mLayoutManager.getScript(LAYOUT_TYPE.LT_ALL_CHARACTER_INFO) as ScriptAllCharacterInfo;
		allInfo.notifyCharacterBanker(character, mBanker);
	}
}