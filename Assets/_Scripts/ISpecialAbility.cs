using UnityEngine;

public interface ISpecialAbility {
    float CoolDown { get; }
    float MPCost { get; }

    bool RequiresAiming { get; }

    void Activate(Vector2 targetLocation, _BaseTurret turret);
}