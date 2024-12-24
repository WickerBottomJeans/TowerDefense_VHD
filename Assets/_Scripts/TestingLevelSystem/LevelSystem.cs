using System;
using UnityEngine;

public class LevelSystem : MonoBehaviour
{
    [SerializeField] private int level;
    [SerializeField] private float exp;
    [SerializeField] private float expToNextLevel;

    public event EventHandler<OnLevelUpEventArgs> OnLevelUp;
    public class OnLevelUpEventArgs : EventArgs {
        public int level;
    }
    public LevelSystem() {
        level = 0;
        exp = 0;
        expToNextLevel = 100;
    }

    public LevelSystem(int newLevel, float newEXP, float newEXPtoNextLevel) {
        level = newLevel;
        exp = newEXP;
        expToNextLevel = newEXPtoNextLevel;
    }

    public void AddEXP(float amount) {
        exp += amount;
        while (exp >= expToNextLevel) {
            exp -= expToNextLevel;
            LevelUp();
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

    public int GetLevel() => level;
    public float GetEXP() => exp;
    public float GetEXPToNextLevel() => expToNextLevel;
}
