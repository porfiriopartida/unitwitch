using UnityEngine;

public class DemoUIManager : MonoBehaviour
{
    public static DemoUIManager Instance;

    public GameObject textElement;
    void Start()
    {
        Instance = this;
    }
}
