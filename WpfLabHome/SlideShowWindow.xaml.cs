using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SlideShowPluginInterface;
using SlideShowMain;

namespace SlideShowSecond
{
    /// <summary>
    /// Interaction logic for SlideShowWindow.xaml
    /// </summary>
    public partial class SlideShowWindow : Window
    {
        private System.Windows.Threading.DispatcherTimer timer;
        private bool isPaused = false;
        private ObservableCollection<ImageFileItem> imageFiles;
        private int currentIndex = 0;
        private ISlideshowEffect currentSlideShowEffect;

        public SlideShowWindow(ObservableCollection<ImageFileItem> imageFiles, ISlideshowEffect slideshowEffect)
        {
            InitializeComponent();
            InitializeContextMenu();
            this.imageFiles = imageFiles;
            this.currentSlideShowEffect = slideshowEffect;
            InitializeSlideshow();
        }

        private void InitializeContextMenu()
        {
            ContextMenu contextMenu = new ContextMenu();
            MenuItem playPauseMenuItem = new MenuItem { Header = "Play/Pause Slideshow" };
            playPauseMenuItem.Click += PlayPause_Click;
            MenuItem stopMenuItem = new MenuItem { Header = "Stop Slideshow" };
            stopMenuItem.Click += Stop_Click;
            contextMenu.Items.Add(playPauseMenuItem);
            contextMenu.Items.Add(stopMenuItem);

            ContextMenu = contextMenu;
        }

        private void InitializeSlideshow()
        {
            currentImageControl.Source = imageFiles[0].Thumbnail;
            currentSlideShowEffect.PlaySlideshow(previousImageControl, currentImageControl, Frame.ActualWidth, Frame.ActualHeight);

            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(4);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            currentIndex++;
            if (currentIndex >= imageFiles.Count)
                currentIndex = 0;

            previousImageControl.Source = currentImageControl.Source;
            currentImageControl.Source = imageFiles[currentIndex].Thumbnail;
            currentSlideShowEffect.PlaySlideshow(previousImageControl, currentImageControl, Frame.ActualWidth, Frame.ActualHeight);
        }

        private void PlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (isPaused)
            {
                currentSlideShowEffect.Play();
                timer.Start();
                isPaused = false;
            }
            else
            {
                currentSlideShowEffect.Pause();
                timer.Stop();
                isPaused = true;
            }
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                ContextMenu.IsOpen = true;
                e.Handled = true;
            }
        }
    }
}
