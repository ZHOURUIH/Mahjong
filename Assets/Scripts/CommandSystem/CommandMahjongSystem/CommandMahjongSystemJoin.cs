using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CommandMahjongSystemJoin : Command
{
	public Character mCharacter;
	public CommandMahjongSystemJoin(bool showInfo = true, bool delay = false)
		:
		base(showInfo, delay)
	{ }
	public override void execute()
	{
		MahjongSystem mahjongSystem = mReceiver as MahjongSystem;
		GameScene gameScene = mGameSceneManager.getCurScene();
		// 只能在麻将场景才能加入
		if (gameScene.getType() != GAME_SCENE_TYPE.GST_MAHJONG)
		{
			return;
		}
		// 判断当前是否还能加入
		if(!mahjongSystem.canPlayerJoin(mCharacter))
		{
			return;
		}
		// 通知玩家加入本局麻将
		mahjongSystem.notifyPlayerJoin(mCharacter);

		// 通知布局有玩家加入
		ScriptAllCharacterInfo allInfo = mLayoutManager.getScript(LAYOUT_TYPE.LT_ALL_CHARACTER_INFO) as ScriptAllCharacterInfo;
		allInfo.notifyCharacterJoin(mCharacter);
	}
	public override string showDebugInfo()
	{
		CharacterData data = mCharacter.getCharacterData();
		return this.GetType().ToString() + ": guid : " + data.mGUID + ", name : " + data.mName;
	}
}