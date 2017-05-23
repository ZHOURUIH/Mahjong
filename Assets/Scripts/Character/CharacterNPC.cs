using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CharacterNPC : Character
{
	public CharacterNPC(CHARACTER_TYPE type, string name)
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