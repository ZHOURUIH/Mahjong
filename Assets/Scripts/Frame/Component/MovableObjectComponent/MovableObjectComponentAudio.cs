using UnityEngine;
using System;
using System.Collections;

public class MovableObjectComponentAudio : ComponentAudio
{
	public MovableObjectComponentAudio(Type type, string name)
		:
		base(type, name)
	{}
	//------------------------------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type){return base.isType(type) || type == typeof(GameSceneComponentAudio);}
	protected override void assignAudioSource()
	{
		MovableObject movableObject = mComponentOwner as MovableObject;
		AudioSource audioSource = movableObject.getAudioSource();
		if (audioSource == null)
		{
			audioSource = movableObject.createAudioSource();
		}
		setAudioSource(audioSource);
	}
}