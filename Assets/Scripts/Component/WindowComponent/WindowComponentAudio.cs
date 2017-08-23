using UnityEngine;
using System;
using System.Collections;

public class WindowComponentAudio : ComponentAudio
{
	public WindowComponentAudio(Type type, string name)
		:
		base(type, name)
	{}
	//--------------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type) { return base.isType(type) || type == typeof(WindowComponentAudio); }
	protected override void assignAudioSource()
	{
		txUIObject window = mComponentOwner as txUIObject;
		AudioSource audioSource = window.getAudioSource();
		if(audioSource == null)
		{
			audioSource = window.createAudioSource();
		}
		setAudioSource(audioSource);
	}
	protected override void setSoundOwner()
	{
		mSoundOwner = CommonDefine.SOUND_OWNER_NAME[(int)SOUND_OWNER.SO_WINDOW];
	}
}