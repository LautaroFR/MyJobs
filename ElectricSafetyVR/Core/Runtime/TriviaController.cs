using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = System.Random;
using Assets._VRN.Core.Runtime.UI.Interactive;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using System.Collections;
using UnityEngine.Events;

public class TriviaController : MonoBehaviour
{
    [SerializeField]
    private Canvas uiCanvas;

    [SerializeField]
    private InteractiveMenuOptionUIController uiController;

    [SerializeField]
    private AudioClip successClip;
    [SerializeField]
    private AudioClip failClip;

    [SerializeField]
    private AudioSource audioSourceNarrator;
    [SerializeField]
    private AudioSource audioSourceNotifications;

    public UnityEvent OnTriviaEnds = new UnityEvent();

    [HideInInspector]
    public ChapterData chapter;

    [HideInInspector]
    public int correctAnswerCount;

    private int _currentQuestionIndex;
    private int _questionsCount;

    private List<AudioClip> _audioClips = new List<AudioClip>();
    private List<Question> _questions   = new List<Question>();

    private Question _currentQuestion;

    private static Random rng = new Random();

    public void InitializeTrivia()
    {
        _questionsCount        = 0;
        correctAnswerCount    = 0;
        _currentQuestionIndex = 0;

        _questions = chapter.questions.OrderBy(_ => rng.Next()).ToList();

        uiCanvas.gameObject.SetActive(true);
        GetNextQuestion();
    }

    public void GetNextQuestion()
    {
        _audioClips.Clear();
        _currentQuestion = _questions[_currentQuestionIndex];
        SetUI(_currentQuestion);
        uiCanvas.enabled = true;
        
        AddClipsToList(_currentQuestion);
        StartCoroutine(Read());
    }

    private void AddClipsToList(Question question)
    {
       var clip = LocalizationSettings.AssetDatabase.GetLocalizedAsset<AudioClip>(
                question.localizationAudioTable.TableCollectionName,
                question.questionKey);
        _audioClips.Add(clip);

        foreach (var option in question.responses)
        {
            var optionClip = LocalizationSettings.AssetDatabase.GetLocalizedAsset<AudioClip>(
                question.localizationAudioTable.TableCollectionName,
                option.DisplayText);
            _audioClips.Add(optionClip);
        }
    }

    private void SetUI(Question question)
    {
        this.uiController.TitleText = this.GetLocalizationText(question.localizationTextTable, question.questionKey);
        this.uiController.MaximumCountSelectedOptions = question.MaximumCountSelectedOptions;
        this.uiController.ClearOptions();
        foreach (InteractiveMenuOptionUIController.Option option in question.responses)
        {
            var castedOption = new InteractiveMenuOptionUIController.Option
            {
                Id = option.Id,
                DisplayText = this.GetLocalizationText(question.localizationTextTable, option.DisplayText),
                IsValid = option.IsValid,
                IsEnabled = option.IsEnabled
            };

            this.uiController.AddOption(castedOption);
        }

        this.uiController.OnSelectedOptions.AddListener(this.HandleSelectedOptions);
    }

    private void HandleSelectedOptions(IEnumerable<InteractiveMenuOptionUIController.Option> arg0)
    {
        if(_currentQuestion.MaximumCountSelectedOptions != 1)
        {
            throw new System.Exception("Trivia does not support multiples responses yet");
        }

        bool correctAnswer = uiController.GetSelectedOptions().Where(option => option.IsSelected).First().Option.IsValid;

        OnQuestionEnds(correctAnswer);
    }

    private void OnQuestionEnds(bool correctAnswer)
    {
        uiCanvas.enabled = false;
        audioSourceNotifications.clip = correctAnswer ? successClip : failClip;
        audioSourceNotifications.Play();
        if(correctAnswer)
        {
            correctAnswerCount++;
        }

        _questionsCount++;
        _currentQuestionIndex++;
        this.uiController.OnSelectedOptions.RemoveListener(this.HandleSelectedOptions);

        audioSourceNarrator.Stop();
        StopAllCoroutines();
        if (_questionsCount < chapter.maximumQuestionCount)
        {
            GetNextQuestion();
        }
        else
        {
            OnTriviaFinished();
        }
    }

    private void OnTriviaFinished()
    {
        uiCanvas.gameObject.SetActive(false);
        OnTriviaEnds?.Invoke();
    }

    private IEnumerator Read()
    {
        foreach (AudioClip clip in _audioClips)
        {
            audioSourceNarrator.clip = clip;
            audioSourceNarrator.Play();

            yield return new WaitForSeconds(clip.length);

            while (audioSourceNarrator.isPlaying)
            {
                yield return null;
            }
        }
    }

    private string GetLocalizationText(LocalizationTable localizationTable, string tableEntryKey)
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString(
                       localizationTable.TableCollectionName,
                       tableEntryKey);
    }
    //public static void Shuffle<T>(IList<T> list)
    //{
    //    int n = list.Count;
    //    while (n > 1)
    //    {
    //        n--;
    //        int k = rng.Next(n + 1);
    //        T value = list[k];
    //        list[k] = list[n];
    //        list[n] = value;
    //    }
    //}
}
