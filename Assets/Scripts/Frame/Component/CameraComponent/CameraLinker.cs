using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum CAMERA_LINKER_SWITCH
{
	CLS_NONE,
	CLS_LINEAR,
	CLS_CIRCLE,
	CLS_AROUND_TARGET,
}

public class CameraLinker : GameComponent
{
	protected MovableObject mLinkObject;
	protected Vector3 mRelativePosition;	//相对位置
	protected bool mLookAtTarget;			// 是否在摄像机运动过程中一直看向目标位置
	protected Vector3 mLookAtOffset;		// 焦点的偏移,实际摄像机的焦点是物体的位置加上偏移
	protected Dictionary<CAMERA_LINKER_SWITCH, CameraLinkerSwitch> mSwitchList; // 转换器列表
	protected CameraLinkerSwitch mCurSwitch;
	protected GameCamera mCamera;
	protected bool mLateUpdate = true;	// 是否在LateUpdate中更新连接器
	public CameraLinker(Type type, string name)
		: base(type, name)
	{
		mCurSwitch = null;
		mLookAtOffset = new Vector3(0.0f, 2.0f, 0.0f);
		mLookAtTarget = false;
		mLinkObject = null;
		mSwitchList = new Dictionary<CAMERA_LINKER_SWITCH, CameraLinkerSwitch>();
	}
	public override void init(ComponentOwner owner)
	{
		base.init(owner);
		initSwitch();
		mCamera = mComponentOwner as GameCamera;
	}
	public override void update(float elapsedTime)
	{
		base.update(elapsedTime);
		if (mLinkObject == null)
		{
			return;
		}
		if (!mLateUpdate)
		{
			if (mCurSwitch != null)
			{
				mCurSwitch.update(elapsedTime);
			}
			updateLinker(elapsedTime);
		}
	}
	public override void lateUpdate(float elapsedTime)
	{
		base.update(elapsedTime);
		if (mLinkObject == null)
		{
			return;
		}
		if (mLateUpdate)
		{
			if (mCurSwitch != null)
			{
				mCurSwitch.update(elapsedTime);
			}
			updateLinker(elapsedTime);
		}
	}
	public virtual void applyRelativePosition(Vector3 relative)
	{
		if (mLinkObject != null)
		{
			mCamera.setPosition(mLinkObject.getWorldPosition() + relative);
			if (mLookAtTarget)
			{
				//让摄像机朝向
				Vector3 cameraDir = mLinkObject.getWorldPosition() + mLookAtOffset - mCamera.getPosition();
				mCamera.setRotation(Quaternion.LookRotation(cameraDir).eulerAngles);
			}
		}
	}
	public override void destroy()
	{
		base.destroy();
		destroySwitch();
	}
	public void setLinkObject(MovableObject obj) { mLinkObject = obj; }
	public MovableObject getLinkObject() { return mLinkObject; }
	public Vector3 getRelativePosition() { return mRelativePosition; }
	public virtual void setRelativePosition(Vector3 pos, CAMERA_LINKER_SWITCH switchType = CAMERA_LINKER_SWITCH.CLS_NONE, 
		bool useDefaultSwitchSpeed = true, float switchSpeed = 1.0f)
	{
		// 如果不使用转换器,则直接设置位置
		if (switchType == CAMERA_LINKER_SWITCH.CLS_NONE)
		{
			mRelativePosition = pos;
		}
		// 如果使用转换器,则查找相应的转换器,设置参数
		else
		{
			mCurSwitch = getSwitch(switchType);
			// 找不到则直接设置位置
			if (mCurSwitch == null)
			{
				mRelativePosition = pos;
			}
			else
			{
				// 如果不适用默认速度,其实是转换器当前的速度,则设置新的速度
				if (!useDefaultSwitchSpeed)
				{
					mCurSwitch.init(mRelativePosition, pos, switchSpeed);
				}
				// 否则就将转换器当前的速度设置进去
				else
				{
					mCurSwitch.init(mRelativePosition, pos, mCurSwitch.getSwitchSpeed());
				}
			}
		}
	}
	public virtual void notifyFinishSwitching(CameraLinkerSwitch fixedSwitch) { mCurSwitch = null; } // 由转换器调用,通知连接器转换已经完成
	public CameraLinkerSwitch getSwitch(CAMERA_LINKER_SWITCH type)
	{
		if(mSwitchList.ContainsKey(type))
		{
			return mSwitchList[type];
		}
		return null;
	}
	public void setLookAtOffset(Vector3 offset) { mLookAtOffset = offset; }
	public Vector3 getLookAtOffset() { return mLookAtOffset; }
	public void setLookAtTarget(bool lookat) { mLookAtTarget = lookat; }
	public bool getLookAtTarget() { return mLookAtTarget; }
	public virtual Vector3 getNormalRelativePosition()
	{
		return MathUtility.rotateVector3(mRelativePosition, mLinkObject.getRotation().y * Mathf.Deg2Rad);
	}
	//------------------------------------------------------------------------------------------------------------------------------------------------
	protected virtual void updateLinker(float elapsedTime) { }
	protected override void setBaseType() { mBaseType = typeof(CameraLinker); }
	protected override bool isType(Type type) {	return type == typeof(CameraLinker);}
	protected void initSwitch()
	{
		CameraLinkerSwitchLinear lineSwitch = new CameraLinkerSwitchLinear(CAMERA_LINKER_SWITCH.CLS_LINEAR, this);
		mSwitchList.Add(lineSwitch.mType, lineSwitch);
		CameraLinkerSwitchCircle circleSwitch = new CameraLinkerSwitchCircle(CAMERA_LINKER_SWITCH.CLS_CIRCLE, this);
		mSwitchList.Add(circleSwitch.mType, circleSwitch);
		CameraLinkerSwitchAroundTarget roateCharacter = new CameraLinkerSwitchAroundTarget(CAMERA_LINKER_SWITCH.CLS_AROUND_TARGET, this);
		mSwitchList.Add(roateCharacter.mType, roateCharacter);
	}
	public void destroySwitch()
	{
		if (mSwitchList.Count == 0)
		{
			return;
		}
		foreach(var item in mSwitchList)
		{
			item.Value.destroy();
		}
		mSwitchList.Clear();
		mCurSwitch = null;
	}
};