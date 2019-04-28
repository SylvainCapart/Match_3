using UnityEngine;

/// <summary>
/// Game master.
/// Static class used for game management : performs intitialization, level loading, restart.
/// </summary>
public class GameMaster : MonoBehaviour
{
    public static GameMaster instance;

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

        // When GameMaster is awake, resolution is forced (handy-like) and full screen is forbidden
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


    public void Init()
    {
        // Level loading : the tiles and the items on them are generated
        GridManager.instance.LoadLevel();
    }

    /// <summary>
    /// Restart the level.
    /// </summary>
    public void Restart()
    {
        // Check if the previous level finished to load, and if all the items are placed on the grid
        if (GridManager.instance.LevelLoaded && GridManager.instance.LastGridCheckValid)
        {
            GridManager.instance.RestartLevel();
            ScoreManager.instance.Init();
            MovesManager.instance.Init();
        }

    }

}
