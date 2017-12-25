using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ComponentKeyFramePhysics : ComponentKeyFrameBase
{
	public ComponentKeyFramePhysics(Type type, string name)
		:
		base(type, name)
	{
		;
	}
	public override void fixedUpdate(float elapsedTime)
	{
		base.fixedUpdate(elapsedTime);
		if (mKeyFrame != null && mPlayState == PLAY_STATE.PS_PLAY)
		{
			mCurrentTime += elapsedTime;
			mPlayedTime += elapsedTime;

			bool done = false;
			// 无限播放当前震动
			if (mPlayLength < 0.0f)
			{
				if (mCurrentTime > mOnceLength)
				{
					mCurrentTime = 0.0f;
				}
			}
			// 播放固定长度的震动
			else
			{
				// 超过时间则停止,暂时不播放最后一帧
				if (mPlayedTime > mPlayLength)
				{
					done = true;
					mCurrentTime = mOffset + mPlayLength;
				}
				else if (mCurrentTime > mOnceLength)
				{
					mCurrentTime = 0.0f;
				}
			}

			if (mKeyFrame != null)
			{
				float value = mKeyFrame.Evaluate(mCurrentTime / mOnceLength) * mAmplitude;
				applyTrembling(value);
				afterApllyTrembling(done);
			}
		}
	}
	//--------------------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type) { return base.isType(type) || type == typeof(ComponentKeyFramePhysics); }
}