using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CommandMahjongSystemReady : Command
{
	public Character mCharacter;
	public CommandMahjongSystemReady(bool showInfo = true, bool delay = false)
		:
		base(showInfo, delay)
	{
	}
	public override void execute()
	{
		MahjongSystem mahjongSystem = mReceiver as MahjongSystem;
		GameScene gameScene = mGameSceneManager.getCurScene();
		// 只能在麻将场景的等待流程才能准备
		if (gameScene.getType() != GAME_SCENE_TYPE.GST_MAHJONG)
		{
			return;
		}
		if(!gameScene.atProcedure(PROCEDURE_TYPE.PT_MAHJONG_WAITING))
		{
			return;
		}
		// 通知麻将系统
		bool allReady = mahjongSystem.notifyPlayerReady(mCharacter);
		// 通知布局,并且只有在没有全部都准备时才通知,因为全部准备后会隐藏准备图片
		if(!allReady)
		{
			ScriptAllCharacterInfo allCharacterInfo = mLayoutManager.getScript(LAYOUT_TYPE.LT_ALL_CHARACTER_INFO) as ScriptAllCharacterInfo;
			allCharacterInfo.notifyCharacterReady(mCharacter);
		}
	}
	public override string showDebugInfo()
	{
		CharacterData data = mCharacter.getCharacterData();
		return this.GetType().ToString() + ": guid : " + data.mGUID + ", name : " + data.mName;
	}
}