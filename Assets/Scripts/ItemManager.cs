using UnityEngine;

public class ItemManager : MonoBehaviour
{

    public static ItemManager instance;

    [SerializeField] private GameObject m_NeutralItem;
    [SerializeField] private Transform m_ItemsParent;

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
        
    }

    public void LoadItem(Vector3 _pos)
    {
        Item clone = Instantiate(m_NeutralItem, _pos, Quaternion.identity).GetComponent<Item>();
        clone.transform.SetParent(m_ItemsParent);

    }
}
