using SlideShowPluginInterface;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace VerticalPlugin
{
    public class VerticalEffect : ISlideshowEffect
    {
        public string Name => "Vertical";
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
            animationIn.From = windowHeight;
            animationIn.To = 0;
            animationIn.Duration = new Duration(TimeSpan.FromSeconds(4));

            DoubleAnimation animationOut = new DoubleAnimation();
            animationOut.From = 0;
            animationOut.To = -windowHeight;
            animationOut.Duration = new Duration(TimeSpan.FromSeconds(4));

            storyboard = new Storyboard();

            Storyboard.SetTarget(animationIn, imageIn);
            Storyboard.SetTargetProperty(animationIn, new PropertyPath(Canvas.TopProperty));
            storyboard.Children.Add(animationIn);

            Storyboard.SetTarget(animationOut, imageOut);
            Storyboard.SetTargetProperty(animationOut, new PropertyPath(Canvas.TopProperty));
            storyboard.Children.Add(animationOut);

            storyboard.Begin();
        }
    }
}
