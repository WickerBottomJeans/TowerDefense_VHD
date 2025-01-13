using UnityEngine;

public interface ISpecialAbility {
    bool CoolDown { get; }

    bool RequiresAiming { get; }

    void Activate(Vector2 targetLocation);
}