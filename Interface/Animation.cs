using System.Windows.Controls;

namespace SlideShowPluginInterface
{
    public interface ISlideshowEffect
    {
        string Name { get; }
        void PlaySlideshow(Image imageIn, Image imageOut, double windowWidth, double windowHeight);
        void Play();
        void Pause();
    }
}
