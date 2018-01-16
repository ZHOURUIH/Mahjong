using UnityEngine;
using System.Collections;

// 管理类初始化完成调用
// 这个父类的添加是方便代码的书写
public class FrameBase
{
	public static GameFramework			mGameFramework			= null;
	public static CommandSystem			mCommandSystem			= null;
	public static AudioManager			mAudioManager			= null;
	public static GameSceneManager		mGameSceneManager		= null;
	public static CharacterManager		mCharacterManager		= null;
	public static GameLayoutManager		mLayoutManager			= null;
	public static KeyFrameManager		mKeyFrameManager		= null;
	public static GlobalTouchSystem		mGlobalTouchSystem		= null;
	public static ShaderManager			mShaderManager			= null;
	public static DataBase				mDataBase				= null;
	public static CameraManager			mCameraManager			= null;
	public static ResourceManager		mResourceManager		= null;
	public static LayoutPrefabManager	mLayoutPrefabManager	= null;
	public static ApplicationConfig		mApplicationConfig		= null;
	public static FrameConfig			mFrameConfig			= null;
	public static ModelManager			mModelManager			= null;
	public static InputManager			mInputManager			= null;
	public static SceneSystem			mSceneSystem			= null;
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
			mDataBase = mGameFramework.getSystem<DataBase>();
			mCameraManager = mGameFramework.getSystem<CameraManager>();
			mResourceManager = mGameFramework.getSystem<ResourceManager>();
			mLayoutPrefabManager = mGameFramework.getSystem<LayoutPrefabManager>();
			mApplicationConfig = mGameFramework.getSystem<ApplicationConfig>();
			mFrameConfig = mGameFramework.getSystem<FrameConfig>();
			mModelManager = mGameFramework.getSystem<ModelManager>();
			mInputManager = mGameFramework.getSystem<InputManager>();
			mSceneSystem = mGameFramework.getSystem<SceneSystem>();
		}
	}
	// 方便书写代码添加的命令相关函数
	public static T newCmd<T>(out T cmd, bool show = true, bool delay = false) where T : Command, new()
	{
		cmd = null;
		return mCommandSystem.newCmd<T>(show, delay);
	}
	public static T newCmd<T>(bool show = true, bool delay = false) where T : Command, new()
	{
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
}