using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class WindowComponentAudio : ComponentAudio
{
	public static Dictionary<SOUND_DEFINE, float> mVolumeCoe;
	public WindowComponentAudio(Type type, string name)
		:
		base(type, name)
	{
		if (mVolumeCoe == null)
		{
			mVolumeCoe = new Dictionary<SOUND_DEFINE, float>();
			int dataCount = mDataBase.getDataCount(DATA_TYPE.DT_GAME_SOUND);
			for (int i = 0; i < dataCount; ++i)
			{
				DataGameSound gameSound = mDataBase.queryData(DATA_TYPE.DT_GAME_SOUND, i) as DataGameSound;
				if (!mVolumeCoe.ContainsKey((SOUND_DEFINE)gameSound.mSoundID))
				{
					mVolumeCoe.Add((SOUND_DEFINE)gameSound.mSoundID, gameSound.mVolumeCoe);
				}
			}
		}
	}
	//--------------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type) { return base.isType(type) || type == typeof(WindowComponentAudio); }
	protected override void assignAudioSource()
	{
		txUIObject window = mComponentOwner as txUIObject;
		AudioSource audioSource = window.getAudioSource();
		if (audioSource == null)
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