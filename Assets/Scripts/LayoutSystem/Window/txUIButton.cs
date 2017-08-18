using UnityEngine;
using System.Collections;

public class txUIButton : txUIStaticSprite
{
	protected UIButton	  mButton;
	protected BoxCollider mBoxCollider;
	protected bool		  mHandleInput = true;
	protected bool		  mPassRay = true;
	public txUIButton()
	{
		mType = UI_OBJECT_TYPE.UBT_BUTTON;
	}
	public override void init(GameLayout layout, GameObject go)
	{
		base.init(layout, go);
		mButton = mObject.GetComponent<UIButton>();
		if (mButton == null)
		{
			mButton = mObject.AddComponent<UIButton>();
		}
		mBoxCollider = mObject.GetComponent<BoxCollider>();
		if(mBoxCollider == null)
		{
			mBoxCollider = mObject.AddComponent<BoxCollider>();
		}
	}
	public void setClickCallback(UIEventListener.VoidDelegate callback)
	{
		UIEventListener.Get(mObject).onClick = callback;
	}
	public void setHoverCallback(UIEventListener.BoolDelegate callback)
	{
		UIEventListener.Get(mObject).onHover = callback;
	}
	public void setPressCallback(UIEventListener.BoolDelegate callback) 
	{
		UIEventListener.Get(mObject).onPress = callback;
	}
	public BoxCollider getBoxCollider(){return mBoxCollider;}
	// 当按钮需要改变透明度或者附加颜色变化时,需要禁用按钮的颜色渐变
	public void setFadeColour(bool fade){mButton.mFadeColour = fade;}
	public UIButton getButton() {return mButton;}
	public void setHandleInput(bool enable)
	{
		mHandleInput = enable;
		mBoxCollider.enabled = enable;
	}
	public bool getHandleInput(){return mHandleInput && mBoxCollider.enabled;}
	public void setPassRay(bool pass) { mPassRay = pass; }
	public bool getPassRay() { return mPassRay; }
	public override void setDepth(int depth)
	{
		base.setDepth(depth);
		mGlobalTouchSystem.notifyButtonDepthChanged(this, depth);
	}
	public bool Raycast(Ray ray, out RaycastHit hitInfo, float maxDistance)
	{
		return mBoxCollider.Raycast(ray, out hitInfo, maxDistance);
	}
}