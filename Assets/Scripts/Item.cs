using UnityEngine;
using Random = UnityEngine.Random;

public class Item : MonoBehaviour
{
    public enum Bonus { NONE, DOUBLE_POINTS, BOMB_NEAR, ERASE_ROW, ERASE_COLUMN }
    public enum Type { BLUE, GREEN, YELLOW, ORANGE, RED }

    private Vector3Int m_GridIndex;

    private int m_Points;

    private Type m_ItemType;

    private Bonus m_ItemBonus;

    private SpriteRenderer m_SpriteRenderer;


    public Vector3Int GridIndex
    {
        get => m_GridIndex;
        set
        {
            m_GridIndex.x = value.x;
            m_GridIndex.y = value.y;
            m_GridIndex.z = value.z;
        }
    }

    public int Points { get => m_Points; set => m_Points = value; }

    public Type ItemType
    {
        get => m_ItemType;
        set
        {
            switch (value)
            {
                case Type.BLUE:
                    m_SpriteRenderer.material.color = Color.blue;
                    break;
                case Type.GREEN:
                    m_SpriteRenderer.material.color = Color.green;
                    break;
                case Type.YELLOW:
                    m_SpriteRenderer.material.color = Color.yellow;
                    break;
                case Type.ORANGE:
                    m_SpriteRenderer.material.color = Color.black;
                    break;
                case Type.RED:
                    m_SpriteRenderer.material.color = Color.red;
                    break;
                default:
                    Debug.LogError("An Item Type was forgotten in Item.cs");
                    throw new System.NotImplementedException();

            }
            m_ItemType = value;
        }
    }

    public Bonus ItemBonus { get => m_ItemBonus; set => m_ItemBonus = value; }

    private void Start()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        if (m_SpriteRenderer == null)
        {
            Debug.LogError("The Item prefab has no Sprite Renderer component.");
            throw new System.NotImplementedException();
        }


        int randomNb = Random.Range(0, 100);

        if (randomNb >= 0 && randomNb < Globals.DISTRIB_ITEM_BLUE)
            ItemType = Type.BLUE;
        else if (randomNb >= Globals.DISTRIB_ITEM_BLUE && randomNb < Globals.DISTRIB_ITEM_YELLOW)
            ItemType = Type.YELLOW;
        else if (randomNb >= Globals.DISTRIB_ITEM_YELLOW && randomNb < Globals.DISTRIB_ITEM_GREEN)
            ItemType = Type.GREEN;
        else if (randomNb >= Globals.DISTRIB_ITEM_GREEN && randomNb < Globals.DISTRIB_ITEM_ORANGE)
            ItemType = Type.ORANGE;
        else if (randomNb >= Globals.DISTRIB_ITEM_ORANGE && randomNb < Globals.DISTRIB_ITEM_RED)
            ItemType = Type.RED;
        else
        {
            Debug.LogError("The probability distribution for the item types in Global.cs is wrong.");
            throw new System.NotImplementedException();
        }

    }

}
