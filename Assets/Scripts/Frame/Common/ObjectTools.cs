using UnityEngine;
using System.Collections;
using System;

public class ObjectTools : GameBase
{
	//--------------------------------------------------------------------------------------------------------------------------------------------
	// 旋转
	#region 用关键帧旋转物体,在普通更新中,与物理更新的函数不能混用,否则效果会混合
	public static void ROTATE_FIXED_OBJECT(MovableObject obj, bool lockRotation = true)
	{
		CommandMovableObjectRotateFixed cmd = mCommandSystem.newCmd<CommandMovableObjectRotateFixed>(false, false);
		cmd.mActive = lockRotation;
		mCommandSystem.pushCommand(cmd, obj);
	}
	public static void ROTATE_FIXED_OBJECT(MovableObject obj, Vector3 rot, bool lockRotation = true)
	{
		CommandMovableObjectRotateFixed cmd = mCommandSystem.newCmd<CommandMovableObjectRotateFixed>(false, false);
		cmd.mActive = lockRotation;
		cmd.mFixedEuler = rot;
		mCommandSystem.pushCommand(cmd, obj);
	}
	public static void ROTATE_OBJECT(MovableObject obj, Vector3 rotation)
	{
		CommandMovableObjectRotate cmd = mCommandSystem.newCmd<CommandMovableObjectRotate>(false, false);
		cmd.mName = "";
		cmd.mOnceLength = 0.0f;
		cmd.mStartRotation = rotation;
		cmd.mTargetRotation = rotation;
		mCommandSystem.pushCommand(cmd, obj);
	}
	public static void ROTATE_OBJECT(MovableObject obj, Vector3 start, Vector3 target, float time)
	{
		ROTATE_OBJECT_EX(obj, CommonDefine.ZERO_ONE, start, target, time, false, 0.0f, null, null);
	}
	public static void ROTATE_OBJECT_EX(MovableObject obj, Vector3 start, Vector3 target, float time, KeyFrameCallback rotatingCallback, KeyFrameCallback doneCallback)
	{
		ROTATE_OBJECT_EX(obj, CommonDefine.ZERO_ONE, start, target, time, false, 0.0f, rotatingCallback, doneCallback);
	}
	public static void ROTATE_OBJECT_EX(MovableObject obj, Vector3 start, Vector3 target, float time, KeyFrameCallback doneCallback)
	{
		ROTATE_OBJECT_EX(obj, CommonDefine.ZERO_ONE, start, target, time, false, 0.0f, null, doneCallback);
	}
	public static void ROTATE_OBJECT(MovableObject obj, string keyframe, Vector3 start, Vector3 target, float onceLength)
	{
		ROTATE_OBJECT_EX(obj, keyframe, start, target, onceLength, false, 0.0f, null, null);
	}
	public static void ROTATE_OBJECT(MovableObject obj, string keyframe, Vector3 start, Vector3 target, float onceLength, bool loop, float offset)
	{
		ROTATE_OBJECT_EX(obj, keyframe, start, target, onceLength, loop, offset, null, null);
	}
	public static void ROTATE_OBJECT_EX(MovableObject obj, string keyframe, Vector3 start, Vector3 target, float onceLength, bool loop, float offset, KeyFrameCallback rotatingCallback, KeyFrameCallback doneCallback)
	{
		if(keyframe == "" || MathUtility.isFloatZero(onceLength))
		{
			UnityUtility.logError("时间或关键帧不能为空,如果要停止组件,请使用void ROTATE_OBJECT(txUIObject obj, Vector3 rotation)");
		}
		CommandMovableObjectRotate cmd = mCommandSystem.newCmd<CommandMovableObjectRotate>(false, false);
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
	public static CommandMovableObjectRotate ROTATE_OBJECT_DELAY(MovableObject obj, float delayTime, Vector3 rotation)
	{
		CommandMovableObjectRotate cmd = mCommandSystem.newCmd<CommandMovableObjectRotate>(false, true);
		cmd.mName = "";
		cmd.mOnceLength = 0.0f;
		cmd.mStartRotation = rotation;
		cmd.mTargetRotation = rotation;
		mCommandSystem.pushDelayCommand(cmd, obj, delayTime);
		return cmd;
	}
	public static CommandMovableObjectRotate ROTATE_OBJECT_DELAY(MovableObject obj, float delayTime, Vector3 start, Vector3 target, float time)
	{
		return ROTATE_OBJECT_DELAY(obj, delayTime, CommonDefine.ZERO_ONE, start, target, time, false, 0.0f);
	}
	public static CommandMovableObjectRotate ROTATE_OBJECT_DELAY(MovableObject obj, float delayTime, string keyframe, Vector3 start, Vector3 target, float onceLength)
	{
		return ROTATE_OBJECT_DELAY(obj, delayTime, keyframe, start, target, onceLength, false, 0.0f);
	}
	public static CommandMovableObjectRotate ROTATE_OBJECT_DELAY(MovableObject obj, float delayTime, string keyframe, Vector3 start, Vector3 target, float onceLength, bool loop)
	{
		return ROTATE_OBJECT_DELAY(obj, delayTime, keyframe, start, target, onceLength, loop, 0.0f);
	}
	public static CommandMovableObjectRotate ROTATE_OBJECT_DELAY(MovableObject obj, float delayTime, string keyframe, Vector3 start, Vector3 target, float onceLength, bool loop, float offset)
	{
		if (keyframe == "" || MathUtility.isFloatZero(onceLength))
		{
			UnityUtility.logError("时间或关键帧不能为空,如果要停止组件,请使用CommandMovableObjectRotate ROTATE_OBJECT_DELAY(MovableObject obj, float delayTime, Vector3 rotation)");
		}
		CommandMovableObjectRotate cmd = mCommandSystem.newCmd<CommandMovableObjectRotate>(false, true);
		cmd.mName = keyframe;
		cmd.mOnceLength = onceLength;
		cmd.mStartRotation = start;
		cmd.mTargetRotation = target;
		cmd.mOffset = offset;
		cmd.mLoop = loop;
		mCommandSystem.pushDelayCommand(cmd, obj, delayTime);
		return cmd;
	}
	public static void ROTATE_SPEED_OBJECT(MovableObject obj)
	{
		ROTATE_SPEED_OBJECT(obj, Vector3.zero, Vector3.zero, Vector3.zero);
	}
	public static void ROTATE_SPEED_OBJECT(MovableObject obj, Vector3 speed)
	{
		ROTATE_SPEED_OBJECT(obj, speed, Vector3.zero, Vector3.zero);
	}
	public static void ROTATE_SPEED_OBJECT(MovableObject obj, Vector3 speed, Vector3 startAngle)
	{
		ROTATE_SPEED_OBJECT(obj, speed, startAngle, Vector3.zero);
	}
	public static void ROTATE_SPEED_OBJECT(MovableObject obj, Vector3 speed, Vector3 startAngle, Vector3 rotateAccelerationValue)
	{
		CommandMovableObjectRotateSpeed cmd = mCommandSystem.newCmd<CommandMovableObjectRotateSpeed>(false, false);
		cmd.mRotateSpeed = speed;
		cmd.mStartAngle = startAngle;
		cmd.mRotateAcceleration = rotateAccelerationValue;
		mCommandSystem.pushCommand(cmd, obj);
	}
	public static CommandMovableObjectRotateSpeed ROTATE_SPEED_OBJECT_DELAY(MovableObject obj, float delayTime, Vector3 speed)
	{
		return ROTATE_SPEED_OBJECT_DELAY(obj, delayTime, speed, Vector3.zero, Vector3.zero);
	}
	public static CommandMovableObjectRotateSpeed ROTATE_SPEED_OBJECT_DELAY(MovableObject obj, float delayTime, Vector3 speed, Vector3 startAngle)
	{
		return ROTATE_SPEED_OBJECT_DELAY(obj, delayTime, speed, startAngle, Vector3.zero);
	}
	public static CommandMovableObjectRotateSpeed ROTATE_SPEED_OBJECT_DELAY(MovableObject obj, float delayTime, Vector3 speed, Vector3 startAngle, Vector3 rotateAccelerationValue)
	{
		CommandMovableObjectRotateSpeed cmd = mCommandSystem.newCmd<CommandMovableObjectRotateSpeed>(false, true);
		cmd.mRotateSpeed = speed;
		cmd.mStartAngle = startAngle;
		cmd.mRotateAcceleration = rotateAccelerationValue;
		mCommandSystem.pushDelayCommand(cmd, obj, delayTime);
		return cmd;
	}
	#endregion
	#region 用关键帧旋转物体,在物理更新中,与普通更新的函数不能混用,否则效果会混合
	public static void ROTATE_FIXED_OBJECT_PHY(MovableObject obj, bool lockRotation = true)
	{
		CommandMovableObjectRotateFixedPhysics cmd = mCommandSystem.newCmd<CommandMovableObjectRotateFixedPhysics>(false, false);
		cmd.mActive = lockRotation;
		mCommandSystem.pushCommand(cmd, obj);
	}
	public static void ROTATE_FIXED_OBJECT_PHY(MovableObject obj, Vector3 rot, bool lockRotation = true)
	{
		CommandMovableObjectRotateFixedPhysics cmd = mCommandSystem.newCmd<CommandMovableObjectRotateFixedPhysics>(false, false);
		cmd.mActive = lockRotation;
		cmd.mFixedEuler = rot;
		mCommandSystem.pushCommand(cmd, obj);
	}
	public static void ROTATE_OBJECT_PHY(MovableObject obj, Vector3 rotation)
	{
		CommandMovableObjectRotatePhysics cmd = mCommandSystem.newCmd<CommandMovableObjectRotatePhysics>(false, false);
		cmd.mName = "";
		cmd.mOnceLength = 0.0f;
		cmd.mStartRotation = rotation;
		cmd.mTargetRotation = rotation;
		mCommandSystem.pushCommand(cmd, obj);
	}
	public static void ROTATE_OBJECT_PHY(MovableObject obj, Vector3 start, Vector3 target, float time)
	{
		ROTATE_OBJECT_PHY_EX(obj, CommonDefine.ZERO_ONE, start, target, time, false, 0.0f, null, null);
	}
	public static void ROTATE_OBJECT_PHY_EX(MovableObject obj, Vector3 start, Vector3 target, float time, KeyFrameCallback rotatingCallback, KeyFrameCallback doneCallback)
	{
		ROTATE_OBJECT_PHY_EX(obj, CommonDefine.ZERO_ONE, start, target, time, false, 0.0f, rotatingCallback, doneCallback);
	}
	public static void ROTATE_OBJECT_PHY_EX(MovableObject obj, Vector3 start, Vector3 target, float time, KeyFrameCallback doneCallback)
	{
		ROTATE_OBJECT_PHY_EX(obj, CommonDefine.ZERO_ONE, start, target, time, false, 0.0f, null, doneCallback);
	}
	public static void ROTATE_OBJECT_PHY(MovableObject obj, string keyframe, Vector3 start, Vector3 target, float onceLength)
	{
		ROTATE_OBJECT_PHY_EX(obj, keyframe, start, target, onceLength, false, 0.0f, null, null);
	}
	public static void ROTATE_OBJECT_PHY(MovableObject obj, string keyframe, Vector3 start, Vector3 target, float onceLength, bool loop, float offset)
	{
		ROTATE_OBJECT_PHY_EX(obj, keyframe, start, target, onceLength, loop, offset, null, null);
	}
	public static void ROTATE_OBJECT_PHY_EX(MovableObject obj, string keyframe, Vector3 start, Vector3 target, float onceLength, bool loop, float offset, KeyFrameCallback rotatingCallback, KeyFrameCallback doneCallback)
	{
		if (keyframe == "" || MathUtility.isFloatZero(onceLength))
		{
			UnityUtility.logError("时间或关键帧不能为空,如果要停止组件,请使用void ROTATE_OBJECT_PHY(txUIObject obj, Vector3 rotation)");
		}
		CommandMovableObjectRotatePhysics cmd = mCommandSystem.newCmd<CommandMovableObjectRotatePhysics>(false, false);
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
	public static CommandMovableObjectRotatePhysics ROTATE_OBJECT_PHY_DELAY(MovableObject obj, float delayTime, Vector3 rotation)
	{
		CommandMovableObjectRotatePhysics cmd = mCommandSystem.newCmd<CommandMovableObjectRotatePhysics>(false, true);
		cmd.mName = "";
		cmd.mOnceLength = 0.0f;
		cmd.mStartRotation = rotation;
		cmd.mTargetRotation = rotation;
		mCommandSystem.pushDelayCommand(cmd, obj, delayTime);
		return cmd;
	}
	public static CommandMovableObjectRotatePhysics ROTATE_OBJECT_PHY_DELAY(MovableObject obj, float delayTime, Vector3 start, Vector3 target, float time)
	{
		return ROTATE_OBJECT_PHY_DELAY(obj, delayTime, CommonDefine.ZERO_ONE, start, target, time, false, 0.0f);
	}
	public static CommandMovableObjectRotatePhysics ROTATE_OBJECT_PHY_DELAY(MovableObject obj, float delayTime, string keyframe, Vector3 start, Vector3 target, float onceLength)
	{
		return ROTATE_OBJECT_PHY_DELAY(obj, delayTime, keyframe, start, target, onceLength, false, 0.0f);
	}
	public static CommandMovableObjectRotatePhysics ROTATE_OBJECT_PHY_DELAY(MovableObject obj, float delayTime, string keyframe, Vector3 start, Vector3 target, float onceLength, bool loop)
	{
		return ROTATE_OBJECT_PHY_DELAY(obj, delayTime, keyframe, start, target, onceLength, loop, 0.0f);
	}
	public static CommandMovableObjectRotatePhysics ROTATE_OBJECT_PHY_DELAY(MovableObject obj, float delayTime, string keyframe, Vector3 start, Vector3 target, float onceLength, bool loop, float offset)
	{
		if (keyframe == "" || MathUtility.isFloatZero(onceLength))
		{
			UnityUtility.logError("时间或关键帧不能为空,如果要停止组件,请使用CommandMovableObjectRotatePhysics ROTATE_OBJECT_PHY_DELAY(MovableObject obj, float delayTime, Vector3 rotation)");
		}
		CommandMovableObjectRotatePhysics cmd = mCommandSystem.newCmd<CommandMovableObjectRotatePhysics>(false, true);
		cmd.mName = keyframe;
		cmd.mOnceLength = onceLength;
		cmd.mStartRotation = start;
		cmd.mTargetRotation = target;
		cmd.mOffset = offset;
		cmd.mLoop = loop;
		mCommandSystem.pushDelayCommand(cmd, obj, delayTime);
		return cmd;
	}
	public static void ROTATE_SPEED_OBJECT_PHY(MovableObject obj)
	{
		ROTATE_SPEED_OBJECT(obj, Vector3.zero, Vector3.zero, Vector3.zero);
	}
	public static void ROTATE_SPEED_OBJECT_PHY(MovableObject obj, Vector3 speed)
	{
		ROTATE_SPEED_OBJECT(obj, speed, Vector3.zero, Vector3.zero);
	}
	public static void ROTATE_SPEED_OBJECT_PHY(MovableObject obj, Vector3 speed, Vector3 startAngle)
	{
		ROTATE_SPEED_OBJECT(obj, speed, startAngle, Vector3.zero);
	}
	public static void ROTATE_SPEED_OBJECT_PHY(MovableObject obj, Vector3 speed, Vector3 startAngle, Vector3 rotateAccelerationValue)
	{
		CommandMovableObjectRotateSpeedPhysics cmd = mCommandSystem.newCmd<CommandMovableObjectRotateSpeedPhysics>(false, false);
		cmd.mRotateSpeed = speed;
		cmd.mStartAngle = startAngle;
		cmd.mRotateAcceleration = rotateAccelerationValue;
		mCommandSystem.pushCommand(cmd, obj);
	}
	public static CommandMovableObjectRotateSpeedPhysics ROTATE_SPEED_OBJECT_PHY_DELAY(MovableObject obj, float delayTime, Vector3 speed)
	{
		return ROTATE_SPEED_OBJECT_PHY_DELAY(obj, delayTime, speed, Vector3.zero, Vector3.zero);
	}
	public static CommandMovableObjectRotateSpeedPhysics ROTATE_SPEED_OBJECT_PHY_DELAY(MovableObject obj, float delayTime, Vector3 speed, Vector3 startAngle)
	{
		return ROTATE_SPEED_OBJECT_PHY_DELAY(obj, delayTime, speed, startAngle, Vector3.zero);
	}
	public static CommandMovableObjectRotateSpeedPhysics ROTATE_SPEED_OBJECT_PHY_DELAY(MovableObject obj, float delayTime, Vector3 speed, Vector3 startAngle, Vector3 rotateAccelerationValue)
	{
		CommandMovableObjectRotateSpeedPhysics cmd = mCommandSystem.newCmd<CommandMovableObjectRotateSpeedPhysics>(false, true);
		cmd.mRotateSpeed = speed;
		cmd.mStartAngle = startAngle;
		cmd.mRotateAcceleration = rotateAccelerationValue;
		mCommandSystem.pushDelayCommand(cmd, obj, delayTime);
		return cmd;
	}
	#endregion
	//-------------------------------------------------------------------------------------------------------------------------------------------------------
	// 移动
	#region 用关键帧移动物体,在普通更新中
	public static void MOVE_OBJECT(MovableObject obj, Vector3 pos)
	{
		CommandMovableObjectMove cmd = mCommandSystem.newCmd<CommandMovableObjectMove>(false, false);
		cmd.mName = "";
		cmd.mOnceLength = 0.0f;
		cmd.mStartPos = pos;
		cmd.mTargetPos = pos;
		mCommandSystem.pushCommand(cmd, obj);
	}
	public static void MOVE_OBJECT(MovableObject obj, Vector3 start, Vector3 target, float onceLength)
	{
		MOVE_OBJECT_EX(obj, CommonDefine.ZERO_ONE, start, target, onceLength, false, 0.0f, null, null);
	}
	public static void MOVE_OBJECT_EX(MovableObject obj, Vector3 start, Vector3 target, float onceLength, KeyFrameCallback moveDoneCallback)
	{
		MOVE_OBJECT_EX(obj, CommonDefine.ZERO_ONE, start, target, onceLength, false, 0.0f, null, moveDoneCallback);
	}
	public static void MOVE_OBJECT_EX(MovableObject obj, Vector3 start, Vector3 target, float onceLength, KeyFrameCallback movingCallback, KeyFrameCallback moveDoneCallback)
	{
		MOVE_OBJECT_EX(obj, CommonDefine.ZERO_ONE, start, target, onceLength, false, 0.0f, movingCallback, moveDoneCallback);
	}
	public static void MOVE_OBJECT_EX(MovableObject obj, Vector3 start, Vector3 target, float onceLength, float offsetTime, KeyFrameCallback moveDoneCallback)
	{
		MOVE_OBJECT_EX(obj, CommonDefine.ZERO_ONE, start, target, onceLength, false, offsetTime, null, moveDoneCallback);
	}
	public static void MOVE_OBJECT_EX(MovableObject obj, Vector3 start, Vector3 target, float onceLength, float offsetTime, KeyFrameCallback movingCallback, KeyFrameCallback moveDoneCallback)
	{
		MOVE_OBJECT_EX(obj, CommonDefine.ZERO_ONE, start, target, onceLength, false, offsetTime, movingCallback, moveDoneCallback);
	}
	public static void MOVE_OBJECT(MovableObject obj, string fileName, Vector3 startPos, Vector3 targetPos, float onceLength)
	{
		MOVE_OBJECT_EX(obj, fileName, startPos, targetPos, onceLength, false, 0.0f, null, null);
	}
	public static void MOVE_OBJECT(MovableObject obj, string fileName, Vector3 startPos, Vector3 targetPos, float onceLength, bool loop)
	{
		MOVE_OBJECT_EX(obj, fileName, startPos, targetPos, onceLength, loop, 0.0f, null, null);
	}
	public static void MOVE_OBJECT(MovableObject obj, string fileName, Vector3 startPos, Vector3 targetPos, float onceLength, bool loop, float offset)
	{
		MOVE_OBJECT_EX(obj, fileName, startPos, targetPos, onceLength, loop, offset, null, null);
	}
	public static void MOVE_OBJECT_EX(MovableObject obj, string fileName, Vector3 startPos, Vector3 targetPos, float onceLength, KeyFrameCallback TremblingCallBack, KeyFrameCallback TrembleDoneCallBack)
	{
		MOVE_OBJECT_EX(obj, fileName, startPos, targetPos, onceLength, false, 0.0f, TremblingCallBack, TrembleDoneCallBack);
	}
	public static void MOVE_OBJECT_EX(MovableObject obj, string fileName, Vector3 startPos, Vector3 targetPos, float onceLength, bool loop, KeyFrameCallback TremblingCallBack, KeyFrameCallback TrembleDoneCallBack)
	{
		MOVE_OBJECT_EX(obj, fileName, startPos, targetPos, onceLength, loop, 0.0f, TremblingCallBack, TrembleDoneCallBack);
	}
	public static void MOVE_OBJECT_EX(MovableObject obj, string fileName, Vector3 startPos, Vector3 targetPos, float onceLength, bool loop, float offset, KeyFrameCallback TremblingCallBack, KeyFrameCallback TrembleDoneCallBack)
	{
		if (fileName == "" || MathUtility.isFloatZero(onceLength))
		{
			UnityUtility.logError("时间或关键帧不能为空,如果要停止组件,请使用void MOVE_OBJECT(MovableObject obj, Vector3 pos)");
		}
		CommandMovableObjectMove cmd = mCommandSystem.newCmd<CommandMovableObjectMove>(false, false);
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
	public static CommandMovableObjectMove MOVE_OBJECT_DELAY(MovableObject obj, float delayTime, Vector3 pos)
	{
		CommandMovableObjectMove cmd = mCommandSystem.newCmd<CommandMovableObjectMove>(false, true);
		cmd.mName = "";
		cmd.mStartPos = pos;
		cmd.mTargetPos = pos;
		cmd.mOnceLength = 0.0f;
		mCommandSystem.pushDelayCommand(cmd, obj, delayTime);
		return cmd;
	}
	public static CommandMovableObjectMove MOVE_OBJECT_DELAY(MovableObject obj, float delayTime, Vector3 start, Vector3 target, float onceLength)
	{
		return MOVE_OBJECT_DELAY_EX(obj, delayTime, CommonDefine.ZERO_ONE, start, target, onceLength, false, 0.0f, null, null);
	}
	public static CommandMovableObjectMove MOVE_OBJECT_DELAY_EX(MovableObject obj, float delayTime, Vector3 start, Vector3 target, float onceLength, KeyFrameCallback moveDoneCallback)
	{
		return MOVE_OBJECT_DELAY_EX(obj, delayTime, CommonDefine.ZERO_ONE, start, target, onceLength, false, 0.0f, null, moveDoneCallback);
	}
	public static CommandMovableObjectMove MOVE_OBJECT_DELAY_EX(MovableObject obj, float delayTime, Vector3 start, Vector3 target, float onceLength, KeyFrameCallback movingCallback, KeyFrameCallback moveDoneCallback)
	{
		return MOVE_OBJECT_DELAY_EX(obj, delayTime, CommonDefine.ZERO_ONE, start, target, onceLength, false, 0.0f, movingCallback, moveDoneCallback);
	}
	public static CommandMovableObjectMove MOVE_OBJECT_DELAY(MovableObject obj, float delayTime, string keyframe, Vector3 startPos, Vector3 targetPos, float onceLength)
	{
		return MOVE_OBJECT_DELAY_EX(obj, delayTime, keyframe, startPos, targetPos, onceLength, false, 0.0f, null, null);
	}
	public static CommandMovableObjectMove MOVE_OBJECT_DELAY(MovableObject obj, float delayTime, string keyframe, Vector3 startPos, Vector3 targetPos, float onceLength, bool loop)
	{
		return MOVE_OBJECT_DELAY_EX(obj, delayTime, keyframe, startPos, targetPos, onceLength, loop, 0.0f, null, null);
	}
	public static CommandMovableObjectMove MOVE_OBJECT_DELAY(MovableObject obj, float delayTime, string keyframe, Vector3 startPos, Vector3 targetPos, float onceLength, bool loop, float offset)
	{
		return MOVE_OBJECT_DELAY_EX(obj, delayTime, keyframe, startPos, targetPos, onceLength, loop, offset, null, null);
	}
	public static CommandMovableObjectMove MOVE_OBJECT_DELAY_EX(MovableObject obj, float delayTime, string keyframe, Vector3 startPos, Vector3 targetPos, float onceLength, bool loop, float offset, KeyFrameCallback movingCallback, KeyFrameCallback moveDoneCallback)
	{
		if (keyframe == "" || MathUtility.isFloatZero(onceLength))
		{
			UnityUtility.logError("时间或关键帧不能为空,如果要停止组件,请使用CommandMovableObjectMove MOVE_OBJECT_DELAY(MovableObject obj, float delayTime, Vector3 pos)");
		}
		CommandMovableObjectMove cmd = mCommandSystem.newCmd<CommandMovableObjectMove>(false, true);
		cmd.mName = keyframe;
		cmd.mStartPos = startPos;
		cmd.mTargetPos = targetPos;
		cmd.mOnceLength = onceLength;
		cmd.mOffset = offset;
		cmd.mLoop = loop;
		cmd.setTremblingCallback(movingCallback, null);
		cmd.setTrembleDoneCallback(moveDoneCallback, null);
		mCommandSystem.pushDelayCommand(cmd, obj, delayTime);
		return cmd;
	}
	#endregion
	#region 用关键帧移动物体,在物理更新中
	public static void MOVE_OBJECT_PHY(MovableObject obj, Vector3 pos)
	{
		CommandMovableObjectMovePhysics cmd = mCommandSystem.newCmd<CommandMovableObjectMovePhysics>(false, false);
		cmd.mName = "";
		cmd.mOnceLength = 0.0f;
		cmd.mStartPos = pos;
		cmd.mTargetPos = pos;
		mCommandSystem.pushCommand(cmd, obj);
	}
	public static void MOVE_OBJECT_PHY(MovableObject obj, Vector3 start, Vector3 target, float onceLength)
	{
		MOVE_OBJECT_PHY_EX(obj, CommonDefine.ZERO_ONE, start, target, onceLength, false, 0.0f, null, null);
	}
	public static void MOVE_OBJECT_PHY_EX(MovableObject obj, Vector3 start, Vector3 target, float onceLength, KeyFrameCallback moveDoneCallback)
	{
		MOVE_OBJECT_PHY_EX(obj, CommonDefine.ZERO_ONE, start, target, onceLength, false, 0.0f, null, moveDoneCallback);
	}
	public static void MOVE_OBJECT_PHY_EX(MovableObject obj, Vector3 start, Vector3 target, float onceLength, KeyFrameCallback movingCallback, KeyFrameCallback moveDoneCallback)
	{
		MOVE_OBJECT_PHY_EX(obj, CommonDefine.ZERO_ONE, start, target, onceLength, false, 0.0f, movingCallback, moveDoneCallback);
	}
	public static void MOVE_OBJECT_PHY_EX(MovableObject obj, Vector3 start, Vector3 target, float onceLength, float offsetTime, KeyFrameCallback moveDoneCallback)
	{
		MOVE_OBJECT_PHY_EX(obj, CommonDefine.ZERO_ONE, start, target, onceLength, false, offsetTime, null, moveDoneCallback);
	}
	public static void MOVE_OBJECT_PHY_EX(MovableObject obj, Vector3 start, Vector3 target, float onceLength, float offsetTime, KeyFrameCallback movingCallback, KeyFrameCallback moveDoneCallback)
	{
		MOVE_OBJECT_PHY_EX(obj, CommonDefine.ZERO_ONE, start, target, onceLength, false, offsetTime, movingCallback, moveDoneCallback);
	}
	public static void MOVE_OBJECT_PHY(MovableObject obj, string fileName, Vector3 startPos, Vector3 targetPos, float onceLength)
	{
		MOVE_OBJECT_PHY_EX(obj, fileName, startPos, targetPos, onceLength, false, 0.0f, null, null);
	}
	public static void MOVE_OBJECT_PHY(MovableObject obj, string fileName, Vector3 startPos, Vector3 targetPos, float onceLength, bool loop)
	{
		MOVE_OBJECT_PHY_EX(obj, fileName, startPos, targetPos, onceLength, loop, 0.0f, null, null);
	}
	public static void MOVE_OBJECT_PHY(MovableObject obj, string fileName, Vector3 startPos, Vector3 targetPos, float onceLength, bool loop, float offset)
	{
		MOVE_OBJECT_PHY_EX(obj, fileName, startPos, targetPos, onceLength, loop, offset, null, null);
	}
	public static void MOVE_OBJECT_PHY_EX(MovableObject obj, string fileName, Vector3 startPos, Vector3 targetPos, float onceLength, KeyFrameCallback TremblingCallBack, KeyFrameCallback TrembleDoneCallBack)
	{
		MOVE_OBJECT_PHY_EX(obj, fileName, startPos, targetPos, onceLength, false, 0.0f, TremblingCallBack, TrembleDoneCallBack);
	}
	public static void MOVE_OBJECT_PHY_EX(MovableObject obj, string fileName, Vector3 startPos, Vector3 targetPos, float onceLength, bool loop, KeyFrameCallback TremblingCallBack, KeyFrameCallback TrembleDoneCallBack)
	{
		MOVE_OBJECT_PHY_EX(obj, fileName, startPos, targetPos, onceLength, loop, 0.0f, TremblingCallBack, TrembleDoneCallBack);
	}
	public static void MOVE_OBJECT_PHY_EX(MovableObject obj, string fileName, Vector3 startPos, Vector3 targetPos, float onceLength, bool loop, float offset, KeyFrameCallback TremblingCallBack, KeyFrameCallback TrembleDoneCallBack)
	{
		if (fileName == "" || MathUtility.isFloatZero(onceLength))
		{
			UnityUtility.logError("时间或关键帧不能为空,如果要停止组件,请使用void MOVE_OBJECT_PHY(MovableObject obj, Vector3 pos)");
		}
		CommandMovableObjectMovePhysics cmd = mCommandSystem.newCmd<CommandMovableObjectMovePhysics>(false, false);
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
	public static CommandMovableObjectMovePhysics MOVE_OBJECT_PHY_DELAY(MovableObject obj, float delayTime, Vector3 pos)
	{
		CommandMovableObjectMovePhysics cmd = mCommandSystem.newCmd<CommandMovableObjectMovePhysics>(false, true);
		cmd.mName = "";
		cmd.mStartPos = pos;
		cmd.mTargetPos = pos;
		cmd.mOnceLength = 0.0f;
		mCommandSystem.pushDelayCommand(cmd, obj, delayTime);
		return cmd;
	}
	public static CommandMovableObjectMovePhysics MOVE_OBJECT_PHY_DELAY(MovableObject obj, float delayTime, Vector3 start, Vector3 target, float onceLength)
	{
		return MOVE_OBJECT_PHY_DELAY_EX(obj, delayTime, CommonDefine.ZERO_ONE, start, target, onceLength, false, 0.0f, null, null);
	}
	public static CommandMovableObjectMovePhysics MOVE_OBJECT_PHY_DELAY_EX(MovableObject obj, float delayTime, Vector3 start, Vector3 target, float onceLength, KeyFrameCallback moveDoneCallback)
	{
		return MOVE_OBJECT_PHY_DELAY_EX(obj, delayTime, CommonDefine.ZERO_ONE, start, target, onceLength, false, 0.0f, null, moveDoneCallback);
	}
	public static CommandMovableObjectMovePhysics MOVE_OBJECT_PHY_DELAY_EX(MovableObject obj, float delayTime, Vector3 start, Vector3 target, float onceLength, KeyFrameCallback movingCallback, KeyFrameCallback moveDoneCallback)
	{
		return MOVE_OBJECT_PHY_DELAY_EX(obj, delayTime, CommonDefine.ZERO_ONE, start, target, onceLength, false, 0.0f, movingCallback, moveDoneCallback);
	}
	public static CommandMovableObjectMovePhysics MOVE_OBJECT_PHY_DELAY(MovableObject obj, float delayTime, string keyframe, Vector3 startPos, Vector3 targetPos, float onceLength)
	{
		return MOVE_OBJECT_PHY_DELAY_EX(obj, delayTime, keyframe, startPos, targetPos, onceLength, false, 0.0f, null, null);
	}
	public static CommandMovableObjectMovePhysics MOVE_OBJECT_PHY_DELAY(MovableObject obj, float delayTime, string keyframe, Vector3 startPos, Vector3 targetPos, float onceLength, bool loop)
	{
		return MOVE_OBJECT_PHY_DELAY_EX(obj, delayTime, keyframe, startPos, targetPos, onceLength, loop, 0.0f, null, null);
	}
	public static CommandMovableObjectMovePhysics MOVE_OBJECT_PHY_DELAY(MovableObject obj, float delayTime, string keyframe, Vector3 startPos, Vector3 targetPos, float onceLength, bool loop, float offset)
	{
		return MOVE_OBJECT_PHY_DELAY_EX(obj, delayTime, keyframe, startPos, targetPos, onceLength, loop, offset, null, null);
	}
	public static CommandMovableObjectMovePhysics MOVE_OBJECT_PHY_DELAY_EX(MovableObject obj, float delayTime, string keyframe, Vector3 startPos, Vector3 targetPos, float onceLength, bool loop, float offset, KeyFrameCallback movingCallback, KeyFrameCallback moveDoneCallback)
	{
		if (keyframe == "" || MathUtility.isFloatZero(onceLength))
		{
			UnityUtility.logError("时间或关键帧不能为空,如果要停止组件,CommandMovableObjectMovePhysics MOVE_OBJECT_PHY_DELAY(MovableObject obj, float delayTime, Vector3 pos)");
		}
		CommandMovableObjectMovePhysics cmd = mCommandSystem.newCmd<CommandMovableObjectMovePhysics>(false, true);
		cmd.mName = keyframe;
		cmd.mStartPos = startPos;
		cmd.mTargetPos = targetPos;
		cmd.mOnceLength = onceLength;
		cmd.mOffset = offset;
		cmd.mLoop = loop;
		cmd.setTremblingCallback(movingCallback, null);
		cmd.setTrembleDoneCallback(moveDoneCallback, null);
		mCommandSystem.pushDelayCommand(cmd, obj, delayTime);
		return cmd;
	}
	#endregion
	//---------------------------------------------------------------------------------------------------------------------------------------------------------
	#region 追踪物体
	public static void TRACK_TARGET(MovableObject obj, float speed, MovableObject target, TrackDoneCallback doneCallback)
	{
		CommandMovableObjectTrackTarget cmd = mCommandSystem.newCmd<CommandMovableObjectTrackTarget>(false);
		cmd.mObject = target;
		cmd.mSpeed = speed;
		cmd.mDoneCallback = doneCallback;
		mCommandSystem.pushCommand(cmd, obj);
	}
	#endregion
	//----------------------------------------------------------------------------------------------------------------------------------------------------------
	// 显示
	#region 物体的显示和隐藏
	public static void ACTIVE_OBJECT(txUIObject obj, bool active = true)
	{
		CommandMovableObjectActive cmd = mCommandSystem.newCmd<CommandMovableObjectActive>(false);
		cmd.mActive = active;
		mCommandSystem.pushCommand(cmd, obj);
	}
	public static CommandMovableObjectActive ACTIVE_OBJECT_DELAY(txUIObject obj, bool active, float delayTime)
	{
		return ACTIVE_OBJECT_DELAY_EX(obj, active, delayTime, null, null);
	}
	public static CommandMovableObjectActive ACTIVE_OBJECT_DELAY_EX(txUIObject obj, bool active, float dealyTime, CommandCallback startCallback, object userData = null)
	{
		CommandMovableObjectActive cmd = mCommandSystem.newCmd<CommandMovableObjectActive>(false, true);
		cmd.mActive = active;
		cmd.addStartCommandCallback(startCallback, userData);
		mCommandSystem.pushDelayCommand(cmd, obj, dealyTime);
		return cmd;
	}
	#endregion
	//----------------------------------------------------------------------------------------------------------------------------------------------------------
	// 缩放
	#region 用关键帧缩放物体
	public static void SCALE_OBJECT(txUIObject obj, Vector2 scale)
	{
		CommandWindowScaleTrembling cmd = mCommandSystem.newCmd<CommandWindowScaleTrembling>(false);
		cmd.mName = "";
		cmd.mOnceLength = 0.0f;
		cmd.mStartScale = scale;
		cmd.mTargetScale = scale;
		mCommandSystem.pushCommand(cmd, obj);
	}
	public static void SCALE_OBJECT(txUIObject obj, Vector2 start, Vector2 target, float onceLength)
	{
		SCALE_OBJECT_EX(obj, CommonDefine.ZERO_ONE, start, target, onceLength, false, 0.0f, null, null);
	}
	public static void SCALE_OBJECT_EX(txUIObject obj, Vector2 start, Vector2 target, float onceLength, KeyFrameCallback doneCallback)
	{
		SCALE_OBJECT_EX(obj, CommonDefine.ZERO_ONE, start, target, onceLength, false, 0.0f, null, doneCallback);
	}
	public static void SCALE_OBJECT_EX(txUIObject obj, Vector2 start, Vector2 target, float onceLength, KeyFrameCallback scalingCallback, KeyFrameCallback doneCallback)
	{
		SCALE_OBJECT_EX(obj, CommonDefine.ZERO_ONE, start, target, onceLength, false, 0.0f, scalingCallback, doneCallback);
	}
	public static void SCALEOBJECT(txUIObject obj, string fileName, Vector2 start, Vector2 target, float onceLength)
	{
		SCALE_OBJECT_EX(obj, fileName, start, target, onceLength, false, 0.0f, null, null);
	}
	public static void SCALE_OBJECT(txUIObject obj, string fileName, Vector2 start, Vector2 target, float onceLength, bool loop)
	{
		SCALE_OBJECT_EX(obj, fileName, start, target, onceLength, loop, 0.0f, null, null);
	}
	public static void SCALE_OBJECT(txUIObject obj, string fileName, Vector2 start, Vector2 target, float onceLength, bool loop, float offset)
	{
		SCALE_OBJECT_EX(obj, fileName, start, target, onceLength, loop, offset, null, null);
	}
	public static void SCALE_OBJECT_EX(txUIObject obj, string fileName, Vector2 start, Vector2 target, float onceLength, KeyFrameCallback scaleTrembleDoneCallback)
	{
		SCALE_OBJECT_EX(obj, fileName, start, target, onceLength, false, 0.0f, null, scaleTrembleDoneCallback);
	}
	public static void SCALE_OBJECT_EX(txUIObject obj, string fileName, Vector2 start, Vector2 target, float onceLength, bool loop, KeyFrameCallback scaleTrembleDoneCallback)
	{
		SCALE_OBJECT_EX(obj, fileName, start, target, onceLength, loop, 0.0f, null, scaleTrembleDoneCallback);
	}
	public static void SCALE_OBJECT_EX(txUIObject obj, string fileName, Vector2 start, Vector2 target, float onceLength, bool loop, float offset, KeyFrameCallback scaleTrembleDoneCallback)
	{
		SCALE_OBJECT_EX(obj, fileName, start, target, onceLength, loop, offset, null, scaleTrembleDoneCallback);
	}
	public static void SCALE_OBJECT_EX(txUIObject obj, string fileName, Vector2 start, Vector2 target, float onceLength, KeyFrameCallback scaleTremblingCallback, KeyFrameCallback scaleTrembleDoneCallback)
	{
		SCALE_OBJECT_EX(obj, fileName, start, target, onceLength, false, 0.0f, scaleTremblingCallback, scaleTrembleDoneCallback);
	}
	public static void SCALE_OBJECT_EX(txUIObject obj, string fileName, Vector2 start, Vector2 target, float onceLength, bool loop, KeyFrameCallback scaleTremblingCallback, KeyFrameCallback scaleTrembleDoneCallback)
	{
		SCALE_OBJECT_EX(obj, fileName, start, target, onceLength, loop, 0.0f, scaleTremblingCallback, scaleTrembleDoneCallback);
	}
	public static void SCALE_OBJECT_EX(txUIObject obj, string fileName, Vector2 start, Vector2 target, float onceLength, bool loop, float offset, KeyFrameCallback scaleTremblingCallback, KeyFrameCallback scaleTrembleDoneCallback)
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
	public static CommandWindowScaleTrembling SCALE_OBJECT_DELAY(txUIObject obj, float delayTime, Vector2 scale)
	{
		CommandWindowScaleTrembling cmd = mCommandSystem.newCmd<CommandWindowScaleTrembling>(false, true);
		cmd.mName = "";
		cmd.mOnceLength = 0.0f;
		cmd.mStartScale = scale;
		cmd.mTargetScale = scale;
		mCommandSystem.pushDelayCommand(cmd, obj, delayTime);
		return cmd;
	}
	public static CommandWindowScaleTrembling SCALE_OBJECT_DELAY(txUIObject obj, float delayTime, Vector2 start, Vector2 target, float onceLength)
	{
		return SCALE_OBJECT_DELAY_EX(obj, delayTime, CommonDefine.ZERO_ONE, start, target, onceLength, false, 0.0f, null, null);
	}
	public static CommandWindowScaleTrembling SCALE_OBJECT_DELAY_EX(txUIObject obj, float delayTime, Vector2 start, Vector2 target, float onceLength, KeyFrameCallback scalingCallback, KeyFrameCallback doneCallback)
	{
		return SCALE_OBJECT_DELAY_EX(obj, delayTime, CommonDefine.ZERO_ONE, start, target, onceLength, false, 0.0f, scalingCallback, doneCallback);
	}
	public static CommandWindowScaleTrembling SCALE_OBJECT_DELAY(txUIObject obj, float delayTime, string keyframe, Vector2 start, Vector2 target, float onceLength)
	{
		return SCALE_OBJECT_DELAY_EX(obj, delayTime, keyframe, start, target, onceLength, false, 0.0f, null, null);
	}
	public static CommandWindowScaleTrembling SCALE_OBJECT_DELAY(txUIObject obj, float delayTime, string keyframe, Vector2 start, Vector2 target, float onceLength, bool loop)
	{
		return SCALE_OBJECT_DELAY_EX(obj, delayTime, keyframe, start, target, onceLength, loop, 0.0f, null, null);
	}
	public static CommandWindowScaleTrembling SCALE_OBJECT_DELAY(txUIObject obj, float delayTime, string keyframe, Vector2 start, Vector2 target, float onceLength, bool loop, float offset)
	{
		return SCALE_OBJECT_DELAY_EX(obj, delayTime, keyframe, start, target, onceLength, loop, offset, null, null);
	}
	public static CommandWindowScaleTrembling SCALE_OBJECT_DELAY_EX(txUIObject obj, float delayTime, string keyframe, Vector2 start, Vector2 target, float onceLength, KeyFrameCallback scaleTremblingCallback, KeyFrameCallback scaleTrembleDoneCallback)
	{
		return SCALE_OBJECT_DELAY_EX(obj, delayTime, keyframe, start, target, onceLength, false, 0.0f, scaleTremblingCallback, scaleTrembleDoneCallback);
	}
	public static CommandWindowScaleTrembling SCALE_OBJECT_DELAY_EX(txUIObject obj, float delayTime, string keyframe, Vector2 start, Vector2 target, float onceLength, bool loop, KeyFrameCallback scaleTremblingCallback, KeyFrameCallback scaleTrembleDoneCallback)
	{
		return SCALE_OBJECT_DELAY_EX(obj, delayTime, keyframe, start, target, onceLength, loop, 0.0f, scaleTremblingCallback, scaleTrembleDoneCallback);
	}
	public static CommandWindowScaleTrembling SCALE_OBJECT_DELAY_EX(txUIObject obj, float delayTime, string keyframe, Vector2 start, Vector2 target, float onceLength, bool loop, float offset, KeyFrameCallback scaleTremblingCallback, KeyFrameCallback scaleTrembleDoneCallback)
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
		return cmd;
	}
	#endregion
}