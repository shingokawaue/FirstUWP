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
// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace UWPmatsuno
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class SettingPage : Page
    {

ApplicationDataContainer container = ApplicationData.Current.LocalSettings;

        public SettingPage()
        {
            this.InitializeComponent();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {

            if(Txtbx_Jikyuu.Text != container.Values["Jikyuu"].ToString())
            {
                int i;
                if(int.TryParse(Txtbx_Jikyuu.Text , out i))
                {
                    container.Values["Jikyuu"] = i;
                }

            }

            this.Frame.Navigate(typeof(MainPage), "SettingPage");

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        // ページ遷移直後にOnNavigatedToイベントハンドラーが呼び出される
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
                Txtbx_Jikyuu.Text = container.Values["Jikyuu"].ToString();
        
            base.OnNavigatedTo(e);

        }







    }
}
