using UnityEngine;
using System.Collections;

public class TrailSection
{
	public Vector3 mStart;		// 世界空间下的点
	public Vector3 mEnd;		// 世界空间下的点
	public float mLifeTime;
	public TrailSection(Vector3 start, Vector3 end, float life)
	{
		mStart = start;
		mEnd = end;
		mLifeTime = life;
	}
	public void update(float elapsedTime)
	{
		if(mLifeTime > 0.0f)
		{
			mLifeTime -= elapsedTime;
		}
	}
	public bool isDead()
	{
		return mLifeTime <= 0.0f;
	}
	public void move(Vector3 delta)
	{
		mStart += delta;
		mEnd += delta;
	}
}