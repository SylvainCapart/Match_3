using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Score manager.
/// Static class responsible for the display of the score
/// </summary>
public class ScoreManager : MonoBehaviour
{

    public static ScoreManager instance;

    /// <summary>
    /// Reference to the Text displaying the score
    /// </summary>
    [SerializeField] private Text m_ScoreCount;

    /// <summary>
    /// Current score. Initialized at 0.
    /// </summary>
    private int m_CurrentScore;

    public int CurrentScore
    {
        get => m_CurrentScore; 
        set
        {
            m_CurrentScore = value;
            // Text update.
            ScoreRefresh();
        }
    }

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        CurrentScore = 0;
    }

    void Awake()
    {
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            instance = this;
        }
    }

    public void ScoreIncrement(int _points)
    {
        CurrentScore += _points;
    }

    public void ScoreRefresh()
    {
        m_ScoreCount.text = m_CurrentScore.ToString();
    }
}
