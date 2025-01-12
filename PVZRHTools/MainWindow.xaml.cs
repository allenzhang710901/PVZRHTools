using CommunityToolkit.Mvvm.DependencyInjection;
using FastHotKeyForWPF;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ToolModData;

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
            DataContext = new ModifierViewModel();
            ModifierSprite = new ModifierSprite();
            Sprite.Show(ModifierSprite);
            ModifierSprite.Hide();
        }

        public ModifierSprite ModifierSprite { get; set; }
        public static MainWindow? Instance { get; private set; }

        private void TitleBar_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if ((e.LeftButton is MouseButtonState.Pressed) && (e.RightButton is MouseButtonState.Released) && (e.MiddleButton is MouseButtonState.Released))
            {
                DragMove();
            }
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex re = new("[^0-9]+");
            e.Handled = re.IsMatch(e.Text);
        }

        private void DataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = false;
        }

        public ModifierViewModel ViewModel => (ModifierViewModel)DataContext;

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
            var scrollViewer = (HandyControl.Controls.ScrollViewer)sender;
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
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

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            //File.WriteAllText("./PVZRHTools/ModifierSettings.json", JsonSerializer.Serialize(ViewModel));
            GlobalHotKey.Destroy();
            Application.Current.Shutdown();
        }

        void Handler1(object sender, HotKeyEventArgs e) => CreatePlant.Command.Execute(null);

        void Handler2(object sender, HotKeyEventArgs e) => createZombie.Command.Execute(null);

        void Handler3(object sender, HotKeyEventArgs e) => CreateItem.Command.Execute(null);

        void Handler4(object sender, HotKeyEventArgs e) => CreateUltimateMateorite.Command.Execute(null);

        void Handler5(object sender, HotKeyEventArgs e) => WriteField.Command.Execute(null);

        void Handler6(object sender, HotKeyEventArgs e) => WriteFieldZombies.Command.Execute(null);

        void Handler7(object sender, HotKeyEventArgs e) => ZombieSea.IsChecked = !ZombieSea.IsChecked;

        void Handler8(object sender, HotKeyEventArgs e) => KillAllZombies.Command.Execute(null);

        void Handler9(object sender, HotKeyEventArgs e) => ClearAllPlants.Command.Execute(null);

        void Handler10(object sender, HotKeyEventArgs e) => MindCtrlAll.Command.Execute(null);

        void Handler11(object sender, HotKeyEventArgs e) => NextWave.Command.Execute(null);

        void Handler12(object sender, HotKeyEventArgs e) => ChangeLevelName.Command.Execute(null);

        void Handler13(object sender, HotKeyEventArgs e) => ShowText.Command.Execute(null);

        void Handler14(object sender, HotKeyEventArgs e) => FreePlanting.IsChecked = !FreePlanting.IsChecked;
    }

    //copy from csdn
    public class SelectedItemsExt : DependencyObject
    {
        public static IList GetSelectedItems(DependencyObject obj)
        {
            return (IList)obj.GetValue(SelectedItemsProperty);
        }

        public static void SetSelectedItems(DependencyObject obj, IList value)
        {
            obj.SetValue(SelectedItemsProperty, value);
        }

        // Using a DependencyProperty as the backing store for SelectedItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.RegisterAttached("SelectedItems", typeof(IList), typeof(SelectedItemsExt), new PropertyMetadata(OnSelectedItemsChanged));

        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listBox = d as ListBox;
            if ((listBox != null) && (listBox.SelectionMode == SelectionMode.Multiple))
            {
                if (e.OldValue != null)
                {
                    listBox.SelectionChanged -= OnlistBoxSelectionChanged;
                }
                IList collection = (e.NewValue as IList)!;
                listBox.SelectedItems.Clear();
                if (collection != null)
                {
                    foreach (object item in collection)
                    {
                        listBox.SelectedItems.Add(item);
                    }
                    listBox.OnApplyTemplate();
                    listBox.SelectionChanged += OnlistBoxSelectionChanged;
                }
            }
        }

        private static void OnlistBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IList dataSource = GetSelectedItems((sender as DependencyObject)!);
            foreach (var item in e.AddedItems)
            {
                dataSource.Add(item);
            }
            foreach (var item in e.RemovedItems)
            {
                dataSource.Remove(item);
            }
            SetSelectedItems((sender as DependencyObject)!, dataSource);
        }
    }
}