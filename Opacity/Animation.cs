using SlideShowPluginInterface;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace OpacityTransitionPlugin
{
    public class OpacityEffect : ISlideshowEffect
    {
        public string Name => "Opacity";
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
            DoubleAnimation animationOut = new DoubleAnimation();
            animationOut.From = 1.0;
            animationOut.To = 0.0;
            animationOut.Duration = new Duration(TimeSpan.FromSeconds(4));

            DoubleAnimation animationIn = new DoubleAnimation();
            animationIn.From = 0.0;
            animationIn.To = 1.0;
            animationIn.Duration = new Duration(TimeSpan.FromSeconds(4));

            Storyboard.SetTarget(animationOut, imageIn);
            Storyboard.SetTargetProperty(animationOut, new PropertyPath("Opacity"));

            Storyboard.SetTarget(animationIn, imageOut);
            Storyboard.SetTargetProperty(animationIn, new PropertyPath("Opacity"));

            storyboard = new Storyboard();
            storyboard.Children.Add(animationOut);
            storyboard.Children.Add(animationIn);
            storyboard.Begin();
        }
    }
}
