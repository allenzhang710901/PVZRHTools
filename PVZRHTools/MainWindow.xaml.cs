using FastHotKeyForWPF;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;

namespace PVZRHTools
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Title = $"PVZ融合版修改器{ModifierVersion.GameVersion}-{ModifierVersion.Version} b站@Infinite75制作";
            WindowTitle.Content = Title;
            Instance = this;
            ModifierSprite = new ModifierSprite();
            Sprite.Show(ModifierSprite);
            ModifierSprite.Hide();
            if (File.Exists("UserData/ModifierSettings.json"))
            {
                ModifierSaveModel s = JsonSerializer.Deserialize(File.ReadAllText("UserData/ModifierSettings.json"), ModifierSaveModelSGC.Default.ModifierSaveModel);
                DataContext = s.NeedSave ? new ModifierViewModel(s) : new ModifierViewModel(s.Hotkeys);
            }
            else
            {
                DataContext = new ModifierViewModel();
            }
            App.inited = true;
        }

        public void DataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = false;
        }

        public void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
            var scrollViewer = (ScrollViewer)sender;
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
        }

        public void TitleBar_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if ((e.LeftButton is MouseButtonState.Pressed) && (e.RightButton is MouseButtonState.Released) && (e.MiddleButton is MouseButtonState.Released))
            {
                DragMove();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            GlobalHotKey.Destroy();
            ViewModel.Save();
            Application.Current.Shutdown();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            GlobalHotKey.Awake();
            foreach (var hvm in ViewModel.Hotkeys)
            {
                hvm.UpdateHotKey();
            }
        }

        public static MainWindow? Instance { get; set; }
        public ModifierSprite ModifierSprite { get; set; }
        public ModifierViewModel ViewModel => (ModifierViewModel)DataContext;
    }
}