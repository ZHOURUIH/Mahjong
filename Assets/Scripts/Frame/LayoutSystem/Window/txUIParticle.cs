using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class txUIParticle : txUIObject
{
	protected ParticleSystem mParticle;
	protected PLAY_STATE mState;
	public txUIParticle()
	{
		mType = UI_TYPE.UT_PARTICLE;
	}
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
		mParticle = go.GetComponent<ParticleSystem>();
		setBack(false);
	}
	public void setLoop(bool loop)
	{
		mParticle.loop = loop;
	}
	public void play()
	{
		mState = PLAY_STATE.PS_PLAY;
		mParticle.Play();
	}
	public void pause()
	{
		mState = PLAY_STATE.PS_PAUSE;
		mParticle.Pause();
	}
	public void stop()
	{
		mState = PLAY_STATE.PS_STOP;
		mParticle.Stop();
	}
	public void setPlayState(PLAY_STATE state)
	{
		if(mState == state)
		{
			return;
		}
		if(state == PLAY_STATE.PS_PLAY)
		{
			play();
		}
		else if(state == PLAY_STATE.PS_PAUSE)
		{
			pause();
		}
		else if(state == PLAY_STATE.PS_STOP)
		{
			stop();
		}
	}
	public void setBack(bool back)
	{
		UnityUtility.setGameObjectLayer(this, back ? "UIBackEffect" : "UIForeEffect");
	}
}