using UnityEngine;

public class MouseController : MonoBehaviour
{
    public static MouseController instance;
    public bool m_MouseActive = false;

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
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_MouseActive)
            return;
    }
}
