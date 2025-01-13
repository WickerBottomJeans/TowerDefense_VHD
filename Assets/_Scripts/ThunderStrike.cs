using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderStrike : MonoBehaviour, ISpecialAbility {
    public float damage = 300f;
    public float range = 1f;
    public float spawnTimer = 0f;
    public float spawnTime = 1f;
    private HashSet<Enemy> enemiesInRange = new HashSet<Enemy>();
    private CircleCollider2D areaCollider;
    public float mpCost = 50f;
    public float MPCost => mpCost;
    [SerializeField] private float cooldown = 5f;

    public enum State {
        Spawn,
        Strike,
    }

    private State currentState;

    public float CoolDown => cooldown;
    public bool RequiresAiming => true;

    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;

    public class OnStateChangedEventArgs : EventArgs {
        public State state;
    }

    private void Awake() {
        areaCollider = GetComponent<CircleCollider2D>();
        areaCollider.radius = range;
        FireOnStateChanged(State.Spawn);
    }

    private void Update() {
        if (spawnTimer > 0f) {
            spawnTimer -= Time.deltaTime;
        } else if (spawnTimer <= 0f && currentState != State.Strike) {
            currentState = State.Strike;
            DealDamage();
            FireOnStateChanged(State.Strike);
        }
    }

    private void DealDamage() {
        foreach (Enemy enemy in enemiesInRange) {
            enemy.TakeDamage(damage);
        }
    }

    public void Activate(Vector2 targetLocation, _BaseTurret turret) {
        transform.position = new Vector3(targetLocation.x, targetLocation.y, 0);
        spawnTimer = spawnTime;
    }

    private void FireOnStateChanged(State state) {
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
            state = state
        });
        Debug.Log("Fired" + state);
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.CompareTag("Enemy") && collision.TryGetComponent(out Enemy enemyScript) && currentState == State.Strike) {
            enemyScript.TakeDamage(damage);
        }
    }

    public float GetCoolDown() {
        return cooldown;
    }
}