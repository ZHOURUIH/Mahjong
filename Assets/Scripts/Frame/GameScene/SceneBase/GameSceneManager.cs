using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameSceneManager : FrameComponent
{
	protected Dictionary<GAME_SCENE_TYPE, Type> mGameSceneRegisteList;
	public GameScene			mCurScene;
	public List<GameScene>		mLastSceneList;
	public GameObject			mManagerObject;
	public GameSceneManager(string name)
		:base(name)
	{
		mCurScene = null;
		mLastSceneList = new List<GameScene>();
		mGameSceneRegisteList = new Dictionary<GAME_SCENE_TYPE, Type>();
	}
	public override void init() 
	{
		mManagerObject = getGameObject(mGameFramework.getGameFrameObject(), "GameSceneManager", true);
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
		mGameSceneRegisteList.Add(type, classType);
	}
	public int getSceneCount()
	{
		return mGameSceneRegisteList.Count;
	}
	//----------------------------------------------------------------------------------------------
	protected GameScene createScene(GAME_SCENE_TYPE type)
	{
		Type classType = mGameSceneRegisteList[type];
		return UnityUtility.createInstance<GameScene>(classType, type, classType.ToString());
	}
}