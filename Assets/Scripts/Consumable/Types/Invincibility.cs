using UnityEngine;
using System;
using System.Collections;

public class Invincibility : Consumable
{
    public override string GetConsumableName()
    {
        return "Invincible";
    }

    public override ConsumableType GetConsumableType()
    {
        return ConsumableType.INVINCIBILITY;
    }

    public override int GetConsumableId()
    {

        return (int)ConsumableType.INVINCIBILITY;
    }
    public override int GetPrice()
    {
        return 100;
    }

	public override int GetPremiumCost()
	{
		return 0;
	}

	public override void Tick(CharacterInputController c)
    {
        base.Tick(c);

        c.characterCollider.SetInvincibleExplicit(true);
    }

    public override IEnumerator Started(CharacterInputController c)
    {
        yield return base.Started(c);
        c.characterCollider.SetInvincible(duration);
    }

    public override void Ended(CharacterInputController c)
    {
        base.Ended(c);
        c.characterCollider.SetInvincibleExplicit(false);
    }
}
