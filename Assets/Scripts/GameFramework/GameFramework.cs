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
	public static GameFramework		instance			= null;
	protected GameObject            mGameFrameObject    = null;
	protected ApplicationConfig		mApplicationConfig	= null;
	protected GameConfig			mGameConfig			= null;
	protected GameUtility			mGameUtility		= null;
	protected BinaryUtility			mBinaryUtility		= null;
	protected FileUtility			mFileUtility		= null;
	protected MathUtility			mMathUtility		= null;
	protected StringUtility			mStringUtility		= null;
	protected UnityUtility			mUnityUtility		= null;
	protected CommandSystem			mCommandSystem		= null;
	protected GameLayoutManager		mLayoutManager		= null;
	protected AudioManager			mAudioManager		= null;
	protected GameSceneManager		mGameSceneManager	= null;
	protected CharacterManager		mCharacterManager	= null;
	protected SocketManager			mSocketManager		= null;
	protected HttpServerManager		mHttpServerManager	= null;
	protected KeyFrameManager		mKeyFrameManager	= null;
	protected GlobalTouchSystem		mGlobalTouchSystem	= null;
	protected DllImportExtern		mDllImportExtern	= null;
	protected ShaderManager			mShaderManager		= null;
	protected DataBase				mDataBase			= null;
	protected MahjongSystem			mMahjongSystem		= null;
	protected CameraManager			mCameraManager		= null;
	protected ResourceManager		mResourcesManager	= null;
	protected LayoutPrefabManager	mLayoutPrefabManager = null;
	protected MaterialManager		mMaterialManager	= null;
	protected PlayerHeadManager		mPlayerHeadManager	= null;
	protected bool					mPauseFrame			= false; // 暂停整个程序
	public void Start()
	{
		Screen.SetResolution(1920, 1080, true);
		instance = this;
		mGameFrameObject = this.transform.gameObject;
		mApplicationConfig = new ApplicationConfig();
		mGameConfig = new GameConfig();
		mGameUtility = new GameUtility();
		mBinaryUtility = new BinaryUtility();
		mFileUtility = new FileUtility();
		mMathUtility = new MathUtility();
		mStringUtility = new StringUtility();
		mUnityUtility = new UnityUtility();
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
		mHttpServerManager = new HttpServerManager();
		mMahjongSystem = new MahjongSystem();
		mCameraManager = new CameraManager();
		mResourcesManager = new ResourceManager();
		mLayoutPrefabManager = new LayoutPrefabManager();
		mMaterialManager = new MaterialManager();
		mPlayerHeadManager = new PlayerHeadManager();

		// 所有类都构造完成后通知GameBase
		GameBase.notifyConstructDone();

		// 必须先初始化配置文件
		mApplicationConfig.init();
		int width = (int)mApplicationConfig.getFloatParam(GAME_DEFINE_FLOAT.GDF_SCREEN_WIDTH);
		int height = (int)mApplicationConfig.getFloatParam(GAME_DEFINE_FLOAT.GDF_SCREEN_HEIGHT);
		bool fullscreen = (int)mApplicationConfig.getFloatParam(GAME_DEFINE_FLOAT.GDF_FULL_SCREEN) != 0;
		Screen.SetResolution(width, height, fullscreen);
		mGameConfig.init();
		mResourcesManager.init();
		mShaderManager.init();
		mDllImportExtern.init();
		mGameUtility.init();
		mBinaryUtility.init();
		mFileUtility.init(); 
		mMathUtility.init();
		mStringUtility.init();
		mUnityUtility.init();
		mGlobalTouchSystem.init();
		mAudioManager.init();
		mLayoutManager.init();
		bool showDebug = mGameConfig.getFloatParam(GAME_DEFINE_FLOAT.GDF_SHOW_COMMAND_DEBUG_INFO) > 0.0f;
		mCommandSystem.init(showDebug);
		mGameSceneManager.init();
		mCharacterManager.init();
		mSocketManager.init();
		mKeyFrameManager.init();
		mDataBase.init();
		mMahjongSystem.init();
		mHttpServerManager.init("", "", "");
		mCameraManager.init();
		mLayoutPrefabManager.init();
		mMaterialManager.init();
		mPlayerHeadManager.init();

		CommandGameSceneManagerEnter cmd = mCommandSystem.newCmd<CommandGameSceneManagerEnter>(false, false);
		cmd.mSceneType = GAME_SCENE_TYPE.GST_START;
		mCommandSystem.pushCommand(cmd, getGameSceneManager());
	}
	public void Update()
	{
		try
		{
			float elapsedTime = Time.deltaTime;
			if (mPauseFrame)
			{
				return;
			}
			elapsedTime = 0.015f;
			mResourcesManager.update(elapsedTime);
			mGlobalTouchSystem.update(elapsedTime);
			mCommandSystem.update(elapsedTime);
			//mMahjongSystem.update(elapsedTime);
			mAudioManager.update(elapsedTime);
			mGameSceneManager.update(elapsedTime);
			mCharacterManager.update(elapsedTime);
			mSocketManager.update(elapsedTime);
			mLayoutManager.update(elapsedTime);
			mCameraManager.update(elapsedTime);
			keyProcess();
		}
		catch(Exception e)
		{
			UnityUtility.logError(e.Message + "stack : " + e.StackTrace);
		}
	}
	public void OnDestroy()
	{
		mLayoutPrefabManager.destroy();
		mMahjongSystem.destroy();
		mSocketManager.destroy();
		mCharacterManager.destroy();
		mGameSceneManager.destroy();
		mAudioManager.destroy();
		mLayoutManager.destroy();
		mCommandSystem.destroy();
		mGameConfig.destory();
		mKeyFrameManager.destroy();
		mHttpServerManager.destroy();
		mGlobalTouchSystem.destroy();
		mDllImportExtern.destroy();
		mShaderManager.destroy();
		mDataBase.destroy();
		mCameraManager.destroy();
		mResourcesManager.destroy();
		mMaterialManager.destroy();
		mApplicationConfig.destory();
		mPlayerHeadManager.destroy();
		mLayoutPrefabManager = null;
		mMahjongSystem = null;
		mGameConfig = null;
		mGameUtility = null;
		mBinaryUtility = null;
		mFileUtility = null;
		mMathUtility = null;
		mStringUtility = null;
		mUnityUtility = null;
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
		mResourcesManager = null;
		mMaterialManager = null;
		mApplicationConfig = null;
		mPlayerHeadManager = null;
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
			mCommandSystem.setShowDebugInfo(!mCommandSystem.getShowDebugInfo());
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
	public HttpServerManager getHttpServerManager() { return mHttpServerManager; }
	public GlobalTouchSystem getGlobalTouchSystem() { return mGlobalTouchSystem; }
	public ShaderManager getShaderManager() { return mShaderManager; }
	public DataBase getDataBase() { return mDataBase; }
	public CameraManager getCameraManager() { return mCameraManager; }
	public MahjongSystem getMahjongSystem() { return mMahjongSystem; }
	public ResourceManager getResourceManager() { return mResourcesManager; }
	public LayoutPrefabManager getLayoutPrefabManager() { return mLayoutPrefabManager; }
	public GameConfig getGameConfig() { return mGameConfig; }
	public MaterialManager getMaterialManager() { return mMaterialManager; }
	public PlayerHeadManager getPlayerHeadManager() { return mPlayerHeadManager; }
}