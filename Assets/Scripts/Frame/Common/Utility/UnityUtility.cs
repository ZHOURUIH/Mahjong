using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
#if UNITY_STANDALONE_WIN
using Microsoft.Win32;
using System.Windows.Forms;
#endif
using UnityEngine;
using System.Diagnostics;

// 日志等级
public enum LOG_LEVEL
{
	LL_FORCE,   // 强制显示
	LL_HIGH,    // 高
	LL_NORMAL,  // 正常
	LL_MAX,
}

public class UnityUtility : FrameComponent
{
	protected static GameCamera mForeEffectCamera;
	protected static GameCamera mBackEffectCamera;
	protected static LOG_LEVEL mLogLevel;
	protected static bool mShowMessageBox = true;
	protected static int mIDMaker;
	public UnityUtility(string name)
		:base(name)
	{
		;
	}
	public override void init()
	{
		mForeEffectCamera = mCameraManager.getCamera("UIForeEffectCamera");
		mBackEffectCamera = mCameraManager.getCamera("UIBackEffectCamera");
#if UNITY_EDITOR
		setLogLevel(LOG_LEVEL.LL_NORMAL);
#else
		setLogLevel((LOG_LEVEL)(int)mFrameConfig.getFloatParam(GAME_DEFINE_FLOAT.GDF_LOG_LEVEL));
#endif
	}
	public static void setLogLevel(LOG_LEVEL level)
	{
		mLogLevel = level;
		logInfo("log level : " + mLogLevel, LOG_LEVEL.LL_FORCE);
	}
	public static LOG_LEVEL getLogLevel()
	{
		return mLogLevel;
	}
	public static new void logError(string info, bool isMainThread = true)
	{
		if (isMainThread && mShowMessageBox)
		{
			messageBox(info, true);
			// 运行一次只显示一次提示框,避免在循环中报错时一直弹窗
			mShowMessageBox = false;
		}
		string trackStr = new StackTrace().ToString();
		if (mLocalLog != null)
		{
			mLocalLog.log("error : " + info + ", stack : " + trackStr);
		}
#if UNITY_EDITOR
		UnityEngine.Debug.LogError("error : " + info + ", stack : " + trackStr);
		UnityEditor.EditorApplication.isPaused = true;
#endif
		// 游戏中的错误日志
		if (mFrameLogSystem != null)
		{
			mFrameLogSystem.logGameError(info);
		}	
	}
	public static new void logInfo(string info, LOG_LEVEL level = LOG_LEVEL.LL_NORMAL)
	{
		if ((int)level <= (int)mLogLevel)
		{
#if UNITY_EDITOR
			UnityEngine.Debug.Log(getTime() + " : " + info);
#endif
			if(mLocalLog != null)
			{
				mLocalLog.log(getTime() + " : " + info);
			}
		}
	}
	public static string getTime()
	{
		DateTime time = DateTime.Now;
		return time.Hour + ":" + time.Minute + ":" + time.Second + ":" + time.Millisecond;
	}
	public static void copyTextToClipbord(string str)
	{
#if UNITY_STANDALONE_WIN
		Clipboard.SetText(str);
#endif
	}
	public static string getTextFromClipboard()
	{
#if UNITY_STANDALONE_WIN
		return Clipboard.GetText();
#else
		return "";
#endif
	}
	public static void messageBox(string info, bool errorOrInfo)
	{
		string title = errorOrInfo ? "错误" : "提示";
		// 在编辑器中显示对话框
#if UNITY_EDITOR
		UnityEditor.EditorUtility.DisplayDialog(title, info, "确认");
#elif UNITY_STANDALONE_WIN
		// 游戏运行过程中显示窗口提示框
		MessageBox.Show(info, title, MessageBoxButtons.OK, errorOrInfo ? MessageBoxIcon.Error : MessageBoxIcon.Information);
#endif
	}
	public static Ray getRay(Vector2 screenPos)
	{
		Camera camera = mCameraManager.getUICamera().getCamera();
		return camera.ScreenPointToRay(screenPos);
	}
	public static List<txUIObject> raycast(Ray ray, SortedDictionary<UIDepth, List<txUIObject>> buttonList, int maxCount = 0)
	{
		bool cast = true;
		List<txUIObject> retList = new List<txUIObject>();
		RaycastHit hit = new RaycastHit();
		foreach (var box in buttonList)
		{
			int count = box.Value.Count;
			for (int i = 0; i < count; ++i)
			{
				txUIObject window = box.Value[i];
				BoxCollider collider = window.getBoxCollider();
				if (window.getHandleInput() && collider.Raycast(ray, out hit, 10000.0f))
				{
					retList.Add(window);
					// 如果射线不能穿透当前按钮,或者已经达到最大数量,则不再继续
					if (!window.getPassRay() || maxCount > 0 && retList.Count >= maxCount)
					{
						cast = false;
						break;
					}
				}
			}
			if (!cast)
			{
				break;
			}
		}
		return retList;
	}
	public static void destroyGameObject(UnityEngine.Object obj, bool immediately = false, bool allowDestroyAssets = false)
	{
		if(immediately)
		{
			GameObject.DestroyImmediate(obj, allowDestroyAssets);
		}
		else
		{
			GameObject.Destroy(obj);
		}
	}
	public static new GameObject getGameObject(GameObject parent, string name, bool errorIfNull = false)
	{
		GameObject go = null;
		if (parent == null)
		{
			go = GameObject.Find(name);
		}
		else
		{
			Transform trans = parent.transform.Find(name);
			if (trans != null)
			{
				go = trans.gameObject;
			}
			// 如果父节点的第一级子节点中找不到,就递归查找
			else
			{
				int childCount = parent.transform.childCount;
				for(int i = 0; i < childCount; ++i)
				{
					go = getGameObject(parent.transform.GetChild(i).gameObject, name, false);
					if (go != null)
					{
						break;
					}
				}
			}
		}
		if(go == null && errorIfNull)
		{
			string file = getCurSourceFileName(2);
			int line = getLineNum(2);
			logError("can not find " + name + ". file : " + file + ", line : " + line);
		}
		return go;
	}
	public static GameObject cloneObject(GameObject oriObj, string name)
	{
		GameObject obj = GameObject.Instantiate(oriObj);
		obj.name = name;
		return obj;
	}
	public static GameObject createObject(string name, GameObject parent = null)
	{
		GameObject obj = new GameObject(name);
		if(parent != null)
		{
			obj.GetComponent<Transform>().SetParent(parent.GetComponent<Transform>());
		}
		return obj;
	}
	// 根据预设名实例化
	public static GameObject instantiatePrefab(GameObject parent, string prefabName, string name, Vector3 scale, Vector3 rot, Vector3 pos)
	{
		GameObject prefab = mResourceManager.loadResource<GameObject>(prefabName, true);
		if (prefab == null)
		{
			logError("can not find prefab : " + prefabName);
			return null;
		}
		GameObject obj = instantiatePrefab(parent, prefab, name, scale, rot, pos);
		return obj;
	}
	// prefabName为Resource下的相对路径
	public static GameObject instantiatePrefab(GameObject parent, string prefabName)
	{
		string name = StringUtility.getFileName(prefabName);
		return instantiatePrefab(parent, prefabName, name, Vector3.one, Vector3.zero, Vector3.zero);
	}
	// 根据预设对象实例化
	public static GameObject instantiatePrefab(GameObject parent, GameObject prefab)
	{
		string name = StringUtility.getFileName(prefab.name);
		return instantiatePrefab(parent, prefab, name, Vector3.one, Vector3.zero, Vector3.zero);
	}
	public static GameObject instantiatePrefab(GameObject parent, GameObject prefab, string name)
	{
		return instantiatePrefab(parent, prefab, name, Vector3.one, Vector3.zero, Vector3.zero);
	}
	public static GameObject instantiatePrefab(GameObject parent, string prefabName, string name)
	{
		return instantiatePrefab(parent, prefabName, name, Vector3.one, Vector3.zero, Vector3.zero);
	}
	// parent为实例化后挂接的父节点
	// prefabName为预设名,带Resources下相对路径
	// name为实例化后的名字
	// 其他三个是实例化后本地的变换
	public static GameObject instantiatePrefab(GameObject parent, GameObject prefab, string name, Vector3 scale, Vector3 rot, Vector3 pos)
	{
		GameObject obj = GameObject.Instantiate(prefab);
		setNormalProperty(ref obj, parent, name, scale, rot, pos);
		return obj;
	}
	public static void setNormalProperty(ref GameObject obj, GameObject parent, string name, Vector3 scale, Vector3 rot, Vector3 pos)
	{
		if(parent != null)
		{
			obj.transform.SetParent(parent.transform);
		}
		obj.transform.localPosition = pos;
		obj.transform.localEulerAngles = rot;
		obj.transform.localScale = scale;
		obj.transform.name = name;
	}
	public static T createInstance<T>(Type classType, params object[] param) where T : class
	{
		return Activator.CreateInstance(classType, param) as T;
	}
	public static Vector2 screenPosToEffectPos(Vector2 screenPos, Vector2 parentWorldPos, Vector2 parentWorldScale, float effectDepth, bool isBack, bool isRelative = true, bool isNGUI = true)
	{
		GameCamera camera = isBack ? mBackEffectCamera : mForeEffectCamera;
		if (isRelative)
		{
			// 转到世界坐标系下,首先计算UI根节点的缩放值
			txUIObject root = isNGUI ? mLayoutManager.getNGUIRoot() : mLayoutManager.getUGUIRoot();
			Vector3 scale = root.getScale();
			screenPos.x = screenPos.x / scale.x;
			screenPos.y = screenPos.y / scale.y;
			parentWorldPos.x = parentWorldPos.x / scale.x;
			parentWorldPos.y = parentWorldPos.y / scale.y;
		}
		Vector2 pos = (effectDepth / -camera.getPosition().z + 1) * screenPos - parentWorldPos;
		return new Vector2(pos.x / parentWorldScale.x, pos.y / parentWorldScale.y);
	}
	public static Vector3 worldPosToScreenPos(Vector3 worldPos)
	{
		Camera camera = mCameraManager.getMainCamera().getCamera();
		return camera.WorldToScreenPoint(worldPos);
	}
	public static bool whetherGameObjectInScreen(Vector3 worldPos)
	{
		Vector3 screenPos = worldPosToScreenPos(worldPos);
		return screenPos.z >= 0.0f && 
			(screenPos.x > 0 && screenPos.x < UnityEngine.Screen.currentResolution.width) && 
			(screenPos.y > 0 && screenPos.y < UnityEngine.Screen.currentResolution.height);
	}
	public static Vector2 screenPosToWindowPos(Vector2 screenPos, txUIObject parent, bool screenCenterOsZero = false, bool isNGUI = true)
	{
		Camera camera = mCameraManager.getUICamera().getCamera();
		screenPos.x = screenPos.x / camera.pixelWidth * UnityEngine.Screen.currentResolution.width;
		screenPos.y = screenPos.y / camera.pixelHeight * UnityEngine.Screen.currentResolution.height;
		Vector3 parentWorldPosition = parent != null ? parent.getWorldPosition() : Vector3.zero;
		txUIObject root = isNGUI ? mLayoutManager.getNGUIRoot() : mLayoutManager.getUGUIRoot();
		Vector3 scale = root.getScale();
		parentWorldPosition.x = parentWorldPosition.x / scale.x;
		parentWorldPosition.y = parentWorldPosition.y / scale.y;
		Vector2 parentWorldScale = parent != null ? parent.getWorldScale() : Vector2.one;
		Vector2 pos = new Vector2(screenPos.x - parentWorldPosition.x, screenPos.y - parentWorldPosition.y);
		Vector2 windowPos = new Vector2(pos.x / parentWorldScale.x, pos.y / parentWorldScale.y);
		if(screenCenterOsZero)
		{
			windowPos -= new Vector2(UnityEngine.Screen.currentResolution.width / 2.0f, UnityEngine.Screen.currentResolution.height / 2.0f);
		}
		return windowPos;
	}
	public static void setGameObjectLayer(txUIObject obj, string layerName)
	{
		int layer = LayerMask.NameToLayer(layerName);
		if (layer < 0 || layer >= 32)
		{
			return;
		}
		setGameObjectLayer(obj, layer);
	}
	public static void setGameObjectLayer(txUIObject obj, int layer)
	{
		GameObject go = obj.mObject;
		go.layer = layer;
		Transform[] childTransformList = go.transform.GetComponentsInChildren<Transform>(true);
		foreach (Transform t in childTransformList)
		{
			t.gameObject.layer = layer;
		}
	}
	// preFrameCount为1表示返回调用getLineNum的行号
	public static int getLineNum(int preFrameCount = 1)
	{
		StackTrace st = new StackTrace(preFrameCount, true);
	    return st.GetFrame(0).GetFileLineNumber();
	}
	// preFrameCount为1表示返回调用getCurSourceFileName的文件名
	public static string getCurSourceFileName(int preFrameCount = 1)
	{
		StackTrace st = new StackTrace(preFrameCount, true);
	    return st.GetFrame(0).GetFileName();
	}
	public static int makeID() { return ++mIDMaker; }
	public static void notifyIDUsed(int id)
	{
		mIDMaker = Mathf.Max(mIDMaker, id);
	}
	public static Sprite texture2DToSprite(Texture2D tex)
	{
		return Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
	}
	// 从注册表中获取注册码,如果注册码不存在,则自动创建
	public static string readRegistryValue(string path, string keyName)
	{
		string value = "";
#if UNITY_STANDALONE_WIN
		RegistryKey key = Registry.CurrentUser.CreateSubKey(path);
		string codeValue = key.GetValue(keyName) as string;
		if (codeValue != null)
		{
			value = key.GetValue(keyName) as string;
		}
		key.Close();
#endif
		return value;
	}
	// 读取注册表
	public static void writeRegistryValue(string path, string value, string keyName)
	{
#if UNITY_STANDALONE_WIN
		RegistryKey key = Registry.CurrentUser.CreateSubKey(path);
		key.SetValue(keyName, value);
		key.Close();
#endif
	}
}