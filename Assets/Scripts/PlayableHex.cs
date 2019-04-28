
/// <summary>
/// Playable hex.
/// Base class used to form the playable grid used in the GridManager. The class allows to link the tiles and the items that are on them,
/// because they hold information on both of them. 
/// </summary>
public class PlayableHex
{
    /// <summary>
    /// State of the tile, indicating if is highlighted or not.
    /// </summary>
    private bool m_Highlighted;

    /// <summary>
    /// State of the tile, indicating if there is an item on the tile or not.
    /// </summary>
    private bool m_Filled;

    /// <summary>
    /// If there is an item on the tile, the reference to the Item is stored in m_Item.
    /// </summary>
    private Item m_Item;

    public bool Highlighted { get => m_Highlighted; set => m_Highlighted = value; }
    public bool Filled { get => m_Filled; set => m_Filled = value; }
    public Item Item { get => m_Item; set => m_Item = value; }

    public PlayableHex(Item _item)
    {
        m_Highlighted = false;
        // When a PlayableHex is created, its Item is initialized as well, and considered as filled.
        m_Filled = true;
        m_Item = _item;
    }
}
