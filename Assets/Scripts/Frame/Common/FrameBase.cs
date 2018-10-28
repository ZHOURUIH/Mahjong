using UnityEngine;
using System.Collections;

// 管理类初始化完成调用
// 这个父类的添加是方便代码的书写
// 继承FileUtility是为了在调用工具函数时方便,把四个完全独立的工具函数类串起来继承,所有继承自FrameBase的类都可以直接访问四大工具类中的函数
public class FrameBase : FileUtility
{
	// FrameComponent
	public static GameFramework				mGameFramework			= null;
	public static CommandSystem				mCommandSystem			= null;
	public static AudioManager				mAudioManager			= null;
	public static GameSceneManager			mGameSceneManager		= null;
	public static CharacterManager			mCharacterManager		= null;
	public static GameLayoutManager			mLayoutManager			= null;
	public static KeyFrameManager			mKeyFrameManager		= null;
	public static GlobalTouchSystem			mGlobalTouchSystem		= null;
	public static ShaderManager				mShaderManager			= null;
	public static SQLite					mSQLite					= null;
	public static DataBase					mDataBase				= null;
	public static CameraManager				mCameraManager			= null;
	public static ResourceManager			mResourceManager		= null;
	public static LayoutSubPrefabManager	mLayoutSubPrefabManager	= null;
	public static ApplicationConfig			mApplicationConfig		= null;
	public static FrameConfig				mFrameConfig			= null;
	public static ObjectManager				mObjectManager			= null;
	public static InputManager				mInputManager			= null;
	public static SceneSystem				mSceneSystem			= null;
	public static IFrameLogSystem			mFrameLogSystem			= null;
	public static ClassObjectPool			mClassObjectPool		= null;
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
	public static LocalLog					mLocalLog				= null;
#endif
	// SQLiteTable
	public static SQLiteSound				mSQLiteSound			= null;
	public virtual void notifyConstructDone()
	{
		if (mGameFramework == null)
		{
			mGameFramework = GameFramework.instance;
			mCommandSystem = mGameFramework.getSystem<CommandSystem>();
			mAudioManager = mGameFramework.getSystem<AudioManager>();
			mGameSceneManager = mGameFramework.getSystem<GameSceneManager>();
			mCharacterManager = mGameFramework.getSystem<CharacterManager>();
			mLayoutManager = mGameFramework.getSystem<GameLayoutManager>();
			mKeyFrameManager = mGameFramework.getSystem<KeyFrameManager>();
			mGlobalTouchSystem = mGameFramework.getSystem<GlobalTouchSystem>();
			mShaderManager = mGameFramework.getSystem<ShaderManager>();
			mSQLite = mGameFramework.getSystem<SQLite>();
			mDataBase = mGameFramework.getSystem<DataBase>();
			mCameraManager = mGameFramework.getSystem<CameraManager>();
			mResourceManager = mGameFramework.getSystem<ResourceManager>();
			mLayoutSubPrefabManager = mGameFramework.getSystem<LayoutSubPrefabManager>();
			mApplicationConfig = mGameFramework.getSystem<ApplicationConfig>();
			mFrameConfig = mGameFramework.getSystem<FrameConfig>();
			mObjectManager = mGameFramework.getSystem<ObjectManager>();
			mInputManager = mGameFramework.getSystem<InputManager>();
			mSceneSystem = mGameFramework.getSystem<SceneSystem>();
			mClassObjectPool = mGameFramework.getSystem<ClassObjectPool>();
			mSQLiteSound = mSQLite.getTable<SQLiteSound>();
		}
	}
	// 方便书写代码添加的命令相关函数
	public static T newCmd<T>(out T cmd, bool show = true, bool delay = false) where T : Command, new()
	{
		cmd = null;
		return mCommandSystem.newCmd<T>(show, delay);
	}
	public static void pushCommand<T>(CommandReceiver cmdReceiver, bool show = true) where T : Command, new()
	{
		mCommandSystem.pushCommand<T>(cmdReceiver, show);
	}
	public static void pushCommand(Command cmd, CommandReceiver cmdReceiver)
	{
		mCommandSystem.pushCommand(cmd, cmdReceiver);
	}
	public static void pushDelayCommand<T>(CommandReceiver cmdReceiver, float delayExecute = 0.001f, bool show = true) where T : Command, new()
	{
		mCommandSystem.pushDelayCommand<T>(cmdReceiver, delayExecute);
	}
	public static void pushDelayCommand(Command cmd, CommandReceiver cmdReceiver, float delayExecute = 0.001f)
	{
		mCommandSystem.pushDelayCommand(cmd, cmdReceiver, delayExecute);
	}
	public static void logError(string info, bool isMainThread = true)
	{
		UnityUtility.logError(info, isMainThread);
	}
	public static void logInfo(string info, LOG_LEVEL level = LOG_LEVEL.LL_NORMAL)
	{
		UnityUtility.logInfo(info, level);
	}
	public static GameObject getGameObject(GameObject parent, string name, bool errorIfNull = false)
	{
		return UnityUtility.getGameObject(parent, name, errorIfNull);
	}
	public static bool getKeyCurrentDown(KeyCode key)
	{
		return mInputManager.getKeyCurrentDown(key);
	}
	public static bool getKeyCurrentUp(KeyCode key)
	{
		return mInputManager.getKeyCurrentUp(key);
	}
	public static bool getKeyDown(KeyCode key)
	{
		return mInputManager.getKeyDown(key);
	}
	public static bool getKeyUp(KeyCode key)
	{
		return mInputManager.getKeyUp(key);
	}
	public static Vector2 getMousePosition()
	{
		return mInputManager.getMousePosition();
	}
}