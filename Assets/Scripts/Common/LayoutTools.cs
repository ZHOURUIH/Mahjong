using UnityEngine;
using System.Collections;
using System;

public class LayoutTools : GameBase
{
	// 布局
	//------------------------------------------------------------------------------------------------------------------------------------
	public static void LOAD_LAYOUT(LAYOUT_TYPE type, int renderOrder, bool visible)
	{
		CommandLayoutManagerLoadLayout cmd = mCommandSystem.newCmd<CommandLayoutManagerLoadLayout>(true, false);
		cmd.mLayoutType = type;
		cmd.mVisible = visible;
		cmd.mRenderOrder = renderOrder;
		cmd.mAsync = false;
		mCommandSystem.pushCommand(cmd, mLayoutManager);
	}
	public static void LOAD_LAYOUT_ASYNC(LAYOUT_TYPE type, int renderOrder, LayoutAsyncDone callback)
	{
		CommandLayoutManagerLoadLayout cmd = mCommandSystem.newCmd<CommandLayoutManagerLoadLayout>(true, false);
		cmd.mLayoutType = type;
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
	public static void HIDE_LAYOUT_FORCE(LAYOUT_TYPE type)
	{
		VISIBLE_LAYOUT_FORCE(type, false);
	}
	public static void SHOW_LAYOUT(LAYOUT_TYPE type, bool immediately = false, string param = "")
	{
		VISIBLE_LAYOUT(type, true, immediately, param);
	}
	public static void SHOW_LAYOUT_FORCE(LAYOUT_TYPE type)
	{
		VISIBLE_LAYOUT_FORCE(type, true);
	}
	public static void VISIBLE_LAYOUT(LAYOUT_TYPE type, bool visible, bool immediately = false, string param = "")
	{
		CommandLayoutManagerLayoutVisible cmd = mCommandSystem.newCmd<CommandLayoutManagerLayoutVisible>(true, false);
		cmd.mLayoutType = type;
		cmd.mForce = false;
		cmd.mVisibility = visible;
		cmd.mImmediately = immediately;
		cmd.mParam = param;
		mCommandSystem.pushCommand(cmd, mLayoutManager);
	}
	public static void VISIBLE_LAYOUT_FORCE(LAYOUT_TYPE type, bool visible)
	{
		CommandLayoutManagerLayoutVisible cmd = mCommandSystem.newCmd<CommandLayoutManagerLayoutVisible>(true, false);
		cmd.mLayoutType = type;
		cmd.mForce = true;
		cmd.mVisibility = visible;
		cmd.mImmediately = false;
		mCommandSystem.pushCommand(cmd, mLayoutManager);
	}
	public static CommandLayoutManagerLayoutVisible HIDE_LAYOUT_DELAY(SceneProcedure procedure, float delayTime, LAYOUT_TYPE type, bool immediately = false, string param = "")
	{
		return VISIBLE_LAYOUT_DELAY(procedure, delayTime, type, false, immediately, param);
	}
	public static CommandLayoutManagerLayoutVisible HIDE_LAYOUT_DELAY_FORCE(float delayTime, LAYOUT_TYPE type)
	{
		return VISIBLE_LAYOUT_DELAY_FORCE(delayTime, type, false);
	}
	public static CommandLayoutManagerLayoutVisible SHOW_LAYOUT_DELAY(SceneProcedure procedure, float delayTime, LAYOUT_TYPE type, bool immediately = false, string param = "")
	{
		return VISIBLE_LAYOUT_DELAY(procedure, delayTime, type, true, immediately, param);
	}
	public static CommandLayoutManagerLayoutVisible SHOW_LAYOUT_DELAY_EX(SceneProcedure procedure, float delayTime, LAYOUT_TYPE type, CommandCallback start, bool immediately = false, string param = "")
	{
		return VISIBLE_LAYOUT_DELAY_EX(procedure, delayTime, type, true, start, immediately, param);
	}
	public static CommandLayoutManagerLayoutVisible SHOW_LAYOUT_DELAY_FORCE(float delayTime, LAYOUT_TYPE type)
	{
		return VISIBLE_LAYOUT_DELAY_FORCE(delayTime, type, true);
	}
	public static CommandLayoutManagerLayoutVisible VISIBLE_LAYOUT_DELAY(SceneProcedure procedure, float delayTime, LAYOUT_TYPE type, bool visible, bool immediately = false, string param = "")
	{
		CommandLayoutManagerLayoutVisible cmd = mCommandSystem.newCmd<CommandLayoutManagerLayoutVisible>(true, true);
		cmd.mLayoutType = type;
		cmd.mForce = false;
		cmd.mVisibility = visible;
		cmd.mImmediately = immediately;
		cmd.mParam = param;
		mCommandSystem.pushDelayCommand(cmd, mLayoutManager, delayTime);
		procedure.addDelayCmd(cmd);
		return cmd;
	}
	public static CommandLayoutManagerLayoutVisible VISIBLE_LAYOUT_DELAY_EX(SceneProcedure procedure, float delayTime, LAYOUT_TYPE type, bool visible, CommandCallback start, bool immediately = false, string param = "")
	{
		CommandLayoutManagerLayoutVisible cmd = mCommandSystem.newCmd<CommandLayoutManagerLayoutVisible>(true, true);
		cmd.mLayoutType = type;
		cmd.mForce = false;
		cmd.mVisibility = visible;
		cmd.mImmediately = immediately;
		cmd.mParam = param;
		cmd.addStartCommandCallback(start, null);
		mCommandSystem.pushDelayCommand(cmd, mLayoutManager, delayTime);
		procedure.addDelayCmd(cmd);
		return cmd;
	}
	public static CommandLayoutManagerLayoutVisible VISIBLE_LAYOUT_DELAY_FORCE(float delayTime, LAYOUT_TYPE type, bool visible)
	{
		CommandLayoutManagerLayoutVisible cmd = mCommandSystem.newCmd<CommandLayoutManagerLayoutVisible>(true, true);
		cmd.mLayoutType = type;
		cmd.mForce = true;
		cmd.mVisibility = visible;
		cmd.mImmediately = false;
		mCommandSystem.pushDelayCommand(cmd, mLayoutManager, delayTime);
		return cmd;
	}
	//--------------------------------------------------------------------------------------------------------------------------------------------
	// 旋转
	public static void ROTATE_LOCK_WINDOW(txUIObject obj, bool lockRotation = true)
	{
		CommandWindowRotateFixed cmd = mCommandSystem.newCmd<CommandWindowRotateFixed>(false);
		cmd.mActive = lockRotation;
		mCommandSystem.pushCommand(cmd, obj);
	}
	public static void ROTATE_WINDOW(txUIObject obj, Vector3 rotation)
	{
		CommandWindowKeyFrameRotate cmd = mCommandSystem.newCmd<CommandWindowKeyFrameRotate>(false, false);
		cmd.mName = "";
		cmd.mOnceLength = 0.0f;
		cmd.mStartRotation = rotation;
		cmd.mTargetRotation = rotation;
		mCommandSystem.pushCommand(cmd, obj);
	}
	public static void ROTATE_WINDOW(txUIObject obj, Vector3 start, Vector3 target, float time)
	{
		ROTATE_KEYFRMAE_WINDOW_EX(obj, CommonDefine.ZERO_ONE, start, target, time, false, 0.0f, null, null);
	}
	public static void ROTATE_WINDOW_EX(txUIObject obj, Vector3 start, Vector3 target, float time, KeyFrameCallback rotatingCallback, KeyFrameCallback doneCallback)
	{
		ROTATE_KEYFRMAE_WINDOW_EX(obj, CommonDefine.ZERO_ONE, start, target, time, false, 0.0f, rotatingCallback, doneCallback);
	}
	public static void ROTATE_WINDOW_EX(txUIObject obj, Vector3 start, Vector3 target, float time, KeyFrameCallback doneCallback)
	{
		ROTATE_KEYFRMAE_WINDOW_EX(obj, CommonDefine.ZERO_ONE, start, target, time, false, 0.0f, null, doneCallback);
	}
	public static void ROTATE_KEYFRMAE_WINDOW(txUIObject obj, string keyframe, Vector3 start, Vector3 target, float onceLength)
	{
		ROTATE_KEYFRMAE_WINDOW_EX(obj, keyframe, start, target, onceLength, false, 0.0f, null, null);
	}
	public static void ROTATE_KEYFRMAE_WINDOW(txUIObject obj, string keyframe, Vector3 start, Vector3 target, float onceLength, bool loop, float offset)
	{
		ROTATE_KEYFRMAE_WINDOW_EX(obj, keyframe, start, target, onceLength, loop, offset, null, null);
	}
	public static void ROTATE_KEYFRMAE_WINDOW_EX(txUIObject obj, string keyframe, Vector3 start, Vector3 target, float onceLength, bool loop, float offset, KeyFrameCallback rotatingCallback, KeyFrameCallback doneCallback)
	{
		if(keyframe == "" || MathUtility.isFloatZero(onceLength))
		{
			UnityUtility.logError("时间或关键帧不能为空,如果要停止组件,请使用void ROTATE_WINDOW(txUIObject obj, Vector3 rotation)");
		}
		CommandWindowKeyFrameRotate cmd = mCommandSystem.newCmd<CommandWindowKeyFrameRotate>(false, false);
		cmd.mName = keyframe;
		cmd.mOnceLength = onceLength;
		cmd.mStartRotation = start;
		cmd.mTargetRotation = target;
		cmd.mOffset = offset;
		cmd.mLoop = loop;
		cmd.setTremblingCallback(rotatingCallback, null);
		cmd.setTrembleDoneCallback(doneCallback, null);
		mCommandSystem.pushCommand(cmd, obj);
	}
	public static CommandWindowKeyFrameRotate ROTATE_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, Vector3 rotation)
	{
		CommandWindowKeyFrameRotate cmd = mCommandSystem.newCmd<CommandWindowKeyFrameRotate>(false, true);
		cmd.mName = "";
		cmd.mOnceLength = 0.0f;
		cmd.mStartRotation = rotation;
		cmd.mTargetRotation = rotation;
		mCommandSystem.pushDelayCommand(cmd, obj, delayTime);
		script.addDelayCmd(cmd);
		return cmd;
	}
	public static CommandWindowKeyFrameRotate ROTATE_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, Vector3 start, Vector3 target, float time)
	{
		return ROTATE_KEYFRMAE_WINDOW_DELAY(script, obj, delayTime, CommonDefine.ZERO_ONE, start, target, time, false, 0.0f);
	}
	public static CommandWindowKeyFrameRotate ROTATE_KEYFRMAE_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, string keyframe, Vector3 start, Vector3 target, float onceLength)
	{
		return ROTATE_KEYFRMAE_WINDOW_DELAY(script, obj, delayTime, keyframe, start, target, onceLength, false, 0.0f);
	}
	public static CommandWindowKeyFrameRotate ROTATE_KEYFRMAE_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, string keyframe, Vector3 start, Vector3 target, float onceLength, bool loop)
	{
		return ROTATE_KEYFRMAE_WINDOW_DELAY(script, obj, delayTime, keyframe, start, target, onceLength, loop, 0.0f);
	}
	public static CommandWindowKeyFrameRotate ROTATE_KEYFRMAE_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, string keyframe, Vector3 start, Vector3 target, float onceLength, bool loop, float offset)
	{
		if (keyframe == "" || MathUtility.isFloatZero(onceLength))
		{
			UnityUtility.logError("时间或关键帧不能为空,如果要停止组件,请使用CommandWindowKeyFrameRotate ROTATE_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, Vector3 rotation)");
		}
		CommandWindowKeyFrameRotate cmd = mCommandSystem.newCmd<CommandWindowKeyFrameRotate>(false, true);
		cmd.mName = keyframe;
		cmd.mOnceLength = onceLength;
		cmd.mStartRotation = start;
		cmd.mTargetRotation = target;
		cmd.mOffset = offset;
		cmd.mLoop = loop;
		mCommandSystem.pushDelayCommand(cmd, obj, delayTime);
		script.addDelayCmd(cmd);
		return cmd;
	}
	public static void ROTATE_SPEED_WINDOW(txUIObject obj)
	{
		ROTATE_SPEED_WINDOW(obj, Vector3.zero, Vector3.zero, Vector3.zero);
	}
	public static void ROTATE_SPEED_WINDOW(txUIObject obj, Vector3 speed)
	{
		ROTATE_SPEED_WINDOW(obj, speed, Vector3.zero, Vector3.zero);
	}
	public static void ROTATE_SPEED_WINDOW(txUIObject obj, Vector3 speed, Vector3 startAngle)
	{
		ROTATE_SPEED_WINDOW(obj, speed, startAngle, Vector3.zero);
	}
	public static void ROTATE_SPEED_WINDOW(txUIObject obj, Vector3 speed, Vector3 startAngle, Vector3 rotateAccelerationValue)
	{
		CommandWindowRotateSpeed cmd = mCommandSystem.newCmd<CommandWindowRotateSpeed>(false);
		cmd.mRotateSpeed = speed;
		cmd.mStartAngle = startAngle;
		cmd.mRotateAcceleration = rotateAccelerationValue;
		mCommandSystem.pushCommand(cmd, obj);
	}
	public static CommandWindowRotateSpeed ROTATE_SPEED_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, Vector3 speed)
	{
		return ROTATE_SPEED_WINDOW_DELAY(script, obj, delayTime, speed, Vector3.zero, Vector3.zero);
	}
	public static CommandWindowRotateSpeed ROTATE_SPEED_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, Vector3 speed, Vector3 startAngle)
	{
		return ROTATE_SPEED_WINDOW_DELAY(script, obj, delayTime, speed, startAngle, Vector3.zero);
	}
	public static CommandWindowRotateSpeed ROTATE_SPEED_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, Vector3 speed, Vector3 startAngle, Vector3 rotateAccelerationValue)
	{
		CommandWindowRotateSpeed cmd = mCommandSystem.newCmd<CommandWindowRotateSpeed>(false, true);
		cmd.mRotateSpeed = speed;
		cmd.mStartAngle = startAngle;
		cmd.mRotateAcceleration = rotateAccelerationValue;
		mCommandSystem.pushDelayCommand(cmd, obj, delayTime);
		script.addDelayCmd(cmd);
		return cmd;
	}
	//-------------------------------------------------------------------------------------------------------------------------------------------------------
	// 移动
	// 用关键帧移动窗口
	public static void MOVE_WINDOW(txUIObject obj, Vector3 pos)
	{
		CommandWindowKeyFrameMove cmd = mCommandSystem.newCmd<CommandWindowKeyFrameMove>(false);
		cmd.mName = "";
		cmd.mOnceLength = 0.0f;
		cmd.mStartPos = pos;
		cmd.mTargetPos = pos;
		mCommandSystem.pushCommand(cmd, obj);
	}
	public static void MOVE_WINDOW(txUIObject obj, Vector3 start, Vector3 target, float onceLength)
	{
		MOVE_KEYFRAME_WINDOW_EX(obj, CommonDefine.ZERO_ONE, start, target, onceLength, false, 0.0f, null, null);
	}
	public static void MOVE_WINDOW_EX(txUIObject obj, Vector3 start, Vector3 target, float onceLength, KeyFrameCallback moveDoneCallback)
	{
		MOVE_KEYFRAME_WINDOW_EX(obj, CommonDefine.ZERO_ONE, start, target, onceLength, false, 0.0f, null, moveDoneCallback);
	}
	public static void MOVE_WINDOW_EX(txUIObject obj, Vector3 start, Vector3 target, float onceLength, KeyFrameCallback movingCallback, KeyFrameCallback moveDoneCallback)
	{
		MOVE_KEYFRAME_WINDOW_EX(obj, CommonDefine.ZERO_ONE, start, target, onceLength, false, 0.0f, movingCallback, moveDoneCallback);
	}
	public static void MOVE_WINDOW_EX(txUIObject obj, Vector3 start, Vector3 target, float onceLength, float offsetTime, KeyFrameCallback moveDoneCallback)
	{
		MOVE_KEYFRAME_WINDOW_EX(obj, CommonDefine.ZERO_ONE, start, target, onceLength, false, offsetTime, null, moveDoneCallback);
	}
	public static void MOVE_WINDOW_EX(txUIObject obj, Vector3 start, Vector3 target, float onceLength, float offsetTime, KeyFrameCallback movingCallback, KeyFrameCallback moveDoneCallback)
	{
		MOVE_KEYFRAME_WINDOW_EX(obj, CommonDefine.ZERO_ONE, start, target, onceLength, false, offsetTime, movingCallback, moveDoneCallback);
	}
	public static void MOVE_KEYFRAME_WINDOW(txUIObject obj, string fileName, Vector3 startPos, Vector3 targetPos, float onceLength)
	{
		MOVE_KEYFRAME_WINDOW_EX(obj, fileName, startPos, targetPos, onceLength, false, 0.0f, null, null);
	}
	public static void MOVE_KEYFRAME_WINDOW(txUIObject obj, string fileName, Vector3 startPos, Vector3 targetPos, float onceLength, bool loop)
	{
		MOVE_KEYFRAME_WINDOW_EX(obj, fileName, startPos, targetPos, onceLength, loop, 0.0f, null, null);
	}
	public static void MOVE_KEYFRAME_WINDOW(txUIObject obj, string fileName, Vector3 startPos, Vector3 targetPos, float onceLength, bool loop, float offset)
	{
		MOVE_KEYFRAME_WINDOW_EX(obj, fileName, startPos, targetPos, onceLength, loop, offset, null, null);
	}
	public static void MOVE_KEYFRAME_WINDOW_EX(txUIObject obj, string fileName, Vector3 startPos, Vector3 targetPos, float onceLength, KeyFrameCallback TremblingCallBack, KeyFrameCallback TrembleDoneCallBack)
	{
		MOVE_KEYFRAME_WINDOW_EX(obj, fileName, startPos, targetPos, onceLength, false, 0.0f, TremblingCallBack, TrembleDoneCallBack);
	}
	public static void MOVE_KEYFRAME_WINDOW_EX(txUIObject obj, string fileName, Vector3 startPos, Vector3 targetPos, float onceLength, bool loop, KeyFrameCallback TremblingCallBack, KeyFrameCallback TrembleDoneCallBack)
	{
		MOVE_KEYFRAME_WINDOW_EX(obj, fileName, startPos, targetPos, onceLength, loop, 0.0f, TremblingCallBack, TrembleDoneCallBack);
	}
	public static void MOVE_KEYFRAME_WINDOW_EX(txUIObject obj, string fileName, Vector3 startPos, Vector3 targetPos, float onceLength, bool loop, float offset, KeyFrameCallback TremblingCallBack, KeyFrameCallback TrembleDoneCallBack)
	{
		if (fileName == "" || MathUtility.isFloatZero(onceLength))
		{
			UnityUtility.logError("时间或关键帧不能为空,如果要停止组件,请使用void MOVE_WINDOW(txUIObject obj, Vector3 pos)");
		}
		CommandWindowKeyFrameMove cmd = mCommandSystem.newCmd<CommandWindowKeyFrameMove>(false);
		cmd.mName = fileName;
		cmd.mOnceLength = onceLength;
		cmd.mStartPos = startPos;
		cmd.mTargetPos = targetPos;
		cmd.mOffset = offset;
		cmd.mLoop = loop;
		cmd.setTremblingCallback(TremblingCallBack, null);
		cmd.setTrembleDoneCallback(TrembleDoneCallBack, null);
		mCommandSystem.pushCommand(cmd, obj);
	}
	public static CommandWindowKeyFrameMove MOVE_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, Vector3 pos)
	{
		CommandWindowKeyFrameMove cmd = mCommandSystem.newCmd<CommandWindowKeyFrameMove>(false, true);
		cmd.mName = "";
		cmd.mStartPos = pos;
		cmd.mTargetPos = pos;
		cmd.mOnceLength = 0.0f;
		mCommandSystem.pushDelayCommand(cmd, obj, delayTime);
		script.addDelayCmd(cmd);
		return cmd;
	}
	public static CommandWindowKeyFrameMove MOVE_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, Vector3 start, Vector3 target, float onceLength)
	{
		return MOVE_KEYFRAME_WINDOW_DELAY_EX(script, obj, delayTime, CommonDefine.ZERO_ONE, start, target, onceLength, false, 0.0f, null, null);
	}
	public static CommandWindowKeyFrameMove MOVE_WINDOW_DELAY_EX(LayoutScript script, txUIObject obj, float delayTime, Vector3 start, Vector3 target, float onceLength, KeyFrameCallback moveDoneCallback)
	{
		return MOVE_KEYFRAME_WINDOW_DELAY_EX(script, obj, delayTime, CommonDefine.ZERO_ONE, start, target, onceLength, false, 0.0f, null, moveDoneCallback);
	}
	public static CommandWindowKeyFrameMove MOVE_WINDOW_DELAY_EX(LayoutScript script, txUIObject obj, float delayTime, Vector3 start, Vector3 target, float onceLength, KeyFrameCallback movingCallback, KeyFrameCallback moveDoneCallback)
	{
		return MOVE_KEYFRAME_WINDOW_DELAY_EX(script, obj, delayTime, CommonDefine.ZERO_ONE, start, target, onceLength, false, 0.0f, movingCallback, moveDoneCallback);
	}
	public static CommandWindowKeyFrameMove MOVE_KEYFRAME_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, string keyframe, Vector3 startPos, Vector3 targetPos, float onceLength)
	{
		return MOVE_KEYFRAME_WINDOW_DELAY_EX(script, obj, delayTime, keyframe, startPos, targetPos, onceLength, false, 0.0f, null, null);
	}
	public static CommandWindowKeyFrameMove MOVE_KEYFRAME_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, string keyframe, Vector3 startPos, Vector3 targetPos, float onceLength, bool loop)
	{
		return MOVE_KEYFRAME_WINDOW_DELAY_EX(script, obj, delayTime, keyframe, startPos, targetPos, onceLength, loop, 0.0f, null, null);
	}
	public static CommandWindowKeyFrameMove MOVE_KEYFRAME_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, string keyframe, Vector3 startPos, Vector3 targetPos, float onceLength, bool loop, float offset)
	{
		return MOVE_KEYFRAME_WINDOW_DELAY_EX(script, obj, delayTime, keyframe, startPos, targetPos, onceLength, loop, offset, null, null);
	}
	public static CommandWindowKeyFrameMove MOVE_KEYFRAME_WINDOW_DELAY_EX(LayoutScript script, txUIObject obj, float delayTime, string keyframe, Vector3 startPos, Vector3 targetPos, float onceLength, bool loop, float offset, KeyFrameCallback movingCallback, KeyFrameCallback moveDoneCallback)
	{
		if (keyframe == "" || MathUtility.isFloatZero(onceLength))
		{
			UnityUtility.logError("时间或关键帧不能为空,如果要停止组件,请使用CommandWindowKeyFrameMove MOVE_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, Vector3 pos)");
		}
		CommandWindowKeyFrameMove cmd = mCommandSystem.newCmd<CommandWindowKeyFrameMove>(false, true);
		cmd.mName = keyframe;
		cmd.mStartPos = startPos;
		cmd.mTargetPos = targetPos;
		cmd.mOnceLength = onceLength;
		cmd.mOffset = offset;
		cmd.mLoop = loop;
		cmd.setTremblingCallback(movingCallback, null);
		cmd.setTrembleDoneCallback(moveDoneCallback, null);
		mCommandSystem.pushDelayCommand(cmd, obj, delayTime);
		script.addDelayCmd(cmd);
		return cmd;
	}
	//----------------------------------------------------------------------------------------------------------------------------------------------------------
	// 显示
	// 窗口的显示和隐藏()
	public static void ACTIVE_WINDOW(txUIObject obj, bool active = true)
	{
		CommandWindowActive cmd = mCommandSystem.newCmd<CommandWindowActive>(false);
		cmd.mActive = active;
		mCommandSystem.pushCommand(cmd, obj);
	}
	public static CommandWindowActive ACTIVE_WINDOW_DELAY(LayoutScript script, txUIObject obj, bool active, float delayTime)
	{
		return ACTIVE_WINDOW_DELAY_EX(script, obj, active, delayTime, null, null);
	}
	public static CommandWindowActive ACTIVE_WINDOW_DELAY_EX(LayoutScript script, txUIObject obj, bool active, float dealyTime, CommandCallback startCallback, object userData = null)
	{
		CommandWindowActive cmd = mCommandSystem.newCmd<CommandWindowActive>(false, true);
		cmd.mActive = active;
		cmd.addStartCommandCallback(startCallback, userData);
		mCommandSystem.pushDelayCommand(cmd, obj, dealyTime);
		script.addDelayCmd(cmd);
		return cmd;
	}
	//----------------------------------------------------------------------------------------------------------------------------------------------------------
	// 缩放
	public static void SCALE_WINDOW(txUIObject obj, Vector2 scale)
	{
		CommandWindowScaleTrembling cmd = mCommandSystem.newCmd<CommandWindowScaleTrembling>(false);
		cmd.mName = "";
		cmd.mOnceLength = 0.0f;
		cmd.mStartScale = scale;
		cmd.mTargetScale = scale;
		mCommandSystem.pushCommand(cmd, obj);
	}
	public static void SCALE_WINDOW(txUIObject obj, Vector2 start, Vector2 target, float onceLength)
	{
		SCALE_KEYFRAME_WINDOW_EX(obj, CommonDefine.ZERO_ONE, start, target, onceLength, false, 0.0f, null, null);
	}
	public static void SCALE_WINDOW_EX(txUIObject obj, Vector2 start, Vector2 target, float onceLength, KeyFrameCallback doneCallback)
	{
		SCALE_KEYFRAME_WINDOW_EX(obj, CommonDefine.ZERO_ONE, start, target, onceLength, false, 0.0f, null, doneCallback);
	}
	public static void SCALE_WINDOW_EX(txUIObject obj, Vector2 start, Vector2 target, float onceLength, KeyFrameCallback scalingCallback, KeyFrameCallback doneCallback)
	{
		SCALE_KEYFRAME_WINDOW_EX(obj, CommonDefine.ZERO_ONE, start, target, onceLength, false, 0.0f, scalingCallback, doneCallback);
	}
	public static void SCALE_KEYFRAME_WINDOW(txUIObject obj, string fileName, Vector2 start, Vector2 target, float onceLength)
	{
		SCALE_KEYFRAME_WINDOW_EX(obj, fileName, start, target, onceLength, false, 0.0f, null, null);
	}
	public static void SCALE_KEYFRAME_WINDOW(txUIObject obj, string fileName, Vector2 start, Vector2 target, float onceLength, bool loop)
	{
		SCALE_KEYFRAME_WINDOW_EX(obj, fileName, start, target, onceLength, loop, 0.0f, null, null);
	}
	public static void SCALE_KEYFRAME_WINDOW(txUIObject obj, string fileName, Vector2 start, Vector2 target, float onceLength, bool loop, float offset)
	{
		SCALE_KEYFRAME_WINDOW_EX(obj, fileName, start, target, onceLength, loop, offset, null, null);
	}
	public static void SCALE_KEYFRAME_WINDOW_EX(txUIObject obj, string fileName, Vector2 start, Vector2 target, float onceLength, KeyFrameCallback scaleTrembleDoneCallback)
	{
		SCALE_KEYFRAME_WINDOW_EX(obj, fileName, start, target, onceLength, false, 0.0f, null, scaleTrembleDoneCallback);
	}
	public static void SCALE_KEYFRAME_WINDOW_EX(txUIObject obj, string fileName, Vector2 start, Vector2 target, float onceLength, bool loop, KeyFrameCallback scaleTrembleDoneCallback)
	{
		SCALE_KEYFRAME_WINDOW_EX(obj, fileName, start, target, onceLength, loop, 0.0f, null, scaleTrembleDoneCallback);
	}
	public static void SCALE_KEYFRAME_WINDOW_EX(txUIObject obj, string fileName, Vector2 start, Vector2 target, float onceLength, bool loop, float offset, KeyFrameCallback scaleTrembleDoneCallback)
	{
		SCALE_KEYFRAME_WINDOW_EX(obj, fileName, start, target, onceLength, loop, offset, null, scaleTrembleDoneCallback);
	}
	public static void SCALE_KEYFRAME_WINDOW_EX(txUIObject obj, string fileName, Vector2 start, Vector2 target, float onceLength, KeyFrameCallback scaleTremblingCallback, KeyFrameCallback scaleTrembleDoneCallback)
	{
		SCALE_KEYFRAME_WINDOW_EX(obj, fileName, start, target, onceLength, false, 0.0f, scaleTremblingCallback, scaleTrembleDoneCallback);
	}
	public static void SCALE_KEYFRAME_WINDOW_EX(txUIObject obj, string fileName, Vector2 start, Vector2 target, float onceLength, bool loop, KeyFrameCallback scaleTremblingCallback, KeyFrameCallback scaleTrembleDoneCallback)
	{
		SCALE_KEYFRAME_WINDOW_EX(obj, fileName, start, target, onceLength, loop, 0.0f, scaleTremblingCallback, scaleTrembleDoneCallback);
	}
	public static void SCALE_KEYFRAME_WINDOW_EX(txUIObject obj, string fileName, Vector2 start, Vector2 target, float onceLength, bool loop, float offset, KeyFrameCallback scaleTremblingCallback, KeyFrameCallback scaleTrembleDoneCallback)
	{
		if (fileName == "" || MathUtility.isFloatZero(onceLength))
		{
			UnityUtility.logError("时间或关键帧不能为空,如果要停止组件,请使用void SCALE_WINDOW(txUIObject obj, Vector2 scale)");
		}
		CommandWindowScaleTrembling cmd = mCommandSystem.newCmd<CommandWindowScaleTrembling>(false);
		cmd.mName = fileName;
		cmd.mOnceLength = onceLength;
		cmd.mOffset = offset;
		cmd.mLoop = loop;
		cmd.mStartScale = start;
		cmd.mTargetScale = target;
		cmd.setTremblingCallback(scaleTremblingCallback, null);
		cmd.setTrembleDoneCallback(scaleTrembleDoneCallback, null);
		mCommandSystem.pushCommand(cmd, obj);
	}
	public static CommandWindowScaleTrembling SCALE_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, Vector2 scale)
	{
		CommandWindowScaleTrembling cmd = mCommandSystem.newCmd<CommandWindowScaleTrembling>(false, true);
		cmd.mName = "";
		cmd.mOnceLength = 0.0f;
		cmd.mStartScale = scale;
		cmd.mTargetScale = scale;
		mCommandSystem.pushDelayCommand(cmd, obj, delayTime);
		script.addDelayCmd(cmd);
		return cmd;
	}
	public static CommandWindowScaleTrembling SCALE_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, Vector2 start, Vector2 target, float onceLength)
	{
		return SCALE_KEYFRAME_WINDOW_DELAY_EX(script, obj, delayTime, CommonDefine.ZERO_ONE, start, target, onceLength, false, 0.0f, null, null);
	}
	public static CommandWindowScaleTrembling SCALE_WINDOW_DELAY_EX(LayoutScript script, txUIObject obj, float delayTime, Vector2 start, Vector2 target, float onceLength, KeyFrameCallback scalingCallback, KeyFrameCallback doneCallback)
	{
		return SCALE_KEYFRAME_WINDOW_DELAY_EX(script, obj, delayTime, CommonDefine.ZERO_ONE, start, target, onceLength, false, 0.0f, scalingCallback, doneCallback);
	}
	public static CommandWindowScaleTrembling SCALE_KEYFRAME_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, string keyframe, Vector2 start, Vector2 target, float onceLength)
	{
		return SCALE_KEYFRAME_WINDOW_DELAY_EX(script, obj, delayTime, keyframe, start, target, onceLength, false, 0.0f, null, null);
	}
	public static CommandWindowScaleTrembling SCALE_KEYFRAME_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, string keyframe, Vector2 start, Vector2 target, float onceLength, bool loop)
	{
		return SCALE_KEYFRAME_WINDOW_DELAY_EX(script, obj, delayTime, keyframe, start, target, onceLength, loop, 0.0f, null, null);
	}
	public static CommandWindowScaleTrembling SCALE_KEYFRAME_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, string keyframe, Vector2 start, Vector2 target, float onceLength, bool loop, float offset)
	{
		return SCALE_KEYFRAME_WINDOW_DELAY_EX(script, obj, delayTime, keyframe, start, target, onceLength, loop, offset, null, null);
	}
	public static CommandWindowScaleTrembling SCALE_KEYFRAME_WINDOW_DELAY_EX(LayoutScript script, txUIObject obj, float delayTime, string keyframe, Vector2 start, Vector2 target, float onceLength, KeyFrameCallback scaleTremblingCallback, KeyFrameCallback scaleTrembleDoneCallback)
	{
		return SCALE_KEYFRAME_WINDOW_DELAY_EX(script, obj, delayTime, keyframe, start, target, onceLength, false, 0.0f, scaleTremblingCallback, scaleTrembleDoneCallback);
	}
	public static CommandWindowScaleTrembling SCALE_KEYFRAME_WINDOW_DELAY_EX(LayoutScript script, txUIObject obj, float delayTime, string keyframe, Vector2 start, Vector2 target, float onceLength, bool loop, KeyFrameCallback scaleTremblingCallback, KeyFrameCallback scaleTrembleDoneCallback)
	{
		return SCALE_KEYFRAME_WINDOW_DELAY_EX(script, obj, delayTime, keyframe, start, target, onceLength, loop, 0.0f, scaleTremblingCallback, scaleTrembleDoneCallback);
	}
	public static CommandWindowScaleTrembling SCALE_KEYFRAME_WINDOW_DELAY_EX(LayoutScript script, txUIObject obj, float delayTime, string keyframe, Vector2 start, Vector2 target, float onceLength, bool loop, float offset, KeyFrameCallback scaleTremblingCallback, KeyFrameCallback scaleTrembleDoneCallback)
	{
		if (keyframe == "" || MathUtility.isFloatZero(onceLength))
		{
			UnityUtility.logError("时间或关键帧不能为空,如果要停止组件,请使用CommandWindowScaleTrembling SCALE_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, Vector2 scale)");
		}
		CommandWindowScaleTrembling cmd = mCommandSystem.newCmd<CommandWindowScaleTrembling>(false, true);
		cmd.mName = keyframe;
		cmd.mOnceLength = onceLength;
		cmd.mOffset = offset;
		cmd.mLoop = loop;
		cmd.mStartScale = start;
		cmd.mTargetScale = target;
		cmd.setTremblingCallback(scaleTremblingCallback, null);
		cmd.setTrembleDoneCallback(scaleTrembleDoneCallback, null);
		mCommandSystem.pushDelayCommand(cmd, obj, delayTime);
		script.addDelayCmd(cmd);
		return cmd;
	}
	//-----------------------------------------------------------------------------------------------------------------------------------------------------------
	// 进度条
	public static void SMOOTH_WINDOW_SLIDER(txUIObject obj, float start, float target, float time) 
	{
		CommandWindowSmoothSlider cmd = mCommandSystem.newCmd<CommandWindowSmoothSlider>(false);
		cmd.mStartSliderValue = start;
		cmd.mTargetSliderValue = target;
		cmd.mFadeTime = time;
		mCommandSystem.pushCommand(cmd, obj);
	}
	public static void SMOOTH_WINDOW_FILL_AMOUNT(txUIObject obj, float start, float target, float time)
	{
		CommandWindowSmoothFillAmount cmd = mCommandSystem.newCmd<CommandWindowSmoothFillAmount>(false);
		cmd.mStartFillAmount = start;
		cmd.mTargetFillAmount = target;
		cmd.mFadeTime = time;
		mCommandSystem.pushCommand(cmd, obj);
	}
	public static void SMOOTH_WINDOW_FILL_AMOUNT_DELAY(LayoutScript script, txUIObject obj, float delayTime, float start, float target, float time)
	{
		CommandWindowSmoothFillAmount cmd = mCommandSystem.newCmd<CommandWindowSmoothFillAmount>(false, true);
		cmd.mStartFillAmount = start;
		cmd.mTargetFillAmount = target;
		cmd.mFadeTime = time;
		mCommandSystem.pushDelayCommand(cmd, obj, delayTime);
	}
	//----------------------------------------------------------------------------------------------------------------------------------------------------------------------
	// 透明度
	public static void ALPHA_WINDOW(txUIObject obj, float alpha)
	{
		CommandWindowAlphaTremble cmd = mCommandSystem.newCmd<CommandWindowAlphaTremble>(false);
		cmd.mName = "";
		cmd.mOnceLength = 0.0f;
		cmd.mStartAlpha = alpha;
		cmd.mTargetAlpha = alpha;
		mCommandSystem.pushCommand(cmd, obj);
	}
	public static void ALPHA_WINDOW(txUIObject obj, float start, float target, float onceLength)
	{
		ALPHA_KEYFRAME_WINDOW_EX(obj, CommonDefine.ZERO_ONE, start, target, onceLength, false, 0.0f, null, null);
	}
	public static void ALPHA_WINDOW_EX(txUIObject obj, float start, float target, float onceLength, KeyFrameCallback trembleDoneCallback)
	{
		ALPHA_KEYFRAME_WINDOW_EX(obj, CommonDefine.ZERO_ONE, start, target, onceLength, false, 0.0f, null, trembleDoneCallback);
	}
	public static void ALPHA_WINDOW_EX(txUIObject obj, float start, float target, float onceLength, KeyFrameCallback tremblingCallback, KeyFrameCallback trembleDoneCallback)
	{
		ALPHA_KEYFRAME_WINDOW_EX(obj, CommonDefine.ZERO_ONE, start, target, onceLength, false, 0.0f, tremblingCallback, trembleDoneCallback);
	}
	public static void ALPHA_KEYFRAME_WINDOW(txUIObject obj, string name, float start, float target, float onceLength)
	{
		ALPHA_KEYFRAME_WINDOW_EX(obj, name, start, target, onceLength, false, 0.0f, null, null);
	}
	public static void ALPHA_KEYFRAME_WINDOW(txUIObject obj, string name, float start, float target, float onceLength, bool loop)
	{
		ALPHA_KEYFRAME_WINDOW_EX(obj, name, start, target, onceLength, loop, 0.0f, null, null);
	}
	public static void ALPHA_KEYFRAME_WINDOW(txUIObject obj, string name, float start, float target, float onceLength, bool loop, float offset)
	{
		ALPHA_KEYFRAME_WINDOW_EX(obj, name, start, target, onceLength, loop, offset, null, null);
	}
	public static void ALPHA_KEYFRAME_WINDOW_EX(txUIObject obj, string name, float start, float target, float onceLength, KeyFrameCallback trembleDoneCallback)
	{
		ALPHA_KEYFRAME_WINDOW_EX(obj, name, start, target, onceLength, false, 0.0f, null, trembleDoneCallback);
	}
	public static void ALPHA_KEYFRAME_WINDOW_EX(txUIObject obj, string name, float start, float target, float onceLength, KeyFrameCallback tremblingCallback, KeyFrameCallback trembleDoneCallback)
	{
		ALPHA_KEYFRAME_WINDOW_EX(obj, name, start, target, onceLength, false, 0.0f, tremblingCallback, trembleDoneCallback);
	}
	public static void ALPHA_KEYFRAME_WINDOW_EX(txUIObject obj, string name, float start, float target, float onceLength, bool loop, float offset, KeyFrameCallback tremblingCallback, KeyFrameCallback trembleDoneCallback)
	{
		if (name == "" || MathUtility.isFloatZero(onceLength))
		{
			UnityUtility.logError("时间或关键帧不能为空,如果要停止组件,请使用void ALPHA_WINDOW(txUIObject obj, float alpha)");
		}
		CommandWindowAlphaTremble cmd = mCommandSystem.newCmd<CommandWindowAlphaTremble>(false);
		cmd.mName = name;
		cmd.mLoop = loop;
		cmd.mOnceLength = onceLength;
		cmd.mOffset = offset;
		cmd.mStartAlpha = start;
		cmd.mTargetAlpha = target;
		cmd.setTremblingCallback(tremblingCallback, null);
		cmd.setTrembleDoneCallback(trembleDoneCallback, null);
		mCommandSystem.pushCommand(cmd, obj);
	}
	public static CommandWindowAlphaTremble ALPHA_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, float alpha)
	{
		CommandWindowAlphaTremble cmd = mCommandSystem.newCmd<CommandWindowAlphaTremble>(false, true);
		cmd.mName = "";
		cmd.mOnceLength = 0.0f;
		cmd.mStartAlpha = alpha;
		cmd.mTargetAlpha = alpha;
		mCommandSystem.pushDelayCommand(cmd, obj, delayTime);
		script.addDelayCmd(cmd);
		return cmd;
	}
	public static CommandWindowAlphaTremble ALPHA_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, float start, float target, float onceLength)
	{
		return ALPHA_KEYFRAME_WINDOW_DELAY_EX(script, obj, delayTime, CommonDefine.ZERO_ONE, start, target, onceLength, false, 0.0f, null, null);
	}
	public static CommandWindowAlphaTremble ALPHA_WINDOW_DELAY_EX(LayoutScript script, txUIObject obj, float delayTime, float start, float target, float onceLength, KeyFrameCallback trembleDoneCallback)
	{
		return ALPHA_KEYFRAME_WINDOW_DELAY_EX(script, obj, delayTime, CommonDefine.ZERO_ONE, start, target, onceLength, false, 0.0f, null, trembleDoneCallback);
	}
	public static CommandWindowAlphaTremble ALPHA_WINDOW_DELAY_EX(LayoutScript script, txUIObject obj, float delayTime, float start, float target, float onceLength, KeyFrameCallback tremblingCallback, KeyFrameCallback trembleDoneCallback)
	{
		return ALPHA_KEYFRAME_WINDOW_DELAY_EX(script, obj, delayTime, CommonDefine.ZERO_ONE, start, target, onceLength, false, 0.0f, tremblingCallback, trembleDoneCallback);
	}
	public static CommandWindowAlphaTremble ALPHA_KEYFRAME_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, string keyframe, float start, float target, float onceLength)
	{
		return ALPHA_KEYFRAME_WINDOW_DELAY_EX(script, obj, delayTime, keyframe, start, target, onceLength, false, 0.0f, null, null);
	}
	public static CommandWindowAlphaTremble ALPHA_KEYFRAME_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, string keyframe, float start, float target, float onceLength, bool loop)
	{
		return ALPHA_KEYFRAME_WINDOW_DELAY_EX(script, obj, delayTime, keyframe, start, target, onceLength, loop, 0.0f, null, null);
	}
	public static CommandWindowAlphaTremble ALPHA_KEYFRAME_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, string keyframe, float start, float target, float onceLength, bool loop, float offset)
	{
		return ALPHA_KEYFRAME_WINDOW_DELAY_EX(script, obj, delayTime, keyframe, start, target, onceLength, loop, offset, null, null);
	}
	public static CommandWindowAlphaTremble ALPHA_KEYFRAME_WINDOW_DELAY_EX(LayoutScript script, txUIObject obj, float delayTime, string keyframe, float start, float target, float onceLength, KeyFrameCallback tremblingCallback, KeyFrameCallback trembleDoneCallback)
	{
		return ALPHA_KEYFRAME_WINDOW_DELAY_EX(script, obj, delayTime, keyframe, start, target, onceLength, false, 0.0f, tremblingCallback, trembleDoneCallback);
	}
	public static CommandWindowAlphaTremble ALPHA_KEYFRAME_WINDOW_DELAY_EX(LayoutScript script, txUIObject obj, float delayTime, string keyframe, float start, float target, float onceLength, bool loop, float offset, KeyFrameCallback tremblingCallback, KeyFrameCallback trembleDoneCallback)
	{
		if (keyframe == "" || MathUtility.isFloatZero(onceLength))
		{
			UnityUtility.logError("时间或关键帧不能为空,如果要停止组件,请使用CommandWindowAlphaTremble ALPHA_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, float alpha)");
		}
		CommandWindowAlphaTremble cmd = mCommandSystem.newCmd<CommandWindowAlphaTremble>(false, true);
		cmd.mName = keyframe;
		cmd.mLoop = loop;
		cmd.mOnceLength = onceLength;
		cmd.mOffset = offset;
		cmd.mStartAlpha = start;
		cmd.mTargetAlpha = target;
		cmd.setTremblingCallback(tremblingCallback, null);
		cmd.setTrembleDoneCallback(trembleDoneCallback, null);
		mCommandSystem.pushDelayCommand(cmd, obj, delayTime);
		script.addDelayCmd(cmd);
		return cmd;
	}
	//--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	// HSL
	public static void HSL_WINDOW(txUIObject obj, Vector3 hsl)
	{
		CommandWindowHSLTremble cmd = mCommandSystem.newCmd<CommandWindowHSLTremble>(false, false);
		cmd.mName = "";
		cmd.mOnceLength = 0.0f;
		cmd.mStartHSL = hsl;
		cmd.mTargetHSL = hsl;
		mCommandSystem.pushCommand(cmd, obj);
	}
	public static void HSL_WINDOW(txUIObject obj, Vector3 start, Vector3 target, float onceLength)
	{
		HSL_KEYFRAME_WINDOW(obj, CommonDefine.ZERO_ONE, start, target, onceLength, false, 0.0f);
	}
	public static void HSL_KEYFRAME_WINDOW(txUIObject obj, string keyframe, Vector3 start, Vector3 target, float onceLength)
	{
		HSL_KEYFRAME_WINDOW(obj, keyframe, start, target, onceLength, false, 0.0f);
	}
	public static void HSL_KEYFRAME_WINDOW(txUIObject obj, string keyframe, Vector3 start, Vector3 target, float onceLength, bool loop, float offset)
	{
		if (keyframe == "" || MathUtility.isFloatZero(onceLength))
		{
			UnityUtility.logError("时间或关键帧不能为空,如果要停止组件,请使用void HSL_WINDOW(txUIObject obj, Vector3 hsl)");
		}
		CommandWindowHSLTremble cmd = mCommandSystem.newCmd<CommandWindowHSLTremble>(false, false);
		cmd.mName = keyframe;
		cmd.mLoop = loop;
		cmd.mOnceLength = onceLength;
		cmd.mOffset = offset;
		cmd.mStartHSL = start;
		cmd.mTargetHSL = target;
		mCommandSystem.pushCommand(cmd, obj);
	}
	//--------------------------------------------------------------------------------------------------------------------------------------------------------
	// 音效
	public static void PLAY_AUDIO_WINDOW(txUIObject obj, SOUND_DEFINE sound, bool loop, float volume)
	{
		CommandWindowPlayAudio cmd = mCommandSystem.newCmd<CommandWindowPlayAudio>(false);
		cmd.mSound = sound;
		cmd.mLoop = loop;
		cmd.mVolume = volume;
		mCommandSystem.pushCommand(cmd, obj);
	}
	public static CommandWindowPlayAudio PLAY_AUDIO_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, SOUND_DEFINE sound, bool loop, float volume)
	{
		CommandWindowPlayAudio cmd = mCommandSystem.newCmd<CommandWindowPlayAudio>(false, true);
		cmd.mSound = sound;
		cmd.mLoop = loop;
		cmd.mVolume = volume;
		mCommandSystem.pushDelayCommand(cmd, obj, delayTime);
		script.addDelayCmd(cmd);
		return cmd;
	}
}