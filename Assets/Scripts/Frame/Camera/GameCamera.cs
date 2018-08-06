using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GameCamera : MovableObject
{
	protected Camera mCamera;
	protected CameraLinker mCurLinker;   // 只是记录当前连接器方便外部获取
	protected Dictionary<string, CameraLinker> mLinkerList; // 由于对连接器的操作比较多,所以单独放到一个表中,组件列表中不变
	protected float mMoveSpeed;         // 使用按键控制时的移动速度
	protected float mMouseSpeed;        // 鼠标转动摄像机的速度
	protected bool mKeyProcess;         // 锁定摄像机的按键控制
	// 如果要实现摄像机震动,则需要将摄像机挂接到一个节点上,一般操作的是父节点的Transform,震动时是操作摄像机自身节点的Transform
	public GameCamera(string name)
		:
		base(name)
	{
		mUsePreUpdate = true;
		mUseLateUpdate = true;
		mCurLinker = null;
		mMoveSpeed = 30.0f;
		mMouseSpeed = 0.1f;
		mKeyProcess = false;
		mLinkerList = new Dictionary<string, CameraLinker>();
		setDestroyObject(false);
	}
	public override void init()
	{
		base.init();
	}
	public override void setObject(GameObject obj, bool destroyOld = true)
	{
		base.setObject(obj, destroyOld);
		mCamera = mObject.GetComponent<Camera>();
		if (mObject.GetComponent<CameraInfo>() == null)
		{
			mObject.AddComponent<CameraInfo>();
		}
		mObject.GetComponent<CameraInfo>().setCamera(this);
	}
	public override void initComponents()
	{
		base.initComponents();
		addComponent<CameraLinkerAcceleration>("acceleration");
		addComponent<CameraLinkerFixed>("fixed");
		addComponent<CameraLinkerSmoothFollow>("SmoothFollow");
		addComponent<CameraLinkerSmoothRotate>("SmoothRotate");
	}
	public override void update(float elapsedTime)
	{
		base.update(elapsedTime);
		if (mCurLinker == null && mKeyProcess)
		{
			float cameraSpeed = mMoveSpeed;
			if (!MathUtility.isFloatZero(cameraSpeed))
			{
				// 键盘移动摄像机
				if (Input.GetKey(KeyCode.LeftShift))
				{
					cameraSpeed *= 2.0f;
				}
				// 向前移动摄像机
				if (Input.GetKey(KeyCode.W))
				{
					move(Vector3.forward * cameraSpeed * elapsedTime);
				}
				// 向左移动摄像机
				if (Input.GetKey(KeyCode.A))
				{
					move(Vector3.left * cameraSpeed * elapsedTime);
				}
				// 向后移动摄像机
				if (Input.GetKey(KeyCode.S))
				{
					move(Vector3.back * cameraSpeed * elapsedTime);
				}
				// 向右移动摄像机
				if (Input.GetKey(KeyCode.D))
				{
					move(Vector3.right * cameraSpeed * elapsedTime);
				}
				// 竖直向上移动摄像机
				if (Input.GetKey(KeyCode.Q))
				{
					move(Vector3.up * cameraSpeed * elapsedTime, Space.World);
				}
				// 竖直向下移动摄像机
				if (Input.GetKey(KeyCode.E))
				{
					move(Vector3.down * cameraSpeed * elapsedTime, Space.World);
				}
			}
			// 鼠标旋转摄像机
			if (mInputManager.getMouseKeepDown(MOUSE_BUTTON.MB_RIGHT) || mInputManager.getMouseCurrentDown(MOUSE_BUTTON.MB_RIGHT))
			{
				Vector2 moveDelta = mInputManager.getMouseDelta();
				if (!MathUtility.isFloatZero(moveDelta.x) || !MathUtility.isFloatZero(moveDelta.y))
				{
					yawpitch(moveDelta.x * mMouseSpeed, -moveDelta.y * mMouseSpeed);
				}
			}
			// 鼠标滚轮移动摄像机
			float mouseWheelDelta = mInputManager.getMouseWheelDelta();
			if (!MathUtility.isFloatZero(mouseWheelDelta))
			{
				move(Vector3.forward * mouseWheelDelta / 120.0f * 10.0f);
			}
		}
	}
	public override void notifyAddComponent(GameComponent component)
	{
		base.notifyAddComponent(component);
		// 如果是连接器,则还要加入连接器列表中
		if (component.getBaseType() == typeof(CameraLinker))
		{
			mLinkerList.Add(component.getName(), (CameraLinker)component);
		}
	}
	public override void notifyComponentDestroied(GameComponent component)
	{
		base.notifyComponentDestroied(component);
		// 如果是销毁的当前正在使用的连接器,则将记录的连接器清空
		if (mCurLinker == component)
		{
			mCurLinker = null;
		}
		if (mLinkerList.ContainsKey(component.getName()))
		{
			mLinkerList.Remove(component.getName());
		}
	}
	public void linkTarget(string linkerName, MovableObject target)
	{
		GameComponent com = null;
		if (mAllComponentList.ContainsKey(linkerName))
		{
			com = mAllComponentList[linkerName];
		}
		// 如果不是连接器则直接返回
		if (com != null && com.getBaseType() != typeof(CameraLinker))
		{
			return;
		}
		foreach (var item in mLinkerList)
		{
			// 如果使用了该连接器,则激活该连接器
			item.Value.setLinkObject(item.Value == com ? target : null);
			item.Value.setActive(item.Value == com && target != null);
		}
		// 如果是要断开当前连接器,则将连接器名字清空,否则记录当前连接器
		if(target != null && com != null)
		{
			mCurLinker = com as CameraLinker;
		}
		else
		{
			mCurLinker = null;
		}
	}
	public Camera getCamera(){ return mCamera;}
	public void setKeyProcess(bool process) { mKeyProcess = process; }
	public bool getKeyProcess() { return mKeyProcess; }
	public CameraLinker getCurLinker() { return mCurLinker; }
	public void setFOV(float fov) { mCamera.fieldOfView = fov; }
	public void copyCamera(GameObject obj)
	{
		copyObjectTransform(obj);
		Camera camera = obj.GetComponent<Camera>();
		mCamera.fieldOfView = camera.fieldOfView;
		mCamera.cullingMask = camera.cullingMask;
	}
}