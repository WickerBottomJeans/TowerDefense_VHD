using UnityEngine;

public class DarkSpellAnimation : MonoBehaviour {
    public DarkSpell darkspell;

    public void DestroyAfterAnimation() {
        Destroy(darkspell.gameObject);
    }
}