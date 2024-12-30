using System;
using UnityEngine;

public class LevelSystem : MonoBehaviour {
    public int level = 0;
    public float exp = 0f;
    public float expToNextLevel = 100f;
    public int maxLevel = 5;

    public event EventHandler<OnLevelUpEventArgs> OnLevelUp;

    public class OnLevelUpEventArgs : EventArgs {
        public int level;
        public bool isMaxLevel;
    }

    public void AddEXP(float amount) {
        if (level < maxLevel) {
            exp += amount;

            while (exp >= expToNextLevel) {
                exp -= expToNextLevel;
                LevelUp();
            }
        }
    }

    public void LevelUp() {
        level++;
        //tell the turret to upgrade()
        OnLevelUp?.Invoke(this, new OnLevelUpEventArgs {
            level = level,
            isMaxLevel = (level == maxLevel)
        });
    }

    public void SetexpToNextLevel(float amount) {
        expToNextLevel = amount;
    }

    public int GetLevel() => level;

    public float GetEXP() => exp;

    public float GetEXPToNextLevel() => expToNextLevel;

    public int GetMaxLevel() => maxLevel;
}