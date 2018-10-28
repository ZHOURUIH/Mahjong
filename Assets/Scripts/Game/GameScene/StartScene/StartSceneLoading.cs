using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class LogoSceneLoading : SceneProcedure
{
	public LogoSceneLoading(PROCEDURE_TYPE type, GameScene gameScene)
		:
	base(type, gameScene)
	{
		;
	}
	protected override void onInit(SceneProcedure lastProcedure, string intent)
	{
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 30;
		// 用于显示信息的界面需要预先加载
		LT.LOAD_NGUI_HIDE(LAYOUT_TYPE.LT_MESSAGE_OK, 20);
		LT.LOAD_NGUI_SHOW(LAYOUT_TYPE.LT_LOGIN, 0);
		LT.LOAD_NGUI_HIDE(LAYOUT_TYPE.LT_REGISTER, 0);
		// 由于使用了较多的NGUI控件,所以禁用全局触摸检测,只加载不显示
		LT.LOAD_NGUI_HIDE(LAYOUT_TYPE.LT_GLOBAL_TOUCH, 100);
		// 开始加载关键帧资源,音效资源,布局使用预设资源
		mKeyFrameManager.loadAll(true);
		mAudioManager.loadAll(true);
		mLayoutSubPrefabManager.loadAll(true);
		// 加载结束后进入登录流程
		CommandGameSceneChangeProcedure cmd = newCmd(out cmd, true, true);
		cmd.mProcedure = PROCEDURE_TYPE.PT_START_LOGIN;
		pushDelayCommand(cmd, mGameScene);
	}
	protected override void onUpdate(float elapsedTime)
	{
		;
	}
	protected override void onExit(SceneProcedure nextProcedure)
	{
		;
	}
	protected override void onKeyProcess(float elapsedTime)
	{
		;
	}
}