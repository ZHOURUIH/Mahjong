using UnityEngine;
using System.Collections;

public class txNGUIEditbox : txNGUIStaticSprite
{
	protected UIInput mInput;
	public txNGUIEditbox()
	{
		mType = UI_TYPE.UT_NGUI_EDITBOX;
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
}
