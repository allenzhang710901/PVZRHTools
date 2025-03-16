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
            if (File.Exists((App.IsBepInEx ? "BepInEx/config" : "UserData") + "/ModifierSettings.json"))
            {
                try
                {
                    ModifierSaveModel s = JsonSerializer.Deserialize(File.ReadAllText((App.IsBepInEx ? "BepInEx/config" : "UserData") + "/ModifierSettings.json"), ModifierSaveModelSGC.Default.ModifierSaveModel);
                    DataContext = s.NeedSave ? new ModifierViewModel(s) : new ModifierViewModel(s.Hotkeys);
                }
                catch
                {
                    File.Delete((App.IsBepInEx ? "BepInEx/config" : "UserData") + "/ModifierSettings.json");
                    DataContext = new ModifierViewModel();
                }
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

            ViewModel.Save();
            GlobalHotKey.Destroy();
            Application.Current.Shutdown();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            GlobalHotKey.Awake();
            foreach (var hvm in from hvm in ViewModel.Hotkeys where hvm.CurrentKeyB != Key.None select hvm)
            {
                hvm.UpdateHotKey();
            }
        }

        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (App.inited)
            {
                Application.Current.Resources.MergedDictionaries.RemoveAt(2);
                Application.Current.Resources.MergedDictionaries.Add(!(sender is ComboBox box && box.Text == "English") ? LangEN_US : LangZH_CN);
                OnApplyTemplate();
            }
        }

        public static MainWindow? Instance { get; set; }
        public static ResourceDictionary LangEN_US => new() { Source = new("/Lang.en-us.xaml", UriKind.Relative) };
        public static ResourceDictionary LangZH_CN => new() { Source = new("/Lang.zh-cn.xaml", UriKind.Relative) };
        public ModifierSprite ModifierSprite { get; set; }
        public ModifierViewModel ViewModel => (ModifierViewModel)DataContext;

    }
}