using UnityEngine;

[CreateAssetMenu(fileName ="Level", menuName="States/Level")]
public class Level : ScriptableObject
{
    public Recording.Sentiment[] Sentiments { get; set; }
}
