using UnityEngine;

public class DarkSpellCircleAnimator : MonoBehaviour {
    public DarkSpellCircleAnimator darkSpellCircleAnimator;

    public void DestroyAfterAnimation() {
        Destroy(darkSpellCircleAnimator.gameObject);
    }
}