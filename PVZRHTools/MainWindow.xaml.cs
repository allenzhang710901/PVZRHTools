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
                ModifierSaveModel s = JsonSerializer.Deserialize<ModifierSaveModel>(File.ReadAllText("UserData/ModifierSettings.json"));
                DataContext = s.NeedSave ? new ModifierViewModel(s) : new ModifierViewModel();
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
            GlobalHotKey.Add(ModelKeys.SHIFT | ModelKeys.ALT, NormalKeys.A, Handler1);
            GlobalHotKey.Add(ModelKeys.SHIFT | ModelKeys.ALT, NormalKeys.S, Handler2);
            GlobalHotKey.Add(ModelKeys.SHIFT | ModelKeys.ALT, NormalKeys.D, Handler3);
            GlobalHotKey.Add(ModelKeys.SHIFT | ModelKeys.ALT, NormalKeys.H, Handler4);
            GlobalHotKey.Add(ModelKeys.ALT, NormalKeys.J, Handler5);
            GlobalHotKey.Add(ModelKeys.ALT, NormalKeys.K, Handler6);
            GlobalHotKey.Add(ModelKeys.ALT, NormalKeys.L, Handler7);
            GlobalHotKey.Add(ModelKeys.ALT, NormalKeys.F1, Handler8);
            GlobalHotKey.Add(ModelKeys.ALT, NormalKeys.F2, Handler9);
            GlobalHotKey.Add(ModelKeys.ALT, NormalKeys.F3, Handler10);
            GlobalHotKey.Add(ModelKeys.ALT, NormalKeys.F6, Handler11);
            GlobalHotKey.Add(ModelKeys.ALT, NormalKeys.N, Handler12);
            GlobalHotKey.Add(ModelKeys.ALT, NormalKeys.M, Handler13);
            GlobalHotKey.Add(ModelKeys.ALT, NormalKeys.F9, Handler14);
        }

        void Handler1(object sender, HotKeyEventArgs e) => CreatePlant.Command.Execute(null);

        void Handler10(object sender, HotKeyEventArgs e) => MindCtrlAll.Command.Execute(null);

        void Handler11(object sender, HotKeyEventArgs e) => NextWave.Command.Execute(null);

        void Handler12(object sender, HotKeyEventArgs e) => ChangeLevelName.Command.Execute(null);

        void Handler13(object sender, HotKeyEventArgs e) => ShowText.Command.Execute(null);

        void Handler14(object sender, HotKeyEventArgs e) => FreePlanting.IsChecked = !FreePlanting.IsChecked;

        void Handler2(object sender, HotKeyEventArgs e) => createZombie.Command.Execute(null);

        void Handler3(object sender, HotKeyEventArgs e) => CreateItem.Command.Execute(null);

        void Handler4(object sender, HotKeyEventArgs e) => CreateUltimateMateorite.Command.Execute(null);

        void Handler5(object sender, HotKeyEventArgs e) => WriteField.Command.Execute(null);

        void Handler6(object sender, HotKeyEventArgs e) => WriteFieldZombies.Command.Execute(null);

        void Handler7(object sender, HotKeyEventArgs e) => ZombieSeaEnabled.IsChecked = !ZombieSeaEnabled.IsChecked;

        void Handler8(object sender, HotKeyEventArgs e) => KillAllZombies.Command.Execute(null);

        void Handler9(object sender, HotKeyEventArgs e) => ClearAllPlants.Command.Execute(null);

        public static MainWindow? Instance { get; set; }
        public ModifierSprite ModifierSprite { get; set; }
        public ModifierViewModel ViewModel => (ModifierViewModel)DataContext;
    }
}