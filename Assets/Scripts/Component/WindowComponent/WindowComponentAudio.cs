using UnityEngine;
using System;
using System.Collections;

public class WindowComponentAudio : ComponentAudio
{
	public WindowComponentAudio(Type type, string name)
		:
		base(type, name)
	{}
	public override void init(ComponentOwner owner)
	{
		base.init(owner);
		txUIObject uiObj = owner as txUIObject;
		setAudioSource(uiObj.getAudioSource());
	}
	public override void setSoundOwner()
	{
		mSoundOwner = CommonDefine.SOUND_OWNER_NAME[(int)SOUND_OWNER.SO_WINDOW];
	}
	//--------------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type) { return base.isType(type) || type == typeof(WindowComponentAudio); }
}