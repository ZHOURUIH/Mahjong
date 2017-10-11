using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Net;

public class Game : GameFramework
{
	protected GameUtility mGameUtility;
	protected GameConfig mGameConfig;
	protected MaterialManager mMaterialManager = null;
	protected PlayerHeadManager mPlayerHeadManager = null;
	protected MahjongSystem mMahjongSystem = null;
	public override void start()
	{
		base.start();
		mGameConfig = new GameConfig();
		mGameUtility = new GameUtility();
		mMaterialManager = new MaterialManager();
		mPlayerHeadManager = new PlayerHeadManager();
		mMahjongSystem = new MahjongSystem();
	}
	public override void init()
	{
		base.init();
		mGameConfig.init();
		mGameUtility.init();
		mMaterialManager.init();
		mPlayerHeadManager.init();
		mMahjongSystem.init();
	}
	public override void launch()
	{
		base.launch();
		CommandGameSceneManagerEnter cmd = mCommandSystem.newCmd<CommandGameSceneManagerEnter>(false, false);
		cmd.mSceneType = GAME_SCENE_TYPE.GST_START;
		mCommandSystem.pushCommand(cmd, getGameSceneManager());
	}
	public override void update(float elapsedTime)
	{
		base.update(elapsedTime);
	}
	public override void destroy()
	{
		mGameConfig.destory();
		mMaterialManager.destroy();
		mPlayerHeadManager.destroy();
		mMahjongSystem.destroy();
		mGameConfig = null;
		mGameUtility = null;
		mMaterialManager = null;
		mPlayerHeadManager = null;
		mMahjongSystem = null;
		// 最后调用基类的destroy,确保资源被释放完毕
		base.destroy();
	}
	public MaterialManager getMaterialManager() { return mMaterialManager; }
	public PlayerHeadManager getPlayerHeadManager() { return mPlayerHeadManager; }
	public MahjongSystem getMahjongSystem() { return mMahjongSystem; }
	public GameConfig getGameConfig() { return mGameConfig; }
}