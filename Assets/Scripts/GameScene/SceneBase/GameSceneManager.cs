using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameSceneManager : CommandReceiver
{
	public GameScene			mCurScene;
	public List<GameScene>		mLastSceneList;
	public GameObject			mManagerObject;
	protected SceneFactoryManager	mSceneFactoryManager;
	public GameSceneManager()
	:
	base(typeof(GameSceneManager).ToString())
	{
		mCurScene = null;
		mLastSceneList = new List<GameScene>();
		mSceneFactoryManager = new SceneFactoryManager();
	}
	public void init() 
	{
		registerAllGameScene();
		mManagerObject = UnityUtility.getGameObject(mGameFramework.getGameFrameObject(), "GameSceneManager");
		if(mManagerObject == null)
		{
			UnityUtility.logError("can not find GameSceneManager under GameFramework!");
		}
	}
	public GameScene getCurScene()
	{ 
		return mCurScene; 
	}
	public GameObject getManagerObject()
	{
		return mManagerObject;
	}
	public bool enterScene(GAME_SCENE_TYPE type)
	{
		GameScene pScene = createScene(type);
		// 如果有上一个场景,则先销毁上一个场景,只是暂时保存下上个场景的指针,然后在更新中将场景销毁
		if (mCurScene != null)
		{
			mLastSceneList.Add(mCurScene);
			mCurScene.exit();
			mCurScene = null;
		}
		mCurScene = pScene;
		mCurScene.init();
		return true;
	}
    public void update(float elapsedTime)
	{
		// 如果上一个场景不为空,则将上一个场景销毁
		foreach (var scene in mLastSceneList)
		{
			scene.destroy();
		}
		mLastSceneList.Clear();
        if (mCurScene != null)
        {
            mCurScene.update(elapsedTime);
        }
	}

    public override void destroy()
	{
		foreach (var scene in mLastSceneList)
		{
			scene.destroy();
		}
		mLastSceneList.Clear();
		if (mCurScene != null)
		{
			mCurScene.destroy();
			mCurScene = null;
		}
	}
	//----------------------------------------------------------------------------------------------
	protected void registerAllGameScene()
	{
		registerLayout(typeof(StartScene), GAME_SCENE_TYPE.GST_START);
		registerLayout(typeof(MainScene), GAME_SCENE_TYPE.GST_MAIN);
		registerLayout(typeof(MahjongScene), GAME_SCENE_TYPE.GST_MAHJONG);
	}
	protected void registerLayout(Type classType, GAME_SCENE_TYPE type)
	{
		mSceneFactoryManager.addFactory(classType, type);
	}
	protected GameScene createScene(GAME_SCENE_TYPE type)
	{
		SceneFactory factory = mSceneFactoryManager.getFactory(type);
		if (factory != null)
		{
			string name = factory.getClassType().ToString();
			return factory.createScene(name);
		}
		return null;
	}
}