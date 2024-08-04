using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextWrapper : MonoBehaviour
{
    public int maxCharactersPerLine = 30; // Número máximo de caracteres por línea

    private TextMeshProUGUI textMeshPro;
    private string previousText;

    private void Awake()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
        previousText = textMeshPro.text;
        WrapText();
    }

    private void Update()
    {
        if (textMeshPro.text != previousText)
        {
            WrapText();
            previousText = textMeshPro.text;
        }
    }

    public void WrapText()
    {
        string originalText = textMeshPro.text;
        string[] words = originalText.Split(' ');
        string wrappedText = "";
        string line = "";

        foreach (string word in words)
        {
            if ((line + word).Length > maxCharactersPerLine)
            {
                wrappedText += line.TrimEnd() + "\n";
                line = word + " ";
            }
            else
            {
                line += word + " ";
            }
        }

        wrappedText += line.TrimEnd();
        textMeshPro.text = wrappedText;
    }
}
