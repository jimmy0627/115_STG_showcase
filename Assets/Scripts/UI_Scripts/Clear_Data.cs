using UnityEngine;

public class Clear_Data : MonoBehaviour
{
    //清除存檔的腳本
    public void clear_data()
    {
        Read_Write_data tool=new Read_Write_data();
        tool.delete_json();
    }
}
