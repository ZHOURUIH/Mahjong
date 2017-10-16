using UnityEngine;
using System.Collections;

public class txUIEditbox : txUIStaticSprite
{
	protected UIInput mInput;
	public txUIEditbox()
	{
		mType = UI_OBJECT_TYPE.UBT_EDITBOX;
	}
	public override void init(GameLayout layout, GameObject go)
	{
		base.init(layout, go);
		mInput = mObject.GetComponent<UIInput>();
		if (mInput == null)
		{
			mInput = mObject.AddComponent<UIInput>();
		}
	}
	public void setText(string text)
	{
		mInput.label.text = text;
	}
	public string getText()
	{
		return mInput.label.text;
	}
}
