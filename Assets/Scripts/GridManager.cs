using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Grid manager.
/// Class responsible for managing the grid made of hexagonal tiles and items on them, the mouse control, the coordination of the player selection and resulting actions
/// </summary>
public class GridManager : MonoBehaviour
{
    public static GridManager instance;

    /// <summary>
    /// Reference to the hexagonal tilemap
    /// </summary>
    [SerializeField] private Tilemap m_HexMap;

    /// <summary>
    /// Base hexagonal tile (not highlighted)
    /// </summary>
    [SerializeField] private Tile m_BaseTile;

    /// <summary>
    /// Highlighted hexagonal tile.
    /// </summary>
    [SerializeField] private Tile m_HighlightedTile;

    /// <summary>
    /// The size of grid (horizontal and vertical). Not in Globals in cas it should change for a new level.
    /// </summary>
    private int m_GridHorSize = 6;
    private int m_GridVerSize = 7;

    /// <summary>
    /// The index on the grid of the last overed tile.
    /// </summary>
    private Vector3Int m_LastOveredTileIndex = Vector3Int.zero;

    /// <summary>
    /// The index on the grid of the last added tile (to the chain list).
    /// </summary>
    private Vector3Int m_LastAddedTileIndex = Vector3Int.zero;

    /// <summary>
    /// Is the level loaded
    /// </summary>
    private bool m_LevelLoaded = false;

    /// <summary>
    /// Check the grid for empty tiles (without Item) every m_GridCheckDelay
    /// </summary>
    private float m_GridCheckDelay = 0.3f;

    /// <summary>
    /// Is the grid checking activated
    /// </summary>
    private bool m_GridCheckActive = true;

    /// <summary>
    /// Was the last grid check valid.
    /// In other words, is the grid full of Items now.
    /// </summary>
    private bool m_LastGridCheckValid = true;

    /// <summary>
    /// Chain selecter used by the player.
    /// </summary>
    private ChainSelecter m_Chain;

    /// <summary>
    /// PlayableHex 2D table, making the link between the tiles of the grid and the items on them
    /// </summary>
    private PlayableHex[,] m_PlayableGrid;


    public bool LevelLoaded
    {
        get
        {
            return m_LevelLoaded;
        }
    }

    public bool LastGridCheckValid
    {
        get
        {
            return m_LastGridCheckValid;
        }
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

    void Start()
    {
        m_PlayableGrid = new PlayableHex[m_GridHorSize, m_GridVerSize];

        m_Chain = new ChainSelecter();
        m_Chain.Init();

        IEnumerator checkGridCo = CheckGridCo();
        StartCoroutine(checkGridCo);
    }

    void Update()
    {
        // Index of the tile pointed by the mouse
        Vector3Int tilePos = m_HexMap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        // if the level is loaded
        // and if the grid is full of tiles
        // and the mouse is on the grid
        // and the player still has actions
        if (m_LevelLoaded && m_LastGridCheckValid && !MouseOutGridBounds(tilePos.x, tilePos.y) && MovesManager.instance.MovesLeft > 0)
        {
            // Mouse UseCase 1 : if the mouse is not held pressed
            if (!Input.GetMouseButton(0))
            {
                // and if the mouse points a new tile
                if (m_LastOveredTileIndex != tilePos)
                {
                    // Highlight the new tile
                    m_HexMap.SetTile(tilePos, m_HighlightedTile);
                    m_PlayableGrid[tilePos.x, tilePos.y].Highlighted = true;

                    // Cancel previous tile highlight
                    m_HexMap.SetTile(m_LastOveredTileIndex, m_BaseTile);
                    m_PlayableGrid[m_LastOveredTileIndex.x, m_LastOveredTileIndex.y].Highlighted = false;
                }
            }
            // Mouse UseCase 2 : if the mouse is held pressed
            else
            {
                // Mouse UseCase 2.1 : if the mouse points a new tile
                // and the new tile has an Item
                // and the item on the new tile is not highlighted (not present in the chain)
                // and the new tile is a neighbour from the last added tile to the chain
                if (tilePos != m_LastOveredTileIndex && m_PlayableGrid[tilePos.x, tilePos.y].Filled && !m_PlayableGrid[tilePos.x, tilePos.y].Highlighted && IsNeighbourFrom(tilePos, m_LastAddedTileIndex))
                {
                    // Add the Item to the list
                    if (m_Chain.AddItem(m_PlayableGrid[tilePos.x, tilePos.y].Item))
                    {
                        // Highlight the new added tile
                        m_HexMap.SetTile(tilePos, m_HighlightedTile);
                        m_PlayableGrid[tilePos.x, tilePos.y].Highlighted = true;
                        // last added item is on the overed tile
                        m_LastAddedTileIndex = tilePos;
                    }
                }
                // Mouse UseCase 2.2 : if the mouse points a new tile
                // and the new tile is already highlighted (player points backwards on the chain)
                else if (tilePos != m_LastOveredTileIndex && m_PlayableGrid[tilePos.x, tilePos.y].Highlighted)
                {
                    // if the Item is effectively in the chain (additional check, relevance to examine)
                    // TODO : analyze relevance of this additional list check
                    if (m_Chain.ItemsList.Contains(m_PlayableGrid[tilePos.x, tilePos.y].Item))
                    {
                        // for each item in the list after the one selected
                        for (int i = m_Chain.ItemsList.Count - 1; i > m_Chain.ItemsList.IndexOf(m_PlayableGrid[tilePos.x, tilePos.y].Item); i--)
                        {
                            // Cancel Highlight
                            m_PlayableGrid[m_Chain.ItemsList[i].GridIndex.x, m_Chain.ItemsList[i].GridIndex.y].Highlighted = false;
                            m_HexMap.SetTile(m_Chain.ItemsList[i].GridIndex, m_BaseTile);
                        }
                        m_LastAddedTileIndex = tilePos;
                        // Clear the chain of all the items after the own selected
                        m_Chain.ClearChainListAfter(m_Chain.ItemsList.IndexOf(m_PlayableGrid[tilePos.x, tilePos.y].Item));
                    }
                }

            }

            m_LastOveredTileIndex = tilePos;

            // Mouse UseCase 3 : mouse is clicked for the first time
            if (Input.GetMouseButtonDown(0))
            {
                // if the pointed tile is filled with an Item
                if (m_PlayableGrid[tilePos.x, tilePos.y].Filled)
                {
                    // set it as being the origin of the chain
                    m_Chain.SetChainOrigin(m_PlayableGrid[tilePos.x, tilePos.y].Item.ItemType);

                    // if the Item is successfully added to the chain
                    if (m_Chain.AddItem(m_PlayableGrid[tilePos.x, tilePos.y].Item))
                    {
                        // Highligh the tile
                        m_HexMap.SetTile(tilePos, m_HighlightedTile);
                        m_PlayableGrid[tilePos.x, tilePos.y].Highlighted = true;
                        m_LastAddedTileIndex = tilePos;
                    }
                }

            }

            // Mouse UseCase 4 : mouse button is released for the first time
            if (Input.GetMouseButtonUp(0))
            {
                // for each item of the chain
                foreach (Item it in m_Chain.ItemsList)
                {
                    // set all tiles to normal (cancel highlight)
                    m_HexMap.SetTile(it.GridIndex, m_BaseTile);
                    m_PlayableGrid[it.GridIndex.x, it.GridIndex.y].Highlighted = false;

                    // if the chain is long enough
                    if (m_Chain.IsChainValid())
                    {
                        // Consider the tile as being empty
                        m_PlayableGrid[it.GridIndex.x, it.GridIndex.y].Filled = false;

                        // Following the item bonus on the tile
                        switch (it.ItemBonus)
                        {
                            case Item.Bonus.NONE:
                                // score points 
                                // TODO : gather score formula in a method held by the score manager
                                it.ScorePop((int)(it.Points * (1.0f + ((float)m_Chain.ItemsList.Count * Globals.COMBO_MULTIPLIER))));
                                break;
                            case Item.Bonus.DOUBLE_POINTS:
                                // score points 
                                // TODO : gather score formula in a method held by the score manager
                                it.ScorePop((int)(2 * it.Points * (1.0f + ((float)m_Chain.ItemsList.Count * Globals.COMBO_MULTIPLIER))));
                                break;
                            case Item.Bonus.BOMB:
                                // score points 
                                // TODO : gather score formula in a method held by the score manager
                                it.ScorePop((int)(it.Points * (1.0f + ((float)m_Chain.ItemsList.Count * Globals.COMBO_MULTIPLIER))));
                                ItemManager.instance.IsBombPresent = false;

                                Vector3Int itemIndex = Vector3Int.zero;

                                // Overall check in grid to find the neighbours of the bomb
                                // TODO : non optimized, write a function that returns the neighbours on the tile at specific index
                                for (int i = 0; i < m_GridHorSize; i++)
                                {
                                    for (int j = 0; j < m_GridVerSize; j++)
                                    {
                                        itemIndex.x = i;
                                        itemIndex.y = j;

                                        // if the searched tile in the grid is neighbour from the bomb
                                        // and is not present in the chain
                                        if (IsNeighbourFrom(itemIndex, it.GridIndex) && !m_Chain.ItemsList.Contains(m_PlayableGrid[itemIndex.x, itemIndex.y].Item))
                                        {
                                            m_PlayableGrid[itemIndex.x, itemIndex.y].Filled = false;
                                            // score points 
                                            // TODO : gather score formula in a method held by the score manager
                                            m_PlayableGrid[itemIndex.x, itemIndex.y].Item.ScorePop((int)(m_PlayableGrid[itemIndex.x, itemIndex.y].Item.Points * (1.0f + ((float)m_Chain.ItemsList.Count * Globals.COMBO_MULTIPLIER))));

                                            // destroy neighbour
                                            Destroy(m_PlayableGrid[itemIndex.x, itemIndex.y].Item.gameObject);
                                        }

                                    }
                                }
                                break;
                            case Item.Bonus.ERASE_COLUMN:
                                // score points 
                                // TODO : gather score formula in a method held by the score manager
                                it.ScorePop((int)(it.Points * (1.0f + ((float)m_Chain.ItemsList.Count * Globals.COMBO_MULTIPLIER))));

                                for (int k = 0; k < m_GridHorSize; k++)
                                {
                                    // if the searched item in the column is not already in the chain
                                    if (!m_Chain.ItemsList.Contains(m_PlayableGrid[k, it.GridIndex.y].Item))
                                    {
                                        m_PlayableGrid[k, it.GridIndex.y].Filled = false;
                                        // score points 
                                        // TODO : gather score formula in a method held by the score manager
                                        m_PlayableGrid[k, it.GridIndex.y].Item.ScorePop((int)(m_PlayableGrid[k, it.GridIndex.y].Item.Points * (1.0f + ((float)m_Chain.ItemsList.Count * Globals.COMBO_MULTIPLIER))));

                                        // destroy item in the column
                                        Destroy(m_PlayableGrid[k, it.GridIndex.y].Item.gameObject);
                                    }

                                }


                                break;
                            default:
                                Debug.LogError("An Bonus Type is missing in GridManager.cs");
                                throw new System.NotImplementedException();
                        }

                        // Destroy item from the list.
                        Destroy(it.gameObject);

                    }

                }
                // Decrease the number of possible actions
                if (m_Chain.IsChainValid())
                {
                    MovesManager.instance.MovesDecrease();
                }

                // Clear the list from all null references
                m_Chain.ClearChainList();


            }
            m_LastOveredTileIndex = tilePos;

        }
        // if the level is loaded
        // and if the grid is full of tiles
        // and the mouse is not on the grid
        else if (m_LevelLoaded && m_LastGridCheckValid && MouseOutGridBounds(tilePos.x, tilePos.y))
        {
            // Cancel highlight
            foreach (Item it in m_Chain.ItemsList)
            {
                m_HexMap.SetTile(it.GridIndex, m_BaseTile);
                m_PlayableGrid[it.GridIndex.x, it.GridIndex.y].Highlighted = false;
            }
            // Completely clear the chain
            m_Chain.ClearChainList();
        }
    }

    /// <summary>
    /// Checks the grid to detect if a tile has no item.
    /// </summary>
    private IEnumerator CheckGridCo()
    {
        // while grid checking is activated
        while (m_GridCheckActive)
        {
            // if the level is laoded
            if (m_LevelLoaded)
            {
                // returns the first empty index found, -1 otherwise
                int emptyHexIndex = CheckEmptyHex();

                // if an empty tile is found, refill the grid with items.
                if (emptyHexIndex != -1)
                    FillEmptyHex(emptyHexIndex);

                yield return new WaitForSeconds(m_GridCheckDelay);
            }
            else
                yield return null;
        }

    }

    /// <summary>
    /// Checks the grid for empty hex slots.
    /// </summary>
    /// <returns>The first empty hex slot index found, or -1 if the grid is full.</returns>
    public int CheckEmptyHex()
    {
        for (int i = 0; i < m_GridHorSize; i++)
        {
            for (int j = 0; j < m_GridVerSize; j++)
            {
                if (!m_PlayableGrid[i, j].Filled)
                {
                    m_LastGridCheckValid = false;
                    return j;
                }

            }
        }
        m_LastGridCheckValid = true;
        return -1;
    }

    /// <summary>
    /// Fills the empty hex tiles with items, on the column _indexY.
    /// </summary>
    /// <param name="_indexY">Index y.</param>
    public void FillEmptyHex(int _indexY)
    {
        IEnumerator fillEmptyCo = FillEmptyHexCo(_indexY);
        StartCoroutine(fillEmptyCo);
    }

    /// <summary>
    /// Fills the empty hex tiles with items, on the column _indexY.
    /// </summary>
    /// <param name="_indexY">Index y.</param>
    private IEnumerator FillEmptyHexCo(int _indexY)
    {
        // for each row except the last one 
        for (int i = 0; i < m_GridHorSize - 1; i++)
        {
            // if the tile is not filled and the tile above it is filled
            if (!m_PlayableGrid[i, _indexY].Filled && m_PlayableGrid[i + 1, _indexY].Filled)
            {
                // move the Item above on the tile below
                ItemManager.instance.MoveItem(m_PlayableGrid[i + 1, _indexY].Item, m_HexMap.CellToWorld(new Vector3Int(i, _indexY, 0)));
                m_PlayableGrid[i + 1, _indexY].Item.GridIndex = new Vector3Int(i, _indexY, 0);
                m_PlayableGrid[i, _indexY].Filled = true;
                m_PlayableGrid[i + 1, _indexY].Filled = false;

                // assign the item at its new place
                m_PlayableGrid[i, _indexY].Item = m_PlayableGrid[i + 1, _indexY].Item;
            }
            yield return null;
        }

        int topIndex = m_GridHorSize - 1;

        // if tiles of column _indexY and of last row, is not filled
        if (!m_PlayableGrid[topIndex, _indexY].Filled)
        {
            // load a new random Item on the grid
            Item item = ItemManager.instance.LoadItem(m_HexMap.CellToWorld(new Vector3Int(topIndex, _indexY, 0)), new Vector3Int(topIndex, _indexY, 0));
            m_PlayableGrid[topIndex, _indexY] = new PlayableHex(item);
        }
        yield return null;

    }

    /// <summary>
    /// Is the tile with index _tilePos a neighbour of the tile with index _centerTile
    /// </summary>
    /// <returns><c>true</c>, if _tilePos is a neighbour, <c>false</c> otherwise.</returns>
    /// <param name="_tilePos">Tile position.</param>
    /// <param name="_centerTile">Center tile.</param>
    private bool IsNeighbourFrom(Vector3Int _tilePos, Vector3Int _centerTile)
    {
        // Odd column
        if (_centerTile.y % 2 == 1)
        {
            return (_tilePos.y == _centerTile.y && _tilePos.x == _centerTile.x + 1) ||
                   (_tilePos.y == _centerTile.y && _tilePos.x == _centerTile.x - 1) ||
                   (_tilePos.y == _centerTile.y - 1 && _tilePos.x == _centerTile.x) ||
                   (_tilePos.y == _centerTile.y + 1 && _tilePos.x == _centerTile.x) ||
                   (_tilePos.y == _centerTile.y - 1 && _tilePos.x == _centerTile.x + 1) ||
                   (_tilePos.y == _centerTile.y + 1 && _tilePos.x == _centerTile.x + 1);
        }
        // Even column
        else
        {
            return (_tilePos.y == _centerTile.y && _tilePos.x == _centerTile.x + 1) ||
                   (_tilePos.y == _centerTile.y && _tilePos.x == _centerTile.x - 1) ||
                   (_tilePos.y == _centerTile.y - 1 && _tilePos.x == _centerTile.x) ||
                   (_tilePos.y == _centerTile.y + 1 && _tilePos.x == _centerTile.x) ||
                   (_tilePos.y == _centerTile.y - 1 && _tilePos.x == _centerTile.x - 1) ||
                   (_tilePos.y == _centerTile.y + 1 && _tilePos.x == _centerTile.x - 1);
        }
    }

    /// <summary>
    /// Starts the loading level Coroutine
    /// </summary>
    public void LoadLevel()
    {
        IEnumerator loadLevelCo = LoadLevelCo();
        StartCoroutine(loadLevelCo);
    }

    /// <summary>
    /// Loads the level.
    /// </summary>
    private IEnumerator LoadLevelCo()
    {
        m_LevelLoaded = false;


        Vector3Int newTilePos = Vector3Int.zero;

        // First loop to place the tiles on by one
        for (int i = 0; i < m_GridHorSize; i++)
        {
            for (int j = 0; j < m_GridVerSize; j++)
            {
                yield return new WaitForSeconds(Globals.TILE_INIT_DELAY);

                newTilePos.x = i;
                newTilePos.y = j;

                m_HexMap.SetTile(newTilePos, m_BaseTile);
            }
        }

        // Second loop to make all items appear at the same time
        for (int i = 0; i < m_GridHorSize; i++)
        {
            for (int j = 0; j < m_GridVerSize; j++)
            {
                newTilePos.x = i;
                newTilePos.y = j;

                // load each item on the grid
                Item item = ItemManager.instance.LoadItem(m_HexMap.CellToWorld(newTilePos), newTilePos);
                m_PlayableGrid[i, j] = new PlayableHex(item);

            }
        }
        m_LevelLoaded = true;
    }

    /// <summary>
    /// Restarts the level.
    /// </summary>
    public void RestartLevel()
    {
        // if the level finished to load
        if (m_LevelLoaded)
        {
            m_LevelLoaded = false;

            // Delete all the Items on the grid
            ItemManager.instance.ClearItems();

            Vector3Int tileIndex = Vector3Int.zero;

            for (int i = 0; i < m_GridHorSize; i++)
            {
                for (int j = 0; j < m_GridVerSize; j++)
                {
                    tileIndex.x = i;
                    tileIndex.y = j;

                    // Load a new item on each tile of the grid
                    Item item = ItemManager.instance.LoadItem(m_HexMap.CellToWorld(tileIndex), tileIndex);
                    m_PlayableGrid[i, j] = new PlayableHex(item);
                }
            }
        }
        m_LevelLoaded = true;
    }

    /// <summary>
    /// Is the mouse out of the bounds of the grid.
    /// </summary>
    /// <returns><c>true</c>, if mouse position is out of the grid, <c>false</c> otherwise.</returns>
    /// <param name="_x">I.</param>
    /// <param name="_y">Y.</param>
    private bool MouseOutGridBounds(int _x, int _y)
    {
        return (_x < 0 || _x >= m_GridHorSize || _y < 0 || _y >= m_GridVerSize);
    }

}

