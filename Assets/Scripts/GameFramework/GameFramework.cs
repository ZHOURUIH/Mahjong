using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Net;

/// <summary>
/// desc: 程序脚本的启动
///pc 端 可以用 resource 加载一些我们需要的东西
///StreamingAssets 在打包后路径会发生改变
/// Application.Quit(); 离开游戏 在PC端也可以适用。
/// </summary>
public class GameFramework : MonoBehaviour
{
	public static GameFramework		instance;
	protected GameObject			mGameFrameObject;
	protected ApplicationConfig		mApplicationConfig;
	protected BinaryUtility			mBinaryUtility;
	protected FileUtility			mFileUtility;
	protected MathUtility			mMathUtility;
	protected StringUtility			mStringUtility;
	protected UnityUtility			mUnityUtility;
	protected PluginUtility			mPluginUtility;
	protected CommandSystem			mCommandSystem;
	protected GameLayoutManager		mLayoutManager;
	protected AudioManager			mAudioManager;
	protected GameSceneManager		mGameSceneManager;
	protected CharacterManager		mCharacterManager;
	protected FrameConfig			mFrameConfig;
	protected SocketManager			mSocketManager;
	protected KeyFrameManager		mKeyFrameManager;
	protected GlobalTouchSystem		mGlobalTouchSystem;
	protected DllImportExtern		mDllImportExtern;
	protected ShaderManager			mShaderManager;
	protected DataBase				mDataBase;
	protected CameraManager			mCameraManager;
	protected ResourceManager		mResourceManager;
	protected LayoutPrefabManager	mLayoutPrefabManager;
	protected bool					mPauseFrame;
	public void Start()
	{
		UnityUtility.logInfo("start game!", LOG_LEVEL.LL_FORCE);
		start();
		notifyBase();
		registe();
		init();
		// 初始化完毕后启动游戏
		launch();
	}
	public virtual void start()
	{
		mPauseFrame = false;
		instance = this;
		mGameFrameObject = this.transform.gameObject;
		mApplicationConfig = new ApplicationConfig();
		mResourceManager = new ResourceManager();
		mBinaryUtility = new BinaryUtility();
		mFileUtility = new FileUtility();
		mMathUtility = new MathUtility();
		mStringUtility = new StringUtility();
		mUnityUtility = new UnityUtility();
		mPluginUtility = new PluginUtility();
		mCommandSystem = new CommandSystem();
		mLayoutManager = new GameLayoutManager();
		mAudioManager = new AudioManager();
		mGameSceneManager = new GameSceneManager();
		mCharacterManager = new CharacterManager();
		mSocketManager = new SocketManager();
		mKeyFrameManager = new KeyFrameManager();
		mGlobalTouchSystem = new GlobalTouchSystem();
		mDllImportExtern = new DllImportExtern();
		mShaderManager = new ShaderManager();
		mDataBase = new DataBase();
		mCameraManager = new CameraManager();
		mLayoutPrefabManager = new LayoutPrefabManager();
		mFrameConfig = new FrameConfig();
	}
	public virtual void registe(){}
	public virtual void init()
	{
		// 必须先初始化配置文件
		mApplicationConfig.init();
		mDllImportExtern.init();
		mFrameConfig.init();
		mDataBase.init();
		mResourceManager.init();
		mShaderManager.init();
		mBinaryUtility.init();
		mFileUtility.init();
		mMathUtility.init();
		mStringUtility.init();
		mUnityUtility.init();
		mPluginUtility.init();
		mGlobalTouchSystem.init();
		mAudioManager.init();
		mLayoutManager.init();
		bool showDebug = (int)mFrameConfig.getFloatParam(GAME_DEFINE_FLOAT.GDF_SHOW_COMMAND_DEBUG_INFO) != 0;
		mCommandSystem.init(showDebug);
		mGameSceneManager.init();
		mCharacterManager.init();
		mSocketManager.init();
		mKeyFrameManager.init();
		mCameraManager.init();
		mLayoutPrefabManager.init();
		System.Net.ServicePointManager.DefaultConnectionLimit = 200;
		int width = (int)mApplicationConfig.getFloatParam(GAME_DEFINE_FLOAT.GDF_SCREEN_WIDTH);
		int height = (int)mApplicationConfig.getFloatParam(GAME_DEFINE_FLOAT.GDF_SCREEN_HEIGHT);
		int fullscreen = (int)mApplicationConfig.getFloatParam(GAME_DEFINE_FLOAT.GDF_FULL_SCREEN);
		Screen.SetResolution(width, height, fullscreen == 1);
		int screenCount = (int)mApplicationConfig.getFloatParam(GAME_DEFINE_FLOAT.GDF_SCREEN_COUNT);
		processResolution(width, height, screenCount);
		// 设置为无边框窗口
		if (fullscreen == 2)
		{
			User32.SetWindowLong(User32.GetForegroundWindow(), -16, CommonDefine.WS_POPUP | CommonDefine.WS_VISIBLE);
		}
	}
	public virtual void notifyBase()
	{
		// 所有类都构造完成后通知FrameBase
		FrameBase frameBase = new FrameBase();
		frameBase.notifyConstructDone();
	}
	public virtual void launch() { }
	public void Update()
	{
		try
		{
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
	public virtual void update(float elapsedTime)
	{
		mResourceManager.update(elapsedTime);
		mGlobalTouchSystem.update(elapsedTime);
		mCommandSystem.update(elapsedTime);
		mAudioManager.update(elapsedTime);
		mGameSceneManager.update(elapsedTime);
		mCharacterManager.update(elapsedTime);
		mSocketManager.update(elapsedTime);
		mLayoutManager.update(elapsedTime);
		mCameraManager.update(elapsedTime);
	}
	public void OnDestroy()
	{
		destroy();
		UnityUtility.logInfo("程序退出完毕!", LOG_LEVEL.LL_FORCE);
	}
	public virtual void destroy()
	{
		mLayoutPrefabManager.destroy();
		mSocketManager.destroy();
		mCharacterManager.destroy();
		mGameSceneManager.destroy();
		mAudioManager.destroy();
		mLayoutManager.destroy();
		mCommandSystem.destroy();
		mKeyFrameManager.destroy();
		mGlobalTouchSystem.destroy();
		mDllImportExtern.destroy();
		mShaderManager.destroy();
		mDataBase.destroy();
		mCameraManager.destroy();
		mPluginUtility.destroy();
		mResourceManager.destroy();         // 资源管理器必须最后销毁,作为最后的资源清理
		mLayoutPrefabManager = null;
		mResourceManager = null;
		mBinaryUtility = null;
		mFileUtility = null;
		mMathUtility = null;
		mStringUtility = null;
		mUnityUtility = null;
		mPluginUtility = null;
		mCommandSystem = null;
		mLayoutManager = null;
		mAudioManager = null;
		mGameSceneManager = null;
		mCharacterManager = null;
		mSocketManager = null;
		mKeyFrameManager = null;
		mGlobalTouchSystem = null;
		mDllImportExtern = null;
		mShaderManager = null;
		mDataBase = null;
		mCameraManager = null;
	}
	public void stop()
	{
		Application.Quit();
	}
	public void keyProcess()
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
	public void setPasueFrame(bool value) { mPauseFrame = value; }
	public bool getPasueFrame() { return mPauseFrame; }
	public CommandSystem getCommandSystem() { return mCommandSystem; }
	public GameLayoutManager getLayoutManager() { return mLayoutManager; }
	public AudioManager getAudioManager() { return mAudioManager; }
	public GameSceneManager getGameSceneManager() { return mGameSceneManager; }
	public CharacterManager getCharacterManager() { return mCharacterManager; }
	public SocketManager getSocketManager() { return mSocketManager; }
	public KeyFrameManager getKeyFrameManager() { return mKeyFrameManager; }
	public GameObject getGameFrameObject() { return mGameFrameObject; }
	public FrameConfig getFrameConfig() { return mFrameConfig; }
	public GlobalTouchSystem getGlobalTouchSystem() { return mGlobalTouchSystem; }
	public ShaderManager getShaderManager() { return mShaderManager; }
	public DataBase getDataBase() { return mDataBase; }
	public CameraManager getCameraManager() { return mCameraManager; }
	public ResourceManager getResourceManager() { return mResourceManager;}
	public LayoutPrefabManager getLayoutPrefabManager() { return mLayoutPrefabManager; }
	public ApplicationConfig getApplicationConfig() { return mApplicationConfig; }
	public void setCameraTargetTexture(GameObject parent, string cameraName, UITexture renderTexture)
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
	public void setTextureWindowSize(GameObject go, int positionX, int width, int height)
	{
		go.transform.localPosition = new Vector3((float)positionX, 0.0f, 0.0f);
		UIWidget widget0 = go.GetComponent<UIWidget>();
		widget0.width = width;
		widget0.height = height;
	}
	public void setCameraPosition(string cameraName, GameObject parent, Vector3 pos)
	{
		GameObject camera = UnityUtility.getGameObject(parent, cameraName);
		camera.transform.localPosition = pos;
	}
	protected void processResolution(int width, int height, int screenCount)
	{
		GameObject uiRootObj = UnityUtility.getGameObject(null, "UI Root");
		GameObject rootTarget = UnityUtility.getGameObject(null, "UIRootTarget");
		if (screenCount == 1)
		{
			setCameraTargetTexture(null, "MainCamera", null);
			setCameraTargetTexture(uiRootObj, "UICamera", null);
			setCameraTargetTexture(uiRootObj, "UIBackEffectCamera", null);
			setCameraTargetTexture(uiRootObj, "UIForeEffectCamera", null);
			setCameraTargetTexture(uiRootObj, "UIBlurCamera", null);
			rootTarget.SetActive(false);
		}
		else
		{
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