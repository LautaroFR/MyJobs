using Assets._VRN.Core.Runtime.UI.Interactive;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Tables;

[CreateAssetMenu(fileName = "NewQuestion", menuName = "Question")]
public class Question : ScriptableObject
{
    public int id;

    public LocalizationTable localizationTextTable;
    public LocalizationTable localizationAudioTable;

    public string questionKey;
    public List<InteractiveMenuOptionUIController.Option> responses = new List<InteractiveMenuOptionUIController.Option>();

    [Min (0)]
    public int MaximumCountSelectedOptions = 1;
}