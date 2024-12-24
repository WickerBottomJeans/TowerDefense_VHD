using System;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeIconTemplate : MonoBehaviour
{
    [SerializeField] private Image image;

    public void SetLevelSO(LevelSO levelSO) {
        image.sprite = levelSO.levelSprite;
    }


}
