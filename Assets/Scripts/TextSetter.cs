using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Text setter.
/// Used to set at runtime the points displayed when a chain is broken
/// </summary>
public class TextSetter : MonoBehaviour
{
    private string m_StringToSet;
    [SerializeField] private Text m_TextToSet;

    public string StringToSet
    {
        get => m_StringToSet; 
        set
        {
            if (m_StringToSet == value) return;
            m_StringToSet = value;
            TextToSet.text = value;
        }
    }

    public Text TextToSet { get => m_TextToSet; set => m_TextToSet = value; }
}
