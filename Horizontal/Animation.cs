using SlideShowPluginInterface;
using System.Windows;
using System;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace HorizontalPlugin
{
    public class HorizontalEffect : ISlideshowEffect
    {
        public string Name => "Horizontal";
        public Storyboard storyboard { get; set; }

        public void Play()
        {
            storyboard.Resume();
        }
        public void Pause()
        {
            storyboard.Pause();
        }

        public void PlaySlideshow(Image imageIn, Image imageOut, double windowWidth, double windowHeight)
        {
            DoubleAnimation animationIn = new DoubleAnimation();
            animationIn.From = windowWidth;
            animationIn.To = 0;
            animationIn.Duration = new Duration(TimeSpan.FromSeconds(4));

            DoubleAnimation animationOut = new DoubleAnimation();
            animationOut.From = 0;
            animationOut.To = -windowWidth;
            animationOut.Duration = new Duration(TimeSpan.FromSeconds(4));

            storyboard = new Storyboard();

            Storyboard.SetTarget(animationIn, imageIn);
            Storyboard.SetTargetProperty(animationIn, new PropertyPath(Canvas.LeftProperty));
            storyboard.Children.Add(animationIn);

            Storyboard.SetTarget(animationOut, imageOut);
            Storyboard.SetTargetProperty(animationOut, new PropertyPath(Canvas.LeftProperty));
            storyboard.Children.Add(animationOut);

            storyboard.Begin();
        }
    }
}
