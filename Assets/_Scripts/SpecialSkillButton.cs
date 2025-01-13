using System;
using UnityEngine;
using UnityEngine.UI;

public class SpecialSkillButton : MonoBehaviour {
    [SerializeField] private LevelSystem levelSystem;
    [SerializeField] private Image specialSkillButton;
    [SerializeField] private Image cooldownMask;
    private float cooldownTime;
    [SerializeField] private Sprite aimingMouseSprite;
    [SerializeField] private int SpecialSkillIndex = 1;

    private Button button;

    private GameObject aimingMouseObject;
    private _BaseTurret turret;
    private bool isAiming = false;
    private bool isCooldown = false;
    private float cooldownTimer = 0f;
    private ISpecialAbility iSpecialAbility;

    public event EventHandler<OnSpecialButtonClickedEventArgs> OnSpecialButtonClicked;

    private void Start() {
        // Get the turret from the parent of the parent
        turret = transform.parent.parent.GetComponentInChildren<_BaseTurret>();
        if (SpecialSkillIndex == 1) {
            iSpecialAbility = turret.iSpecialAbility;
        } else if (SpecialSkillIndex == 2) {
            iSpecialAbility = turret.secondISpecialAbility;
        }
        //set the cooldown

        cooldownTime = iSpecialAbility.CoolDown;

        specialSkillButton.enabled = false;
        cooldownMask.fillAmount = 0f; // Ensure the mask is not visible initially

        levelSystem.OnLevelUp += LevelSystem_OnLevelUp;

        Button button = specialSkillButton.GetComponent<Button>();
        if (button != null) {
            button.onClick.AddListener(HandleSpecialSkillButtonClick);
        }

        // Create a new GameObject to hold the SpriteRenderer and set it to the aiming mouse sprite
        aimingMouseObject = new GameObject("AimingMouse");
        SpriteRenderer spriteRenderer = aimingMouseObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = aimingMouseSprite;
        aimingMouseObject.SetActive(false); // Hide it initially
    }

    private void LevelSystem_OnLevelUp(object sender, LevelSystem.OnLevelUpEventArgs e) {
        if (e.isMaxLevel) {
            specialSkillButton.enabled = true;
        }
    }

    private void HandleSpecialSkillButtonClick() {
        if (isCooldown) return; // Prevent clicking if on cooldown

        if (iSpecialAbility.RequiresAiming) {
            // If the special ability requires aiming
            isAiming = true; // Start aiming
            aimingMouseObject.SetActive(true); // Show the aiming cursor
            Cursor.visible = false; // Hide the default cursor
        } else {
            // If the special ability does not require aiming
            Vector2 targetLocation = turret.transform.position; // Use the turret's position or a default position
            OnSpecialButtonClicked?.Invoke(this, new OnSpecialButtonClickedEventArgs { targetLocation = targetLocation, turret = turret, specialAbilityIndex = SpecialSkillIndex });
            StartCooldown(); // Start cooldown immediately
        }
    }

    private void Update() {
        if (isAiming) {
            // Update the position of the aiming mouse to follow the mouse position
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            aimingMouseObject.transform.position = new Vector3(mousePosition.x, mousePosition.y, aimingMouseObject.transform.position.z); // Ensure the Z value is preserved

            if (Input.GetMouseButtonDown(0)) { // Left click
                Vector2 targetLocation = mousePosition; // Get target location
                OnSpecialButtonClicked?.Invoke(this, new OnSpecialButtonClickedEventArgs { targetLocation = targetLocation, turret = turret, specialAbilityIndex = SpecialSkillIndex });

                // Hide the aiming cursor and show the default cursor
                aimingMouseObject.SetActive(false);
                Cursor.visible = true; // Make the default cursor visible again
                isAiming = false; // Stop aiming

                // Start cooldown
                StartCooldown();
            }
        }

        // Handle cooldown timer
        if (isCooldown) {
            cooldownTimer -= Time.deltaTime;
            cooldownMask.fillAmount = cooldownTimer / cooldownTime; // Update mask fill
            if (cooldownTimer <= 0f) {
                isCooldown = false;
                cooldownMask.fillAmount = 0f; // Ensure the mask is cleared
            }
        }
    }

    private void StartCooldown() {
        isCooldown = true;
        cooldownTimer = cooldownTime;
        cooldownMask.fillAmount = 1f; // Fully visible at the start of cooldown
    }

    public class OnSpecialButtonClickedEventArgs : EventArgs {
        public int specialAbilityIndex;
        public Vector2 targetLocation;
        public _BaseTurret turret;
    }
}