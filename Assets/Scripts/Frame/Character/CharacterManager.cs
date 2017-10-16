using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterManager : CommandReceiver
{
	protected Dictionary<CHARACTER_TYPE, Dictionary<string, Character>> mCharacterTypeList;    // 角色分类列表
	protected Dictionary<string, Character>		mCharacterList;     // 角色名字索引表
	protected Dictionary<int, Character>		mCharacterGUIDList; // 角色ID索引表
	protected CharacterMyself					mMyself;            // 玩家自己,方便获取
	protected GameObject						mManagerObject;     // 角色管理器物体
	protected CharacterFactoryManager			mCharacterFactoryManager;                             // 角色工厂
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
		if (mManagerObject == null)
		{
			UnityUtility.logError("can not find Character Manager under GameFramework!");
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
	public void update(float fElapsedTime)
	{
		foreach (var characterIter in mCharacterList)
		{
			Character character = characterIter.Value;
			if (character != null)
			{
				character.update(fElapsedTime);
			}
		}
	}
	public CharacterMyself getMyself() { return mMyself; }
	public void registeCharacter<T>(CHARACTER_TYPE type) where T : Character
	{
		mCharacterFactoryManager.addFactory<T>(type);
	}
	public Character getCharacter(string name)
	{
		if (!mCharacterList.ContainsKey(name))
		{
			return null;
		}
		return mCharacterList[name];
	}
	public Character getCharacter(int characterID)
	{
		if (!mCharacterGUIDList.ContainsKey(characterID))
		{
			return null;
		}
		return mCharacterGUIDList[characterID];
	}
	public void getCharacterListByType(CHARACTER_TYPE type, ref Dictionary<string, Character> characterList)
	{
		if (!mCharacterTypeList.ContainsKey(type))
		{
			return;
		}
		characterList = mCharacterTypeList[type];
	}
	public Character createCharacter(string name, CHARACTER_TYPE type, int id)
	{
		if (mCharacterList.ContainsKey(name))
		{
			UnityUtility.logError("error : there is a character named : " + name + "! can not create again!");
			return null;
		}
		if (type == CHARACTER_TYPE.CT_MYSELF)
		{
			if(mMyself != null)
			{
				Debug .LogError ("error : Myself has exist ! can not create again ,name " + name);
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
		if (newCharacter != null)
		{
			// 将角色挂接到管理器下
			newCharacter.getObject().transform.parent = mManagerObject.transform;
			newCharacter.init(id);
			addCharacterToList(newCharacter);
		}
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
	public void destroyCharacter(int id)
	{
		Character character = getCharacter(id);
		if(character != null)
		{
			destroyCharacter(character);
		}
	}
	public void notifyCharacterIDChanged(int oldID)
	{
		if (mCharacterGUIDList.ContainsKey(oldID))
		{
			Character character = mCharacterGUIDList[oldID];
			mCharacterGUIDList.Remove(oldID);
			mCharacterGUIDList.Add(character.getCharacterData().mGUID, character);
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
	//------------------------------------------------------------------------------------------------------------
	protected void addCharacterToList(Character character)
	{
		if (character == null)
		{
			return;
		}
		// 加入到全部角色列表
		mCharacterList.Add(character.getName(), character);
		// 加入到角色分类列表
		if (mCharacterTypeList.ContainsKey(character.getType()))
		{
			mCharacterTypeList[character.getType()].Add(character.getName(), character);
		}
		else
		{
			Dictionary<string, Character> characterMap = new Dictionary<string, Character>();
			characterMap.Add(character.getName(), character);
			mCharacterTypeList.Add(character.getType(), characterMap);
		}
		// 加入ID索引表
		int characterID = character.getCharacterData().mGUID;
		if (!mCharacterGUIDList.ContainsKey(characterID))
		{
			mCharacterGUIDList.Add(characterID, character);
		}
		else
		{
			UnityUtility.logError("error : there is a character id : " + characterID + ", can not add again!");
		}
	}
	protected void removeCharacterFromList(Character character)
	{
		if (character == null)
		{
			return;
		}

		// 从全部角色列表中移除
		if (mCharacterList.ContainsKey(character.getName()))
		{
			mCharacterList.Remove(character.getName());
		}
		// 从角色分类列表中移除
		if (mCharacterTypeList.ContainsKey(character.getType()))
		{
			if (mCharacterTypeList[character.getType()].ContainsKey(character.getName()))
			{
				mCharacterTypeList[character.getType()].Remove(character.getName());
			}
		}
		// 从ID索引表中移除
		if (mCharacterGUIDList.ContainsKey(character.getCharacterData().mGUID))
		{
			mCharacterGUIDList.Remove(character.getCharacterData().mGUID);
		}
		character.destroy();
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
