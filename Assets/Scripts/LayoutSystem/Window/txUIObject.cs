using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class txUIObject : ComponentOwner
{
	public UI_OBJECT_TYPE mType = UI_OBJECT_TYPE.UBT_BASE;
	public GameObject mObject;
	public AudioSource mAudioSource;
	public Transform mTransform;
	public GameLayout mLayout;
	public static int mIDSeed = 0;
	public int mID;
	public txUIObject()
		:
			base("")
	{
		mID = mIDSeed++;
	}
	~txUIObject()
	{
		if (mLayout != null)
		{
			mLayout.unregisterUIObject(this);
			mLayout = null;
		}
	}
	public override void destroy()
	{
		base.destroy();
		base.destroyAllComponents();
	}
	public virtual void init(GameLayout layout, GameObject go)
	{
		mLayout = layout;
		setGameObject(go);
		initComponents();
		if (mLayout != null)
		{
			mLayout.registerUIObject(this);
		}
	}
	public override void initComponents()
	{
		addComponent<WindowComponentAudio>("Audio");
		addComponent<WindowComponentScale>("Scale").setActive(false);
		addComponent<WindowComponentRotateToTarget>("RotateToTarget").setActive(false);
		addComponent<WindowComponentRotateSpeed>("RotateSpeed").setActive(false);
		addComponent<WindowComponentKeyFrameMove>("KeyFrameMove").setActive(false);
		addComponent<WindowComponentScaleTrembling>("ScaleTrembling").setActive(false);
		addComponent<WindowComponentAlphaTrembling>("AlphaTrembling").setActive(false);
		addComponent<WindowComponentAlpha>("Alpha").setActive(false);
		addComponent<WindowComponentMove>("Move").setActive(false);
		addComponent<WindowComponentKeyFrameRotate>("KeyFrameRotate").setActive(false);
		addComponent<WindowComponentHSL>("HSL").setActive(false);
		addComponent<WindowComponentSmoothSlider>("slider").setActive(false);
		addComponent<WindowComponentSmoothFillAmount>("fillAmount").setActive(false);
		addComponent<ComponentRotateFixed>("RotateFixed").setActive(false);
	}
	public Transform getTransform() { return mTransform; }
	public AudioSource getAudioSource() { return mAudioSource; }
	private void setGameObject(GameObject go)
	{
		setName(go.name);
		mObject = go;
		mTransform = mObject.transform;
		mAudioSource = go.GetComponent<AudioSource>();
		if (mAudioSource == null)
		{
			mAudioSource = go.AddComponent<AudioSource>();
		}
	}
	public virtual void update(float elapsedTime)
	{
		base.updateComponents(elapsedTime);
	}
	public void setActive(bool active) { mObject.SetActive(active); }
	public bool isActive() { return mObject.activeSelf; }
	public void setLocalScale(Vector2 scale) { mTransform.localScale = new Vector3(scale.x, scale.y, 1.0f); }
	public void setLocalPosition(Vector3 pos) { mTransform.localPosition = pos; }
	public void setLocalRotation(Vector3 rot) { mTransform.localEulerAngles = rot; }
	public void setWorldRotation(Vector3 rot) { mTransform.eulerAngles = rot; }
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
	public virtual void setAlpha(float alpha) { }
	public virtual float getAlpha() { return 1.0f; }
	public virtual void setFillPercent(float percent) { }
	public virtual float getFillPercent() { return 1.0f; }
}