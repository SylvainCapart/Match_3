using System.Collections;
using UnityEngine;

/// <summary>
/// Item manager.
/// Class responsible for the Items : movement, creation, delete.
/// </summary>
public class ItemManager : MonoBehaviour
{

    public static ItemManager instance;

    /// <summary>
    /// The neutral item prefab. (Color of the sprite white)
    /// </summary>
    [SerializeField] private GameObject m_NeutralItem;

    /// <summary>
    /// Empty Transform to store the Items
    /// </summary>
    [SerializeField] private Transform m_ItemsParent;

    /// <summary>
    /// Smooth Time for the Items movements.
    /// </summary>
    private float m_MoveSmoothTime = 0.2f;

    /// <summary>
    /// Tolerance for the Items movements.
    /// </summary>
    private float m_MoveTol = 0.1f;

    /// <summary>
    /// Reference velocity used for the movement.
    /// </summary>
    private Vector3 velocity = Vector3.zero;

    /// <summary>
    /// Is the bomb bonus present.
    /// </summary>
    private bool isBombPresent = false;

    public bool IsBombPresent { get => isBombPresent; set => isBombPresent = value; }

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

    /// <summary>
    /// Loads the item.
    /// </summary>
    /// <returns>The item.</returns>
    /// <param name="_pos">Position : used the set the object in the world</param>
    /// <param name="_index">Index : used to set the index in the Item class</param>
    public Item LoadItem(Vector3 _pos, Vector3Int _index)
    {
        Item clone = Instantiate(m_NeutralItem, _pos, Quaternion.identity).GetComponent<Item>();
        clone.transform.SetParent(m_ItemsParent);
        return clone.Init(_index);
    }

    /// <summary>
    /// Starts the moving coroutine
    /// </summary>
    /// <param name="_item">Item.</param>
    /// <param name="_target">Target.</param>
    public void MoveItem(Item _item, Vector3 _target)
    {
        IEnumerator moveItemCo = MoveItemCo(_item, _target);
        StartCoroutine(moveItemCo);
    }

    /// <summary>
    /// Moves the item.
    /// </summary>
    /// <param name="_item">Item.</param>
    /// <param name="_target">Target.</param>
    private IEnumerator MoveItemCo(Item _item, Vector3 _target)
    {
        // check if the item is not null as it may havebeen destroyed while the coroutine is still running
        // and check if the target is reached
        while (_item != null && Vector3.Distance(_item.transform.position, _target) > m_MoveTol)
        {
            _item.transform.position = Vector3.SmoothDamp(_item.transform.position, _target, ref velocity, m_MoveSmoothTime);
            yield return null;
        }
    }

    /// <summary>
    /// Clears the items from the grid.
    /// Used at restart.
    /// </summary>
    public void ClearItems()
    {
        foreach (Transform child in m_ItemsParent)
        {
            Destroy(child.gameObject);
        }
        IsBombPresent = false;
    }

}
