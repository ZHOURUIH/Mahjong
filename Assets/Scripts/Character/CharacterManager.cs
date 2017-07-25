using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterManager : CommandReceiver
{
	protected Dictionary<string, Character> mCharacterList;                                 // 角色名字索引表
	protected Dictionary<int, Character> mCharacterGUIDList;								// 角色GUID索引表
	protected Dictionary<CHARACTER_TYPE, Dictionary<string, Character>> mCharacterTypeList; // 角色分类列表
	protected CharacterFactoryManager mCharacterFactoryManager;								// 角色工厂
	protected GameObject mManagerObject;													// 角色管理器节点
	protected CharacterMyself mMyself;														// 玩家自己的实例,方便获取

	public CharacterManager()
		:
		base(typeof(CharacterManager).ToString())
	{
		mCharacterList = new Dictionary<string, Character>();
		mCharacterTypeList = new Dictionary<CHARACTER_TYPE, Dictionary<string, Character>>();
		mCharacterGUIDList = new Dictionary<int, Character>();
		mCharacterFactoryManager = new CharacterFactoryManager();
	}
	public void init()
	{
		mManagerObject = UnityUtility.getGameObject(mGameFramework.getGameFrameObject(), "CharacterManager");
		if(mManagerObject == null)
		{
			UnityUtility.logError("can not find Character Manager under GameFramework!");
		}
		mCharacterFactoryManager.addFactory<Character>(CHARACTER_TYPE.CT_NORMAL);
		mCharacterFactoryManager.addFactory<CharacterNPC>(CHARACTER_TYPE.CT_NPC);
		mCharacterFactoryManager.addFactory<CharacterOther>(CHARACTER_TYPE.CT_OTHER);
		mCharacterFactoryManager.addFactory<CharacterMyself>(CHARACTER_TYPE.CT_MYSELF);
		if(mCharacterFactoryManager.getFactoryCount() != (int)CHARACTER_TYPE.CT_MAX)
		{
			UnityUtility.logError("not all character registered!");
		}
	}
	public override void destroy()
	{
		base.destroy();
		foreach (var character in mCharacterGUIDList)
		{
			character.Value.destroy();
		}
		mCharacterList = null;
		mCharacterTypeList = null;
		mCharacterGUIDList = null;
		mMyself = null;
	}
	public void update(float elapsedTime)
	{
		foreach (var characterIter in mCharacterList)
		{
			Character character = characterIter.Value;
			if (character != null)
			{
				character.update(elapsedTime);
			}
		}
	}
	public CharacterMyself getMyself() { return mMyself; }
	public Character getCharacter(string name)
	{
		if (!mCharacterList.ContainsKey(name))
		{
			return null;
		}
		return mCharacterList[name];
	}
	public Character getCharacterByGUID(int guid)
	{
		if(!mCharacterGUIDList.ContainsKey(guid))
		{
			return null;
		}
		return mCharacterGUIDList[guid];
	}
	public void getCharacterListByType(CHARACTER_TYPE type, ref Dictionary<string, Character> characterList)
	{
		if (!mCharacterTypeList.ContainsKey(type))
		{
			return;
		}
		characterList = mCharacterTypeList[type];
	}
	
	public Character createCharacter(string name, CHARACTER_TYPE type, int guid)
	{
		if (mCharacterList.ContainsKey(name))
		{
			Debug.LogError("error : there is a character named : " + name + "! can not create again!");
			return null;
		}
		if (type == CHARACTER_TYPE.CT_MYSELF)
		{
			if (mMyself != null)
			{
				Debug.LogError("error : Myself has exist ! can not create again ,name " + name);
				return null;
			}
		}
		CharacterFactory factory = mCharacterFactoryManager.getFactory(type);
		Character newCharacter = factory.createCharacter(name);
		// 如果是玩家自己,则记录下来
		if (type == CHARACTER_TYPE.CT_MYSELF)
		{
			mMyself = newCharacter as CharacterMyself;
		}
		// 将创建的角色挂接到角色管理器下
		newCharacter.setParent(mManagerObject);
		newCharacter.init( guid);
		addCharacterToList(newCharacter);
		return newCharacter;
	}
	public void destroyCharacter(string name)
	{
		Character character = getCharacter(name);
		if (character != null)
		{
			destroyCharacter(character);
		}
	}
	public void destroyCharacterByGUID(int guid)
	{
		Character character = getCharacterByGUID(guid);
		if (character != null)
		{
			destroyCharacter(character);
		}
	}
	public void notifyCharacterNameChanged(string oldName)
	{
		Character character = null;
		if (mCharacterList.ContainsKey(oldName))
		{
			character = mCharacterList[oldName];
			mCharacterList.Remove(oldName);
			mCharacterList.Add(character.getName(), character);
		}
		if (character != null)
		{
			if (mCharacterTypeList.ContainsKey(character.getType()))
			{
				if (mCharacterTypeList[character.getType()].ContainsKey(oldName))
				{
					mCharacterTypeList[character.getType()].Remove(oldName);
					mCharacterTypeList[character.getType()].Add(character.getName(), character);
				}
			}
		}
	}
	//------------------------------------------------------------------------------------------------------------------------------
	protected void removeCharacterFromList(Character character)
	{
		if (character == null)
		{
			return;
		}
		CharacterData data = character.getCharacterData();
		// 从全部角色列表中移除
		if (mCharacterList.ContainsKey(data.mName))
		{
			mCharacterList.Remove(data.mName);
		}
		// 从角色分类列表中移除
		if (mCharacterTypeList.ContainsKey(character.getType()))
		{
			if (mCharacterTypeList[character.getType()].ContainsKey(data.mName))
			{
				mCharacterTypeList[character.getType()].Remove(data.mName);
			}
		}
		// 从GUID索引表中移除
		if (mCharacterGUIDList.ContainsKey(data.mGUID))
		{
			mCharacterGUIDList.Remove(data.mGUID);
		}
		character.destroy();
	}
	protected void addCharacterToList(Character character)
	{
		if (character == null)
		{
			return;
		}
		CharacterData data = character.getCharacterData();
		// 加入到全部角色列表
		mCharacterList.Add(data.mName, character);
		// 加入到角色分类列表
		if (mCharacterTypeList.ContainsKey(character.getType()))
		{
			mCharacterTypeList[character.getType()].Add(data.mName, character);
		}
		else
		{
			Dictionary<string, Character> characterMap = new Dictionary<string, Character>();
			characterMap.Add(data.mName, character);
			mCharacterTypeList.Add(character.getType(), characterMap);
		}
		// 如果不是非法GUID才能加入列表
		if(data.mGUID != CommonDefine.INVALID_ID && !mCharacterGUIDList.ContainsKey(data.mGUID))
		{
			mCharacterGUIDList.Add(data.mGUID, character);
		}
	}
	protected void destroyCharacter(Character character)
	{
		character.destroy();
		removeCharacterFromList(character);
		if (mMyself == character)
		{
			mMyself = null;
		}
	}
}
