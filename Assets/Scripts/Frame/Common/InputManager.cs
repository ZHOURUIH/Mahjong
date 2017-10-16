using UnityEngine;
using System.Collections;

public class InputManager
{
	#region 单例的配置类
	private static InputManager _instance = null;

	public static InputManager GetInstance()
	{
		if (_instance == null)
		{
			_instance = new InputManager();
		}
		return _instance;
	}

	#endregion
	public bool confirmDown(bool ui = true)
	{
		if (ui && UICamera.hoveredObject != null)
		{
			return false;
		}
#if (UNITY_EDITOR|| UNITY_WEBPLAYER || UNITY_STANDALONE_WIN)
		return Input.GetMouseButtonDown(0);
#elif (UNITY_ANDROID || UNITY_IPHONE)
       if(Input.touchCount > 0)
       {
           return Input.GetTouch(0).phase == TouchPhase.Began;
       }else
       {
           return false;
       }
#endif
	}
	public bool confirmUp()
	{

#if (UNITY_EDITOR||UNITY_WEBPLAYER || UNITY_STANDALONE_WIN)
		return Input.GetMouseButtonUp(0);
#elif (UNITY_ANDROID || UNITY_IPHONE)
       if(Input.touchCount > 0)
       {
           return Input.GetTouch(0).phase == TouchPhase.Ended;
       }else
       {
           return false;
       }
#endif
	}
	public bool confirmClick()
	{
		if (UICamera.hoveredObject != null)
			return false;

#if (UNITY_EDITOR||UNITY_WEBPLAYER || UNITY_STANDALONE_WIN)
		return Input.GetMouseButton(0);
#elif (UNITY_ANDROID || UNITY_IPHONE)
       if(Input.touchCount > 0)
       {
           return Input.GetTouch(0).phase == TouchPhase.Ended;
       }
       else
       {
           return false;
       }
#endif
	}
	public Vector3 confirmPostion()
	{
#if (UNITY_WEBPLAYER || UNITY_STANDALONE_WIN)
		return Input.mousePosition;
#elif (UNITY_ANDROID || UNITY_IPHONE)
		if (Input.touchCount > 0)
		{
			return new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
		}
		else
		{
			return Vector3.zero;
		}
#endif
	}
	public bool twoTouchPointDown()
	{
#if (UNITY_ANDROID || UNITY_IPHONE)
		if (Input.touchCount == 2)
		{
			if ((Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Stationary) && Input.GetTouch(1).phase == TouchPhase.Began)
			{
				return true;
			}
		}
#endif
		return false;
	}
	public Vector3[] getTwoTouchPostion()
	{
#if (UNITY_ANDROID || UNITY_IPHONE)
		if (Input.touchCount == 2)
		{
			Vector3[] vector3 = new Vector3[2];
			vector3[0] = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
			vector3[1] = new Vector3(Input.GetTouch(1).position.x, Input.GetTouch(1).position.y);
			return vector3;
		}
		else
		{
			return null;
		}
#else
		return null;
#endif
	}
	public bool twoTouchPointMove()
	{
#if (UNITY_ANDROID || UNITY_IPHONE)
		if (Input.touchCount == 2)
		{
			if (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved)
			{
				return true;
			}
		}
#endif
		return false;
	}
	public bool twoTouchPointUp()
	{
#if (UNITY_ANDROID || UNITY_IPHONE)
		if (Input.touchCount == 2)
		{
			if (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(1).phase == TouchPhase.Ended)
			{
				return true;
			}
		}
#endif
		return false;
	}
	public GameObject getRaycastGameObject(Camera startCamera, Vector3 point)
	{
		Ray ray = startCamera.ScreenPointToRay(point);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit))
		{
			return hit.transform.gameObject;
		}
		return null;
	}
	public Vector3 getRaycastPoint(Camera startCamera, Vector3 point)
	{
		Ray ray = startCamera.ScreenPointToRay(point);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit))
		{
			return hit.point;
		}
		return new Vector3();
	}
}
