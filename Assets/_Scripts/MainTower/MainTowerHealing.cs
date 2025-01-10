using System;
using UnityEngine;

public class MainTowerHealing : MonoBehaviour
{
    [SerializeField] private GameObject shield;
    private float _curHp;
    private float _maxHp;
    private float _curMp;
    private float _maxMp;
    public Action<float> OnTakeDamage;

    public void InitData(float curHp, float maxHp, float curMp, float maxMp)
    {
        _curHp = curHp;
        _maxHp = maxHp;
        _curMp = curMp;
        _maxMp = maxMp;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(GameTags.TAG_ENEMY))
        {
            OnTakeDamage?.Invoke(5);
            Debug.Log("Trigger");
        }
    }
    
    //healing
}
