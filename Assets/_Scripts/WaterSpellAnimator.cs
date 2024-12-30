using UnityEngine;

public class WaterSpellAnimator : MonoBehaviour {
    private const string SPAWN = "Spawn";
    private const string IDLE = "Idle";
    private const string DESTROY = "Destroy";

    private Animator animator;
    private WaterSpell waterSpell;

    private void Awake() {
        animator = GetComponent<Animator>();
        waterSpell = GetComponentInParent<WaterSpell>();
    }

    private void Start() {
        waterSpell.OnStateChanged += WaterSpell_OnStateChanged;
    }

    public void DestroyAfterAnimation() {
        Destroy(waterSpell.gameObject);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.E)) {
            animator.SetTrigger(IDLE);
        }

        if (Input.GetKeyDown(KeyCode.D)) {
            animator.SetTrigger(DESTROY);
        }
    }

    private void WaterSpell_OnStateChanged(object sender, WaterSpell.OnStateChangedEventArgs e) {
        switch (e.state) {
            case WaterSpell.State.Spawn:
                animator.SetTrigger(SPAWN);
                break;

            case WaterSpell.State.Idle:
                animator.SetTrigger(IDLE);
                break;

            case WaterSpell.State.Destroy:
                animator.SetTrigger(DESTROY);
                break;
        }
    }

    private void OnDestroy() {
        // Unsubscribe from the event when this component is destroyed
        if (waterSpell != null) {
            waterSpell.OnStateChanged -= WaterSpell_OnStateChanged;
        }
    }
}