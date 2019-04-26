using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public static GridManager instance;
    [SerializeField] private Tilemap m_HexMap;
    [SerializeField] private Tile m_BaseTile;



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

    // Start is called before the first frame update
    void Start()
    {
        Tile tile = new Tile();

        m_HexMap.SetTile(new Vector3Int(0,0,0), tile);


    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Debug.Log(m_HexMap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition)));

            Vector3Int tilePos = m_HexMap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));


        }

    }

    public void LoadLevel(int nHexSlotHor, int nHexSlotVer)
    {
        IEnumerator loadLevelCo = LoadLevelCo(nHexSlotHor, nHexSlotVer);
        StartCoroutine(loadLevelCo);
    }

    private IEnumerator LoadLevelCo(int nHexSlotHor, int nHexSlotVer)
    {


        Vector3Int newTilePos = Vector3Int.zero;

        for (int i = 0; i < nHexSlotVer; i++)
        {
            for (int j = 0; j < nHexSlotHor; j++)
            {
                yield return new WaitForSeconds(Globals.TILE_INIT_DELAY);

                newTilePos.x = i;
                newTilePos.y = j;
                m_HexMap.SetTile(newTilePos, m_BaseTile);
            }
        }

        for (int i = 0; i < nHexSlotVer; i++)
        {
            for (int j = 0; j < nHexSlotHor; j++)
            {
                newTilePos.x = i;
                newTilePos.y = j;
                ItemManager.instance.LoadItem(m_HexMap.CellToWorld(newTilePos));
            }
        }

        MouseController.instance.m_MouseActive = true;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere( Camera.main.ScreenToWorldPoint(Input.mousePosition), 0.2f);
    }

}

