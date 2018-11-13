using UnityEngine;
using System.Collections;

// 管理类初始化完成调用
// 这个父类的添加是方便代码的书写
public class GameBase : FrameBase
{
	public static Game mGame;
	public static GameConfig mGameConfig;
	public static MahjongSystem mMahjongSystem;
	public static HeadTextureManager mHeadTextureManager;
	public static SocketManager mSocketManager;
	public static LogSystem mLogSystem;
	public static RoomSystem mRoomSystem;
	// SQLiteTable
	public static SQLiteLog mSQLiteLog;
	// 以下是用于快速访问的布局脚本
	public static ScriptGlobalTouch mScriptGlobalTouch;
	public static ScriptLogin mScriptLogin;
	public static ScriptRegister mScriptRegister;
	public static ScriptMainFrame mScriptMainFrame;
	public static ScriptBillboard mScriptBillboard;
	public static ScriptCharacter mScriptCharacter;
	public static ScriptRoomMenu mScriptRoomMenu;
	public static ScriptMahjongHandIn mScriptMahjongHandIn;
	public static ScriptMahjongDrop mScriptMahjongDrop;
	public static ScriptAllCharacterInfo mScriptAllCharacterInfo;
	public static ScriptDice mScriptDice;
	public static ScriptMahjongBackFrame mScriptMahjongBackFrame;
	public static ScriptPlayerAction mScriptPlayerAction;
	public static ScriptGameEnding mScriptGameEnding;
	public static ScriptAddPlayer mScriptAddPlayer;
	public static ScriptMahjongFrame mScriptMahjongFrame;
	public static ScriptJoinRoomDialog mScriptJoinRoomDialog;
	public static ScriptMessageOK mScriptMessageOK;
	public static ScriptMainLoading mScriptMainLoading;
	public static ScriptMahjongLoading mScriptMahjongLoading;
	public static ScriptFreeMatchTip mScriptFreeMatchTip;
	public static ScriptBackToMainHall mScriptBackToMainHall;
	public static ScriptMainFrameBack mScriptMainFrameBack;
	public static ScriptRoomList mScriptRoomList;
	public override void notifyConstructDone()
	{
		base.notifyConstructDone();
		if (mGame == null)
		{
			mGame = GameFramework.instance as Game;
			mGame.getSystem(out mGameConfig);
			mGame.getSystem(out mMahjongSystem);
			mGame.getSystem(out mHeadTextureManager);
			mGame.getSystem(out mSocketManager);
			mGame.getSystem(out mLogSystem);
			mGame.getSystem(out mRoomSystem);
		}
	}
}
