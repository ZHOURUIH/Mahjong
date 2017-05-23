using UnityEngine;
using System.Collections;

// 管理类初始化完成调用
// 这个父类的添加是方便代码的书写
public class GameBase
{
	public static GameFramework		mGameFramework		= null;
	public static CommandSystem		mCommandSystem		= null;
	public static AudioManager		mAudioManager		= null;
	public static GameSceneManager	mGameSceneManager	= null;
	public static CharacterManager	mCharacterManager	= null;
	public static GameLayoutManager mLayoutManager		= null;
	public static SocketManager		mSocketNetManager	= null;
	public static KeyFrameManager	mKeyFrameManager	= null;
	public static GlobalTouchSystem mGlobalTouchSystem	= null;
	public static ShaderManager		mShaderManager		= null;
	public static DataBase			mDataBase			= null;
	public static MahjongSystem		mMahjongSystem		= null;
	public static ResourceManager	mResourceManager	= null;
	public static CameraManager		mCameraManager		= null;
	public static GameConfig		mGameConfig = null;
	public static LayoutPrefabManager mLayoutPrefabManager = null;
	public static void notifyConstructDone()
	{
		if (mGameFramework == null)
		{
			mGameFramework = GameFramework.instance;
			mCommandSystem = mGameFramework.getCommandSystem();
			mAudioManager = mGameFramework.getAudioManager();
			mGameSceneManager = mGameFramework.getGameSceneManager();
			mCharacterManager = mGameFramework.getCharacterManager();
			mLayoutManager = mGameFramework.getLayoutManager();
			mSocketNetManager = mGameFramework.getSocketManager();
			mKeyFrameManager = mGameFramework.getKeyFrameManager();
			mGlobalTouchSystem = mGameFramework.getGlobalTouchSystem();
			mShaderManager = mGameFramework.getShaderManager();
			mDataBase = mGameFramework.getDataBase();
			mMahjongSystem = mGameFramework.getMahjongSystem();
			mResourceManager = mGameFramework.getResourceManager();
			mCameraManager = mGameFramework.getCameraManager();
			mLayoutPrefabManager = mGameFramework.getLayoutPrefabManager();
			mGameConfig = mGameFramework.getGameConfig();
		}
	}
}