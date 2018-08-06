using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum CHECK_DIRECTION
{
	CD_DOWN,        // 向下碰撞检测,检测到碰撞后,摄像机向上移动
	CD_UP,
	CD_LEFT,
	CD_RIGHT,
	CD_FORWARD,
	CD_BACK,
}

public class CheckLayer
{
	public int mLayerIndex;
	public CHECK_DIRECTION mDirection;
	public float mCheckDistance;
	public float mMinDistance;
	public Vector3 mDirectionVector;
	public CheckLayer(int layerIndex, CHECK_DIRECTION direction, float checkDistance, float minDistance)
	{
		mLayerIndex = layerIndex;
		mDirection = direction;
		mCheckDistance = checkDistance;
		mMinDistance = minDistance;
		if (direction == CHECK_DIRECTION.CD_DOWN)
		{
			mDirectionVector = Vector3.down;
		}
		else if (direction == CHECK_DIRECTION.CD_UP)
		{
			mDirectionVector = Vector3.up;
		}
		else if (direction == CHECK_DIRECTION.CD_LEFT)
		{
			mDirectionVector = Vector3.left;
		}
		else if (direction == CHECK_DIRECTION.CD_RIGHT)
		{
			mDirectionVector = Vector3.right;
		}
		else if (direction == CHECK_DIRECTION.CD_FORWARD)
		{
			mDirectionVector = Vector3.forward;
		}
		else if (direction == CHECK_DIRECTION.CD_BACK)
		{
			mDirectionVector = Vector3.back;
		}
	}
}

public class CameraLinkerSmoothFollow : CameraLinker
{
	protected float mSpeedRecover = 0.5f;
	protected float mNormalSpeed = 5.0f;
	protected float mFollowPositionSpeed = 5.0f;
	protected bool mIgnoreY = false;      // 是否忽略Y轴的变化,当Y轴变化时摄像机在Y轴上的位置不会根据时间改变
	protected List<CheckLayer> mCheckLayer;     // 当摄像机碰撞到某些层的时候,需要移动摄像机的目标位置,避免穿插
	protected Dictionary<CHECK_DIRECTION, List<CheckLayer>> mCheckDirectionList;   // 对于任意层可以进行多个方向的检测,但是同一层在同一方向不能多次检测
	public CameraLinkerSmoothFollow(Type type, string name)
		: base(type, name)
	{
		mCheckLayer = new List<CheckLayer>();
		mCheckDirectionList = new Dictionary<CHECK_DIRECTION, List<CheckLayer>>();
		mFollowPositionSpeed = mNormalSpeed;
	}
	public override void init(ComponentOwner owner)
	{
		base.init(owner);
	}
	public void setFollowPositionSpeed(float speed) { mFollowPositionSpeed = speed; }
	public void setIgnoreY(bool ignore) { mIgnoreY = ignore; }
	public bool getIgnoreY() { return mIgnoreY; }
	public void setNormalSpeed(float speed) { mNormalSpeed = speed; }
	public float getNormalSpeed() { return mNormalSpeed; }
	public void addCheckLayer(int layer, CHECK_DIRECTION direction, float checkDistance, float minDistance)
	{
		CheckLayer checkLayer = new CheckLayer(layer, direction, checkDistance, minDistance);
		mCheckLayer.Add(checkLayer);
		if (!mCheckDirectionList.ContainsKey(direction))
		{
			mCheckDirectionList.Add(direction, new List<CheckLayer>());
		}
		mCheckDirectionList[direction].Add(checkLayer);
	}
	public void removeCheckLayer(int layer, CHECK_DIRECTION direction)
	{
		if (mCheckDirectionList.ContainsKey(direction))
		{
			CheckLayer checkLayer = null;
			List<CheckLayer> layerList = mCheckDirectionList[direction];
			int count = layerList.Count;
			for (int i = 0; i < count; ++i)
			{
				if (layerList[i].mLayerIndex == layer)
				{
					checkLayer = layerList[i];
					layerList.RemoveAt(i);
					break;
				}
			}
			if (checkLayer != null)
			{
				mCheckLayer.Remove(checkLayer);
			}
		}
	}
	//---------------------------------------------------------------------------------------------------------
	protected override bool isType(Type type) { return base.isType(type) || type == typeof(CameraLinkerSmoothFollow); }
	protected override void updateLinker(float elapsedTime)
	{
		if (!MathUtility.isFloatEqual(mNormalSpeed, mFollowPositionSpeed))
		{
			mFollowPositionSpeed = MathUtility.lerp(mFollowPositionSpeed, mNormalSpeed, mSpeedRecover * elapsedTime);
		}
		Vector3 targetPos = mLinkObject.getWorldPosition();
		Vector3 relative = MathUtility.rotateVector3(mRelativePosition, mLinkObject.getRotation().y * Mathf.Deg2Rad);
		Vector3 nextPos = targetPos + relative;
		// 判断与地面的交点,使摄像机始终位于地面上方
		if (mCheckLayer != null && mCheckLayer.Count > 0)
		{
			// 从摄像机目标点检测
			foreach (var item in mCheckDirectionList)
			{
				List<CheckLayer> layerList = item.Value;
				int count = layerList.Count;
				for (int i = 0; i < count; ++i)
				{
					Ray ray = new Ray(nextPos - layerList[i].mDirectionVector, layerList[i].mDirectionVector);
#if UNITY_EDITOR
					Debug.DrawLine(ray.origin, ray.origin + ray.direction * layerList[i].mCheckDistance, Color.blue);
#endif
					RaycastHit hit;
					if (Physics.Raycast(ray, out hit, layerList[i].mCheckDistance, 1 << layerList[i].mLayerIndex))
					{
						// 如果有碰撞到物体,交点距离在一定范围内
						Vector3 hitPoint = ray.origin + ray.direction * hit.distance;
						if (MathUtility.getLength(nextPos - hitPoint) < layerList[i].mMinDistance)
						{
							nextPos = hitPoint - layerList[i].mDirectionVector * layerList[i].mMinDistance;
						}
					}
				}
			}
		}
		// 得到摄像机当前位置
		Vector3 cameraNewPos = MathUtility.lerp(mCamera.getPosition(), nextPos, mFollowPositionSpeed * elapsedTime, 0.01f);
		applyRelativePosition(cameraNewPos - targetPos);
	}
};