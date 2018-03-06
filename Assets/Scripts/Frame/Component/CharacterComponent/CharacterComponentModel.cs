using UnityEngine;
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
	public override void destroy()
	{
		if(mModel != null)
		{
			mObjectManager.destroyObject(mModel);
			mModel = null;
		}
		base.destroy();
	}
	public override void update(float elapsedTime)
	{
		base.update(elapsedTime);
	}
	public void setModel(GameObject model, string modelPath)
	{
		if(mModel != null)
		{
			UnityUtility.logError("model is not null! can not set again!");
			return;
		}
		mModelPath = modelPath;
		mModel = model;
		if (mModel != null)
		{
			mModel.SetActive(mActive);
			mAnimator = mModel.GetComponent<Animator>();
			mModelTransform = mModel.GetComponent<Transform>();
			mAnimation = mModel.GetComponent<Animation>();
		}
	}
	public GameObject getModel(){return mModel;}
	public string getModelPath(){return mModelPath;}
	public override void setActive(bool active)
	{
		base.setActive(active);
		if(mModel != null)
		{
			mModel.SetActive(active);
		}
	}
	//-----------------------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type){ return type == typeof(CharacterComponentModel);}
	protected override void setBaseType(){mBaseType = typeof(CharacterComponentModel);}
}