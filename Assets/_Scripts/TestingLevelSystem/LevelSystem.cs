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
    }

    public void AddEXP(float amount) {
        if (level < maxLevel) {
            exp += amount;
            Debug.Log($"Added EXP: {amount}. Current EXP: {exp}/{expToNextLevel}");

            while (exp >= expToNextLevel) {
                exp -= expToNextLevel;
                LevelUp();
            }
        }
    }

    public void LevelUp() {
        level++;
        Debug.Log("LevelUP!");
        //tell the turret to upgrade()
        OnLevelUp?.Invoke(this, new OnLevelUpEventArgs {
            level = level,
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