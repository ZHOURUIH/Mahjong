using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class LayoutAsyncInfo
{
	public string mName;
	public int mRenderOrder;
	public LAYOUT_TYPE mType;
	public GameLayout mLayout;
	public GameObject mLayoutObject;
	public LayoutAsyncDone mCallback;
}

public class GameLayoutManager : CommandReceiver
{
	protected ScriptFactoryManager					mScriptFactoryManager;
	protected Dictionary<LAYOUT_TYPE, string>		mLayoutTypeToName;
	protected Dictionary<string, LAYOUT_TYPE>		mLayoutNameToType;
	protected Dictionary<LAYOUT_TYPE, GameLayout>	mLayoutTypeList;
	protected Dictionary<string, GameLayout>		mLayoutNameList;
	protected txUIObject							mUIRoot = null;
	protected Dictionary<string, LayoutAsyncInfo> mLayoutAsyncList;
	public GameLayoutManager()
		:
		base(typeof(GameLayoutManager).ToString())
	{
		mScriptFactoryManager = new ScriptFactoryManager();
		mLayoutTypeToName = new Dictionary<LAYOUT_TYPE, string>();
		mLayoutNameToType = new Dictionary<string, LAYOUT_TYPE>();
		mLayoutTypeList = new Dictionary<LAYOUT_TYPE, GameLayout>();
		mLayoutNameList = new Dictionary<string, GameLayout>();
		mLayoutAsyncList = new Dictionary<string, LayoutAsyncInfo>();
		mUIRoot = null;
	}
	public void init()
	{
		addLayoutNameType();
		mUIRoot = new txUIObject();
		mUIRoot.init(null, UnityUtility.getGameObject(null, "UI Root"));
		if (mUIRoot == null)
		{
			UnityUtility.logError("can not find ui root! please add it to scene!");
			return;
		}
	}
	public GameObject getUIRootObject()
	{
		return mUIRoot.mObject;
	}
	public txUIObject getUIRoot()
	{
		return mUIRoot;
	}
	public void update(float fElapsedTime)
	{
		foreach (var layout in mLayoutTypeList)
		{
			layout.Value.update(fElapsedTime);
		}
	}
	public override void destroy()
	{
		mLayoutTypeToName.Clear();
		mLayoutNameToType.Clear();
	}
	public string getLayoutNameByType(LAYOUT_TYPE type)
	{
		if (mLayoutTypeToName.ContainsKey(type))
		{
			return mLayoutTypeToName[type];
		}
		else
		{
			UnityUtility.logError("can not find LayoutType: " + type);
		}
		return "";
	}
	public LAYOUT_TYPE getLayoutTypeByName(string name)
	{
		if (mLayoutNameToType.ContainsKey(name))
		{
			return mLayoutNameToType[name];
		}
		else
		{
			UnityUtility.logError("can not  find LayoutName:" + name);
		}
		return LAYOUT_TYPE.LT_MAX;
	}
	public GameLayout getGameLayout(LAYOUT_TYPE type)
	{
		if (mLayoutTypeList.ContainsKey(type))
		{
			return mLayoutTypeList[type];
		}
		return null;
	}
	public LayoutScript getScript(LAYOUT_TYPE type)
	{
		GameLayout layout = getGameLayout(type);
		if(layout != null)
		{
			return layout.getScript();
		}
		return null;
	}
	public GameLayout createLayout(LAYOUT_TYPE type, int renderOrder, bool async, LayoutAsyncDone callback)
	{
		if (mLayoutTypeList.ContainsKey(type))
		{
			return mLayoutTypeList[type];
		}
		string name = getLayoutNameByType(type);
		// 如果是异步加载则,则先加入列表中
		if(async)
		{
			LayoutAsyncInfo info = new LayoutAsyncInfo();
			info.mName = name;
			info.mType = type;
			info.mRenderOrder = renderOrder;
			info.mLayout = null;
			info.mLayoutObject = null;
			info.mCallback = callback;
			mLayoutAsyncList.Add(info.mName, info);
			// 首先加载资源
			bool ret = mResourceManager.loadResourceAsync<GameObject>(CommonDefine.R_UI_PREFAB_PATH + name, onLayoutPrefabAsyncDone, true);
			if(!ret)
			{
				UnityUtility.logError("can not find layout : " + name);
			}
			return null;
		}
		else
		{
			GameObject layoutObject = UnityUtility.instantiatePrefab(getUIRootObject(), CommonDefine.R_UI_PREFAB_PATH + name);
			GameLayout layout = layoutObject.GetComponent<GameLayout>();
			addLayoutToList(layout, name, type);
			layout.init(type, name, renderOrder, false, null);
			return layout;
		}
	}
	protected void onLayoutPrefabAsyncDone(UnityEngine.Object res)
	{
		LayoutAsyncInfo info = mLayoutAsyncList[res.name];
		info.mLayoutObject = GameObject.Instantiate(res) as GameObject;
		info.mLayout = info.mLayoutObject.GetComponent<GameLayout>();
		addLayoutToList(info.mLayout, info.mName, info.mType);
		UnityUtility.setNormalProperty(ref info.mLayoutObject, getUIRootObject(), info.mName, Vector3.one, Vector3.zero, Vector3.zero);
		info.mLayout.init(info.mType, info.mName, info.mRenderOrder, true, info.mCallback);
	}
	public void destroyLayout(LAYOUT_TYPE type)
	{
		GameLayout layout = getGameLayout(type);
		if (layout == null)
		{
			UnityUtility.logError("not find layout");
			return;
		}
		removeLayoutFromList(layout);
	}
	public LayoutScript createScript(LAYOUT_TYPE type, string name, GameLayout layout)
	{
		ScriptFactory factory = mScriptFactoryManager.getFactory(type);
		if(factory != null)
		{
			return factory.createScript(layout, name);
		}
		return null;
	}
	public List<BoxCollider> getAllLayoutBoxCollider()
	{
		List<BoxCollider> allBoxList = new List<BoxCollider>();
		foreach (var layout in mLayoutTypeList)
		{
			List<BoxCollider> boxList = layout.Value.getAllBoxCollider();
			foreach(var box in boxList)
			{
				allBoxList.Add(box);
			}
		}
		return allBoxList; 
	}
	//-------------------------------------------------------------------------------------------------------------------------
	protected void addLayoutNameType()
	{
		registerLayout(typeof(ScriptGlobalTouch), LAYOUT_TYPE.LT_GLOBAL_TOUCH, "UIGlobalTouch");
		registerLayout(typeof(ScriptStart), LAYOUT_TYPE.LT_START, "UIStart");
		registerLayout(typeof(ScriptMainFrame), LAYOUT_TYPE.LT_MAIN_FRAME, "UIMainFrame");
		registerLayout(typeof(ScriptBillboard), LAYOUT_TYPE.LT_BILLBOARD, "UIBillboard");
		registerLayout(typeof(ScriptCharacter), LAYOUT_TYPE.LT_CHARACTER, "UICharacter");
		registerLayout(typeof(ScriptRoomMenu), LAYOUT_TYPE.LT_ROOM_MENU, "UIRoomMenu");
		registerLayout(typeof(ScriptMahjongHandIn), LAYOUT_TYPE.LT_MAHJONG_HAND_IN, "UIMahjongHandIn");
		registerLayout(typeof(ScriptMahjongDrop), LAYOUT_TYPE.LT_MAHJONG_DROP, "UIMahjongDrop");
		registerLayout(typeof(ScriptAllCharacterInfo), LAYOUT_TYPE.LT_ALL_CHARACTER_INFO, "UIAllCharacterInfo");
		registerLayout(typeof(ScriptDice), LAYOUT_TYPE.LT_DICE, "UIDice");
		registerLayout(typeof(ScriptMahjongGameFrame), LAYOUT_TYPE.LT_MAHJONG_GAME_FRAME, "UIMahjongGameFrame");
		registerLayout(typeof(ScriptPlayerAction), LAYOUT_TYPE.LT_PLAYER_ACTION, "UIPlayerAction");
		registerLayout(typeof(ScriptGameEnding), LAYOUT_TYPE.LT_GAME_ENDING, "UIGameEnding");
		registerLayout(typeof(ScriptAddPlayer), LAYOUT_TYPE.LT_ADD_PLAYER, "UIAddPlayer");
		if (mLayoutTypeToName.Count < (int)LAYOUT_TYPE.LT_MAX)
		{
			Debug.LogError("error : not all script added! max count : " + (int)LAYOUT_TYPE.LT_MAX + ", added count :" + mLayoutTypeToName.Count);
		}
	}
	protected void registerLayout(Type classType, LAYOUT_TYPE type, string name)
	{
		mLayoutTypeToName.Add(type, name);
		mLayoutNameToType.Add(name, type);
		mScriptFactoryManager.addFactory(classType, type);
	}
	protected void addLayoutToList(GameLayout layout, string name, LAYOUT_TYPE type)
	{
		mLayoutTypeList.Add(type, layout);
		mLayoutNameList.Add(name, layout);
	}
	protected void removeLayoutFromList(GameLayout layout)
	{
		if (layout != null)
		{
			mLayoutTypeList.Remove(layout.getType());
			mLayoutNameList.Remove(layout.getName());
		}
	}
	protected void onLayoutPrefabAsyncDone()
	{
		;
	}
}