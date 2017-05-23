using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterManager : CommandReceiver
{
	protected Dictionary<string, Character> mCharacterList;                                 // 角色名字索引表
	protected Dictionary<int, Character> mCharacterIDList;                                  // 角色ID索引表
	protected Dictionary<CHARACTER_TYPE, Dictionary<string, Character>> mCharacterTypeList; // 角色分类列表
	protected CharacterFactoryManager mCharacterFactoryManager;								// 角色工厂
	protected GameObject mManagerObject;
	protected bool mLockUpdate;                                                             //角色状态锁
	protected CharacterMyself mMyself;

	public CharacterManager()
		:
		base(typeof(CharacterManager).ToString())
	{
		mCharacterList = new Dictionary<string, Character>();
		mCharacterTypeList = new Dictionary<CHARACTER_TYPE, Dictionary<string, Character>>();
		mCharacterIDList = new Dictionary<int, Character>();
		mCharacterFactoryManager = new CharacterFactoryManager();
		mLockUpdate = false;
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
		if (mCharacterIDList != null)
		{
			foreach (var character in mCharacterIDList)
			{
				character.Value.destroy();
			}
			mCharacterIDList.Clear();
			mCharacterList.Clear();
			mCharacterTypeList.Clear();
			mCharacterList = null;
			mCharacterTypeList = null;
			mCharacterIDList = null;
			mMyself = null;
		}
	}
	public void update(float fElapsedTime)
	{
		if (isLocked())
		{
			return;
		}
		if (mCharacterIDList != null)
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
	}
	public bool isLocked() { return mLockUpdate; }
	public void setLock() { mLockUpdate = true; }
	public void unLock() { mLockUpdate = false; }
	public CharacterMyself getMyself() { return mMyself; }
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
		if (!mCharacterIDList.ContainsKey(characterID))
		{
			return null;
		}
		return mCharacterIDList[characterID];
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
		newCharacter.init(id);
		addCharacterToList(newCharacter);
		return newCharacter;
	}
	public void destroyCharacter(string name)
	{
		Character character = getCharacter(name);
		if (character == null)
		{
			return;
		}
		destroyCharacter(character);
	}
	public void destroyCharacter(int clientID)
	{
		Character character = getCharacter(clientID);
		if (character == null)
		{
			return;
		}
		destroyCharacter(character);
	}
	public void notifyCharacterIDChanged(int oldID)
	{
		if (mCharacterIDList.ContainsKey(oldID))
		{
			Character character = mCharacterIDList[oldID];
			mCharacterIDList.Remove(oldID);
			mCharacterIDList.Add(character.getCharacterData().mClientID, character);
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
		if (mCharacterIDList.ContainsKey(character.getCharacterData().mClientID))
		{
			mCharacterIDList.Remove(character.getCharacterData().mClientID);
		}
		character.destroy();
	}
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
		int characterID = character.getCharacterData().mClientID;
		if (!mCharacterIDList.ContainsKey(characterID))
		{
			mCharacterIDList.Add(characterID, character);
		}
		else
		{
			UnityUtility.logError("error : there is a character id : " + characterID + ", can not add again!");
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
