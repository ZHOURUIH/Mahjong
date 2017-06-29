using UnityEngine;
using System;
using System.Collections;

public class GameSceneComponentAudio : ComponentAudio
{
	public GameSceneComponentAudio(Type type, string name)
		:
		base(type, name)
	{}
	public override void init(ComponentOwner owner)
	{
		base.init(owner);
		GameScene gameScene = owner as GameScene;
		setAudioSource(gameScene.getAudioSource());
	}
	public override void setSoundOwner()
	{
		mSoundOwner = CommonDefine.SOUND_OWNER_NAME[(int)SOUND_OWNER.SO_GAME_SCENE];
	}
	protected override bool isType(Type type){return base.isType(type) || type == typeof(GameSceneComponentAudio);}
}