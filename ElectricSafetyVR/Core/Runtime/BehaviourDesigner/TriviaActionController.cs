using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

[TaskCategory("ISVR")]
public class TriviaActionController : Action
{
    public SharedObject chapterData;
    public SharedGameObject triviaGameObject;
    public SharedInt chapterScore;

    private bool _completed;

#if VRN_GOIDS_INCLUDE_DEPRECATED
    [UnityEngine.Header("DEPRECATED")]
    public VRN.GOIDs.GOIDAsset element;
#endif
    private TriviaController _trivia;

    public override void OnStart()
    {
        var data = chapterData.Value as ChapterData;

        if (triviaGameObject.Value != null)
        {
            _trivia = triviaGameObject.Value.GetComponent<TriviaController>();
        }
        _trivia.chapter = data;
        chapterScore.SetValue(0);

        _trivia.InitializeTrivia();

        _trivia.OnTriviaEnds.AddListener(OnTriviaFinished);
        _completed = false;
    }

    public override TaskStatus OnUpdate()
    {
        if (_completed)
            return TaskStatus.Success;

        else return TaskStatus.Running;
    }

    private void OnTriviaFinished()
    {
        chapterScore.SetValue(_trivia.correctAnswerCount);
        _completed = true;
    }
}
