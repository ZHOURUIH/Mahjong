using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameSceneManager : FrameComponent
{
	public GameScene			mCurScene;
	public List<GameScene>		mLastSceneList;
	public GameObject			mManagerObject;
	protected SceneFactoryManager	mSceneFactoryManager;
	public GameSceneManager(string name)
	:base(name)
	{
		mCurScene = null;
		mLastSceneList = new List<GameScene>();
		mSceneFactoryManager = new SceneFactoryManager();
	}
	public override void init() 
	{
		mManagerObject = UnityUtility.getGameObject(mGameFramework.getGameFrameObject(), "GameSceneManager");
		if(mManagerObject == null)
		{
			UnityUtility.logError("can not find GameSceneManager under GameFramework!");
		}
	}
	public GameScene getCurScene(){ return mCurScene; }
	public GameObject getManagerObject(){return mManagerObject;}
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
    public override void update(float elapsedTime)
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
		mManagerObject = null;
		base.destroy();
	}
	public void registeGameScene(Type classType, GAME_SCENE_TYPE type)
	{
		mSceneFactoryManager.addFactory(classType, type);
	}
	public int getSceneCount()
	{
		return mSceneFactoryManager.getFactoryCount();
	}
	//----------------------------------------------------------------------------------------------
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