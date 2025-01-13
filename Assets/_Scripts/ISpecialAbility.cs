using UnityEngine;

public interface ISpecialAbility {
    float CoolDown { get; }

    bool RequiresAiming { get; }

    void Activate(Vector2 targetLocation);
}