using UnityEngine;
using System;
using System.Collections.Generic;
#if !UNITY_5_3_5
using UnityEngine.Profiling;
#endif

/// <summary>
/// desc: 程序脚本的启动
///pc 端 可以用 resource 加载一些我们需要的东西
///StreamingAssets 在打包后路径会发生改变
/// Application.Quit(); 离开游戏 在PC端也可以适用。
/// </summary>
public class GameFramework : MonoBehaviour
{
	public static GameFramework		instance;
	protected Dictionary<string, FrameComponent> mFrameComponentMap;	// 存储框架组件,用于查找
	protected List<FrameComponent>	mFrameComponentList;				// 存储框架组件,用于初始化,更新,销毁
	protected GameObject			mGameFrameObject;
	protected bool					mPauseFrame;
	protected bool					mEnableKeyboard;
	protected int					mFPS;
	protected DateTime				mCurTime;
	protected int					mCurFrameCount;
	public void Start()
	{
		if (instance != null)
		{
			UnityUtility.logError("game framework can not start again!");
			return;
		}
		UnityUtility.logInfo("start game!", LOG_LEVEL.LL_FORCE);
		mFrameComponentMap = new Dictionary<string, FrameComponent>();
		mFrameComponentList = new List<FrameComponent>();
		try
		{
			start();
			notifyBase();
			registe();
			init();
		}
		catch(Exception e)
		{
			UnityUtility.logError("init failed! " + e.Message + ", stack : " + e.StackTrace);
		}
		// 初始化完毕后启动游戏
		launch();
		mCurTime = DateTime.Now;
	}
	public void Update()
	{
		try
		{
			++mCurFrameCount;
			DateTime now = DateTime.Now;
			if ((now - mCurTime).TotalMilliseconds >= 1000.0f)
			{
				mFPS = mCurFrameCount;
				UnityUtility.logInfo("FPS : " + mFPS, LOG_LEVEL.LL_HIGH);
				mCurFrameCount = 0;
				mCurTime = now;
			}
			float elapsedTime = Time.deltaTime;
			if (mPauseFrame)
			{
				return;
			}
			update(elapsedTime);
			keyProcess();
		}
		catch (Exception e)
		{
			UnityUtility.logError(e.Message + ", stack : " + e.StackTrace);
		}
	}
	public void FixedUpdate()
	{
		try
		{
			if (mPauseFrame)
			{
				return;
			}
			fixedUpdate(Time.fixedDeltaTime);
		}
		catch (Exception e)
		{
			UnityUtility.logError(e.Message + ", stack : " + e.StackTrace);
		}
	}
	public virtual void update(float elapsedTime)
	{
		int count = mFrameComponentList.Count;
		for (int i = 0; i < count; ++i)
		{
			Profiler.BeginSample(mFrameComponentList[i].getName());
			mFrameComponentList[i].update(elapsedTime);
			Profiler.EndSample();
		}
	}
	public virtual void fixedUpdate(float elapsedTime)
	{
		int count = mFrameComponentList.Count;
		for (int i = 0; i < count; ++i)
		{
			mFrameComponentList[i].fixedUpdate(elapsedTime);
		}
	}
	public void OnApplicationQuit()
	{
		destroy();
		UnityUtility.logInfo("程序退出完毕!", LOG_LEVEL.LL_FORCE);
	}
	public virtual void destroy()
	{
		int count = mFrameComponentList.Count;
		for (int i = 0; i < count; ++i)
		{
			mFrameComponentList[i].destroy();
			mFrameComponentList[i] = null;
		}
		mFrameComponentList.Clear();
		mFrameComponentMap.Clear();
	}
	public void stop()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}
	public virtual void keyProcess()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			stop();
		}
		else if(Input.GetKeyDown(KeyCode.D))
		{
			LOG_LEVEL level = UnityUtility.getLogLevel();
			int newLevel = ((int)level + 1) % (int)LOG_LEVEL.LL_MAX;
			UnityUtility.setLogLevel((LOG_LEVEL)newLevel);
		}
	}
	public T getSystem<T>()where T : FrameComponent
	{
		string name = typeof(T).ToString();
		if(mFrameComponentMap.ContainsKey(name))
		{
			return mFrameComponentMap[name] as T;
		}
		return null;
	}
	public void setPasueFrame(bool value) { mPauseFrame = value; }
	public bool getPasueFrame() { return mPauseFrame; }
	public GameObject getGameFrameObject() { return mGameFrameObject; }
	public bool getEnableKeyboard() { return mEnableKeyboard; }
	public int getFPS() { return mFPS; }
	//------------------------------------------------------------------------------------------------------
	protected virtual void notifyBase()
	{
		// 所有类都构造完成后通知FrameBase
		FrameBase frameBase = new FrameBase();
		frameBase.notifyConstructDone();
	}
	protected virtual void start()
	{
		mPauseFrame = false;
		instance = this;
		mGameFrameObject = gameObject;
		initComponent();
		// 物体管理器和资源管理器必须最后注册,以便最后销毁,作为最后的资源清理
		registeComponent<ObjectManager>();
		registeComponent<ResourceManager>();
	}
	protected virtual void init()
	{
		// 必须先初始化配置文件
		int count = mFrameComponentList.Count;
		for (int i = 0; i < count; ++i)
		{
			try
			{
				mFrameComponentList[i].init();
			}
			catch(Exception e)
			{
				UnityUtility.logError("init failed! : " + mFrameComponentList[i].getName() + ", info : " + e.Message + ", stack : " + e.StackTrace);
			}
		}
		System.Net.ServicePointManager.DefaultConnectionLimit = 200;
		int width = (int)FrameBase.mApplicationConfig.getFloatParam(GAME_DEFINE_FLOAT.GDF_SCREEN_WIDTH);
		int height = (int)FrameBase.mApplicationConfig.getFloatParam(GAME_DEFINE_FLOAT.GDF_SCREEN_HEIGHT);
		int fullscreen = (int)FrameBase.mApplicationConfig.getFloatParam(GAME_DEFINE_FLOAT.GDF_FULL_SCREEN);
		int screenCount = (int)FrameBase.mApplicationConfig.getFloatParam(GAME_DEFINE_FLOAT.GDF_SCREEN_COUNT);
		processScreen(width, height, screenCount, fullscreen);
		mEnableKeyboard = (int)FrameBase.mFrameConfig.getFloatParam(GAME_DEFINE_FLOAT.GDF_ENABLE_KEYBOARD) > 0;
	}
	protected virtual void registe() { }
	protected virtual void launch() { }
	protected virtual void initComponent()
	{
		registeComponent<ApplicationConfig>();
		registeComponent<FrameConfig>();
		registeComponent<UnityUtility>();
		registeComponent<HttpUtility>();
		registeComponent<DataBase>();
		registeComponent<CommandSystem>();
		registeComponent<CharacterManager>();
		registeComponent<GameLayoutManager>();
		registeComponent<AudioManager>();
		registeComponent<GameSceneManager>();
		registeComponent<KeyFrameManager>();
		registeComponent<GlobalTouchSystem>();
		registeComponent<DllImportExtern>();
		registeComponent<ShaderManager>();
		registeComponent<CameraManager>();
		registeComponent<LayoutSubPrefabManager>();
		registeComponent<InputManager>();
		registeComponent<SceneSystem>();
		registeComponent<GamePluginManager>();
	}
	protected void registeComponent<T>() where T : FrameComponent
	{
		string name = typeof(T).ToString();
		T component = UnityUtility.createInstance<T>(typeof(T), name);
		mFrameComponentMap.Add(name, component);
		mFrameComponentList.Add(component);
	}
	protected void setCameraTargetTexture(GameObject parent, string cameraName, UITexture renderTexture)
	{
		GameObject cameraObject = UnityUtility.getGameObject(parent, cameraName);
		Camera camera = cameraObject.GetComponent<Camera>();
		if (renderTexture != null)
		{
			camera.targetTexture = renderTexture.mainTexture as RenderTexture;
		}
		else
		{
			camera.targetTexture = null;
		}
	}
	protected void setTextureWindowSize(GameObject go, int positionX, int width, int height)
	{
		go.transform.localPosition = new Vector3((float)positionX, 0.0f, 0.0f);
		UIWidget widget0 = go.GetComponent<UIWidget>();
		widget0.width = width;
		widget0.height = height;
	}
	protected void setCameraPosition(string cameraName, GameObject parent, Vector3 pos)
	{
		GameObject camera = UnityUtility.getGameObject(parent, cameraName);
		camera.transform.localPosition = pos;
	}
	protected void processScreen(int width, int height, int screenCount, int fullScreen)
	{
#if UNITY_ANDROID || UNITY_IOS
		// 移动平台下固定为全屏
		fullScreen = 1;
		screenCount = 1;
#endif
		if (fullScreen == 1)
		{
			width = Screen.width;
			height = Screen.height;
		}
		Screen.SetResolution(width, height, fullScreen == 1);
		// 设置为无边框窗口
		if (fullScreen == 2)
		{
			User32.SetWindowLong(User32.GetForegroundWindow(), -16, CommonDefine.WS_POPUP | CommonDefine.WS_VISIBLE);
		}
		GameObject uiRootObj = UnityUtility.getGameObject(null, "NGUIRoot");
		GameObject rootTarget = UnityUtility.getGameObject(null, "UIRootTarget");
		if (screenCount == 1)
		{
			setCameraTargetTexture(null, "MainCamera", null);
			setCameraTargetTexture(uiRootObj, "UICamera", null);
			setCameraTargetTexture(uiRootObj, "UIBackEffectCamera", null);
			setCameraTargetTexture(uiRootObj, "UIForeEffectCamera", null);
			setCameraTargetTexture(uiRootObj, "UIBlurCamera", null);
			if(rootTarget != null)
			{
				rootTarget.SetActive(false);
			}
		}
		else
		{
			
			if (rootTarget == null)
			{
				return;
			}
			// 激活渲染目标
			GameObject camera = UnityUtility.getGameObject(rootTarget, "Camera");
			GameObject cameraTexture0 = UnityUtility.getGameObject(rootTarget, "UICameraTexture0");
			GameObject cameraTexture1 = UnityUtility.getGameObject(rootTarget, "UICameraTexture1");
			rootTarget.SetActive(true);
			camera.SetActive(true);
			cameraTexture0.SetActive(true);
			cameraTexture1.SetActive(true);
			// 设置渲染目标属性
			UIRoot uiRoot = rootTarget.GetComponent<UIRoot>();
			uiRoot.scalingStyle = UIRoot.Scaling.Constrained;
			uiRoot.manualWidth = width;
			uiRoot.manualHeight = height;
			camera.transform.localPosition = new Vector3(0.0f, 0.0f, -height / 2.0f);
			setTextureWindowSize(cameraTexture0, -width / screenCount, width / screenCount, height);
			setTextureWindowSize(cameraTexture1, -width / screenCount + width / 2, width / screenCount * (screenCount - 1), height);
			// 设置各个摄像机的渲染目标
			UITexture uiTexture = cameraTexture0.GetComponent<UITexture>();
			setCameraTargetTexture(null, "MainCamera", uiTexture);
			setCameraTargetTexture(uiRootObj, "UICamera", uiTexture);
			setCameraTargetTexture(uiRootObj, "UIBackEffectCamera", uiTexture);
			setCameraTargetTexture(uiRootObj, "UIForeEffectCamera", uiTexture);
			setCameraTargetTexture(uiRootObj, "UIBlurCamera", uiTexture);
		}
	}
}