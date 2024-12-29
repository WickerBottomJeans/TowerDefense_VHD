using UnityEngine;
using UnityEngine.UI;

public class HPBarUI : MonoBehaviour {
    private IHasHPBar hasHPBar;
    [SerializeField] private GameObject hasHPBarGameObject;
    [SerializeField] private Image hpBar;
    [SerializeField] private Image mpBar;
    [SerializeField] private Image levelHeart;

    private void Start() {
        hasHPBar = hasHPBarGameObject.GetComponent<IHasHPBar>();
        hasHPBar.OnHPChanged += IHasHPBar_OnHPChanged;
        hpBar.fillAmount = 1f;
        mpBar.fillAmount = 0f;
        levelHeart.fillAmount = 0f;
    }

    private void IHasHPBar_OnHPChanged(object sender, IHasHPBar.OnHPChangedEventArgs e) {
        hpBar.fillAmount = e.HPNormalized;
        mpBar.fillAmount = e.MPNormalized;
        levelHeart.fillAmount = e.levelNormalized;
        Debug.Log(e.levelNormalized);
    }
}