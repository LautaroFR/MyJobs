using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToggleController : MonoBehaviour
{
    [SerializeField]
    private Toggle toggle;

    [SerializeField]
    private TMP_Text toggleText;

    private void Start() => toggle.onValueChanged.AddListener(OnToggleValueChanged);

    private void OnToggleValueChanged(bool arg0) => toggleText.text = toggle.isOn ? "Activado" : "Desactivado";
}
