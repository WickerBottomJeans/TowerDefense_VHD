using System;
using UnityEngine;
using UnityEngine.UI;

public class TurretIconUI : MonoBehaviour
{
    [SerializeField] private BlueTurret blueTurret;
    [SerializeField] private Transform upgradeIconTemplate;
    [SerializeField] private Transform abilityIconTemplate;

    [SerializeField] bool HasUpgradeButtonOn;
    [SerializeField] bool HasAbilityButtonOn;

    private LevelSO currentLevelSO;
    private void Awake() {
        upgradeIconTemplate.gameObject.SetActive(false);
        abilityIconTemplate.gameObject.SetActive(false);
    }
    private void Start() {
        //blueTurret.OnUpgradable += BlueTurret_OnUpgradable;
    }

    private void BlueTurret_OnUpgradable(object sender, BlueTurret.OnUpgradableEventArgs e) {
        currentLevelSO = e.levelSO;
        HasUpgradeButtonOn = true;
        DisplayVisual();
}

    //Event when press upgrade button
    public event EventHandler OnUpgradeButtonClicked;
    public void InvokeOnUpgradeButtonClicked() {
        OnUpgradeButtonClicked?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler OnAbilityButtonClicked;
    public void InvokeOnAbilityButtonClicked() {
        OnAbilityButtonClicked?.Invoke(this, EventArgs.Empty);
    }

    private void DisplayVisual() {
        foreach (Transform child in transform) {
            if (child == upgradeIconTemplate) { continue; }
            Destroy(child.gameObject);
        }
        if(HasAbilityButtonOn) {
            Transform abilityTransform = Instantiate(abilityIconTemplate, transform);
            abilityTransform.gameObject.SetActive(true);
            abilityTransform.GetComponent<UpgradeIconTemplate>().SetLevelSO(currentLevelSO);
            abilityTransform.GetComponent<Button>().onClick.AddListener(InvokeOnUpgradeButtonClicked);
        }
        //pop up the upgrade button
        if (HasUpgradeButtonOn) {
            Transform upgradeTransform = Instantiate(upgradeIconTemplate, transform);
            upgradeTransform.gameObject.SetActive(true);
            upgradeTransform.GetComponent<UpgradeIconTemplate>().SetLevelSO(currentLevelSO);
            upgradeTransform.GetComponent<Button>().onClick.AddListener(InvokeOnUpgradeButtonClicked);
        }
    }


}
