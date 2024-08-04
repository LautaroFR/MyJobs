using UnityEngine;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "NewChapterData", menuName = "Chapter Data")]
public class ChapterData : ScriptableObject
{
    [Min(1)]
    public int maximumQuestionCount;
    public List<Question> questions = new List<Question>();
    public List<PlayableAsset> orderedVideosTimelines = new List<PlayableAsset>();
}
