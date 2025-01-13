using UnityEngine;

public class EarthHammerAnimator : MonoBehaviour {
    public EarthHammer hammer;

    public void DestroyAfterAnimation() {
        Destroy(hammer.gameObject);
    }
}