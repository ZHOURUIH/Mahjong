using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 带状拖尾,有两种方式,一种是主动拖尾,一种是被动拖尾
// 主动拖尾是物体移动时,自动根据轨迹计算拖尾,适用于移动的物体
// 被动拖尾是设置拖尾的相对移动,模拟物体移动效果,适用于静止的物体
public class RibbonTrail
{
	protected GameObject mObject;
	protected Transform mTransform;
	protected Mesh mMesh;
	protected List<TrailSection> mSectionList;
	protected int mMaxSectionCount = 64;
	protected float mTrailLifeTime = 1.0f;
	protected Color mStartColor = Color.white;
	protected Color mEndColor = new Color(1.0f, 1.0f, 1.0f, 0.0f);
	protected float mMinDistance = 2.0f;
	protected float mTrailHeight = 1.0f;
	protected float mTopWidth = 0.12f;
	public RibbonTrail()
	{
		mSectionList = new List<TrailSection>();
	}
	public virtual void init(GameObject obj)
	{
		mObject = obj;
		mTransform = mObject.GetComponent<Transform>();
		MeshFilter filter = mObject.GetComponent<MeshFilter>();
		mMesh = filter.mesh;
	}
	public virtual void update(float elapsedTime)
	{
		// 先将模型数据清空
		mMesh.Clear();
		// 更新所有片段的生命周期
		for (int i = 0; i < mSectionList.Count; )
		{
			TrailSection section = mSectionList[mSectionList.Count - 1 - i];
			section.update(elapsedTime);
			if(section.isDead())
			{
				mSectionList.RemoveAt(mSectionList.Count - 1 - i);
			}
			else
			{
				++i;
			}
		}
		int sectionCount = mSectionList.Count;
		if(sectionCount < 2)
		{
			return;
		}
		// 计算顶点,纹理坐标,颜色
		Vector3[] vertices = new Vector3[sectionCount * 2 + sectionCount * 2];
		Vector2[] uv = new Vector2[sectionCount * 2 + sectionCount * 2];
		Color[] colors = new Color[sectionCount * 2 + sectionCount * 2];
		// 每个点到前一个点的距离
		float[] disList = new float[sectionCount];
		// 计算光带总长度
		float totalDis = 0.0f;
		for (int i = 0; i < sectionCount; ++i)
		{
			int preIndex = i - 1;
			MathUtility.clamp(ref preIndex, 0, sectionCount - 1);
			disList[i] = (mSectionList[i].mStart - mSectionList[preIndex].mStart).magnitude;
			totalDis += disList[i];
		}
		
		float curDis = 0.0f;
		for (int i = 0; i < sectionCount; ++i)
		{
			curDis += disList[i];
			// 竖直的光带
			vertices[2 * i + 0] = mTransform.worldToLocalMatrix.MultiplyPoint(mSectionList[i].mStart);
			vertices[2 * i + 1] = mTransform.worldToLocalMatrix.MultiplyPoint(mSectionList[i].mEnd);
			float u = 1.0f - curDis / totalDis;
			uv[2 * i + 0] = new Vector2(u, 0.0f);
			uv[2 * i + 1] = new Vector2(u, 0.45f);
			colors[2 * i + 0] = MathUtility.lerp(mStartColor, mEndColor, u);
			colors[2 * i + 1] = colors[2 * i + 0];
			// 顶部横向光带
			// 横向的点坐标
			Vector3 v0, v1, v2;
			v0 = mSectionList[i].mEnd - mSectionList[i].mStart;
			if (i == 0)
			{
				v1 = mSectionList[i].mEnd - mSectionList[i + 1].mEnd;
			}
			else
			{
				v1 = mSectionList[i - 1].mEnd - mSectionList[i].mEnd;
			}
			v2 = Vector3.Cross(v0, v1);
			v2.Normalize();
			int startIndex = sectionCount * 2 + 2 * i;
			vertices[startIndex + 0] = mTransform.worldToLocalMatrix.MultiplyPoint(mSectionList[i].mEnd - v2 * mTopWidth * 0.5f - new Vector3(0.0f, 0.03f, 0.0f));
			vertices[startIndex + 1] = mTransform.worldToLocalMatrix.MultiplyPoint(mSectionList[i].mEnd + v2 * mTopWidth * 0.5f - new Vector3(0.0f, 0.03f, 0.0f));
			uv[startIndex + 0] = new Vector2(u, 0.5f);
			uv[startIndex + 1] = new Vector2(u, 1.0f);
			colors[startIndex + 0] = MathUtility.lerp(mStartColor, mEndColor, u);
			colors[startIndex + 1] = MathUtility.lerp(mStartColor, mEndColor, u);
		}
		// 计算顶点索引,每两个点之间两个三角面，每个三角面三个顶点,顶部横向的片是每段有4个三角形
		int[] triangles = new int[(sectionCount - 1) * 6 + (sectionCount - 1) * 6];
		for (int i = 0; i < sectionCount - 1; ++i)
		{
			int startIndex = i * 6;
			int indexValue = i * 2;
			triangles[startIndex + 0] = indexValue + 0;
			triangles[startIndex + 1] = indexValue + 1;
			triangles[startIndex + 2] = indexValue + 2;
			triangles[startIndex + 3] = indexValue + 2;
			triangles[startIndex + 4] = indexValue + 1;
			triangles[startIndex + 5] = indexValue + 3;
		}
		for (int i = 0; i < sectionCount - 1; ++i)
		{
			int startIndex = (sectionCount - 1) * 6 + i * 6;
			int indexValue = sectionCount * 2 + i * 2;
			triangles[startIndex + 0] = indexValue + 0;
			triangles[startIndex + 1] = indexValue + 1;
			triangles[startIndex + 2] = indexValue + 2;
			triangles[startIndex + 3] = indexValue + 2;
			triangles[startIndex + 4] = indexValue + 1;
			triangles[startIndex + 5] = indexValue + 3;
		}
		// 将顶点数据设置到模型中
		mMesh.vertices = vertices;
		mMesh.colors = colors;
		mMesh.uv = uv;
		mMesh.triangles = triangles;
	}
	public virtual void destroy()
	{
		mSectionList.Clear();
	}
	public void addSection(Vector3 start, Vector3 end)
	{
		// 数量达到最大时,移除末尾的元素
		if(mSectionList.Count >= mMaxSectionCount)
		{
			mSectionList.RemoveAt(mSectionList.Count - 1);
		}
		mSectionList.Add(new TrailSection(start, end, mTrailLifeTime));
	}
	public void setTrialLifeTime(float time)
	{
		mTrailLifeTime = time;
	}
}