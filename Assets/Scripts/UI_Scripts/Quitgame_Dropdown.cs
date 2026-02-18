using Unity.VisualScripting;
using UnityEngine;
using TMPro;
public class Quitgame_Dropdown : MonoBehaviour
{
    [SerializeField] GameObject manger;
    public void OnDropdownValueChanged(int index)
    {
        switch (index)
        {
            case 0:
                Application.Quit();
                return;
            case 1:
                manger.GetComponent<Mange_scenes>().Change_Scenes("Main_Scene");
                return;
        }
    }
}
