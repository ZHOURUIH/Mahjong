using UnityEngine;
using System.Collections;

// 管理类初始化完成调用
// 这个父类的添加是方便代码的书写
public class GameBase : FrameBase
{
	public static Game mGame;
	public static GameConfig mGameConfig;
	public static MahjongSystem mMahjongSystem;
	public static MaterialManager mMaterialManager;
	public static PlayerHeadManager mPlayerHeadManager;
	public override void notifyConstructDone()
	{
		base.notifyConstructDone();
		if (mGame == null)
		{
			mGame = Game.instance as Game;
			mGameConfig = mGame.getGameConfig();
			mMahjongSystem = mGame.getMahjongSystem();
			mMaterialManager = mGame.getMaterialManager();
			mPlayerHeadManager = mGame.getPlayerHeadManager();
		}
	}
}