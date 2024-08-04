using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScrollbarController : MonoBehaviour
{
    [SerializeField]
    private TMP_Text text;

    [SerializeField]
    private Scrollbar scrollbar;

    private void Start()
    {
        scrollbar.onValueChanged.AddListener(OnScrollbarValueChanged);
        text.text = "0";
    }

    private void OnScrollbarValueChanged(float value) => text.text = (value * 10).ToString("F0");
}
