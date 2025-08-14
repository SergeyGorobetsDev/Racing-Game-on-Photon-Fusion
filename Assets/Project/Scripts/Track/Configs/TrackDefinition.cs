using UnityEngine;

[CreateAssetMenu(fileName = "New Track Config", menuName = "Racing / Tracks / Track Config")]
public class TrackDefinition : ScriptableObject
{
    [field: SerializeField]
    public string TrackName { get; set; }
    [field: SerializeField]
    public Sprite TrackIcon { get; set; }
    [field: SerializeField]
    public int BuildIndex { get; set; }
}
