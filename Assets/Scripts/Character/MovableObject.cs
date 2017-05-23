using UnityEngine;
using System.Collections;
using System;

public class MovableObject : ComponentOwner
{
	protected GameObject mGameObject;
	protected Transform mTransform;
	public MovableObject(string name)
		:
		base(name)
	{
		mGameObject = new GameObject(name);
		mTransform = mGameObject.GetComponent<Transform>();
	}
	public override void destroy()
	{
		base.destroy();
		base.destroyAllComponents();
	}
	public virtual void init() { initComponents(); }
	public override void initComponents(){}

	public virtual void update(float elapsedTime) { base.updateComponents(elapsedTime); }
	public GameObject getObject()
	{
		return mGameObject;
	}
	public void setParent(GameObject parent)
	{
		mTransform.parent = parent.transform;
	}
	public virtual void setPosition(Vector3 position)
	{
		mTransform.localPosition = position;
	}
	// 角度制的欧拉角
	public virtual void setRotation(Vector3 rotation)
	{
		mTransform.localEulerAngles = rotation;
	}
	public virtual void setScale(Vector3 scale)
	{
		mTransform.localScale = scale;
	}
	public Vector3 getPosition()
	{
		return mTransform.localPosition;
	}
	public Vector3 getRotation()
	{
		return mTransform.localEulerAngles;
	}
	public Vector3 getScale()
	{
		return mTransform.localScale;
	}
}