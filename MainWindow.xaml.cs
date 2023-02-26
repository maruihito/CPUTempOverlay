using CPUTempBigPicture;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace CPUTempOverlay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.SetupTimer();

        }

        // オーバーレイクラス 
        ActOverlay actOvl;

　      // タイマメソッド
        private void MyTimerMethod(object sender, EventArgs e)
        {
            // 画面情報の更新
            GetCPUStatusToText();
        }

        // タイマのインスタンス
        private DispatcherTimer _timer;

        // タイマイベントの回数カウント
        private uint timerCnt = 0;

        // ポインタ取得クラス
        GetWindowPtr wptr = new GetWindowPtr();

        // タイマを設定する
        private void SetupTimer()
        {
            // タイマのインスタンスを生成
            _timer = new DispatcherTimer(); // 優先度はDispatcherPriority.Background
                                            // インターバルを設定
            _timer.Interval = new TimeSpan(0, 0, 3);
            // タイマメソッドを設定
            _timer.Tick += new EventHandler(MyTimerMethod);
            // タイマを開始
            _timer.Start();

            // 画面が閉じられるときに、タイマを停止
            this.Closing += new CancelEventHandler(StopTimer);
        }

        // タイマを停止
        private void StopTimer(object sender, CancelEventArgs e)
        {
            _timer.Stop();

        }


        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            if( (string)startButton.Content == "START")
            {
                // 対象のプロセスID取得
                IntPtr procid = wptr.GetPtrWindow(wptr.iProcId[ListBox1.SelectedIndex]);
                ProcTextBox.Text = "0x" + procid.ToString("X");

                actOvl = new ActOverlay(Convert.ToInt32(OfsTextBoxX.Text), Convert.ToInt32(OfsTextBoxX.Text));
                GameOverlay.TimerService.EnableHighPrecisionTimers();

                // オーバーレイ開始
                actOvl.iptr = procid;
                object value = Task.Run(actOvl.Run);
                startButton.Content = "STOP";
            }
            else
            {
                // オーバーレイ停止

                Task.Run(actOvl.Dispose);

                Task.WaitAll();
                startButton.Content = "START";
            }

        }

        private void getPtrBtn_Click(object sender, RoutedEventArgs e)
        {
            ListBox1.ItemsSource = wptr.GetListWindows();
        }

        /// 画面情報の更新
        private void GetCPUStatusToText()
        {
            // CPU情報表示クラス
            GetCPUGPUInfo GetCGI = new GetCPUGPUInfo();
            // 表示文字列
            string monitorOutput1 = "";
            string monitorOutput2 = "";
            string monitorOutput3 = "";


            // 情報を取得
            Task t1 = Task.Run(GetCGI.DispCPUGPU);

            // 情報取得が完了するのを待つ
            t1.Wait();

            // CPU、GPUの温度
            monitorOutput1 += "CPU: " + GetCGI.cpuTemp.ToString() + " ﾟC\n";
            monitorOutput1 += "GPU: " + GetCGI.gpuTemp.ToString() + " ﾟC\n";

            // クロック
            monitorOutput2 += "CPU Clock: " + GetCGI.cpuMax.ToString("F1") + " MHz\n";
            monitorOutput2 += "GPU Clock: " + GetCGI.gpuClock.ToString("F1") + " MHz\n";

            // 消費電力
            monitorOutput3 += "CPU Power: " + GetCGI.cpuPow.ToString("F3") + " W\n";
            monitorOutput3 += "GPU Power: " + GetCGI.gpuPow.ToString("F3") + " W\n";

            // 取得した情報を表示
            if(actOvl != null)
            {
                actOvl.CPUInfoText = "\n" + monitorOutput1 + "\n" + monitorOutput2 + monitorOutput3;
            }

            GetCGI.Dispose();
        }
    }
}
