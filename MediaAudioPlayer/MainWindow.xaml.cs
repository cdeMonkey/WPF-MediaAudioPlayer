using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
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
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Microsoft.Win32;

namespace MediaAudioPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       
        private bool mediaPlayerIsPlaying = false;
        private bool userIsDraggingSlider = false;
        private bool fullscreen = true;
        private DispatcherTimer DoubleClickTimer = new DispatcherTimer();
        private TimeSpan TotalTime;

        public MainWindow()
        {
            InitializeComponent();
          

         

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
            DoubleClickTimer.Interval = TimeSpan.FromMilliseconds(GetDoubleClickTime());
            DoubleClickTimer.Tick += (s, e) => DoubleClickTimer.Stop();

          
        }

    

        void timer_Tick(object sender, EventArgs e)
         {
            if ((MediaPlayer.Source != null) && (MediaPlayer.NaturalDuration.HasTimeSpan) && (!userIsDraggingSlider))
            {
                sliProgress.Minimum = 0;
                sliProgress.Maximum = MediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                sliProgress.Value = MediaPlayer.Position.TotalSeconds;
            }
            
        }

        

        private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Media files (*.mp3;*.mpg;*.mpeg;*.mp4)|*.mp3;*.mpg;*.mpeg;*.mp4|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
                MediaPlayer.Source = new Uri(openFileDialog.FileName);
        }

        private void Play_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (MediaPlayer != null) && (MediaPlayer.Source != null);
        }

        private void Play_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MediaPlayer.Play();
            mediaPlayerIsPlaying = true;
        }

        private void FastFowrward_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            
          
        }

        private void Pause_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = mediaPlayerIsPlaying;
        }

        private void Pause_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MediaPlayer.Pause();
        }

        private void Stop_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = mediaPlayerIsPlaying;
        }

        private void Stop_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MediaPlayer.Stop();
            mediaPlayerIsPlaying = false;
        }

        private void sliProgress_DragStarted(object sender, DragStartedEventArgs e)
        {
            userIsDraggingSlider = true;
            MediaPlayer.Position = TimeSpan.FromSeconds(sliProgress.Value);
        }

        private void sliProgress_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            userIsDraggingSlider = false;
            MediaPlayer.Position = TimeSpan.FromSeconds(sliProgress.Value);
        }

        private void sliProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MediaPlayer.Position = TimeSpan.FromSeconds(sliProgress.Value);
            lblProgressStatus.Text = TimeSpan.FromSeconds(sliProgress.Value).ToString(@"hh\:mm\:ss");
        }

        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            MediaPlayer.Volume += (e.Delta > 0) ? 0.1 : -0.1;
        }

        private void MediaPlayer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!DoubleClickTimer.IsEnabled)
            {
                DoubleClickTimer.Start();
            }
            else
            {
                if (!fullscreen)
                {
                    this.WindowStyle = WindowStyle.None;
                    this.WindowState = WindowState.Maximized;
                }
                else
                {
                    this.WindowStyle = WindowStyle.SingleBorderWindow;
                    this.WindowState = WindowState.Normal;
                }

                fullscreen = !fullscreen;
            }
        }
      

        [DllImport("user32.dll")]
        private static extern uint GetDoubleClickTime();

       

        private void timeSlider_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sliProgress.Maximum > 0)
            {
                MediaPlayer.Position = TimeSpan.FromSeconds(sliProgress.Value * TotalTime.TotalSeconds);
            }
        }

    }
}
