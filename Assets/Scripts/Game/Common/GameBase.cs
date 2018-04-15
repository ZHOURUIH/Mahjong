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
	public static SocketManager mSocketNetManager;
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
	public override void notifyConstructDone()
	{
		base.notifyConstructDone();
		if (mGame == null)
		{
			mGame = Game.instance as Game;
			mGameConfig = mGame.getSystem<GameConfig>();
			mMahjongSystem = mGame.getSystem<MahjongSystem>();
			mHeadTextureManager = mGame.getSystem<HeadTextureManager>();
			mSocketNetManager = mGame.getSystem<SocketManager>();
		}
	}
}
