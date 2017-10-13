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
	protected MaterialManager mMaterialManager;
	protected HeadTextureManager mHeadTextureManager;
	protected MahjongSystem mMahjongSystem;
	public override void start()
	{
		base.start();
		mGameConfig = new GameConfig();
		mGameUtility = new GameUtility();
		mMaterialManager = new MaterialManager();
		mHeadTextureManager = new HeadTextureManager();
		mMahjongSystem = new MahjongSystem();
	}
	public override void notifyBase()
	{
		base.notifyBase();
		// 所有类都构造完成后通知GameBase
		GameBase frameBase = new GameBase();
		frameBase.notifyConstructDone();
	}
	public override void init()
	{
		base.init();
		LayoutRegister layoutRegister = new LayoutRegister();
		layoutRegister.registeAllLayout();
		GameSceneRegister sceneRegister = new GameSceneRegister();
		sceneRegister.registerAllGameScene();
		DataRegister dataRegister = new DataRegister();
		dataRegister.registeAllData();
		mGameConfig.init();
		mGameUtility.init();
		mMaterialManager.init();
		mHeadTextureManager.init();
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
		mHeadTextureManager.destroy();
		mMahjongSystem.destroy();
		mGameConfig = null;
		mGameUtility = null;
		mMaterialManager = null;
		mHeadTextureManager = null;
		mMahjongSystem = null;
		// 最后调用基类的destroy,确保资源被释放完毕
		base.destroy();
	}
	public MaterialManager getMaterialManager() { return mMaterialManager; }
	public HeadTextureManager getHeadTextureManager() { return mHeadTextureManager; }
	public MahjongSystem getMahjongSystem() { return mMahjongSystem; }
	public GameConfig getGameConfig() { return mGameConfig; }
}