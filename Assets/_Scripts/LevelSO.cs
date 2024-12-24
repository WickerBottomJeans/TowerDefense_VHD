using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Game/Level")]
public class LevelSO : ScriptableObject {
    public int level;
    public Sprite levelSprite;
}
