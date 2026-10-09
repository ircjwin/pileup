using Piles.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Piles.AttachedProperties
{
    public static class ListViewRummagePickBehavior
    {
        public static readonly DependencyProperty RummagePickProperty =
            DependencyProperty.RegisterAttached(
                "RummagePick",
                typeof(int),
                typeof(ListViewRummagePickBehavior),
                new PropertyMetadata(-1, OnRummagePickChanged));

        public static bool GetRummagePick(DependencyObject obj)
        {
            return (bool)obj.GetValue(RummagePickProperty);
        }

        public static void SetRummagePick(DependencyObject obj, bool value)
        {
            obj.SetValue(RummagePickProperty, value);
        }

        private static void OnRummagePickChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListView listView)
            {
                var rummagePick = (int)e.NewValue;

                if (rummagePick >= 0 &&
                    rummagePick < listView.Items.Count)
                {
                    listView.ScrollIntoView(listView.Items[rummagePick]);
                }
            }
        }
    }
}
