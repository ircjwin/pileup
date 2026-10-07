using Piles.ViewModels;
using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Piles.AttachedProperties
{
    public static class ListViewItemDragDropBehavior
    {
        public static readonly DependencyProperty EnableDragDropProperty =
            DependencyProperty.RegisterAttached(
                "EnableDragDrop",
                typeof(bool),
                typeof(ListViewItemDragDropBehavior),
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
            if (d is ListViewItem listViewItem)
            {
                bool isEnabled = (bool)e.NewValue;

                if (isEnabled)
                {
                    listViewItem.PreviewMouseMove += ListViewItem_PreviewMouseMove;
                    listViewItem.Drop += ListViewItem_Drop;
                }
                else
                {
                    listViewItem.PreviewMouseMove -= ListViewItem_PreviewMouseMove;
                    listViewItem.Drop -= ListViewItem_Drop;
                }
            }
        }

        private static ListViewItem _draggedItem;
        private static Point _startPosition;

        private static void ListViewItem_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (FindVisualParent<ListViewItem>(e.Source as DependencyObject) is not ListViewItem listViewItem)
            {
                return;
            }

            if (Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed)
            {
                if (_draggedItem != null)
                {
                    Point currentPosition = e.GetPosition(null);
                    Vector dragDistance = currentPosition - _startPosition;

                    if (Math.Abs(dragDistance.X) >= SystemParameters.MinimumHorizontalDragDistance &&
                        Math.Abs(dragDistance.Y) >= SystemParameters.MinimumVerticalDragDistance)
                    {
                        DragDrop.DoDragDrop(listViewItem, listViewItem, DragDropEffects.Move);
                    }
                }
                else
                {
                    _draggedItem = listViewItem;
                    _startPosition = e.GetPosition(null);
                }
            }
            else
            {
                _draggedItem = null;
            }
        }

        private static void ListViewItem_Drop(object sender, DragEventArgs e)
        {
            if (FindVisualParent<ListViewItem>(e.OriginalSource as DependencyObject) is ListViewItem listViewItemTarget &&
                e.Data.GetData(typeof(ListViewItem)) is ListViewItem listViewItemSource &&
                !listViewItemTarget.Equals(listViewItemSource) &&
                FindVisualParent<ListView>(listViewItemTarget) is ListView listView &&
                listView.DataContext is PileViewModel pileViewModel &&
                listViewItemSource.DataContext is RuminationViewModel sourceRuminationViewModel &&
                listViewItemTarget.DataContext is RuminationViewModel targetRuminationViewModel)
            {
                int sourceIndex = (pileViewModel.Ruminations as IList).IndexOf(sourceRuminationViewModel);
                int targetIndex = (pileViewModel.Ruminations as IList).IndexOf(targetRuminationViewModel);
                Tuple<int, int> sourceAndTargetIndex = new Tuple<int, int>(sourceIndex, targetIndex);
                pileViewModel.ReorderRuminationCommand.Execute(sourceAndTargetIndex);
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
