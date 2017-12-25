using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

abstract public class GameScene : ComponentOwner
{
	protected Dictionary<PROCEDURE_TYPE, SceneProcedure>	mSceneProcedureList;
	protected GAME_SCENE_TYPE								mSceneType;
	protected PROCEDURE_TYPE								mStartProcedure;
	protected PROCEDURE_TYPE								mExitProcedure;
	protected PROCEDURE_TYPE								mLastProcedureType;
	protected SceneProcedure								mCurProcedure;
	protected bool											mDestroyEngineScene;
	protected List<SceneProcedure>							mLastProcedureList;	// 所进入过的所有流程
	protected GameObject									mSceneObject;
	protected AudioSource									mAudioSource;
	protected int											mMaxLastProcedureCount = 8;
    public GameScene(GAME_SCENE_TYPE type, string name) 
        :
        base(name)
    {
		mSceneType = type;
        mCurProcedure = null;
        mDestroyEngineScene = true;
		mLastProcedureType = PROCEDURE_TYPE.PT_NONE;
		mDestroyEngineScene = false;
		mSceneProcedureList = new Dictionary<PROCEDURE_TYPE, SceneProcedure>();
		mLastProcedureList = new List<SceneProcedure>();
		// 创建场景对应的物体,并挂接到场景管理器下
		mSceneObject = new GameObject(name);
		mSceneObject.transform.parent = mGameSceneManager.getManagerObject().transform;
		mAudioSource = mSceneObject.GetComponent<AudioSource>();
	}
	// 进入场景时初始化
    public virtual void init()
    {
        initComponents();
		// 创建出所有的场景流程
		createSceneProcedure();
		// 设置起始流程名
		assignStartExitProcedure();
		// 开始执行起始流程
		CommandGameSceneChangeProcedure cmd = mCommandSystem.newCmd<CommandGameSceneChangeProcedure>(true, false);
        cmd.mProcedure = mStartProcedure;
        mCommandSystem.pushCommand(cmd, this);
    }
	public override void initComponents()
	{
		// 添加音效组件
		addComponent<GameSceneComponentAudio>("audio");
	}
	public override void destroy()
	{
		base.destroy();
		base.destroyAllComponents();
		UnityUtility.destroyGameObject(mSceneObject);
		mSceneObject = null;
	}
	public virtual void update(float elapsedTime)
	{
		// 更新组件
		base.updateComponents(elapsedTime);

		// 更新当前流程
		keyProcess(elapsedTime);
		if (mCurProcedure != null)
		{
			mCurProcedure.update(elapsedTime);
		}
	}
	public virtual void keyProcess(float elapsedTime)
	{
		if (mCurProcedure != null)
		{
			mCurProcedure.keyProcess(elapsedTime);
		}
	}
	// 退出场景
	public virtual void exit()
	{
		// 首先进入退出流程,然后再退出最后的流程
		CommandGameSceneChangeProcedure cmd = mCommandSystem.newCmd<CommandGameSceneChangeProcedure>();
		cmd.mProcedure = mExitProcedure;
		mCommandSystem.pushCommand(cmd, this);
		if (mCurProcedure != null)
		{
			mCurProcedure.exit(null, null);
			mCurProcedure = null;
		}
	}
	public AudioSource getAudioSource() { return mAudioSource; }
	public abstract void assignStartExitProcedure();
	public AudioSource createAudioSource()
	{
		mAudioSource = mSceneObject.AddComponent<AudioSource>();
		return mAudioSource;
	}
    public virtual void createSceneProcedure() { }
	public bool atProcedure(PROCEDURE_TYPE type)
	{
		if (mCurProcedure == null)
		{
			return false;
		}
		return mCurProcedure.isThisOrParent(type);
	}
	// 是否在指定的流程,不考虑子流程
	public bool atSelfProcedure(PROCEDURE_TYPE type)
	{
		if(mCurProcedure == null)
		{
			return false;
		}
		return mCurProcedure.getProcedureType() == type;
	}
	public void backToLastProcedure(string intend)
	{
		if (mLastProcedureList.Count == 0)
		{
			return;
		}
		// 获得上一次进入的流程
		PROCEDURE_TYPE lastType = mLastProcedureList[mLastProcedureList.Count - 1].getProcedureType();
		changeProcedure(lastType, intend, false);
		mLastProcedureList.RemoveAt(mLastProcedureList.Count - 1);
	}
	public bool changeProcedure(PROCEDURE_TYPE procedure, string intent, bool addToLastList = true)
    {
        if (mSceneProcedureList.ContainsKey(procedure))
        {
			// 将上一个流程记录到返回列表中
			if (mCurProcedure != null && addToLastList)
			{
				mLastProcedureList.Add(mCurProcedure);
				if(mLastProcedureList.Count > mMaxLastProcedureCount)
				{
					mLastProcedureList.RemoveAt(0);
				}
			}
			if (mCurProcedure == null || mCurProcedure.getProcedureType() != procedure)
			{
				SceneProcedure targetProcedure = mSceneProcedureList[procedure];
				// 如果当前已经在一个流程中了,则要先退出当前流程,但是不要销毁流程
				if (mCurProcedure != null)
				{
					// 需要找到共同的父节点,退到该父节点时则不再退出
					SceneProcedure exitTo = mCurProcedure.getSameParent(targetProcedure);
					SceneProcedure nextPro = targetProcedure;
					mCurProcedure.exit(exitTo, nextPro);
				}
				SceneProcedure lastProcedure = mCurProcedure;
				mCurProcedure = targetProcedure;
				mCurProcedure.init(lastProcedure, intent);
			}
            return true;
        }
        else
        {
			UnityUtility.logError("can not find scene procedure : " + procedure);
        }
        return false;
    }
	// 流程调用,通知场景当前流程已经准备完毕
	public void notifyProcedurePrepared()
	{
		if(mLastProcedureList.Count > 0)
		{
			mLastProcedureList[mLastProcedureList.Count - 1].onNextProcedurePrepared(mCurProcedure);
		}
	}
    public virtual void notifySceneObjectDestroy(string objectName) { } // 通知场景一个场景物体被销毁了
    public bool getDestroyEngineScene() { return mDestroyEngineScene; }
    public void setDestroyEngineScene(bool value) { mDestroyEngineScene = value; }
    public SceneProcedure getSceneProcedure(PROCEDURE_TYPE type)
    {
        if (mSceneProcedureList.ContainsKey(type))
        {
            return mSceneProcedureList[type];
        }
        return null;
    }
    public SceneProcedure getCurSceneProcedure() { return mCurProcedure; }
	public T getCurOrParentProcedure<T>(PROCEDURE_TYPE type) where T : SceneProcedure
	{
		return mCurProcedure.getThisOrParent<T>(type);
	}
	public GAME_SCENE_TYPE getSceneType() { return mSceneType; }
	public static SceneProcedure createProcedure<T>(GameScene gameScene, PROCEDURE_TYPE type) where T : SceneProcedure, new()
	{
		object[] procedureParams = new object[] { type, gameScene };
		T procedure = UnityUtility.createInstance<T>(typeof(T), procedureParams);
		return procedure;
	}
	public T addProcedure<T>(PROCEDURE_TYPE type, SceneProcedure parent = null) where T : SceneProcedure, new()
    {
		SceneProcedure procedure = createProcedure<T>(this, type);
		if (parent != null)
		{
			parent.addChildProcedure(procedure);
		}
		mSceneProcedureList.Add(procedure.getProcedureType(), procedure);
		return procedure as T;
    }
	public virtual void notifyScreenActived() { }
}