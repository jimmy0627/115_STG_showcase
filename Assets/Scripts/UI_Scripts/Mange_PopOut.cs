using UnityEngine;

public class Mange_PopOut : MonoBehaviour
{
    //開關彈窗
    [SerializeField] GameObject popout;
    public void Open_Popout()
    {
        popout.SetActive(true);
    }
    public void Close_Popout()
    {
        popout.SetActive(false);
    }
}
