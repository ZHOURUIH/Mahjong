using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Net;

public class Game : GameFramework
{
	//-------------------------------------------------------------------------------------------------------------
	protected override void initComponent()
	{
		base.initComponent();
		registeComponent<GameConfig>();
		registeComponent<HeadTextureManager>();
		registeComponent<MahjongSystem>();
		registeComponent<SocketManager>();
		registeComponent<LogSystem>();
		registeComponent<RoomSystem>();
	}
	protected override void init()
	{
		base.init();
		// 当前项目默认禁止使用NGUIButton的颜色自动变化
		txNGUIButton.mFadeColor = false;
	}
	protected override void notifyBase()
	{
		base.notifyBase();
		// 所有类都构造完成后通知GameBase
		GameBase frameBase = new GameBase();
		frameBase.notifyConstructDone();
	}
	protected override void registe()
	{
		LayoutRegister.registeAllLayout();
		GameSceneRegister.registerAllGameScene();
		SQLiteRegister.registeAllTable();
		DataRegister.registeAllData();
		CharacterRegister.registeAllCharacter();
	}
	protected override void launch()
	{
		base.launch();
		CommandGameSceneManagerEnter cmd = GameBase.newCmd(out cmd, false);
		cmd.mSceneType = GAME_SCENE_TYPE.GST_START;
		GameBase.pushCommand(cmd, GameBase.mGameSceneManager);
	}
}