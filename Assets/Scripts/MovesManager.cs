using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Moves manager.
/// Static class responsible for limiting the number of actions of the player.
/// </summary>
public class MovesManager : MonoBehaviour
{
    public static MovesManager instance;

    /// <summary>
    /// Reference to the Text displaying the number of actions left.
    /// </summary>
    [SerializeField] private Text m_MovesLeftText;

    /// <summary>
    /// The number of moves left. Initialized with Globals.MAX_MOVES_NB
    /// </summary>
    private int m_MovesLeft;

    public int MovesLeft
    {
        get => m_MovesLeft;
        set
        {
            m_MovesLeft = value;
            // Text update
            MovesRefresh();
        }
    }

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        MovesLeft = Globals.MAX_MOVES_NB;
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

    public void MovesDecrease()
    {
        MovesLeft -= 1;
    }

    public void MovesRefresh()
    {
        m_MovesLeftText.text = m_MovesLeft.ToString();
    }
}
