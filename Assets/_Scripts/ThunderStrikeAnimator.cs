using UnityEngine;

public class ThunderStrikeAnimator : MonoBehaviour {
    private const string SPAWN = "Spawn";
    private const string STRIKE = "Strike";

    private Animator animator;
    private ThunderStrike thunderStrike;

    private void Awake() {
        animator = GetComponent<Animator>();
        thunderStrike = GetComponentInParent<ThunderStrike>();
    }

    private void Start() {
        thunderStrike.OnStateChanged += ThunderStrike_OnStateChanged; ;
    }

    private void ThunderStrike_OnStateChanged(object sender, ThunderStrike.OnStateChangedEventArgs e) {
        switch (e.state) {
            case ThunderStrike.State.Spawn:
                animator.SetTrigger(SPAWN);
                break;

            case ThunderStrike.State.Strike:
                animator.SetTrigger(STRIKE);
                break;
        }
    }

    public void DestroyAfterAnimation() {
        Destroy(thunderStrike.gameObject);
    }

    private void Update() {
    }

    private void OnDestroy() {
        // Unsubscribe from the event when this component is destroyed
        if (thunderStrike != null) {
            thunderStrike.OnStateChanged -= ThunderStrike_OnStateChanged;
        }
    }
}