using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game State")]
    public int playerGold = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddGold(int amount)
    {
        playerGold += amount;
        Debug.Log($"Gold added! Current Gold: {playerGold}");
    }

    public void SubtractGold(int amount)
    {
        playerGold -= amount;
        if (playerGold < 0) playerGold = 0;
        Debug.Log($"Gold deducted! Current Gold: {playerGold}");
    }
}
