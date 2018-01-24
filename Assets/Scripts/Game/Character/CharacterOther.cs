using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterOther : Character
{
	public CharacterOther(CHARACTER_TYPE type, string name)
		:
		base(type, name)
	{
        ;		
	}
	public override void update(float elapsedTime)
	{
		base.update(elapsedTime);
	}
}