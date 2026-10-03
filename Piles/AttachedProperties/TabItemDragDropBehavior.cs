using Piles.ViewModels;
using System;
using System.Collections;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Piles.AttachedProperties
{
    public static class TabItemDragDropBehavior
    {
        public static readonly DependencyProperty EnableDragDropProperty =
            DependencyProperty.RegisterAttached(
                "EnableDragDrop",
                typeof(bool),
                typeof(TabItemDragDropBehavior),
                new PropertyMetadata(false, OnEnableDragDropChanged));

        public static bool GetEnableDragDrop(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnableDragDropProperty);
        }

        public static void SetEnableDragDrop(DependencyObject obj, bool value)
        {
            obj.SetValue(EnableDragDropProperty, value);
        }

        private static void OnEnableDragDropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TabItem tabItem)
            {
                bool isEnabled = (bool)e.NewValue;

                if (isEnabled)
                {
                    tabItem.PreviewMouseMove += TabItem_PreviewMouseMove;
                    tabItem.Drop += TabItem_Drop;
                }
                else
                {
                    tabItem.PreviewMouseMove -= TabItem_PreviewMouseMove;
                    tabItem.Drop -= TabItem_Drop;
                }
            }
        }

        private static void TabItem_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Source is not TabItem tabItem)
            {
                return;
            }

            if (Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(tabItem, tabItem, DragDropEffects.Move);
            }
        }

        private static void TabItem_Drop(object sender, DragEventArgs e)
        {
            if (FindVisualParent<TabItem>(e.OriginalSource as DependencyObject) is TabItem tabItemTarget &&
                e.Data.GetData(typeof(TabItem)) is TabItem tabItemSource &&
                !tabItemTarget.Equals(tabItemSource) &&
                FindVisualParent<TabControl>(tabItemTarget) is TabControl tabControl &&
                tabControl.DataContext is PileupViewModel pileupViewModel &&
                tabItemSource.DataContext is PileViewModel sourcePileViewModel &&
                tabItemTarget.DataContext is PileViewModel targetPileViewModel)
            {
                int sourceIndex = (pileupViewModel.Piles as IList).IndexOf(sourcePileViewModel);
                int targetIndex = (pileupViewModel.Piles as IList).IndexOf(targetPileViewModel);
                Tuple<int, int> sourceAndTargetIndex = new Tuple<int, int>(sourceIndex, targetIndex);
                pileupViewModel.ReorderPileCommand.Execute(sourceAndTargetIndex);
            }
        }

        private static T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            while (child != null)
            {
                DependencyObject parentObject = VisualTreeHelper.GetParent(child);

                if (parentObject is T parent)
                {
                    return parent;
                }

                child = parentObject;
            }
            
            return null;
        }
    }
}
