using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class txUIObject : ComponentOwner
{
	protected UI_OBJECT_TYPE mType = UI_OBJECT_TYPE.UBT_BASE;
	protected AudioSource mAudioSource;
	protected Transform mTransform;
	protected BoxCollider mBoxCollider;
	protected static int mIDSeed = 0;
	protected bool mPassRay = true;
	protected bool mMouseHovered = false;
	protected txUIObject mParent;
	public GameLayout mLayout;
	public GameObject mObject;
	public int mID;
	public txUIObject()
		:
		base("")
	{
		mID = mIDSeed++;
	}
	public override void destroy()
	{
		base.destroy();
		base.destroyAllComponents();
		if (mLayout != null)
		{
			mLayout.unregisterUIObject(this);
			mLayout = null;
		}
		GameObject.Destroy(mObject);
		mObject = null;
	}
	public virtual void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		mLayout = layout;
		mParent = parent;
		setGameObject(go);
		initComponents();
		if (mLayout != null)
		{
			mLayout.registerUIObject(this);
		}
		mAudioSource = mObject.GetComponent<AudioSource>();
		mBoxCollider = mObject.GetComponent<BoxCollider>();
	}
	public override void initComponents()
	{
		addComponent<WindowComponentAudio>("Audio");
		addComponent<WindowComponentRotateSpeed>("RotateSpeed").setActive(false);
		addComponent<WindowComponentKeyFrameMove>("KeyFrameMove").setActive(false);
		addComponent<WindowComponentScaleTrembling>("ScaleTrembling").setActive(false);
		addComponent<WindowComponentAlphaTrembling>("AlphaTrembling").setActive(false);
		addComponent<WindowComponentKeyFrameRotate>("KeyFrameRotate").setActive(false);
		addComponent<WindowComponentSmoothSlider>("slider").setActive(false);
		addComponent<WindowComponentSmoothFillAmount>("fillAmount").setActive(false);
		addComponent<ComponentRotateFixed>("RotateFixed").setActive(false);
		addComponent<WindowComponentHSLTrembling>("HSLTrembling").setActive(false);
		addComponent<WindowComponentDrag>("Drag").setActive(false);
	}
	public AudioSource createAudioSource()
	{
		mAudioSource = mObject.AddComponent<AudioSource>();
		return mAudioSource;
	}
	public virtual void update(float elapsedTime)
	{
		base.updateComponents(elapsedTime);
	}
	public bool Raycast(Ray ray, out RaycastHit hitInfo, float maxDistance)
	{
		return mBoxCollider.Raycast(ray, out hitInfo, maxDistance);
	}
	//get
	//-------------------------------------------------------------------------------------------------------------------------------------
	public void setParent(txUIObject parent) { mParent = parent; }
	public UI_OBJECT_TYPE getUIType() { return mType; }
	public Transform getTransform() { return mTransform; }
	public BoxCollider getBoxCollider() { return mBoxCollider; }
	public AudioSource getAudioSource() { return mAudioSource; }
	public bool isActive() { return mObject.activeSelf; }
	public Vector3 getRotationEuler()
	{
		Vector3 vector3 = mTransform.localEulerAngles;
		MathUtility.adjustAngle180(ref vector3.z, false);
		return vector3;
	}
	public Vector3 getRotationRadian()
	{
		Vector3 vector3 = mTransform.localEulerAngles * 0.0055f;
		MathUtility.adjustAngle180(ref vector3.z, true);
		return vector3;
	}
	public Vector3 getPosition() { return mTransform.localPosition; }
	public Vector3 getWorldPosition() { return mTransform.position; }
	public Vector2 getScale() { return new Vector2(mTransform.localScale.x, mTransform.localScale.y); }
	public Vector2 getWorldScale()
	{
		Vector3 scale = MathUtility.getMatrixScale(mTransform.localToWorldMatrix);
		Vector3 uiRootScale = mLayoutManager.getUIRoot().getTransform().localScale;
		Vector3 mTransformScale = mTransform.localScale;
		return new Vector3(scale.x * mTransformScale.x / uiRootScale.x, scale.y * mTransformScale.y / uiRootScale.y, scale.z * mTransformScale.z / uiRootScale.z); ;
	}
	public int getChildCount() { return mTransform.childCount; }
	public GameObject getChild(int index) { return mTransform.GetChild(index).gameObject; }
	public virtual float getAlpha() { return 1.0f; }
	public virtual float getFillPercent() { return 1.0f; }
	public virtual int getDepth() { return 0; }
	public virtual bool getHandleInput()
	{
		return mBoxCollider != null && mBoxCollider.enabled;
	}
	public bool getPassRay() { return mPassRay; }
	public bool getMouseHovered() { return mMouseHovered; }
	//set
	//-------------------------------------------------------------------------------------------------------------------------------------
	public txUIObject getParent() { return mParent; }
	private void setGameObject(GameObject go)
	{
		setName(go.name);
		mObject = go;
		mTransform = mObject.transform;
	}
	public virtual void setDepth(int depth)
	{
		mGlobalTouchSystem.notifyButtonDepthChanged(this, depth);
	}
	public virtual void setHandleInput(bool enable)
	{
		if(mBoxCollider != null)
		{
			mBoxCollider.enabled = enable;
		}
	}
	public void setActive(bool active) { mObject.SetActive(active); }
	public void setLocalScale(Vector2 scale) { mTransform.localScale = new Vector3(scale.x, scale.y, 1.0f); }
	public void setLocalPosition(Vector3 pos) { mTransform.localPosition = pos; }
	public void setLocalRotation(Vector3 rot) { mTransform.localEulerAngles = rot; }
	public void setWorldRotation(Vector3 rot) { mTransform.eulerAngles = rot; }
	public void setWorldPosition(Vector3 pos) { mTransform.position = pos; }
	public virtual void setAlpha(float alpha) { }
	public virtual void setFillPercent(float percent) { }
	public void setPassRay(bool pass) { mPassRay = pass; }
	public void setMouseHovered(bool hover) { mMouseHovered = hover; }
	public void setClickCallback(UIEventListener.VoidDelegate callback){UIEventListener.Get(mObject).onClick = callback;}
	public void setHoverCallback(UIEventListener.BoolDelegate callback){UIEventListener.Get(mObject).onHover = callback;}
	public void setPressCallback(UIEventListener.BoolDelegate callback){UIEventListener.Get(mObject).onPress = callback;}
}