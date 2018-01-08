﻿using UnityEngine;
using System;
using System.Collections;

public class CharacterComponentModel : GameComponent
{
	protected GameObject mModel;
	public Transform mModelTransform;
	public Animator mAnimator;
	public Animation mAnimation;
	protected string mModelPath;
	public CharacterComponentModel(Type type, string name)
		:
		base(type, name)
	{ }
	public override void init(ComponentOwner owner)
	{
		base.init(owner);
	}
	public override void update(float elapsedTime)
	{
		base.update(elapsedTime);
		//// 将模型自身的位置属性同步到角色(因为动作会自动改变模型的位置,需要同步给角色),同时重置模型自身的位置
		//Character character = mComponentOwner as Character;
		//character.setPosition(mModel.transform.localPosition);
		//mModel.transform.localPosition = Vector3.zero;
	}
	public void setModel(GameObject model, string modelPath, bool destroyOld = true)
	{
		if(destroyOld && mModel != null)
		{
			mModelManager.destroyModel(mModel.name);
		}
		mModelPath = modelPath;
		mModel = model;
		if (mModel != null)
		{
			mModel.SetActive(mActive);
			mAnimator = mModel.GetComponent<Animator>();
			mModelTransform = mModel.GetComponent<Transform>();
			mAnimation = mModel.GetComponent<Animation>();
			mModelTransform.parent = (mComponentOwner as Character).getTransform();
		}
	}
	public GameObject getModel()
	{
		return mModel;
	}
	public string getModelPath()
	{
		return mModelPath;
	}
	public override void setActive(bool active)
	{
		base.setActive(active);
		if(mModel != null)
		{
			mModel.SetActive(active);
		}
	}
	public void setScale(Vector3 scale) { mModelTransform.localScale = scale; }
	public void setRotation(Vector3 euler) { mModelTransform.localEulerAngles = euler; }
	//-----------------------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type){ return type == typeof(CharacterComponentModel);}
	protected override void setBaseType(){mBaseType = typeof(CharacterComponentModel);}
}