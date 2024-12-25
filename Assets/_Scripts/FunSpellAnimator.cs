using UnityEngine;

public class FunSpellAnimator : MonoBehaviour {
    private const string SPAWN = "Spawn";
    private const string IDLE = "Idle";
    private const string DESTROY = "Destroy";

    private Animator animator;
    private FunSpell funSpell;

    private void Awake() {
        animator = GetComponent<Animator>();
        funSpell = GetComponentInParent<FunSpell>();
    }

    private void Start() {
        funSpell.OnStateChanged += FunSpell_OnStateChanged; ;
    }

    private void FunSpell_OnStateChanged(object sender, FunSpell.OnStateChangedEventArgs e) {
        switch (e.state) {
            case FunSpell.State.Spawn:
                animator.SetTrigger(SPAWN);
                break;

            case FunSpell.State.Idle:
                animator.SetTrigger(IDLE);
                break;

            case FunSpell.State.Destroy:
                animator.SetTrigger(DESTROY);
                break;
        }
    }

    public void DestroyAfterAnimation() {
        Destroy(funSpell.gameObject);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.E)) {
            animator.SetTrigger(IDLE);
        }

        if (Input.GetKeyDown(KeyCode.D)) {
            animator.SetTrigger(DESTROY);
        }
    }

    private void OnDestroy() {
        // Unsubscribe from the event when this component is destroyed
        if (funSpell != null) {
            funSpell.OnStateChanged -= FunSpell_OnStateChanged;
        }
    }
}