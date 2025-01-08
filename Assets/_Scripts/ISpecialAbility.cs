using UnityEngine;

public interface ISpecialAbility {

    float GetCoolDown();

    void Activate(Vector2 targetLocation);
}