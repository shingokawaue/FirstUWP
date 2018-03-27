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


using UWPmatsuno.ToLibrary; //自作クラス

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace UWPmatsuno
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        bool bIsCalculated = false;

        public MainPage()
        {
            this.InitializeComponent();
        }

        /// ////////////////////////////////////////////////イベントハンドラ//////////////////////////////////////////////////////////////////


        private void Button_Calc_Click(object sender, RoutedEventArgs e)
        {
            CalcDailyWage();
        }

        private void ToSw_Kyuukei_Toggled(object sender, RoutedEventArgs e)
        {
            if (bIsCalculated == true) CalcDailyWage();
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void CalcDailyWage()//日給計算
        {
            TextBlock_Message.Text = TextBox_Shukkin.Text + TextBox_Taisha.Text;

            //DateTimeに変換できるか確かめる
            DateTime dt ,dt2;
            if ( !(
                DateTime.TryParseExact(TextBox_Shukkin.Text, "H:m", System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out dt)
                ) 
                &&
                !(
                DateTime.TryParseExact(TextBox_Shukkin.Text, "HHmm", System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out dt)
                ))
            {
                EnterCorrectTime(); return;//変換できなかったら
            }


            if ( !(
                DateTime.TryParseExact(TextBox_Taisha.Text, "H:m", System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out dt2)
                ) 
                &&
                 !(
                DateTime.TryParseExact(TextBox_Taisha.Text, "HHmm", System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out dt2)
                )
                )
            {
                EnterCorrectTime(); return;//変換できなかったら
            }


            //変換出来たら、dt,dt2に値が入っている

            TimeSpan ts;
                
                if (dt2.CompareTo(dt) > 0)
                {

                    ts = (dt2 - dt);
                }
                else
                {
                    ts = (dt - dt2);//日またぎ
                    ts = TimeSpan.FromHours(24) - ts;
                }

                TextBlock_ResultTS.Text = "時間 : " + ts.Hours.ToString().PadLeft(2, '0') + ":" + ts.Minutes.ToString().PadLeft(2, '0');
                TextBlock_Message.Text = "出勤、退社時間";
                //給料の計算

                int kyuukei = 0;
                if (ToSw_Kyuukei.IsOn)//休憩時間を引くトグルスイッチがオンなら
                {
                    kyuukei = CalcWorkingTime.SubBreakTime(ref ts);
                    TextBlock_ResultTS.Text = TextBlock_ResultTS.Text + @"
休憩時間:" + kyuukei + "分";
                }

                TextBlock_ResultMoney.Text = " 給料:\\" +
                    String.Format("{0:#,0}", (ts.Hours * 1000 + ts.Minutes * 1000 / 60)) +
                    ".-";

                bIsCalculated = true;
            


        }



        private void EnterCorrectTime()
        {
            TextBlock_Message.Text = "適切な時刻を入力してください";
            TextBlock_ResultMoney.Text = "";
            TextBlock_ResultTS.Text = "";
        }








    }
}
