using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Chain selecter.
/// This class allows the player to select multiple Items and store them in a list.
/// Only Items of the same color (eg Type) can be stored in the list.
/// Different methods are provided to add and remove Items.
/// </summary>
public class ChainSelecter
{
    /// <summary>
    /// List holding the Items selected
    /// </summary>
    private List<Item> m_ItemsList;

    /// <summary>
    /// Is the origin of the list set (and therefore the color of the following Items)
    /// </summary>

    private bool m_IsOriginSet = false;

    /// <summary>
    /// The color of the chain (the color of the following Items))
    /// </summary>
    private Item.Type m_ChainColor;

    public List<Item> ItemsList { get => m_ItemsList; set => m_ItemsList = value; }

    public void Init()
    {
        m_ItemsList = new List<Item>();
    }


    public bool AddItem(Item _item)
    {
        bool itemAdded = false;

        // if the item to add is of the same color as the list...
        if (_item.ItemType == m_ChainColor)
        {
            // Add the item
            m_ItemsList.Add(_item);

            // return operation success
            itemAdded = true;

            // if there is more than one element in the list and if the item to add has no LineRenderer
            if (m_ItemsList.Count > 1 && _item.GetComponent<LineRenderer>() == null)
            {
                // Add a LineRnderer...
                LineRenderer lineRenderer = _item.transform.gameObject.AddComponent<LineRenderer>();

                // .. and draw a line from the previous item to the new item
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, m_ItemsList[m_ItemsList.Count - 2].transform.position);
                lineRenderer.SetPosition(1, m_ItemsList[m_ItemsList.Count - 1].transform.position);
                lineRenderer.startColor = Color.black;
                lineRenderer.endColor = Color.black;
                lineRenderer.startWidth = 0.02f;
                lineRenderer.endWidth = 0.02f;
                Material defaultMat = (Material)Resources.Load("Materials/LineMat");
                lineRenderer.material = defaultMat;

            }

        }

        return itemAdded;
    }

    /// <summary>
    /// Sets the chain color. If the origin is not set, then the color of the chain is the one of the argument item.
    /// </summary>
    /// <param name="_itemType">Item type.</param>
    public void SetChainOrigin(Item.Type _itemType)
    {
        if (!m_IsOriginSet)
        {
            m_ChainColor = _itemType;
            m_IsOriginSet = true;
        }
    }

    /// <summary>
    /// Clears the chain origin status.
    /// </summary>
    public void ClearChainOrigin()
    {
        m_IsOriginSet = false;
    }

    /// <summary>
    /// Clears the chain list from all Items in it.
    /// </summary>
    public void ClearChainList()
    {
        foreach (Item it in m_ItemsList)
        {
            // Destroy LineRenderer
            LineRenderer lineRenderer = it.transform.GetComponent<LineRenderer>();
            if (lineRenderer != null)
                Object.Destroy(lineRenderer);
        }
        m_ItemsList.Clear();
        ClearChainOrigin();
    }

    /// <summary>
    /// Clears the chain list after the index passed as argument.
    /// </summary>
    /// <param name="_index">Index.</param>
    public void ClearChainListAfter(int _index)
    {

        for (int i = m_ItemsList.Count - 1; i > _index; i--)
        {
            LineRenderer lineRenderer = m_ItemsList[i].transform.GetComponent<LineRenderer>();
            if (lineRenderer != null)
                Object.Destroy(lineRenderer);
            m_ItemsList.RemoveAt(i);
        }

    }

    /// <summary>
    /// Is the chain valid, that means is it longer than Globals.MIN_CHAIN_NB
    /// </summary>
    public bool IsChainValid()
    {
        return (m_ItemsList.Count >= Globals.MIN_CHAIN_NB);
    }

}
