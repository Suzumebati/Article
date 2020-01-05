using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MutexTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        ///// <summary>
        ///// ボタンクリックイベント(素でMutexを使う場合)
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    using (var mutex = new Mutex(false,"MutexTest"))
        //    {
        //        if (mutex.WaitOne())
        //        {
        //            // １多重で実行したい処理
        //        }
        //        mutex.ReleaseMutex();
        //    }
        //}


        ///// <summary>
        ///// ボタンクリックイベント(ラッパーを使う場合)
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    using (var mutex = new MutexWrapper(false, "MutexTest"))
        //    {
        //        if (mutex.WaitOne())
        //        {
        //            // １多重で実行したい処理
        //        }
        //    }
        //}

        /// <summary>
        /// ボタンクリックイベント(awaitで待つ場合)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            using (var mutex = new MutexWrapper(false, "MutexTest"))
            {
                if (await mutex.WaitOneAsync())
                {
                    // １多重で実行したい処理
                    MessageBox.Show("Mutex取得");
                    await Task.Delay(20000);
                    MessageBox.Show("Mutex開放");
                }
            }
        }

    }
}
