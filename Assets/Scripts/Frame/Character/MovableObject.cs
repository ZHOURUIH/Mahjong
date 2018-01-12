using UnityEngine;
using System.Collections;

public class MovableObject : ComponentOwner
{
	protected Vector3 mAcceleration;
	protected Vector3 mLastSpeed;
	protected Vector3 mSpeed;
	protected Vector3 mLastPosition;
	protected GameObject mObject;
	protected Transform mTransform;
	protected bool mDestroyObject = true;
	public MovableObject(string name)
		:
		base(name)
	{
		;
	}
	public override void destroy()
	{
		base.destroy();
		base.destroyAllComponents();
		if(mDestroyObject)
		{
			UnityUtility.destroyGameObject(mObject);
		}
		mObject = null;
		mTransform = null;
	}
	public virtual void setObject(GameObject obj, bool destroyOld = true)
	{
		if(destroyOld && mObject != null)
		{
			UnityUtility.destroyGameObject(mObject);
			mObject = null;
		}
		mObject = obj;
		mObject.name = mName;
		mTransform = mObject.transform;
	}
	public virtual void init() 
	{
		initComponents(); 
	}
	public override void initComponents()
	{
		addComponent<MovableObjectComponentRotateSpeed>("RotateSpeed");
		addComponent<MovableObjectComponentRotateSpeedPhysics>("RotateSpeedPhysics");
		addComponent<MovableObjectComponentRotate>("Rotate");
		addComponent<MovableObjectComponentRotatePhysics>("RotatePhysics");
		addComponent<MovableObjectComponentRotateFixed>("RotateFixed");
		addComponent<MovableObjectComponentRotateFixedPhysics>("RotateFixedPhysics");
		addComponent<MovableObjectComponentMove>("Move");
		addComponent<MovableObjectComponentMovePhysics>("MovePhysics");
		addComponent<MovableObjectComponentScale>("Scale");
		addComponent<MovableObjectComponentTrackTarget>("TrackTarget");
		addComponent<MovableObjectComponentTrackTargetPhysics>("TrackTargetPhysics");
	}
	public virtual void update(float elapsedTime) 
	{ 
		base.updateComponents(elapsedTime);
	}
	public override void fixedUpdate(float elapsedTime)
	{
		base.fixedUpdate(elapsedTime);
		Vector3 curPos = mTransform.localPosition;
		mSpeed = (curPos - mLastPosition) / elapsedTime;
		mLastPosition = curPos;
		mAcceleration = (mSpeed - mLastSpeed) / elapsedTime;
		mLastSpeed = mSpeed;
	}
	// get
	//-------------------------------------------------------------------------------------------------------------------------
	public GameObject getObject() { return mObject; }
	public Vector3 getPosition()
	{
		return mTransform.localPosition;
	}
	public Vector3 getWorldPosition()
	{
		return mTransform.position;
	}
	public Vector3 getRotation()
	{
		return mTransform.localEulerAngles;
	}
	public Vector3 getScale()
	{
		return mTransform.localScale;
	}
	public Vector3 getSpeed() { return mSpeed; }
	public Vector3 getAcceleration() { return mAcceleration; }
	public Quaternion getQuaternionRotation()
	{
		Vector3 rot = getRotation();
		return Quaternion.Euler(rot.x, rot.y, rot.z);
	}
	public Vector3 getWorldRotation()
	{
		return mTransform.rotation.eulerAngles;
	}
	public string getLayer()
	{
		return LayerMask.LayerToName(mObject.layer);
	}
	public Transform getTransform() { return mTransform; }
	public bool getActive()
	{
		return mObject.activeSelf;
	}
	// set
	//-------------------------------------------------------------------------------------------------------------------------
	public virtual void setActive(bool active)
	{
		mObject.SetActive(active);
	}
	public virtual void setPosition(Vector3 position)
	{
		mTransform.localPosition = position;
	}
	public virtual void setWorldPosition(Vector3 position)
	{
		mTransform.position = position;
	}
	// 角度制的欧拉角,分别是绕xyz轴的旋转角度
	public virtual void setRotation(Vector3 eulerAngle)
	{
		mTransform.localEulerAngles = eulerAngle;
	}
	public void setWorldRotation(Vector3 eulerAngle)
	{
		mTransform.eulerAngles = eulerAngle;
	}
	public virtual void setScale(Vector3 scale)
	{
		mTransform.localScale = scale;
	}
	public virtual void move(Vector3 moveDelta, Space space = Space.Self)
	{
		if (space == Space.Self)
		{
			moveDelta = MathUtility.rotateVector3(moveDelta, getQuaternionRotation());
		}
		setPosition(getPosition() + moveDelta);
	}
	public void rotate(Vector3 rotation)
	{
		mTransform.Rotate(rotation, Space.Self);
	}
	public void rotateWorld(Vector3 rotation)
	{
		mTransform.Rotate(rotation, Space.World);
	}
	// angle为角度制
	public void rotateAround(Vector3 axis, float angle)
	{
		mTransform.Rotate(axis, angle, Space.Self);
	}
	public void rotateAroundWorld(Vector3 axis, float angle)
	{
		mTransform.Rotate(axis, angle, Space.World);
	}
	public void lookAt(Vector3 lookat)
	{
		setRotation(Quaternion.LookRotation(lookat).eulerAngles);
	}
	public void yawpitch(float fYaw, float fPitch)
	{
		Vector3 curRot = getRotation();
		curRot.x += fPitch;
		curRot.y += fYaw;
		setRotation(curRot);
	}
	public void resetLocalTransform()
	{
		mTransform.localPosition = Vector3.zero;
		mTransform.localEulerAngles = Vector3.zero;
		mTransform.localScale = Vector3.one;
	}
	public void resetRotation()
	{
		setRotation(Vector3.zero);
	}
	public void setParent(GameObject parent, bool resetTrans = true)
	{
		if (parent == null)
		{
			mTransform.parent = null;
		}
		else
		{
			mTransform.parent = parent.transform;
		}
		if(resetTrans)
		{
			resetLocalTransform();
		}
	}
	public void copyObjectTransform(GameObject obj)
	{
		Transform objTrans = obj.transform;
		ObjectTools.MOVE_OBJECT(this, objTrans.localPosition);
		ObjectTools.ROTATE_OBJECT(this, objTrans.localEulerAngles);
		ObjectTools.SCALE_OBJECT(this, objTrans.localScale);
	}
}