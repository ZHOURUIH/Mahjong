using UnityEngine;
using System.Collections;

/// 不销毁物体
public class DontDestryed : MonoBehaviour
{
	void Start()
	{
		DontDestroyOnLoad(gameObject);
	}
}