using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace PVZRHTools
{
    public class SelectedItemsExt : DependencyObject
    {
        public static IList GetSelectedItems(DependencyObject obj) => (IList)obj.GetValue(SelectedItemsProperty);

        public static void OnlistBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
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

        public static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
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

        public static void SetSelectedItems(DependencyObject obj, IList value) => obj.SetValue(SelectedItemsProperty, value);

        // Using a DependencyProperty as the backing store for SelectedItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.RegisterAttached("SelectedItems", typeof(IList), typeof(SelectedItemsExt), new PropertyMetadata(OnSelectedItemsChanged));
    }
}