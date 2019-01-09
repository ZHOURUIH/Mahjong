using UnityEngine;
using System.Collections;

public class txNGUIEditbox : txNGUISprite
{
	protected UIInput mInput;
	public txNGUIEditbox()
	{
		;
	}
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
		mInput = mObject.GetComponent<UIInput>();
		if (mInput == null)
		{
			mInput = mObject.AddComponent<UIInput>();
		}
	}
	public void setText(string text)
	{
		mInput.value = text;
	}
	public string getText()
	{
		return mInput.value;
	}
	public void cleanUp()
	{
		mInput.RemoveFocus();
		setText("");
	}
	public void setInputSubmitCallback(EventDelegate.Callback callback)
	{
		EventDelegate.Add(mInput.onSubmit, callback);
	}
	public void setOnValidateCallback(UIInput.OnValidate validate)
	{
		mInput.onValidate = validate;
	}
	public void setOnKeyUpCallback(UIInput.OnKeyDelegate onKeyDelegate)
	{
		mInput.onKeyUpCallback = onKeyDelegate;
	}
	public void setOnKeyDownUpCallback(UIInput.OnKeyDelegate onKeyDelegate)
	{
		mInput.onKeyDownCallback = onKeyDelegate;
	}
	public void setOnKeyKeepDownCallback(UIInput.OnKeyDelegate onKeyDelegate)
	{
		mInput.onKeyKeepDownCallback = onKeyDelegate;
	}
	public void setOnInputEventCallback(UIInput.OnInputEvent onInputEvent)
	{
		mInput.onInputEvent = onInputEvent;
	}
}
