using UnityEngine;
using System.Collections;
using System;

public class LayoutTools : GameBase
{
	public static bool checkStaticPanel(txUIObject obj)
	{
		txUIObject panel = obj.mLayout.getLayoutPanel();
		if (panel is txNGUIPanel && (panel as txNGUIPanel).getStatic())
		{
			logError("layout is static! can not move/rotate/scale window! layout : " + obj.mLayout.getName());
			return false;
		}
		return true;
	}
	// 布局
	//------------------------------------------------------------------------------------------------------------------------------------
	public static void LOAD_NGUI(LAYOUT_TYPE type, int renderOrder, bool visible, bool immediately, string param)
	{
		CommandLayoutManagerLoadLayout cmd = newCmd(out cmd, true, false);
		cmd.mLayoutType = type;
		cmd.mVisible = visible;
		cmd.mRenderOrder = renderOrder;
		cmd.mAsync = false;
		cmd.mImmediatelyShow = immediately;
		cmd.mParam = param;
		cmd.mIsNGUI = true;
		pushCommand(cmd, mLayoutManager);
	}
	public static void LOAD_NGUI_ASYNC(LAYOUT_TYPE type, int renderOrder, LayoutAsyncDone callback)
	{
		CommandLayoutManagerLoadLayout cmd = newCmd(out cmd, true, false);
		cmd.mLayoutType = type;
		cmd.mRenderOrder = renderOrder;
		cmd.mAsync = true;
		cmd.mCallback = callback;
		cmd.mIsNGUI = true;
		pushCommand(cmd, mLayoutManager);
	}
	public static void LOAD_UGUI(LAYOUT_TYPE type, int renderOrder, bool visible, bool immediately, string param)
	{
		CommandLayoutManagerLoadLayout cmd = newCmd(out cmd, true, false);
		cmd.mLayoutType = type;
		cmd.mVisible = visible;
		cmd.mRenderOrder = renderOrder;
		cmd.mAsync = false;
		cmd.mImmediatelyShow = immediately;
		cmd.mParam = param;
		cmd.mIsNGUI = false;
		pushCommand(cmd, mLayoutManager);
	}
	public static void LOAD_UGUI_ASYNC(LAYOUT_TYPE type, int renderOrder, LayoutAsyncDone callback)
	{
		CommandLayoutManagerLoadLayout cmd = newCmd(out cmd, true, false);
		cmd.mLayoutType = type;
		cmd.mRenderOrder = renderOrder;
		cmd.mAsync = true;
		cmd.mCallback = callback;
		cmd.mIsNGUI = false;
		pushCommand(cmd, mLayoutManager);
	}
	public static void UNLOAD_LAYOUT(LAYOUT_TYPE type)
	{
		// 需要首先强制隐藏布局
		HIDE_LAYOUT_FORCE(type);
		CommandLayoutManagerUnloadLayout cmd = newCmd(out cmd, true, false);
		cmd.mLayoutType = type;
		pushCommand(cmd, mLayoutManager);
	}
	public static void UNLOAD_LAYOUT_DELAY(LAYOUT_TYPE type, float delayTime)
	{
		CommandLayoutManagerUnloadLayout cmd = newCmd(out cmd, true, true);
		cmd.mLayoutType = type;
		pushDelayCommand(cmd, mLayoutManager, delayTime);
	}
	public static void LOAD_NGUI_HIDE(LAYOUT_TYPE type, int renderOrder)
	{
		LOAD_NGUI(type, renderOrder, false, false, "");
	}
	public static void LOAD_NGUI_SHOW(LAYOUT_TYPE type, int renderOrder)
	{
		LOAD_NGUI(type, renderOrder, true, false, "");
	}
	public static void LOAD_NGUI_SHOW(LAYOUT_TYPE type, int renderOrder, bool immediately, string param = "")
	{
		LOAD_NGUI(type, renderOrder, true, immediately, param);
	}
	public static void LOAD_UGUI_HIDE(LAYOUT_TYPE type, int renderOrder)
	{
		LOAD_UGUI(type, renderOrder, false, false, "");
	}
	public static void LOAD_UGUI_SHOW(LAYOUT_TYPE type, int renderOrder)
	{
		LOAD_UGUI(type, renderOrder, true, false, "");
	}
	public static void LOAD_UGUI_SHOW(LAYOUT_TYPE type, int renderOrder, bool immediately, string param = "")
	{
		LOAD_UGUI(type, renderOrder, true, immediately, param);
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
		CommandLayoutManagerLayoutVisible cmd = newCmd(out cmd, true, false);
		cmd.mLayoutType = type;
		cmd.mForce = false;
		cmd.mVisibility = visible;
		cmd.mImmediately = immediately;
		cmd.mParam = param;
		pushCommand(cmd, mLayoutManager);
	}
	public static void VISIBLE_LAYOUT_FORCE(LAYOUT_TYPE type, bool visible)
	{
		CommandLayoutManagerLayoutVisible cmd = newCmd(out cmd, true, false);
		cmd.mLayoutType = type;
		cmd.mForce = true;
		cmd.mVisibility = visible;
		cmd.mImmediately = false;
		pushCommand(cmd, mLayoutManager);
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
		CommandLayoutManagerLayoutVisible cmd = newCmd(out cmd, true, true);
		cmd.mLayoutType = type;
		cmd.mForce = false;
		cmd.mVisibility = visible;
		cmd.mImmediately = immediately;
		cmd.mParam = param;
		pushDelayCommand(cmd, mLayoutManager, delayTime);
		if(procedure != null)
		{
			procedure.addDelayCmd(cmd);
		}
		return cmd;
	}
	public static CommandLayoutManagerLayoutVisible VISIBLE_LAYOUT_DELAY_EX(SceneProcedure procedure, float delayTime, LAYOUT_TYPE type, bool visible, CommandCallback start, bool immediately = false, string param = "")
	{
		CommandLayoutManagerLayoutVisible cmd = newCmd(out cmd, true, true);
		cmd.mLayoutType = type;
		cmd.mForce = false;
		cmd.mVisibility = visible;
		cmd.mImmediately = immediately;
		cmd.mParam = param;
		cmd.addStartCommandCallback(start);
		pushDelayCommand(cmd, mLayoutManager, delayTime);
		if (procedure != null)
		{
			procedure.addDelayCmd(cmd);
		}
		return cmd;
	}
	public static CommandLayoutManagerLayoutVisible VISIBLE_LAYOUT_DELAY_FORCE(float delayTime, LAYOUT_TYPE type, bool visible)
	{
		CommandLayoutManagerLayoutVisible cmd = newCmd(out cmd, true, true);
		cmd.mLayoutType = type;
		cmd.mForce = true;
		cmd.mVisibility = visible;
		cmd.mImmediately = false;
		pushDelayCommand(cmd, mLayoutManager, delayTime);
		return cmd;
	}
	//--------------------------------------------------------------------------------------------------------------------------------------------
	// 旋转
	public static void ROTATE_LOCK_WINDOW(txUIObject obj, bool lockRotation = true)
	{
		if (!checkStaticPanel(obj))
		{
			return;
		}
		CommandWindowRotateFixed cmd = newCmd(out cmd, false);
		cmd.mActive = lockRotation;
		pushCommand(cmd, obj);
	}
	public static void ROTATE_WINDOW(txUIObject obj)
	{
		ROTATE_WINDOW(obj, Vector3.zero);
	}
	public static void ROTATE_WINDOW(txUIObject obj, Vector3 rotation)
	{
		if (!checkStaticPanel(obj))
		{
			return;
		}
		CommandWindowRotate cmd = newCmd(out cmd, false, false);
		cmd.mName = "";
		cmd.mOnceLength = 0.0f;
		cmd.mStartRotation = rotation;
		cmd.mTargetRotation = rotation;
		pushCommand(cmd, obj);
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
		if (!checkStaticPanel(obj))
		{
			return;
		}
		if (keyframe == "" || MathUtility.isFloatZero(onceLength))
		{
			logError("时间或关键帧不能为空,如果要停止组件,请使用void ROTATE_WINDOW(txUIObject obj, Vector3 rotation)");
		}
		CommandWindowRotate cmd = newCmd(out cmd, false, false);
		cmd.mName = keyframe;
		cmd.mOnceLength = onceLength;
		cmd.mStartRotation = start;
		cmd.mTargetRotation = target;
		cmd.mOffset = offset;
		cmd.mLoop = loop;
		cmd.setTremblingCallback(rotatingCallback, null);
		cmd.setTrembleDoneCallback(doneCallback, null);
		pushCommand(cmd, obj);
	}
	public static CommandWindowRotate ROTATE_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, Vector3 rotation)
	{
		if (!checkStaticPanel(obj))
		{
			return null;
		}
		CommandWindowRotate cmd = newCmd(out cmd, false, true);
		cmd.mName = "";
		cmd.mOnceLength = 0.0f;
		cmd.mStartRotation = rotation;
		cmd.mTargetRotation = rotation;
		pushDelayCommand(cmd, obj, delayTime);
		script.addDelayCmd(cmd);
		return cmd;
	}
	public static CommandWindowRotate ROTATE_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, Vector3 start, Vector3 target, float time)
	{
		return ROTATE_KEYFRMAE_WINDOW_DELAY(script, obj, delayTime, CommonDefine.ZERO_ONE, start, target, time, false, 0.0f);
	}
	public static CommandWindowRotate ROTATE_KEYFRMAE_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, string keyframe, Vector3 start, Vector3 target, float onceLength)
	{
		return ROTATE_KEYFRMAE_WINDOW_DELAY(script, obj, delayTime, keyframe, start, target, onceLength, false, 0.0f);
	}
	public static CommandWindowRotate ROTATE_KEYFRMAE_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, string keyframe, Vector3 start, Vector3 target, float onceLength, bool loop)
	{
		return ROTATE_KEYFRMAE_WINDOW_DELAY(script, obj, delayTime, keyframe, start, target, onceLength, loop, 0.0f);
	}
	public static CommandWindowRotate ROTATE_KEYFRMAE_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, string keyframe, Vector3 start, Vector3 target, float onceLength, bool loop, float offset)
	{
		if (!checkStaticPanel(obj))
		{
			return null;
		}
		if (keyframe == "" || MathUtility.isFloatZero(onceLength))
		{
			logError("时间或关键帧不能为空,如果要停止组件,请使用CommandWindowKeyFrameRotate ROTATE_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, Vector3 rotation)");
		}
		CommandWindowRotate cmd = newCmd(out cmd, false, true);
		cmd.mName = keyframe;
		cmd.mOnceLength = onceLength;
		cmd.mStartRotation = start;
		cmd.mTargetRotation = target;
		cmd.mOffset = offset;
		cmd.mLoop = loop;
		pushDelayCommand(cmd, obj, delayTime);
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
		if (!checkStaticPanel(obj))
		{
			return;
		}
		CommandWindowRotateSpeed cmd = newCmd(out cmd, false);
		cmd.mRotateSpeed = speed;
		cmd.mStartAngle = startAngle;
		cmd.mRotateAcceleration = rotateAccelerationValue;
		pushCommand(cmd, obj);
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
		if (!checkStaticPanel(obj))
		{
			return null;
		}
		CommandWindowRotateSpeed cmd = newCmd(out cmd, false, true);
		cmd.mRotateSpeed = speed;
		cmd.mStartAngle = startAngle;
		cmd.mRotateAcceleration = rotateAccelerationValue;
		pushDelayCommand(cmd, obj, delayTime);
		script.addDelayCmd(cmd);
		return cmd;
	}
	//-------------------------------------------------------------------------------------------------------------------------------------------------------
	// 移动
	// 用关键帧移动窗口
	public static void MOVE_WINDOW(txUIObject obj)
	{
		MOVE_WINDOW(obj, Vector3.zero);
	}
	public static void MOVE_WINDOW(txUIObject obj, Vector3 pos)
	{
		if(!checkStaticPanel(obj))
		{
			return;
		}
		CommandWindowMove cmd = newCmd(out cmd, false);
		cmd.mName = "";
		cmd.mOnceLength = 0.0f;
		cmd.mStartPos = pos;
		cmd.mTargetPos = pos;
		pushCommand(cmd, obj);
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
		if (!checkStaticPanel(obj))
		{
			return;
		}
		if (fileName == "" || MathUtility.isFloatZero(onceLength))
		{
			logError("时间或关键帧不能为空,如果要停止组件,请使用void MOVE_WINDOW(txUIObject obj, Vector3 pos)");
		}
		CommandWindowMove cmd = newCmd(out cmd, false);
		cmd.mName = fileName;
		cmd.mOnceLength = onceLength;
		cmd.mStartPos = startPos;
		cmd.mTargetPos = targetPos;
		cmd.mOffset = offset;
		cmd.mLoop = loop;
		cmd.setTremblingCallback(TremblingCallBack, null);
		cmd.setTrembleDoneCallback(TrembleDoneCallBack, null);
		pushCommand(cmd, obj);
	}
	public static CommandWindowMove MOVE_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, Vector3 pos)
	{
		if (!checkStaticPanel(obj))
		{
			return null;
		}
		CommandWindowMove cmd = newCmd(out cmd, false, true);
		cmd.mName = "";
		cmd.mStartPos = pos;
		cmd.mTargetPos = pos;
		cmd.mOnceLength = 0.0f;
		pushDelayCommand(cmd, obj, delayTime);
		script.addDelayCmd(cmd);
		return cmd;
	}
	public static CommandWindowMove MOVE_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, Vector3 start, Vector3 target, float onceLength)
	{
		return MOVE_KEYFRAME_WINDOW_DELAY_EX(script, obj, delayTime, CommonDefine.ZERO_ONE, start, target, onceLength, false, 0.0f, null, null);
	}
	public static CommandWindowMove MOVE_WINDOW_DELAY_EX(LayoutScript script, txUIObject obj, float delayTime, Vector3 start, Vector3 target, float onceLength, KeyFrameCallback moveDoneCallback)
	{
		return MOVE_KEYFRAME_WINDOW_DELAY_EX(script, obj, delayTime, CommonDefine.ZERO_ONE, start, target, onceLength, false, 0.0f, null, moveDoneCallback);
	}
	public static CommandWindowMove MOVE_WINDOW_DELAY_EX(LayoutScript script, txUIObject obj, float delayTime, Vector3 start, Vector3 target, float onceLength, KeyFrameCallback movingCallback, KeyFrameCallback moveDoneCallback)
	{
		return MOVE_KEYFRAME_WINDOW_DELAY_EX(script, obj, delayTime, CommonDefine.ZERO_ONE, start, target, onceLength, false, 0.0f, movingCallback, moveDoneCallback);
	}
	public static CommandWindowMove MOVE_KEYFRAME_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, string keyframe, Vector3 startPos, Vector3 targetPos, float onceLength)
	{
		return MOVE_KEYFRAME_WINDOW_DELAY_EX(script, obj, delayTime, keyframe, startPos, targetPos, onceLength, false, 0.0f, null, null);
	}
	public static CommandWindowMove MOVE_KEYFRAME_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, string keyframe, Vector3 startPos, Vector3 targetPos, float onceLength, bool loop)
	{
		return MOVE_KEYFRAME_WINDOW_DELAY_EX(script, obj, delayTime, keyframe, startPos, targetPos, onceLength, loop, 0.0f, null, null);
	}
	public static CommandWindowMove MOVE_KEYFRAME_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, string keyframe, Vector3 startPos, Vector3 targetPos, float onceLength, bool loop, float offset)
	{
		return MOVE_KEYFRAME_WINDOW_DELAY_EX(script, obj, delayTime, keyframe, startPos, targetPos, onceLength, loop, offset, null, null);
	}
	public static CommandWindowMove MOVE_KEYFRAME_WINDOW_DELAY_EX(LayoutScript script, txUIObject obj, float delayTime, string keyframe, Vector3 startPos, Vector3 targetPos, float onceLength, bool loop, float offset, KeyFrameCallback movingCallback, KeyFrameCallback moveDoneCallback)
	{
		if (!checkStaticPanel(obj))
		{
			return null;
		}
		if (keyframe == "" || MathUtility.isFloatZero(onceLength))
		{
			logError("时间或关键帧不能为空,如果要停止组件,请使用CommandWindowKeyFrameMove MOVE_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, Vector3 pos)");
		}
		CommandWindowMove cmd = newCmd(out cmd, false, true);
		cmd.mName = keyframe;
		cmd.mStartPos = startPos;
		cmd.mTargetPos = targetPos;
		cmd.mOnceLength = onceLength;
		cmd.mOffset = offset;
		cmd.mLoop = loop;
		cmd.setTremblingCallback(movingCallback, null);
		cmd.setTrembleDoneCallback(moveDoneCallback, null);
		pushDelayCommand(cmd, obj, delayTime);
		script.addDelayCmd(cmd);
		return cmd;
	}
	//------------------------------------------------------------------------------------------------------------------
	public static void TRACK_TARGET(txUIObject obj, float speed, txUIObject target, TrackDoneCallback doneCallback, CheckPosition checkPosition)
	{
		if (!checkStaticPanel(obj))
		{
			return;
		}
		CommandWindowTrackTarget cmd = newCmd(out cmd, false);
		cmd.mObject = target;
		cmd.mSpeed = speed;
		cmd.mDoneCallback = doneCallback;
		cmd.mCheckPosition = checkPosition;
		pushCommand(cmd, obj);
	}
	//----------------------------------------------------------------------------------------------------------------------------------------------------------
	// 显示
	// 窗口的显示和隐藏()
	public static void ACTIVE_WINDOW(txUIObject obj, bool active = true)
	{
		CommandWindowActive cmd = newCmd(out cmd, false);
		cmd.mActive = active;
		pushCommand(cmd, obj);
	}
	public static CommandWindowActive ACTIVE_WINDOW_DELAY(LayoutScript script, txUIObject obj, bool active, float delayTime)
	{
		return ACTIVE_WINDOW_DELAY_EX(script, obj, active, delayTime, null, null);
	}
	public static CommandWindowActive ACTIVE_WINDOW_DELAY_EX(LayoutScript script, txUIObject obj, bool active, float dealyTime, CommandCallback startCallback, object userData = null)
	{
		CommandWindowActive cmd = newCmd(out cmd, false, true);
		cmd.mActive = active;
		cmd.addStartCommandCallback(startCallback, userData);
		pushDelayCommand(cmd, obj, dealyTime);
		script.addDelayCmd(cmd);
		return cmd;
	}
	//----------------------------------------------------------------------------------------------------------------------------------------------------------
	// 缩放
	public static void SCALE_WINDOW(txUIObject obj)
	{
		SCALE_WINDOW(obj, Vector2.one);
	}
	public static void SCALE_WINDOW(txUIObject obj, Vector2 scale)
	{
		if (!checkStaticPanel(obj))
		{
			return;
		}
		CommandWindowScale cmd = newCmd(out cmd, false);
		cmd.mName = "";
		cmd.mOnceLength = 0.0f;
		cmd.mStartScale = scale;
		cmd.mTargetScale = scale;
		pushCommand(cmd, obj);
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
		if (!checkStaticPanel(obj))
		{
			return;
		}
		if (fileName == "" || MathUtility.isFloatZero(onceLength))
		{
			logError("时间或关键帧不能为空,如果要停止组件,请使用void SCALE_WINDOW(txUIObject obj, Vector2 scale)");
		}
		CommandWindowScale cmd = newCmd(out cmd, false);
		cmd.mName = fileName;
		cmd.mOnceLength = onceLength;
		cmd.mOffset = offset;
		cmd.mLoop = loop;
		cmd.mStartScale = start;
		cmd.mTargetScale = target;
		cmd.setTremblingCallback(scaleTremblingCallback, null);
		cmd.setTrembleDoneCallback(scaleTrembleDoneCallback, null);
		pushCommand(cmd, obj);
	}
	public static CommandWindowScale SCALE_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, Vector2 scale)
	{
		if (!checkStaticPanel(obj))
		{
			return null;
		}
		CommandWindowScale cmd = newCmd(out cmd, false, true);
		cmd.mName = "";
		cmd.mOnceLength = 0.0f;
		cmd.mStartScale = scale;
		cmd.mTargetScale = scale;
		pushDelayCommand(cmd, obj, delayTime);
		script.addDelayCmd(cmd);
		return cmd;
	}
	public static CommandWindowScale SCALE_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, Vector2 start, Vector2 target, float onceLength)
	{
		return SCALE_KEYFRAME_WINDOW_DELAY_EX(script, obj, delayTime, CommonDefine.ZERO_ONE, start, target, onceLength, false, 0.0f, null, null);
	}
	public static CommandWindowScale SCALE_WINDOW_DELAY_EX(LayoutScript script, txUIObject obj, float delayTime, Vector2 start, Vector2 target, float onceLength, KeyFrameCallback scalingCallback, KeyFrameCallback doneCallback)
	{
		return SCALE_KEYFRAME_WINDOW_DELAY_EX(script, obj, delayTime, CommonDefine.ZERO_ONE, start, target, onceLength, false, 0.0f, scalingCallback, doneCallback);
	}
	public static CommandWindowScale SCALE_KEYFRAME_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, string keyframe, Vector2 start, Vector2 target, float onceLength)
	{
		return SCALE_KEYFRAME_WINDOW_DELAY_EX(script, obj, delayTime, keyframe, start, target, onceLength, false, 0.0f, null, null);
	}
	public static CommandWindowScale SCALE_KEYFRAME_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, string keyframe, Vector2 start, Vector2 target, float onceLength, bool loop)
	{
		return SCALE_KEYFRAME_WINDOW_DELAY_EX(script, obj, delayTime, keyframe, start, target, onceLength, loop, 0.0f, null, null);
	}
	public static CommandWindowScale SCALE_KEYFRAME_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, string keyframe, Vector2 start, Vector2 target, float onceLength, bool loop, float offset)
	{
		return SCALE_KEYFRAME_WINDOW_DELAY_EX(script, obj, delayTime, keyframe, start, target, onceLength, loop, offset, null, null);
	}
	public static CommandWindowScale SCALE_KEYFRAME_WINDOW_DELAY_EX(LayoutScript script, txUIObject obj, float delayTime, string keyframe, Vector2 start, Vector2 target, float onceLength, KeyFrameCallback scaleTremblingCallback, KeyFrameCallback scaleTrembleDoneCallback)
	{
		return SCALE_KEYFRAME_WINDOW_DELAY_EX(script, obj, delayTime, keyframe, start, target, onceLength, false, 0.0f, scaleTremblingCallback, scaleTrembleDoneCallback);
	}
	public static CommandWindowScale SCALE_KEYFRAME_WINDOW_DELAY_EX(LayoutScript script, txUIObject obj, float delayTime, string keyframe, Vector2 start, Vector2 target, float onceLength, bool loop, KeyFrameCallback scaleTremblingCallback, KeyFrameCallback scaleTrembleDoneCallback)
	{
		return SCALE_KEYFRAME_WINDOW_DELAY_EX(script, obj, delayTime, keyframe, start, target, onceLength, loop, 0.0f, scaleTremblingCallback, scaleTrembleDoneCallback);
	}
	public static CommandWindowScale SCALE_KEYFRAME_WINDOW_DELAY_EX(LayoutScript script, txUIObject obj, float delayTime, string keyframe, Vector2 start, Vector2 target, float onceLength, bool loop, float offset, KeyFrameCallback scaleTremblingCallback, KeyFrameCallback scaleTrembleDoneCallback)
	{
		if (!checkStaticPanel(obj))
		{
			return null;
		}
		if (keyframe == "" || MathUtility.isFloatZero(onceLength))
		{
			logError("时间或关键帧不能为空,如果要停止组件,CommandWindowScale SCALE_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, Vector2 scale)");
		}
		CommandWindowScale cmd = newCmd(out cmd, false, true);
		cmd.mName = keyframe;
		cmd.mOnceLength = onceLength;
		cmd.mOffset = offset;
		cmd.mLoop = loop;
		cmd.mStartScale = start;
		cmd.mTargetScale = target;
		cmd.setTremblingCallback(scaleTremblingCallback, null);
		cmd.setTrembleDoneCallback(scaleTrembleDoneCallback, null);
		pushDelayCommand(cmd, obj, delayTime);
		script.addDelayCmd(cmd);
		return cmd;
	}
	//-----------------------------------------------------------------------------------------------------------------------------------------------------------
	public static void SLIDER_WINDOW(txUIObject obj, float value)
	{
		CommandWindowSlider cmd = newCmd(out cmd, false);
		cmd.mStartValue = value;
		cmd.mTargetValue = value;
		cmd.mOnceLength = 0.0f;
		cmd.mTremblingName = "";
		pushCommand(cmd, obj);
	}
	// 进度条
	public static void SLIDER_WINDOW(txUIObject obj, float start, float target, float time) 
	{
		CommandWindowSlider cmd = newCmd(out cmd, false);
		cmd.mStartValue = start;
		cmd.mTargetValue = target;
		cmd.mOnceLength = time;
		cmd.mTremblingName = CommonDefine.ZERO_ONE;
		pushCommand(cmd, obj);
	}
	public static void FILL_WINDOW(txUIObject obj, float value)
	{
		CommandWindowFill cmd = newCmd(out cmd, false);
		cmd.mStartValue = value;
		cmd.mTargetValue = value;
		cmd.mOnceLength = 0.0f;
		cmd.mTremblingName = "";
		pushCommand(cmd, obj);
	}
	public static void FILL_WINDOW(txUIObject obj, float start, float target, float time)
	{
		CommandWindowFill cmd = newCmd(out cmd, false);
		cmd.mStartValue = start;
		cmd.mTargetValue = target;
		cmd.mOnceLength = time;
		cmd.mTremblingName = CommonDefine.ZERO_ONE;
		pushCommand(cmd, obj);
	}
	public static CommandWindowFill FILL_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, float start, float target, float time)
	{
		CommandWindowFill cmd = newCmd(out cmd, false, true);
		cmd.mStartValue = start;
		cmd.mTargetValue = target;
		cmd.mOnceLength = time;
		cmd.mTremblingName = CommonDefine.ZERO_ONE;
		pushDelayCommand(cmd, obj, delayTime);
		script.addDelayCmd(cmd);
		return cmd;
	}
	public static CommandWindowFill FILL_WINDOW_DELAY_EX(LayoutScript script, txUIObject obj, float delayTime, float start, float target, float time, KeyFrameCallback doneCallback)
	{
		CommandWindowFill cmd = newCmd(out cmd, false, true);
		cmd.mStartValue = start;
		cmd.mTargetValue = target;
		cmd.mOnceLength = time;
		cmd.mTremblingName = CommonDefine.ZERO_ONE;
		cmd.mTrembleDoneCallBack = doneCallback;
		pushDelayCommand(cmd, obj, delayTime);
		script.addDelayCmd(cmd);
		return cmd;
	}
	public static void FILL_WINDOW_EX(txUIObject obj, float start, float target, float time, KeyFrameCallback fillingCallback, KeyFrameCallback doneCallback)
	{
		CommandWindowFill cmd = newCmd(out cmd, false);
		cmd.mStartValue = start;
		cmd.mTargetValue = target;
		cmd.mOnceLength = time;
		cmd.mTremblingName = CommonDefine.ZERO_ONE;
		cmd.mTremblingCallBack = fillingCallback;
		cmd.mTrembleDoneCallBack = doneCallback;
		pushCommand(cmd, obj);
	}
	//----------------------------------------------------------------------------------------------------------------------------------------------------------------------
	// 透明度
	public static void ALPHA_WINDOW(txUIObject obj, float alpha = 1.0f)
	{
		CommandWindowAlpha cmd = newCmd(out cmd, false);
		cmd.mName = "";
		cmd.mOnceLength = 0.0f;
		cmd.mStartAlpha = alpha;
		cmd.mTargetAlpha = alpha;
		pushCommand(cmd, obj);
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
			logError("时间或关键帧不能为空,如果要停止组件,请使用void ALPHA_WINDOW(txUIObject obj, float alpha)");
		}
		CommandWindowAlpha cmd = newCmd(out cmd, false);
		cmd.mName = name;
		cmd.mLoop = loop;
		cmd.mOnceLength = onceLength;
		cmd.mOffset = offset;
		cmd.mStartAlpha = start;
		cmd.mTargetAlpha = target;
		cmd.setTremblingCallback(tremblingCallback, null);
		cmd.setTrembleDoneCallback(trembleDoneCallback, null);
		pushCommand(cmd, obj);
	}
	public static CommandWindowAlpha ALPHA_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, float alpha)
	{
		CommandWindowAlpha cmd = newCmd(out cmd, false, true);
		cmd.mName = "";
		cmd.mOnceLength = 0.0f;
		cmd.mStartAlpha = alpha;
		cmd.mTargetAlpha = alpha;
		pushDelayCommand(cmd, obj, delayTime);
		script.addDelayCmd(cmd);
		return cmd;
	}
	public static CommandWindowAlpha ALPHA_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, float start, float target, float onceLength)
	{
		return ALPHA_KEYFRAME_WINDOW_DELAY_EX(script, obj, delayTime, CommonDefine.ZERO_ONE, start, target, onceLength, false, 0.0f, null, null);
	}
	public static CommandWindowAlpha ALPHA_WINDOW_DELAY_EX(LayoutScript script, txUIObject obj, float delayTime, float start, float target, float onceLength, KeyFrameCallback trembleDoneCallback)
	{
		return ALPHA_KEYFRAME_WINDOW_DELAY_EX(script, obj, delayTime, CommonDefine.ZERO_ONE, start, target, onceLength, false, 0.0f, null, trembleDoneCallback);
	}
	public static CommandWindowAlpha ALPHA_WINDOW_DELAY_EX(LayoutScript script, txUIObject obj, float delayTime, float start, float target, float onceLength, KeyFrameCallback tremblingCallback, KeyFrameCallback trembleDoneCallback)
	{
		return ALPHA_KEYFRAME_WINDOW_DELAY_EX(script, obj, delayTime, CommonDefine.ZERO_ONE, start, target, onceLength, false, 0.0f, tremblingCallback, trembleDoneCallback);
	}
	public static CommandWindowAlpha ALPHA_KEYFRAME_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, string keyframe, float start, float target, float onceLength)
	{
		return ALPHA_KEYFRAME_WINDOW_DELAY_EX(script, obj, delayTime, keyframe, start, target, onceLength, false, 0.0f, null, null);
	}
	public static CommandWindowAlpha ALPHA_KEYFRAME_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, string keyframe, float start, float target, float onceLength, bool loop)
	{
		return ALPHA_KEYFRAME_WINDOW_DELAY_EX(script, obj, delayTime, keyframe, start, target, onceLength, loop, 0.0f, null, null);
	}
	public static CommandWindowAlpha ALPHA_KEYFRAME_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, string keyframe, float start, float target, float onceLength, bool loop, float offset)
	{
		return ALPHA_KEYFRAME_WINDOW_DELAY_EX(script, obj, delayTime, keyframe, start, target, onceLength, loop, offset, null, null);
	}
	public static CommandWindowAlpha ALPHA_KEYFRAME_WINDOW_DELAY_EX(LayoutScript script, txUIObject obj, float delayTime, string keyframe, float start, float target, float onceLength, KeyFrameCallback tremblingCallback, KeyFrameCallback trembleDoneCallback)
	{
		return ALPHA_KEYFRAME_WINDOW_DELAY_EX(script, obj, delayTime, keyframe, start, target, onceLength, false, 0.0f, tremblingCallback, trembleDoneCallback);
	}
	public static CommandWindowAlpha ALPHA_KEYFRAME_WINDOW_DELAY_EX(LayoutScript script, txUIObject obj, float delayTime, string keyframe, float start, float target, float onceLength, bool loop, float offset, KeyFrameCallback tremblingCallback, KeyFrameCallback trembleDoneCallback)
	{
		if (keyframe == "" || MathUtility.isFloatZero(onceLength))
		{
			logError("时间或关键帧不能为空,如果要停止组件,请使用CommandWindowAlphaTremble ALPHA_WINDOW_DELAY(LayoutScript script, txUIObject obj, float delayTime, float alpha)");
		}
		CommandWindowAlpha cmd = newCmd(out cmd, false, true);
		cmd.mName = keyframe;
		cmd.mLoop = loop;
		cmd.mOnceLength = onceLength;
		cmd.mOffset = offset;
		cmd.mStartAlpha = start;
		cmd.mTargetAlpha = target;
		cmd.setTremblingCallback(tremblingCallback, null);
		cmd.setTrembleDoneCallback(trembleDoneCallback, null);
		pushDelayCommand(cmd, obj, delayTime);
		script.addDelayCmd(cmd);
		return cmd;
	}
	//--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	// HSL
	public static void HSL_WINDOW(txUIObject obj, Vector3 hsl)
	{
		CommandWindowHSL cmd = newCmd(out cmd, false, false);
		cmd.mName = "";
		cmd.mOnceLength = 0.0f;
		cmd.mStartHSL = hsl;
		cmd.mTargetHSL = hsl;
		pushCommand(cmd, obj);
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
			logError("时间或关键帧不能为空,如果要停止组件,请使用void HSL_WINDOW(txUIObject obj, Vector3 hsl)");
		}
		CommandWindowHSL cmd = newCmd(out cmd, false, false);
		cmd.mName = keyframe;
		cmd.mLoop = loop;
		cmd.mOnceLength = onceLength;
		cmd.mOffset = offset;
		cmd.mStartHSL = start;
		cmd.mTargetHSL = target;
		pushCommand(cmd, obj);
	}
	//--------------------------------------------------------------------------------------------------------------------------------------------------------
	// 亮度
	public static void LUM_WINDOW(txUIObject obj, float lum)
	{
		CommandWindowLum cmd = newCmd(out cmd, false, false);
		cmd.mName = "";
		cmd.mOnceLength = 0.0f;
		cmd.mStartLum = lum;
		cmd.mTargetLum = lum;
		pushCommand(cmd, obj);
	}
	public static void LUM_WINDOW(txUIObject obj, float start, float target, float onceLength)
	{
		LUM_KEYFRAME_WINDOW(obj, CommonDefine.ZERO_ONE, start, target, onceLength, false, 0.0f);
	}
	public static void LUM_KEYFRAME_WINDOW(txUIObject obj, string keyframe, float start, float target, float onceLength)
	{
		LUM_KEYFRAME_WINDOW(obj, keyframe, start, target, onceLength, false, 0.0f);
	}
	public static void LUM_KEYFRAME_WINDOW(txUIObject obj, string keyframe, float start, float target, float onceLength, bool loop, float offset)
	{
		if (keyframe == "" || MathUtility.isFloatZero(onceLength))
		{
			logError("时间或关键帧不能为空,如果要停止组件,请使用void LUM_WINDOW(txUIObject obj, Vector3 hsl)");
		}
		CommandWindowLum cmd = newCmd(out cmd, false, false);
		cmd.mName = keyframe;
		cmd.mLoop = loop;
		cmd.mOnceLength = onceLength;
		cmd.mOffset = offset;
		cmd.mStartLum = start;
		cmd.mTargetLum = target;
		pushCommand(cmd, obj);
	}
	//--------------------------------------------------------------------------------------------------------------------------------------------------------
	// 音效
	public static void PLAY_AUDIO(txUIObject obj)
	{
		pushCommand<CommandWindowPlayAudio>(obj, false);
	}
	public static void PLAY_AUDIO(txUIObject obj, SOUND_DEFINE sound)
	{
		PLAY_AUDIO(obj, sound, false, 1.0f);
	}
	public static void PLAY_AUDIO(txUIObject obj, SOUND_DEFINE sound, bool loop, float volume)
	{
		CommandWindowPlayAudio cmd = newCmd(out cmd, false);
		cmd.mSound = sound;
		cmd.mLoop = loop;
		cmd.mVolume = volume;
		pushCommand(cmd, obj);
	}
	// fileName为sound文件夹的相对路径,
	public static void PLAY_AUDIO(txUIObject obj, string fileName, bool loop, float volume)
	{
		CommandWindowPlayAudio cmd = newCmd(out cmd, false);
		cmd.mSoundFileName = fileName;
		cmd.mLoop = loop;
		cmd.mVolume = volume;
		pushCommand(cmd, obj);
	}
	public static CommandWindowPlayAudio PLAY_AUDIO_DELAY(LayoutScript script, txUIObject obj, float delayTime, SOUND_DEFINE sound, bool loop, float volume)
	{
		CommandWindowPlayAudio cmd = newCmd(out cmd, false, true);
		cmd.mSound = sound;
		cmd.mLoop = loop;
		cmd.mVolume = volume;
		pushDelayCommand(cmd, obj, delayTime);
		script.addDelayCmd(cmd);
		return cmd;
	}
}