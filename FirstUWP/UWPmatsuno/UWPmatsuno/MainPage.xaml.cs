using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Windows.Storage;
using UWPmatsuno.ToLibrary; //自作クラス
using Windows.UI.Popups;
// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace UWPmatsuno
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {

        int jikyuu;
        bool bIsCalculated = false;
        ApplicationDataContainer container = ApplicationData.Current.LocalSettings;


        public MainPage()
        {
            this.InitializeComponent();

            //Suspendingイベントにイベントハンドラを結びつける
            App.Current.Suspending += this.OnSuspending;


            if (container.Values.ContainsKey("Jikyuu"))
            {
                jikyuu = (int)container.Values["Jikyuu"];
            }
            else
            {
                jikyuu = 1000;
                container.Values["Jikyuu"] = 1000;
            }
            txtblk_Jikyuu.Text = "時給 : " + jikyuu.ToString();
            if (container.Values.ContainsKey("ToSw_Kyuukei.IsOn"))
            {
                ToSw_Kyuukei.IsOn = (container.Values["ToSw_Kyuukei.IsOn"].ToString() == "True");
            }
        }


        //-------------------------------イベントハンドラ↓------------------------------------------


        private void Button_Calc_Click(object sender, RoutedEventArgs e)
        {
            CalcDailyWage();
        }

        private void ToSw_Kyuukei_Toggled(object sender, RoutedEventArgs e)
        {
            if (bIsCalculated == true) CalcDailyWage();
        }

        private void btnSettei_Click(object sender, RoutedEventArgs e)
        {

            SaveSettings();
            this.Frame.Navigate(typeof(SettingPage));
        }

        // ページ遷移直後にOnNavigatedToイベントハンドラーが呼び出される
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // このようにe.Parameterで前のページから渡された値を取得できます。
            // 値はキャストして取り出します。
            string param = e.Parameter as string;
            if (param == "SettingPage")
            {
                jikyuu = (int)container.Values["Jikyuu"];
                LoadSettings();
            }
            base.OnNavigatedTo(e);


        }


        //4. Suspendingイベントのイベントハンドラ
        private void OnSuspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            SaveSettings();
        }









        //----------------------------------------------------------------------------------------------------
        //                                  その他のメソッド↓
        //------------------------------------------------------------------------------------------
        private void CalcDailyWage()//日給計算
        {
            TextBlock_Message.Text = TextBox_Shukkin.Text + TextBox_Taisha.Text;

            //DateTimeに変換できるか確かめる
            DateTime dt, dt2;

            //出勤時間に入力された値がDate型に変換できるか
            if (!(DateTime.TryParseExact(TextBox_Shukkin.Text, "H:m", System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out dt)) &&
                !(DateTime.TryParseExact(TextBox_Shukkin.Text, "HHmm", System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out dt)))
            {
                EnterCorrectTime(); return;//変換できなかったらリターン
            }

            //退社時間に入力された値がDate型に変換できるか
            if (!(DateTime.TryParseExact(TextBox_Taisha.Text, "H:m", System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out dt2)) &&
                 !(DateTime.TryParseExact(TextBox_Taisha.Text, "HHmm", System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out dt2)))
            {
                EnterCorrectTime(); return;//変換できなかったらリターン
            }

            //変換出来たら、dt,dt2に値が入っているので給料計算
            TimeSpan tsTotal;


            if (dt2.CompareTo(dt) > 0)
            {//日をまたがない
                tsTotal = (dt2 - dt);
            }
            else
            {//日またぎ
                tsTotal = (dt - dt2);
                tsTotal = TimeSpan.FromHours(24) - tsTotal;
            }

            TextBlock_ResultTS.Text = "出勤から退社:" 
                + tsTotal.Hours + "時間" + tsTotal.Minutes + "分";

            TextBlock_Message.Text = "日給はこの通りです";
            //給料の計算


            TimeSpan tsNight = CalcWorkingTime.NightWork(dt, dt2);//深夜手当がつく労働時間
            TimeSpan tsNormal = tsTotal - tsNight;//昼間(深夜でない)の労働時間


            int kyuukei = 0;
            if (ToSw_Kyuukei.IsOn)//休憩時間を引くトグルスイッチがオンなら
            {
                kyuukei = CalcWorkingTime.SubBreakTime(ref tsNormal , ref tsNight);//tsNormal,tsNightから休憩時間が引かれる
                TextBlock_ResultTS.Text = TextBlock_ResultTS.Text + @"
休憩時間:" + kyuukei + "分";//さし引いた休憩時間を表示
            }

            tsTotal = tsNormal + tsNight;
            TextBlock_ResultTS.Text = TextBlock_ResultTS.Text + @"
労働時間:" + tsTotal.Hours + "時間" + tsTotal.Minutes + "分";//労働時間表示

            if (tsNight.TotalMinutes > 0)
            {
                TextBlock_ResultTS.Text = TextBlock_ResultTS.Text + @"
うち深夜労働:" + tsNight.Hours + "時間" + tsNight.Minutes + "分";
            }

            TextBlock_ResultMoney.Text = " 給料:\\" +
                    String.Format("{0:#,0}", (
                    (tsNormal.Hours * jikyuu + tsNormal.Minutes * jikyuu / 60) +
                    (tsNight.Hours * jikyuu * 1.25 + tsNight.Minutes * jikyuu / 60 * 1.25)
                    )
                    ) + ".-";

            TextBlock_ResultDetail.Text = tsNormal.Hours + "時間" + tsNormal.Minutes + "分" + " * " + jikyuu;
            if (tsNight.TotalMinutes > 0)
            {
                TextBlock_ResultDetail.Text += "　　" + tsNight.Hours + "時間" + tsNight.Minutes + "分" + " * " + jikyuu * 1.25;
            }


            bIsCalculated = true;



        }





        private void EnterCorrectTime()
        {
            TextBlock_Message.Text = "適切な時刻を入力してください";
            TextBlock_ResultMoney.Text = "";
            TextBlock_ResultTS.Text = "";
        }

        private void SaveSettings()
        {
            container.Values["TextBox_Shukkin.Text"] = TextBox_Shukkin.Text;
            container.Values["TextBox_Taisha.Text"] = TextBox_Taisha.Text;
            container.Values["ToSw_Kyuukei.IsOn"] = ToSw_Kyuukei.IsOn.ToString();
            container.Values["TextBlock_Message.Text"] = TextBlock_Message.Text;
            container.Values["TextBlock_ResultTS.Text"] = TextBlock_ResultTS.Text;
            container.Values["TextBlock_ResultMoney.Text"] = TextBlock_ResultMoney.Text;
            container.Values["bIsCalculated"] = bIsCalculated.ToString();
        }

        private void LoadSettings()
        {
            TextBox_Shukkin.Text = container.Values["TextBox_Shukkin.Text"].ToString();
            TextBox_Taisha.Text = container.Values["TextBox_Taisha.Text"].ToString();
            ToSw_Kyuukei.IsOn = (container.Values["ToSw_Kyuukei.IsOn"].ToString() == "True");
            TextBlock_Message.Text = container.Values["TextBlock_Message.Text"].ToString();
            TextBlock_ResultTS.Text = container.Values["TextBlock_ResultTS.Text"].ToString();
            TextBlock_ResultMoney.Text = container.Values["TextBlock_ResultMoney.Text"].ToString();
            bIsCalculated = (container.Values["bIsCalculated"].ToString() == "True");
        }
    }
}
