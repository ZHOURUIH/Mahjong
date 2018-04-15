using UnityEngine;
using System.Collections;

public class CharacterRegister : GameBase
{
	public static void registeAllCharacter()
	{
		registeCharacter<Character>(CHARACTER_TYPE.CT_NORMAL);
		registeCharacter<CharacterAI>(CHARACTER_TYPE.CT_AI);
		registeCharacter<CharacterOther>(CHARACTER_TYPE.CT_OTHER);
		registeCharacter<CharacterMyself>(CHARACTER_TYPE.CT_MYSELF);
	}
	//-------------------------------------------------------------------------------------------------------
	protected static void registeCharacter<T>(CHARACTER_TYPE type) where T : Character
	{
		mCharacterManager.registeCharacter(typeof(T), type);
	}
}