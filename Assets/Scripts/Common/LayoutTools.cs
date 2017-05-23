using UnityEngine;
using System.Collections;
using System;

public class LayoutTools : GameBase
{
	// 布局
	//------------------------------------------------------------------------------------------------------------------------------------
	public static void LOAD_LAYOUT(LAYOUT_TYPE type, int renderOrder, bool visible)
	{
		CommandLayoutManagerLoadLayout cmd = new CommandLayoutManagerLoadLayout(true, false);
		cmd.mType = type;
		cmd.mVisible = visible;
		cmd.mRenderOrder = renderOrder;
		cmd.mAsync = false;
		mCommandSystem.pushCommand(cmd, mLayoutManager);
	}
	public static void LOAD_LAYOUT_ASYNC(LAYOUT_TYPE type, int renderOrder, LayoutAsyncDone callback)
	{
		CommandLayoutManagerLoadLayout cmd = new CommandLayoutManagerLoadLayout(true, false);
		cmd.mType = type;
		cmd.mRenderOrder = renderOrder;
		cmd.mAsync = true;
		cmd.mCallback = callback;
		mCommandSystem.pushCommand(cmd, mLayoutManager);
	}
	public static void LOAD_LAYOUT_HIDE(LAYOUT_TYPE type, int renderOrder)
	{
		LOAD_LAYOUT(type, renderOrder, false);
	}
	public static void LOAD_LAYOUT_SHOW(LAYOUT_TYPE type, int renderOrder)
	{
		LOAD_LAYOUT(type, renderOrder, true);
	}
	public static void HIDE_LAYOUT(LAYOUT_TYPE type, bool immediately = false, string param = "")
	{
		VISIBLE_LAYOUT(type, false, immediately, param);
	}
	public static CommandLayoutManagerLayoutVisible HIDE_LAYOUT_DELAY(SceneProcedure procedure, float delayTime, LAYOUT_TYPE type, bool immediately = false, string param = "")
	{
		return VISIBLE_LAYOUT_DELAY(procedure, delayTime, type, false, immediately, param);
	}
	public static void HIDE_LAYOUT_FORCE(LAYOUT_TYPE type)
	{
		VISIBLE_LAYOUT_FORCE(type, false);
	}
	public static CommandLayoutManagerLayoutVisible HIDE_LAYOUT_DELAY_FORCE(float delayTime, LAYOUT_TYPE type)
	{
		return VISIBLE_LAYOUT_DELAY_FORCE(delayTime, type, false);
	}
	public static void SHOW_LAYOUT(LAYOUT_TYPE type, bool immediately = false, string param = "")
	{
		VISIBLE_LAYOUT(type, true, immediately, param);
	}
	public static CommandLayoutManagerLayoutVisible SHOW_LAYOUT_DELAY(SceneProcedure procedure, float delayTime, LAYOUT_TYPE type, bool immediately = false, string param = "")
	{
		return VISIBLE_LAYOUT_DELAY(procedure, delayTime, type, true, immediately, param);
	}
	public static CommandLayoutManagerLayoutVisible SHOW_LAYOUT_DELAY_EX(SceneProcedure procedure, float delayTime, LAYOUT_TYPE type, CommandCallback start, object userData, bool immediately = false, string param = "")
	{
		return VISIBLE_LAYOUT_DELAY_EX(procedure, delayTime, type, true, start, userData, immediately, param);
	}
	public static void SHOW_LAYOUT_FORCE(LAYOUT_TYPE type)
	{
		VISIBLE_LAYOUT_FORCE(type, true);
	}
	public static CommandLayoutManagerLayoutVisible SHOW_LAYOUT_DELAY_FORCE(float delayTime, LAYOUT_TYPE type)
	{
		return VISIBLE_LAYOUT_DELAY_FORCE(delayTime, type, true);
	}
	// 设置布局显示状态
	public static void VISIBLE_LAYOUT(LAYOUT_TYPE type, bool visible, bool immediately = false, string param = "")
	{
		CommandLayoutManagerLayoutVisible cmd = new CommandLayoutManagerLayoutVisible(true, false);
		cmd.mType = type;
		cmd.mForce = false;
		cmd.mVisibility = visible;
		cmd.mImmediately = immediately;
		cmd.mParam = param;
		mCommandSystem.pushCommand(cmd, mLayoutManager);
	}
	public static CommandLayoutManagerLayoutVisible VISIBLE_LAYOUT_DELAY(SceneProcedure procedure, float delayTime, LAYOUT_TYPE type, bool visible, bool immediately = false, string param = "")
	{
		CommandLayoutManagerLayoutVisible cmd = new CommandLayoutManagerLayoutVisible(true, true);
		cmd.mType = type;
		cmd.mForce = false;
		cmd.mVisibility = visible;
		cmd.mImmediately = immediately;
		cmd.mParam = param;
		mCommandSystem.pushDelayCommand(cmd, mLayoutManager, delayTime);
		procedure.addDelayCmd(cmd);
		return cmd;
	}
	// 延迟设置布局显示状态
	public static CommandLayoutManagerLayoutVisible VISIBLE_LAYOUT_DELAY_EX(SceneProcedure procedure, float delayTime, LAYOUT_TYPE type, bool visible, CommandCallback start, object userData, bool immediately = false, string param = "")
	{
		CommandLayoutManagerLayoutVisible cmd = new CommandLayoutManagerLayoutVisible(true, true);
		cmd.mType = type;
		cmd.mForce = false;
		cmd.mVisibility = visible;
		cmd.mImmediately = immediately;
		cmd.mParam = param;
		cmd.addStartCommandCallback(start, userData);
		mCommandSystem.pushDelayCommand(cmd, mLayoutManager, delayTime);
		procedure.addDelayCmd(cmd);
		return cmd;
	}
	public static void VISIBLE_LAYOUT_FORCE(LAYOUT_TYPE type, bool visible)
	{
		CommandLayoutManagerLayoutVisible cmd = new CommandLayoutManagerLayoutVisible(true, false);
		cmd.mType = type;
		cmd.mForce = true;
		cmd.mVisibility = visible;
		cmd.mImmediately = false;
		mCommandSystem.pushCommand(cmd, mLayoutManager);
	}
	public static CommandLayoutManagerLayoutVisible VISIBLE_LAYOUT_DELAY_FORCE(float delayTime, LAYOUT_TYPE type, bool visible)
	{
		CommandLayoutManagerLayoutVisible cmd = new CommandLayoutManagerLayoutVisible(true, true);
		cmd.mType = type;
		cmd.mForce = true;
		cmd.mVisibility = visible;
		cmd.mImmediately = false;
		mCommandSystem.pushDelayCommand(cmd, mLayoutManager, delayTime);
		return cmd;
	}
	// 窗口
	//--------------------------------------------------------------------------------------------------------------------------------------------
	// 旋转
	// 固定窗口旋转
	public static void ROTATE_FIXED_WINDOW(txUIObject obj, bool active)
	{
		CommandWindowRotateFixed cmd = new CommandWindowRotateFixed(false);
		cmd.mActive = active;
		mCommandSystem.pushCommand(cmd, obj);
	}
	// 窗口匀速旋转至指定窗口
	public static void ROTATE_TARGET_WINDOW(txUIObject obj, Vector3 start, Vector3 target, float time)
	{
		CommandWindowRotateToTarget cmdRotate = new CommandWindowRotateToTarget(false);
		cmdRotate.mStartAngle = start;
		cmdRotate.mTargetAngle = target;
		cmdRotate.mRotateTime = time;
		mCommandSystem.pushCommand(cmdRotate, obj);
	}
	// 窗口匀速旋转至指定角度,并且可以设置回调函数
	public static void ROTATE_TARGET_WINDOW_EX(txUIObject obj, Vector3 start, Vector3 target, float time, RotateToTargetCallback rotatingCallback, object rotatingUserData, RotateToTargetCallback doneCallback, object doneUserData)
	{
		CommandWindowRotateToTarget cmdRotate = new CommandWindowRotateToTarget(false);
		cmdRotate.mStartAngle = start;
		cmdRotate.mTargetAngle = target;
		cmdRotate.mRotateTime = time;
		cmdRotate.setRotatingCallback(rotatingCallback, rotatingUserData);
		cmdRotate.setRotateDoneCallback(doneCallback, doneUserData);
		mCommandSystem.pushCommand(cmdRotate, obj);
	}
	// 窗口延迟匀速旋转至指定窗口
	public static void ROTATE_TARGET_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, Vector3 start, Vector3 target, float time)
	{
		CommandWindowRotateToTarget cmd = new CommandWindowRotateToTarget(false, true);
		cmd.mStartAngle = start;
		cmd.mTargetAngle = target;
		cmd.mRotateTime = time;
		mCommandSystem.pushDelayCommand(cmd, obj, delayTime);
		script.addDelayCmd(cmd);
	}
	// 恒定速度旋转窗口
	public static void ROTATE_SPEED_WINDOW(txUIObject obj, Vector3 speed, Vector3 startAngle, Vector3 rotateAccelerationValue)
	{
		CommandWindowRotateSpeed cmd = new CommandWindowRotateSpeed(false);
		cmd.mRotateSpeed = speed;
		cmd.mStartAngle = startAngle;
		cmd.mRotateAcceleration = rotateAccelerationValue;
		mCommandSystem.pushCommand(cmd, obj);
	}
	// 延迟恒定速度旋转窗口
	public static void ROTATE_SPEED_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, Vector3 speed, Vector3 startAngle, Vector3 rotateAccelerationValue)
	{
		CommandWindowRotateSpeed cmd = new CommandWindowRotateSpeed(false, true);
		cmd.mRotateSpeed = speed;
		cmd.mStartAngle = startAngle;
		cmd.mRotateAcceleration = rotateAccelerationValue;
		mCommandSystem.pushDelayCommand(cmd, obj, delayTime);
		script.addDelayCmd(cmd);
	}
	// 根据关键帧旋转窗口
	public static void ROTATE_KEY_FRMAE_WINDOW(txUIObject obj, string fileName, Vector3 start, Vector3 target, float onceLength, float offset, bool loop, bool fullonce, bool randomOffset)
	{
		CommandWindowKeyFrameRotate cmd = new CommandWindowKeyFrameRotate(false, false);
		cmd.mName = fileName;
		cmd.mOnceLength = onceLength;
		cmd.mStartRotation = start;
		cmd.mTargetRotation = target;
		cmd.mOffset = offset;
		cmd.mLoop = loop;
		cmd.mFullOnce = fullonce;
		cmd.mRandomOffset = randomOffset;
		mCommandSystem.pushCommand(cmd, obj);
	}
	// 根据关键帧延迟旋转窗口
	public static void ROTATE_KEY_FRMAE_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, string fileName, Vector3 start, Vector3 target, float onceLength, float offset, bool loop, bool fullonce, bool randomOffset)
	{
		CommandWindowKeyFrameRotate cmd = new CommandWindowKeyFrameRotate(false, true);
		cmd.mName = fileName;
		cmd.mOnceLength = onceLength;
		cmd.mStartRotation = start;
		cmd.mTargetRotation = target;
		cmd.mOffset = offset;
		cmd.mLoop = loop;
		cmd.mFullOnce = fullonce;
		cmd.mRandomOffset = randomOffset;
		mCommandSystem.pushDelayCommand(cmd, obj, delayTime);
		script.addDelayCmd(cmd);
	}
	//-------------------------------------------------------------------------------------------------------------------------------------------------------
	// 移动
	// 用关键帧移动窗口
	public static void MOVE_KEYFRAME_WINDOW(txUIObject uiObjct, string fileName, Vector3 startPos, Vector3 targetPos, float onceLength, float offset, bool loop, bool fullonce, bool randomOffset)
	{
		CommandWindowKeyFrameMove cmd = new CommandWindowKeyFrameMove(false);
		cmd.mName = fileName;
		cmd.mOnceLength = onceLength;
		cmd.mStartPos = startPos;
		cmd.mTargetPos = targetPos;
		cmd.mOffset = offset;
		cmd.mLoop = loop;
		cmd.mFullOnce = fullonce;
		cmd.mRandomOffset = randomOffset;
		mCommandSystem.pushCommand(cmd, uiObjct);
	}
	// 用关键帧移动窗口,可以设置回调函数
	public static void MOVE_KEYFRAME_WINDOW_EX(txUIObject uiObjct, string fileName, Vector3 startPos, Vector3 targetPos, float OnceLength, float offset, bool loop, bool fullonce, bool randomOffset, KeyFrameCallback TremblingCallBack, object TremblingUserData, KeyFrameCallback TrembleDoneCallBack, object TrembleDoneUserData)
	{
		CommandWindowKeyFrameMove cmd = new CommandWindowKeyFrameMove(false);
		cmd.mName = fileName;
		cmd.mOnceLength = OnceLength;
		cmd.mStartPos = startPos;
		cmd.mTargetPos = targetPos;
		cmd.mOffset = offset;
		cmd.mLoop = loop;
		cmd.mFullOnce = fullonce;
		cmd.mRandomOffset = randomOffset;
		cmd.setTremblingCallback(TremblingCallBack, TremblingUserData);
		cmd.setTrembleDoneCallback(TrembleDoneCallBack, TrembleDoneUserData);
		mCommandSystem.pushCommand(cmd, uiObjct);
	}
	// 延迟用关键帧移动窗口
	public static CommandWindowKeyFrameMove MOVE_KEYFRAME_WINDOW_DELAY(LayoutScript script, txUIObject uiObjct, float delayTime, string fileName, Vector3 startPos, Vector3 targetPos, float OnceLength, float offset, bool loop, bool fullonce, bool randomOffset)
	{
		CommandWindowKeyFrameMove cmd = new CommandWindowKeyFrameMove(false, true);
		cmd.mName = fileName;
		cmd.mStartPos = startPos;
		cmd.mTargetPos = targetPos;
		cmd.mOnceLength = OnceLength;
		cmd.mOffset = offset;
		cmd.mLoop = loop;
		cmd.mFullOnce = fullonce;
		cmd.mRandomOffset = randomOffset;
		mCommandSystem.pushDelayCommand(cmd, uiObjct, delayTime);
		script.addDelayCmd(cmd);
		return cmd;
	}
	// 移动窗口
	public static void MOVE_WINDOW(txUIObject obj, Vector3 start, Vector3 target, float time)
	{
		CommandWindowMove cmd = new CommandWindowMove(false);
		cmd.mStartPosition = start;
		cmd.mDestPosition = target;
		cmd.mMoveTime = time;
		mCommandSystem.pushCommand(cmd, obj);
	}
	// 延迟移动窗口
	public static void MOVE_WINDOW_DELAY(LayoutScript script, txUIObject obj, float dealyTime, Vector3 start, Vector3 target, float time)
	{
		CommandWindowMove cmd = new CommandWindowMove(false, true);
		cmd.mStartPosition = start;
		cmd.mDestPosition = target;
		cmd.mMoveTime = time;
		mCommandSystem.pushDelayCommand(cmd, obj, dealyTime);
		script.addDelayCmd(cmd);
	}
	// 移动窗口, 并且可以设置移动时和移动完成的回调函数
	public static void MOVE_WINDOW_EX(txUIObject obj, Vector3 start, Vector3 target, float time, float offsetTime, MoveCallback movingCallback, object movingUserData, MoveCallback moveDoneCallback, object moveDoneUserData)
	{
		CommandWindowMove cmd = new CommandWindowMove(false);
		cmd.mStartPosition = start;
		cmd.mDestPosition = target;
		cmd.mMoveTime = time;
		cmd.mTimeOffset = offsetTime;
		cmd.setMovingCallback(movingCallback, movingUserData);
		cmd.setMoveDoneCallback(moveDoneCallback, moveDoneUserData);
		mCommandSystem.pushCommand(cmd, obj);
	}
	//  延迟移动窗口, 并且可以设置移动时和移动完成的回调函数
	public static void MOVE_WINDOW_DELAY_EX(LayoutScript script, txUIObject obj, float delayTime, Vector3 start, Vector3 target, float time, MoveCallback movingCallback, object movingUserData, MoveCallback moveDoneCallback, object moveDoneUserData)
	{
		CommandWindowMove cmd = new CommandWindowMove(false, true);
		cmd.mStartPosition = start;
		cmd.mDestPosition = target;
		cmd.mMoveTime = time;
		cmd.setMovingCallback(movingCallback, movingUserData);
		cmd.setMoveDoneCallback(moveDoneCallback, moveDoneUserData);
		mCommandSystem.pushDelayCommand(cmd, obj, delayTime);
		script.addDelayCmd(cmd);
	}
	public static void MOVE_WINDOW_DELAY_CMDEX(LayoutScript script, txUIObject obj, float delayTime, Vector3 start, Vector3 target, float time, CommandCallback startCmd, object userData)
	{
		CommandWindowMove cmd = new CommandWindowMove(false, true);
		cmd.mStartPosition = start;
		cmd.mDestPosition = target;
		cmd.mMoveTime = time;
		cmd.addStartCommandCallback(startCmd, userData);
		mCommandSystem.pushDelayCommand(cmd, obj, delayTime);
		script.addDelayCmd(cmd);
	}
	//----------------------------------------------------------------------------------------------------------------------------------------------------------
	// 显示
	// 窗口的显示和隐藏()
	public static void ACTIVE_WINDOW(txUIObject obj, bool active = true)
	{
		CommandWindowActive cmd = new CommandWindowActive(false);
		cmd.mActive = active;
		mCommandSystem.pushCommand(cmd, obj);
	}
	// 延迟窗口的显示和隐藏
	public static void ACTIVE_WINDOW_DELAY(LayoutScript script, txUIObject obj, bool active, float dealyTime)
	{
		CommandWindowActive cmd = new CommandWindowActive(false, true);
		cmd.mActive = active;
		mCommandSystem.pushDelayCommand(cmd, obj, dealyTime);
		script.addDelayCmd(cmd);
	}
	// 延迟窗口的显示和隐藏,并且可以设置回调函数
	public static CommandWindowActive ACTIVE_WINDOW_DELAY_EX(LayoutScript script, txUIObject obj, bool active, float dealyTime, CommandCallback startCallback, object userData)
	{
		CommandWindowActive cmd = new CommandWindowActive(false, true);
		cmd.mActive = active;
		cmd.addStartCommandCallback(startCallback, userData);
		mCommandSystem.pushDelayCommand(cmd, obj, dealyTime);
		script.addDelayCmd(cmd);
		return cmd;
	}
	//----------------------------------------------------------------------------------------------------------------------------------------------------------
	// 缩放
	// 窗口缩放
	public static void SCALE_WINDOW(txUIObject obj, Vector2 start, Vector2 target, float time)
	{
		CommandWindowScale cmd = new CommandWindowScale(false);
		cmd.mStartScale = start;
		cmd.mTargetScale = target;
		cmd.mScaleTime = time;
		mCommandSystem.pushCommand(cmd, obj);
	}
	//延迟窗口缩放
	public static void SCALE_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, Vector2 start, Vector2 target, float time)
	{
		CommandWindowScale cmd = new CommandWindowScale(false, true);
		cmd.mStartScale = start;
		cmd.mTargetScale = target;
		cmd.mScaleTime = time;
		mCommandSystem.pushDelayCommand(cmd, obj, delayTime);
		script.addDelayCmd(cmd);
	}
	//窗口缩放,可以设置回调函数
	public static void SCALE_WINDOW_EX(txUIObject obj, Vector2 start, Vector2 target, float time, ScaleCallback ScalingCallback, object ScalingUserData, ScaleCallback ScaleDoneCallback, object ScaleDoneUserData)
	{
		CommandWindowScale cmd = new CommandWindowScale(false);
		cmd.mStartScale = start;
		cmd.mTargetScale = target;
		cmd.mScaleTime = time;
		cmd.setScalingCallback(ScalingCallback, ScalingUserData);
		cmd.setScaleDoneCallback(ScaleDoneCallback, ScaleDoneUserData);
		mCommandSystem.pushCommand(cmd, obj);
	}
	// 延迟窗口缩放,可以设置回调函数
	public static CommandWindowScale SCALE_WINDOW_DELAY_EX(LayoutScript script, txUIObject obj, float delayTime, Vector2 start, Vector2 target, float time, ScaleCallback ScalingCallback, object ScalingUserData, ScaleCallback ScaleDoneCallback, object ScaleDoneUserData)
	{
		CommandWindowScale cmd = new CommandWindowScale(false, true);
		cmd.mStartScale = start;
		cmd.mTargetScale = target;
		cmd.mScaleTime = time;
		cmd.setScalingCallback(ScalingCallback, ScalingUserData);
		cmd.setScaleDoneCallback(ScaleDoneCallback, ScaleDoneUserData);
		mCommandSystem.pushDelayCommand(cmd, obj, delayTime);
		script.addDelayCmd(cmd);
		return cmd;
	}
	// 窗口用关键帧缩放震动
	public static void SCALE_TREMBLE_WINDOW(txUIObject obj, string fileName, Vector2 start, Vector2 target, float OnceLength, float offset, bool loop, bool fullOnce, bool randomOffset)
	{
		CommandWindowScaleTrembling cmd = new CommandWindowScaleTrembling(false);
		cmd.mName = fileName;
		cmd.mOnceLength = OnceLength;
		cmd.mOffset = offset;
		cmd.mLoop = loop;
		cmd.mFullOnce = fullOnce;
		cmd.mRandomOffset = randomOffset;
		cmd.mStartScale = start;
		cmd.mTargetScale = target;
		mCommandSystem.pushCommand(cmd, obj);
	}
	//延迟窗口用关键帧缩放震动
	public static void SCALE_TREMBLE_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, string fileName, Vector2 start, Vector2 target, float OnceLength, float offset, bool loop, bool fullOnce, bool randomOffset)
	{
		CommandWindowScaleTrembling cmd = new CommandWindowScaleTrembling(false, true);
		cmd.mName = fileName;
		cmd.mOnceLength = OnceLength;
		cmd.mOffset = offset;
		cmd.mLoop = loop;
		cmd.mFullOnce = fullOnce;
		cmd.mRandomOffset = randomOffset;
		cmd.mStartScale = start;
		cmd.mTargetScale = target;
		mCommandSystem.pushDelayCommand(cmd, obj);
		script.addDelayCmd(cmd);
	}
	//用关键帧缩放震动窗口,可以设置回调函数
	public static void SCALE_TREMBLE_WINDOW_EX(txUIObject obj, string fileName, Vector2 start, Vector2 target, float onceLength, float offset, bool loop, bool fullOnce, bool randomOffset, KeyFrameCallback scaleTremblingCallback, object scaleTremblingUserData, KeyFrameCallback scaleTrembleDoneCallback, object scaleTrembleDoneUserData)
	{
		CommandWindowScaleTrembling cmd = new CommandWindowScaleTrembling(false);
		cmd.mName = fileName;
		cmd.mOnceLength = onceLength;
		cmd.mOffset = offset;
		cmd.mLoop = loop;
		cmd.mFullOnce = fullOnce;
		cmd.mRandomOffset = randomOffset;
		cmd.mStartScale = start;
		cmd.mTargetScale = target;
		cmd.setTremblingCallback(scaleTremblingCallback, scaleTremblingUserData);
		cmd.setTrembleDoneCallback(scaleTrembleDoneCallback, scaleTrembleDoneUserData);
		mCommandSystem.pushCommand(cmd, obj);
	}
	// 延迟用关键帧缩放震动窗口,可以设置回调函数
	public static void SCALE_TREMBLE_WINDOW_DELAY_EX(LayoutScript script, txUIObject obj, float delayTime, string fileName, Vector2 start, Vector2 target, float onceLength, float offset, bool loop, bool fullOnce, bool randomOffset, KeyFrameCallback scaleTremblingCallback, object scaleTremblingUserData, KeyFrameCallback scaleTrembleDoneCallback, object scaleTrembleDoneUserData)
	{
		CommandWindowScaleTrembling cmd = new CommandWindowScaleTrembling(false, true);
		cmd.mName = fileName;
		cmd.mOnceLength = onceLength;
		cmd.mOffset = offset;
		cmd.mLoop = loop;
		cmd.mFullOnce = fullOnce;
		cmd.mRandomOffset = randomOffset;
		cmd.mStartScale = start;
		cmd.mTargetScale = target;
		cmd.setTremblingCallback(scaleTremblingCallback, scaleTremblingUserData);
		cmd.setTrembleDoneCallback(scaleTrembleDoneCallback, scaleTrembleDoneUserData);
		mCommandSystem.pushDelayCommand(cmd, obj, delayTime);
		script.addDelayCmd(cmd);
	}
	//-----------------------------------------------------------------------------------------------------------------------------------------------------------
	// 透明
	// 用关键帧窗口透明度震动
	public static void ALPHA_TREMBLE_WINDOW(txUIObject obj, string name,float start,float target, bool loop, float onceLength, float offset)
	{
		CommandWindowAlphaTremble cmd = new CommandWindowAlphaTremble(false);
		cmd.mName = name;
		cmd.mLoop = loop;
		cmd.mOnceLength = onceLength;
		cmd.mOffset = offset;
		cmd.mStartAlpha = start;
		cmd.mTargetAlpha = target;
		mCommandSystem.pushCommand(cmd, obj);
	}
	// 延迟用关键帧窗口透明度震动
	public static void ALPHA_TREMBLE_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, string name,float start,float target,bool loop, float onceLength, float offset)
	{
		CommandWindowAlphaTremble cmd = new CommandWindowAlphaTremble(false, true);
		cmd.mName = name;
		cmd.mLoop = loop;
		cmd.mOnceLength = onceLength;
		cmd.mOffset = offset;
		cmd.mStartAlpha = start;
		cmd.mTargetAlpha = target;
		mCommandSystem.pushDelayCommand(cmd, obj, delayTime);
		script.addDelayCmd(cmd);
	}
	// 用关键帧窗口透明度震动,可以设置回调函数
	public static void ALPHA_TREMBLE_WINDOW_EX(txUIObject obj, string name,float start,float target,bool loop, float onceLength, float offset, bool randomOffset, KeyFrameCallback tremblingCallback, object tremblingUserData, KeyFrameCallback trembleDoneCallback, object trembleDoneUserData)
	{
		CommandWindowAlphaTremble cmd = new CommandWindowAlphaTremble(false);
		cmd.mName = name;
		cmd.mLoop = loop;
		cmd.mOnceLength = onceLength;
		cmd.mOffset = offset;
		cmd.mStartAlpha = start;
		cmd.mTargetAlpha = target;
		cmd.mRandomOffset = randomOffset;
		cmd.setTremblingCallback(tremblingCallback, tremblingUserData);
		cmd.setTrembleDoneCallback(trembleDoneCallback, trembleDoneUserData);
		mCommandSystem.pushCommand(cmd, obj);
	}
	// 延迟用关键帧窗口透明度震动,可以设置回调函数
	public static void ALPHA_TREMBLE_WINDOW_DELAY_EX(LayoutScript script, txUIObject obj, float delayTime, string name,float start,float target, bool loop, float onceLength, float offset, bool randomOffset, KeyFrameCallback tremblingCallback, object tremblingUserData, KeyFrameCallback trembleDoneCallback, object trembleDoneUserData)
	{
		CommandWindowAlphaTremble cmd = new CommandWindowAlphaTremble(false, true);
		cmd.mName = name;
		cmd.mLoop = loop;
		cmd.mOnceLength = onceLength;
		cmd.mOffset = offset;
		cmd.mStartAlpha = start;
		cmd.mTargetAlpha = target;
		cmd.mRandomOffset = randomOffset;
		cmd.setTremblingCallback(tremblingCallback, tremblingUserData);
		cmd.setTrembleDoneCallback(trembleDoneCallback, trembleDoneUserData);
		mCommandSystem.pushDelayCommand(cmd, obj, delayTime);
		script.addDelayCmd(cmd);
	}
	//窗口透明度
	public static void ALPHA_WINDOW(txUIObject obj, float start, float target, float time)
	{
		CommandWindowAlpha cmd = new CommandWindowAlpha(false);
		cmd.mStartAlpha = start;
		cmd.mTargetAlpha = target;
		cmd.mFadeTime = time;
		mCommandSystem.pushCommand(cmd, obj);
	}
	//窗口透明度,可以设置回调函数
	public static void ALPHA_WINDOW_EX(txUIObject obj, float start, float target, float time, AlphaFadeCallback FadingCallback, object FadingData, AlphaFadeCallback FadeDoneCallBack, object FadeDoneData)
	{
		CommandWindowAlpha cmd = new CommandWindowAlpha(false);
		cmd.mStartAlpha = start;
		cmd.mTargetAlpha = target;
		cmd.mFadeTime = time;
		cmd.setFadingCallback(FadingCallback, FadingData);
		cmd.setFadeDoneCallback(FadeDoneCallBack, FadeDoneData);
		mCommandSystem.pushCommand(cmd, obj);
	}
	//延迟窗口的透明度变化
	public static void ALPHA_WINDOW_DELAY(LayoutScript script, txUIObject uiObjct, float dealyTime, float start, float target, float time)
	{
		CommandWindowAlpha cmd = new CommandWindowAlpha(false, true);
		cmd.mStartAlpha = start;
		cmd.mTargetAlpha = target;
		cmd.mFadeTime = time;
		mCommandSystem.pushDelayCommand(cmd, uiObjct, dealyTime);
		script.addDelayCmd(cmd);
	}
	//延迟窗口的透明度变化,可以设置回调函数
	public static CommandWindowAlpha ALPHA_WINDOW_DELAY_EX(LayoutScript script, txUIObject uiObjct, float dealyTime, float start, float target, float time, AlphaFadeCallback FadingCallback, object FadingData, AlphaFadeCallback FadeDoneCallBack, object FadeDoneData)
	{
		CommandWindowAlpha cmd = new CommandWindowAlpha(false, true);
		cmd.mStartAlpha = start;
		cmd.mTargetAlpha = target;
		cmd.mFadeTime = time;
		cmd.setFadingCallback(FadingCallback, FadingData);
		cmd.setFadeDoneCallback(FadeDoneCallBack, FadeDoneData);
		mCommandSystem.pushDelayCommand(cmd, uiObjct, dealyTime);
		script.addDelayCmd(cmd);
		return cmd;
	}
	// 变化窗口的HSL偏移
	public static void HSL_WINDOW(txUIObject obj, Vector3 start, Vector3 target, float time)
	{
		CommandWindowHSL cmdHSL = new CommandWindowHSL();
		cmdHSL.mStartHSL = start;
		cmdHSL.mTargetHSL = target;
		cmdHSL.mFadeTime = time;
		mCommandSystem.pushCommand(cmdHSL, obj);
	}
	// 延迟变化窗口的HSL偏移
	public static void HSL_WINDOW_DELAY(txUIObject obj, float delayTime, Vector3 start, Vector3 target, float time)
	{
		CommandWindowHSL cmdHSL = new CommandWindowHSL(true ,true);
		cmdHSL.mStartHSL = start;
		cmdHSL.mTargetHSL = target;
		cmdHSL.mFadeTime = time;
		mCommandSystem.pushDelayCommand(cmdHSL, obj ,delayTime);
	}
	//--------------------------------------------------------------------------------------------------------------------------------------------------------
	// 音效
	public static void PLAY_AUDIO_WINDOW(txUIObject obj, SOUND_DEFINE sound, bool loop, float volume)
	{
		CommandWindowPlayAudio cmd = new CommandWindowPlayAudio(false);
		cmd.mSound = sound;
		cmd.mLoop = loop;
		cmd.mVolume = volume;
		mCommandSystem.pushCommand(cmd, obj);
	}
	public static void PLAY_AUDIO_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, SOUND_DEFINE sound, bool loop, float volume)
	{
		CommandWindowPlayAudio cmd = new CommandWindowPlayAudio(false, true);
		cmd.mSound = sound;
		cmd.mLoop = loop;
		cmd.mVolume = volume;
		mCommandSystem.pushDelayCommand(cmd, obj, delayTime);
		script.addDelayCmd(cmd);
	}
}