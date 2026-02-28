using Unity.VisualScripting;
using UnityEngine;
using TMPro;
public class Quitgame_Dropdown : MonoBehaviour
{
    //用於QuitGame的下拉式選單
    [SerializeField] GameObject manger;
    public void OnDropdownValueChanged(int index)
    {
        switch (index)
        {
            case 0:
                Application.Quit(); //退出遊戲
                return;
            case 1:
                manger.GetComponent<Mange_scenes>().Change_Scenes("Main_Scene"); //切換至主頁面
                return;
        }
    }
}
