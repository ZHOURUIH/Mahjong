using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterMyself : CharacterOther
{
	public CharacterMyself(CHARACTER_TYPE type, string name)
		:
		base(type, name)
	{}
	public override void update(float elapsedTime)
	{
		base.update(elapsedTime);
	}
}
