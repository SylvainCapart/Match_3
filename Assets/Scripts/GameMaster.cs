using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public static GameMaster instance;
    private int m_nHexSlotHor = 6;
    private int m_nHexSlotVer = 7;

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

#if UNITY_STANDALONE
        Screen.SetResolution(Globals.SCREEN_HOR_SIZE, Globals.SCREEN_VER_SIZE, false);
        Screen.fullScreen = false;
#endif

    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Init()
    {
        LoadLevel(m_nHexSlotHor, m_nHexSlotVer);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void LoadLevel(int nHexSlotHor, int nHexSlotVer)
    {
        GridManager.instance.LoadLevel(nHexSlotHor, nHexSlotVer);
    }
}
