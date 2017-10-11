using UnityEngine;
using System.Collections;

public class MovableObject : ComponentOwner
{
	protected GameObject mObject;
	public Vector3 mSpeed;
	public MovableObject(string name)
		:
		base(name)
	{
		mObject = new GameObject(name);
	}
	public override void destroy()
	{
		base.destroy();
		base.destroyAllComponents();
		GameObject.Destroy(mObject);
		mObject = null;
	}
	public GameObject getObject()
	{
		return mObject;
	}
	public virtual void init() 
	{
		initComponents(); 
	}
	public override void initComponents()
	{
		;
	}

	public virtual void update(float elapsedTime) 
	{ 
		base.updateComponents(elapsedTime);
		Vector3 curPos = getRotation();
		curPos += mSpeed;
		setPosition(curPos);
	}

	public virtual void setPosition(Vector3 position)
	{
		mObject.transform.localPosition = position;
	}
	// 角度制的欧拉角,分别是绕xyz轴的旋转角度
	public virtual void setRotation(Vector3 eulerAngle)
	{
		mObject.transform.localEulerAngles = eulerAngle;
	}
	public virtual void setScale(Vector3 scale)
	{
		mObject.transform.localScale = scale;
	}
	public Vector3 getPosition()
	{
		return mObject.transform.localPosition;
	}
	public Vector3 getRotation()
	{
		return mObject.transform.localEulerAngles;
	}
	public Vector3 getScale()
	{
		return mObject.transform.localScale;
	}
	public void rotate(Vector3 rotation)
	{
		mObject.transform.Rotate(rotation, Space.Self);
	}
	public void rotateWorld(Vector3 rotation)
	{
		mObject.transform.Rotate(rotation, Space.World);
	}
	// angle为角度制
	public void rotateAround(Vector3 axis, float angle)
	{
		mObject.transform.Rotate(axis, angle, Space.Self);
	}
	public void rotateAroundWorld(Vector3 axis, float angle)
	{
		mObject.transform.Rotate(axis, angle, Space.World);
	}
	public Vector3 getSpeed() { return mSpeed; }
	public void setSpeed(Vector3 speed) { mSpeed = speed; }
}