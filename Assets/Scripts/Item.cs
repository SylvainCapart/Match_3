using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Item.
/// Class used to represent the items that the player can collect.
/// </summary>
public class Item : MonoBehaviour
{
    /// <summary>
    /// Bonus of the Item
    /// NONE : no bonus
    /// DOUBLE_POINTS : the item on the tile will individually score twice its points
    /// BOMB : breaking a chain with an item with Bomb bonus in it will destroy and score the neigbour items around the bomb
    /// BOMB : breaking a chain with an item with EraseColumn bonus in it will destroy and score the whole column
    /// </summary>
    public enum Bonus { NONE, DOUBLE_POINTS, BOMB, ERASE_COLUMN }
    private Bonus m_ItemBonus;

    /// <summary>
    /// Type of the Item, impacting its graphic and points scored.
    /// </summary>
    public enum Type { BLUE, GREEN, YELLOW, ORANGE, RED }
    private Type m_ItemType;

    /// <summary>
    /// The index of the item on the grid. Z axis is never used and always 0.
    /// </summary>
    private Vector3Int m_GridIndex;

    /// <summary>
    /// Points that will be scored by the Item.
    /// </summary>
    private int m_Points;

    /// <summary>
    /// Reference to the sprite renderer through Editor
    /// </summary>
    [SerializeField] private SpriteRenderer m_SpriteRenderer;

    /// <summary>
    /// Reference to the score text prefab through Editor. Used when the chain selected by the player is released to show the points scored by each item.
    /// </summary>
    [SerializeField] private GameObject m_TextPrefab;

    /// <summary>
    /// Reference to the bonus prefab through Editor
    /// </summary>
    [SerializeField] private GameObject m_BonusPrefab;

    /// <summary>
    /// Reference to the bonus sprites through Editor
    /// </summary>
    [SerializeField] private Sprite[] m_BonusSprites;

    public Vector3Int GridIndex{ get => m_GridIndex; set => m_GridIndex = value; }
    public int Points { get => m_Points; set => m_Points = value; }

    public Type ItemType
    {
        get => m_ItemType;
        set
        {
            // Assigning points and graphics to each type of item
            // TODO : remove raw values for the colors ( define them in Globals or in a color array in Editor )
            switch (value)
            {
                case Type.BLUE:
                    m_SpriteRenderer.material.color = new Color(0, 0.32f, 1, 1);
                    m_Points = Globals.POINTS_ITEM_BLUE;
                    break;
                case Type.GREEN:
                    m_SpriteRenderer.material.color = new Color(0.1f, 0.95f, 0.4f, 1);
                    m_Points = Globals.POINTS_ITEM_GREEN;
                    break;
                case Type.YELLOW:
                    m_SpriteRenderer.material.color = new Color(0.95f, 1, 0.4f, 1);
                    m_Points = Globals.POINTS_ITEM_YELLOW;
                    break;
                case Type.ORANGE:
                    m_SpriteRenderer.material.color = new Color(1, 0.69f, 0, 1);
                    m_Points = Globals.POINTS_ITEM_ORANGE;
                    break;
                case Type.RED:
                    m_SpriteRenderer.material.color = Color.red;
                    m_Points = Globals.POINTS_ITEM_RED;
                    break;
                default:
                    Debug.LogError("An Item Type is missing in Item.cs");
                    throw new System.NotImplementedException();

            }
            m_ItemType = value;
        }
    }

    public Bonus ItemBonus
    {
        get => m_ItemBonus;
        set
        {
            m_ItemBonus = value;
            // No additional sprite needed if there is no Bonus
            if (value == Bonus.NONE) return;

            GameObject clone = Instantiate(m_BonusPrefab, transform);
            clone.GetComponent<SpriteRenderer>().sprite = m_BonusSprites[((int) m_ItemBonus) - 1];
        }
    }

    public Item(Vector3Int _index, Type _type, Bonus _bonus)
    {
        GridIndex = _index;
        ItemType = _type;
        ItemBonus = _bonus;
    }

    /// <summary>
    /// Initialize the Item at the specified Vector3Int _index.
    /// </summary>
    /// <returns>The Item.</returns>
    /// <param name="_index">Index.</param>
    public Item Init(Vector3Int _index)
    {
        int randomNb = Random.Range(0, 100); 
        int randomNbBonus = Random.Range(0, 100);

        // Assigning the type of the Item following the probability distribution
        if (randomNb >= 0 && randomNb < Globals.DISTRIB_ITEM_BLUE)
        {
            ItemType = Type.BLUE;
        }
        else if (randomNb >= Globals.DISTRIB_ITEM_BLUE && randomNb < Globals.DISTRIB_ITEM_YELLOW)
        {
            ItemType = Type.YELLOW;
        }
        else if (randomNb >= Globals.DISTRIB_ITEM_YELLOW && randomNb < Globals.DISTRIB_ITEM_GREEN)
        {
            ItemType = Type.GREEN;
        }
        else if (randomNb >= Globals.DISTRIB_ITEM_GREEN && randomNb < Globals.DISTRIB_ITEM_ORANGE)
        {
            ItemType = Type.ORANGE;
        }
        else if (randomNb >= Globals.DISTRIB_ITEM_ORANGE && randomNb < Globals.DISTRIB_ITEM_RED)
        {
            ItemType = Type.RED;
        }
        else
        {
            Debug.LogError("The probability distribution for the item types in Global.cs is wrong.");
            throw new System.NotImplementedException();
        }

        // Assigning the type of the Bonus following the probability distribution
        if (randomNbBonus >= 0 && randomNbBonus < Globals.DISTRIB_BONUS_X2)
        {
            ItemBonus = Bonus.NONE;
        }
        else if (randomNbBonus >= Globals.DISTRIB_BONUS_X2 && randomNbBonus < Globals.DISTRIB_BONUS_BOMB)
        {
            ItemBonus = Bonus.DOUBLE_POINTS;
        }
        else if (randomNbBonus >= Globals.DISTRIB_BONUS_BOMB && randomNbBonus < Globals.DISTRIB_BONUS_ERAS_COL)
        {
            // To avoid reaction chains with multiple bombs, only one bomb is set
            // TODO : reaction chain with more than one bomb
            if (!ItemManager.instance.IsBombPresent)
            {
                ItemManager.instance.IsBombPresent = true;
                ItemBonus = Bonus.BOMB;
            }
            else
                ItemBonus = Bonus.NONE;
        }
        else
        {
            ItemBonus = Bonus.ERASE_COLUMN;
        }

        m_GridIndex = _index;

        return this;
    }

    /// <summary>
    /// Make a score text pop at the position of the item.
    /// Increment the score through the score manager.
    /// TODO : make the score manager create this Score Text at the position of the Item.
    /// </summary>
    /// <param name="_points">Points.</param>
    public void ScorePop(int _points)
    {
        // Text prefab instantiated
        GameObject clone = Instantiate(m_TextPrefab, this.transform.position, Quaternion.identity);

        TextSetter textSetter = clone.GetComponent<TextSetter>();
        if (textSetter != null)
        {
            textSetter.StringToSet = _points.ToString();
            textSetter.TextToSet.transform.position = Camera.main.WorldToScreenPoint(this.transform.position);
        }

        // Score update
        ScoreManager.instance.ScoreIncrement(_points);
        Destroy(clone, 1.1f);
    }


}
