using UnityEngine;

public class HolySwordAnimator : MonoBehaviour {
    public HolySword holySword;

    public void DestroyAfterAnimation() {
        Destroy(holySword.gameObject);
    }
}