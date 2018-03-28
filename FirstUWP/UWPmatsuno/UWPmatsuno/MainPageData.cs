//自作　ページ遷移などの時にMainPageが保持すべき値(ユーザーが変更した情報)などを格納する構造体



namespace UWPmatsuno
{
    public class MainPageData
    {

        public string TextBox_Shukkin_Text;
        public string TextBox_Taisha_Text;
        public bool ToSw_Kyuukei_IsOn;
        public string TextBlock_Message_Text;
        public string TextBlock_ResultTS_Text;
        public string TextBlock_ResultMoney_Text;
        public bool bIsCalculated;

        public MainPageData()//コンストラクタ
        {
            TextBox_Shukkin_Text = "";
            TextBox_Taisha_Text = "";
            ToSw_Kyuukei_IsOn = true;
            TextBlock_Message_Text = "出勤時間、退社時間を入力して、計算ボタンを押してください";
            TextBlock_ResultTS_Text = "";
            TextBlock_ResultMoney_Text = "";
            bIsCalculated = false;
        }
    }


}//namespace