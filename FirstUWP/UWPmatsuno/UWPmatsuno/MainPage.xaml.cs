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

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace UWPmatsuno
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Button_Calc_Click(object sender, RoutedEventArgs e)
        {
            TextBlock_Message.Text = TextBox_StartingTime.Text + TextBox_LeavingTime.Text;
<<<<<<< HEAD
=======
            //DateTimeに変換できるか確かめる
            DateTime dt, dt2;

            if ( (DateTime.TryParseExact(TextBox_StartingTime.Text, "H:m" , System.Globalization.CultureInfo.InvariantCulture ,
                System.Globalization.DateTimeStyles.None , out dt)) &&
                (DateTime.TryParseExact(TextBox_LeavingTime.Text, "H:m", System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out dt2))   )
            {
                TimeSpan ts;
                //変換出来たら、dtにその値が入る
                if (dt2.CompareTo(dt) > 0)
                {
                    
                    ts = (dt2 - dt);
                }
                else
                {
                    ts = ( dt - dt2);
                    ts = TimeSpan.FromHours(24) - ts;
                }

                TextBlock_Message.Text = ts.ToString();
            }
            else
            {//変換できなかったら
                TextBlock_Message.Text = "適切な時刻を入力してください";
            }
>>>>>>> add timespancalc
        }
    }
}
