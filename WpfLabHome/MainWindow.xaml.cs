using SlideShowPluginInterface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using SlideShowSecond;
using System.Windows.Input;

namespace SlideShowMain
{

    public partial class MainWindow : System.Windows.Window
    {
        public static RoutedCommand OpenFolderDialogCommand = new RoutedCommand();
        public static RoutedCommand AboutCommand = new RoutedCommand();

        public ObservableCollection<DirectoryItem> Drives { get; set; }
        public ObservableCollection<ImageFileItem> ImageFiles { get; set; }
        public string SelectedFolderPath { get; set; }
        private List<ISlideshowEffect> slideshowEffects = new List<ISlideshowEffect>();
        private ISlideshowEffect selectedEffect;

        public List<string> SlideshowEffectNames
        {
            get { return slideshowEffects.Select(effect => effect.Name).ToList(); }
        }

        public MainWindow()
        {
            InitializeComponent();

            Drives = new ObservableCollection<DirectoryItem>();
            ImageFiles = new ObservableCollection<ImageFileItem>();

            foreach (var drivePath in Environment.GetLogicalDrives())
            {
                var driveItem = new DirectoryItem(drivePath);
                LoadImmediateSubdirectories(driveItem);
                Drives.Add(driveItem);
            }

            LoadSlideshowEffects();

            DataContext = this;
            EffectsComboBox.ItemsSource = slideshowEffects;
            SlideShowMenuItem.ItemsSource = slideshowEffects;
        }

        private void EffectMenuItemClick(object sender, RoutedEventArgs e)
        {
            MenuItem clickedItem = sender as MenuItem;
            selectedEffect = clickedItem.DataContext as ISlideshowEffect;

            if (selectedEffect != null && ImageFiles.Count > 0)
            {
                SlideShowWindow slideshowWindow = new SlideShowWindow(ImageFiles, selectedEffect);
                slideshowWindow.ShowDialog();
            }
        }

        private void ExitMenuItemClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as TreeViewItem;
            var directoryItem = item?.DataContext as DirectoryItem;

            if (directoryItem != null && !directoryItem.IsLoaded)
            {
                LoadImmediateSubdirectories(directoryItem);
                directoryItem.IsLoaded = true;
                item.DataContext = directoryItem;
            }
        }

        private void LoadImmediateSubdirectories(DirectoryItem parentItem)
        {
            try
            {
                DirectoryInfo parentDirectory = new DirectoryInfo(parentItem.Path);

                foreach (var subDir in parentDirectory.GetDirectories())
                {
                    if (!DirectoryExists(parentItem.Directories, subDir.FullName))
                    {
                        var subDirectoryItem = new DirectoryItem(subDir.FullName);
                        parentItem.Directories.Add(subDirectoryItem);
                        LoadOneLevelSubdirectories(subDirectoryItem);
                    }
                }

                foreach (var subDirectoryItem in parentItem.Directories)
                {
                    LoadOneLevelSubdirectories(subDirectoryItem);
                }
            }
            catch
            {
                return;
            }
        }

        private void LoadOneLevelSubdirectories(DirectoryItem parentItem)
        {
            try
            {
                DirectoryInfo parentDirectory = new DirectoryInfo(parentItem.Path);

                foreach (var subDir in parentDirectory.GetDirectories())
                {
                    if (!DirectoryExists(parentItem.Directories, subDir.FullName))
                    {
                        var subDirectoryItem = new DirectoryItem(subDir.FullName);
                        parentItem.Directories.Add(subDirectoryItem);
                    }
                }
            }
            catch
            {
                return;
            }
        }

        private bool DirectoryExists(ObservableCollection<DirectoryItem> directories, string path)
        {
            return directories.Any(directory => directory.Path == path);
        }

        private void OpenFolderMenuItemClick(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.ShowNewFolderButton = true;

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    ImageFiles.Clear();
                    LoadImageFiles(dialog.SelectedPath);
                    SelectedFolderPath = dialog.SelectedPath;
                }
            }
        }

        private void LoadImageFiles(string folderPath)
        {
            var supportedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };

            try
            {
                foreach (var file in Directory.GetFiles(folderPath))
                {
                    var extension = System.IO.Path.GetExtension(file).ToLower();
                    var fileName = System.IO.Path.GetFileName(file);

                    if (supportedExtensions.Contains(extension))
                    {
                        var imageFileItem = new ImageFileItem
                        {
                            FileName = fileName,
                            Thumbnail = new BitmapImage(new Uri(file))
                        };

                        ImageFiles.Add(imageFileItem);
                    }
                }

                if (ImageFiles.Count > 0)
                {
                    SlideShowExpander.IsExpanded = true;
                    StartSlideShowButtonUnderComboBox.IsEnabled = true;
                    EffectsComboBox.IsEnabled = true;
                    SlideShowMenuItem.IsEnabled = true;
                }
            }
            catch
            {
                return;
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var selectedFile = e.AddedItems[0] as ImageFileItem;

                if (selectedFile != null)
                {
                    var filePath = Path.Combine(SelectedFolderPath, selectedFile.FileName);
                    var fileInfo = new FileInfo(filePath);
                    var image = System.Drawing.Image.FromFile(fileInfo.FullName);
                    UpdateFileInfoTextBlock(selectedFile.FileName, image.Height, image.Width, fileInfo.Length);

                    FileInfoTextBlock.TextAlignment = TextAlignment.Left;
                    FileInfoTextBlock.Visibility = Visibility.Visible;
                    FileInfoExpander.IsExpanded = true;
                }
            }
            else
            {
                FileInfoTextBlock.Text = "No file selected!";
                FileInfoExpander.IsExpanded = false;
                FileInfoTextBlock.Visibility = Visibility.Collapsed;
            }
        }

        private void UpdateFileInfoTextBlock(string name, int height, int width, long length)
        {
            FileInfoTextBlock.Inlines.Clear();
            FileInfoTextBlock.Inlines.Add(new Run("File name:  ") { FontWeight = FontWeights.Bold });
            FileInfoTextBlock.Inlines.Add(name);
            FileInfoTextBlock.Inlines.Add(new LineBreak());
            FileInfoTextBlock.Inlines.Add(new Run("Height:\t     ") { FontWeight = FontWeights.Bold });
            FileInfoTextBlock.Inlines.Add(height + " px");
            FileInfoTextBlock.Inlines.Add(new LineBreak());
            FileInfoTextBlock.Inlines.Add(new Run("Width:\t     ") { FontWeight = FontWeights.Bold });
            FileInfoTextBlock.Inlines.Add(width + " px");
            FileInfoTextBlock.Inlines.Add(new LineBreak());
            FileInfoTextBlock.Inlines.Add(new Run("Size:\t     ") { FontWeight = FontWeights.Bold });
            FileInfoTextBlock.Inlines.Add((length / 1024.0).ToString("F2") + " KB");
        }

        private void AboutMenuItemClick(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("This is a simple image slide show application.", "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DirectoryTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var selectedDirectoryItem = DirectoryTreeView.SelectedItem as DirectoryItem;

            if (selectedDirectoryItem != null)
            {
                ImageFiles.Clear();
                SelectedFolderPath = selectedDirectoryItem.Path;
                LoadImageFiles(SelectedFolderPath);
                if (ImageFiles.Count == 0)
                {
                    SlideShowMenuItem.IsEnabled = false;
                    EffectsComboBox.IsEnabled = false;
                    StartSlideShowButtonUnderComboBox.IsEnabled = false;
                    SlideShowExpander.IsExpanded = false;
                }
            }
        }

        private void StartSlideShowButtonUnderComboBox_Click(object sender, RoutedEventArgs e)
        {
            if (ImageFiles.Count > 0 && selectedEffect != null)
            {
                SlideShowWindow slideshowWindow = new SlideShowWindow(ImageFiles, selectedEffect);
                slideshowWindow.ShowDialog();
            }
        }


        private void LoadSlideshowEffects()
        {
            string pluginsDirectory = Environment.CurrentDirectory;
            slideshowEffects = new List<ISlideshowEffect>();

            try
            {
                foreach (string file in Directory.GetFiles(pluginsDirectory, "*.dll"))
                {
                    Assembly assembly = Assembly.LoadFrom(file);

                    Type[] types = assembly.GetExportedTypes();
                    foreach (Type type in types)
                    {
                        if (typeof(ISlideshowEffect).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                        {
                            ISlideshowEffect effect = (ISlideshowEffect)Activator.CreateInstance(type);
                            slideshowEffects.Add(effect);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }


        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedEffect = EffectsComboBox.SelectedItem as ISlideshowEffect;

            if (selectedEffect != null)
            {
                this.selectedEffect = selectedEffect;
            }
        }
    }

    public class DirectoryItem
    {
        public string Path { get; }
        public string Name { get; }
        public ObservableCollection<DirectoryItem> Directories { get; set; }
        public bool IsLoaded { get; set; }

        public DirectoryItem(string path)
        {
            Path = path;
            if (IsDriveRoot(path))
            {
                Name = System.IO.Path.GetPathRoot(path);
            }
            else
            {
                Name = System.IO.Path.GetFileName(path);
            }
            Directories = new ObservableCollection<DirectoryItem>();
            IsLoaded = false;
        }

        private bool IsDriveRoot(string path)
        {
            return path == System.IO.Path.GetPathRoot(path);
        }
    }

    public class ImageFileItem
    {
        public string FileName { get; set; }
        public BitmapImage Thumbnail { get; set; }
    }
}
