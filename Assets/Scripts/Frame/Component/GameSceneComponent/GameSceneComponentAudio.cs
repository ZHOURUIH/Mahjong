using UnityEngine;
using System;
using System.Collections;

public class GameSceneComponentAudio : ComponentAudio
{
	public GameSceneComponentAudio(Type type, string name)
		:
		base(type, name)
	{}
	//------------------------------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type){return base.isType(type) || type == typeof(GameSceneComponentAudio);}
	protected override void assignAudioSource()
	{
		GameScene gameScene = mComponentOwner as GameScene;
		AudioSource audioSource = gameScene.getAudioSource();
		if (audioSource == null)
		{
			audioSource = gameScene.createAudioSource();
		}
		setAudioSource(audioSource);
	}
	protected override void setSoundOwner()
	{
		mSoundOwner = CommonDefine.SOUND_OWNER_NAME[(int)SOUND_OWNER.SO_SCENE];
	}
}