using UnityEngine;

[CreateAssetMenu(fileName = "New Game Type", menuName = "Scriptable Object/Game Type")]
public class GameType : ScriptableObject
{
    public string modeName;
    public int lapCount;
}
