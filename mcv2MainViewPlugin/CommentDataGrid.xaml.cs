using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Windows.Threading;

namespace mcv2.MainViewPlugin
{
    /// <summary>
    /// Interaction logic for CommentDataGrid.xaml
    /// </summary>
    public partial class CommentDataGrid : UserControl
    {
        Dispatcher _dispatcher;
        public CommentDataGrid()
        {
            InitializeComponent();
            _dispatcher = Dispatcher.CurrentDispatcher;
            dataGrid.MouseRightButtonUp += DataGrid_MouseRightButtonUp;
            //_AModeTimer.Interval = 100;
            //_AModeTimer.AutoReset = false;
            //_AModeTimer.Elapsed += async (s, e) =>
            //{
            //    try
            //    {
            //        await _dispatcher.BeginInvoke((Action)(() =>
            //        {
            //            var sc = this.dataGrid.GetScrollViewer();
            //            //TODO:_addingCommentToTop=trueの場合はここがバグになる
            //            sc.ScrollToBottom();
            //            Debug.WriteLine("A Mode ScrollToBottom() fired!");
            //        }), DispatcherPriority.Normal);
            //    }
            //    catch (Exception) { }
            //};
        }



        public bool IsShowUserInfoMenuItem
        {
            get { return (bool)GetValue(IsShowUserInfoMenuItemProperty); }
            set { SetValue(IsShowUserInfoMenuItemProperty, value); }
        }

        public static readonly DependencyProperty IsShowUserInfoMenuItemProperty =
            DependencyProperty.Register("IsShowUserInfoMenuItem", typeof(bool), typeof(CommentDataGrid), new PropertyMetadata(true, OnCurrentReadingChanged));

        private static void OnCurrentReadingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CommentDataGrid dataGrid)
            {
                var menu = dataGrid.dataGrid.Resources["commentContext"] as ContextMenu;
                Debug.Assert(menu != null);
                var userInfoMenuItem = LogicalTreeHelper.FindLogicalNode(menu, "UserInfoMenuItem") as MenuItem;
                userInfoMenuItem!.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        /// <summary>
        /// Visibleがfalseに指定されているコメントも強制的に表示するか
        /// 個別ユーザ情報では全てのコメントを表示したい。その場合にtrueにする。
        /// </summary>
        public bool ForceShowInvisibleComments
        {
            get { return (bool)GetValue(ForceShowInvisibleCommentsProperty); }
            set { SetValue(ForceShowInvisibleCommentsProperty, value); }
        }

        public static readonly DependencyProperty ForceShowInvisibleCommentsProperty =
            DependencyProperty.Register(nameof(ForceShowInvisibleComments), typeof(bool), typeof(CommentDataGrid), new PropertyMetadata(true));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// https://social.msdn.microsoft.com/Forums/vstudio/en-US/63fa1e10-1050-4448-a2bc-62dfe0836f25/selecting-datagrid-row-when-right-mouse-button-is-pressed?forum=wpf
        private void DataGrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            DependencyObject dep = (DependencyObject)e.OriginalSource;

            //depがRunだと、VisualTreeHelper.GetParent()で下記の例外が投げられてしまう。
            //'System.Windows.Documents.Run' is not a Visual or Visual3D' InvalidOperationException
            if (e.OriginalSource is Run run)
            {
                dep = run.Parent;
            }
            while ((dep != null) && !(dep is DataGridCell))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }
            if (dep == null) return;

            if (dep is DataGridCell)
            {
                DataGridCell cell = dep as DataGridCell;
                cell.Focus();

                while ((dep != null) && !(dep is DataGridRow))
                {
                    dep = VisualTreeHelper.GetParent(dep);
                }
                DataGridRow row = dep as DataGridRow;
                dataGrid.SelectedItem = row.DataContext;
            }
        }

        //protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        //{
        //    dataGrid.get
        //    //dataGrid.ItemContainerGenerator.ContainerFromIndex(e.)
        //    base.OnMouseRightButtonUp(e);
        //}
        private bool _addingCommentToTop;
        private bool _bottom = true;
        //private bool neverTouch = true;
        private void DataGridScrollChanged(object sender, RoutedEventArgs e)
        {
            //A(sender, e);
            B(sender, e);
        }
        private void B(object sender, RoutedEventArgs a)
        {
            var e = a as ScrollChangedEventArgs;
            // If the entire contents fit on the screen, ignore this event
            if (e.ExtentHeight < e.ViewportHeight)
                return;

            // If no items are available to display, ignore this event
            //if (this.Items.Count <= 0)
            //    return;

            // If the ExtentHeight and ViewportHeight haven't changed, ignore this event
            if (e.ExtentHeightChange == 0.0 && e.ViewportHeightChange == 0.0)
                return;

            // If we were close to the bottom when a new item appeared,
            // scroll the new item into view.  We pick a threshold of 5
            // items since issues were seen when resizing the window with
            // smaller threshold values.
            var oldExtentHeight = e.ExtentHeight - e.ExtentHeightChange;
            var oldVerticalOffset = e.VerticalOffset - e.VerticalChange;
            var oldViewportHeight = e.ViewportHeight - e.ViewportHeightChange;
            if (oldVerticalOffset + oldViewportHeight + 5 >= oldExtentHeight)
            {
                //this.ScrollIntoView(this.Items[this.Items.Count - 1]);
                //ScrollViewer scrollViewer;
                //if (sender is DataGrid dataGrid)
                //{
                //    scrollViewer = dataGrid.GetScrollViewer();
                //}
                //else if (sender is ScrollViewer sv)
                //{
                //    scrollViewer = sv;
                //}
                //else
                //{
                //    return;
                //}
                if (sender is DataGrid dataGrid)
                {
                    var lastItem = dataGrid.Items[dataGrid.Items.Count - 1];
                    dataGrid.ScrollIntoView(lastItem);
                }
                //scrollViewer.ScrollToBottom();
            }
        }
        private void A(object sender, RoutedEventArgs e)
        {
            //if (_addingCommentToTop)
            //    return;
            if (sender == null)
                return;
            ScrollViewer scrollViewer;
            if (sender is DataGrid dataGrid)
            {
                scrollViewer = dataGrid.GetScrollViewer();
            }
            else if (sender is ScrollViewer)
            {
                scrollViewer = sender as ScrollViewer;
            }
            else
            {
                return;
            }
            var a = e as ScrollChangedEventArgs;


            ////2017/09/11
            ////ExtentHeightは表示されていない部分も含めた全てのコンテントの高さ。
            ////ScrollChangedが呼び出されたのにExtentHeightChangeが0ということはアイテムが追加されていないのにも関わらずスクロールがあった。
            ////それはユーザが手動でスクロールした場合のみ起こること。
            //if (a.ExtentHeightChange == 0)
            //{
            //    //ユーザが手動でスクロールした
            //    _bottom = scrollViewer.IsBottom();
            //    //neverTouch = false;
            //}

            ////2017/09/11全体の高さが表示部に収まる間はスクロールがBottomにあるとみなすと、表示部に収まらなくなった瞬間にもBottomにあると判定されて、最初のスクロールが上手くいくかも。

            ////if (bottom && a.ExtentHeightChange != 0)
            //if (_bottom && Test(a))
            //{
            //    scrollViewer.ScrollToBottom();
            //}
            AutoScrollTest(scrollViewer, a.ExtentHeightChange);
        }
        private bool AutoScroll = true;
        private bool IsAmode = false;
        //System.Timers.Timer _AModeTimer = new System.Timers.Timer();
        private void AutoScrollTest(ScrollViewer sc, double extentHeightChange)
        {
            Debug.WriteLine($"extentHeightChange:{extentHeightChange}");
            // User scroll event : set or unset autoscroll mode
            if (extentHeightChange == 0)
            {   // Content unchanged : user scroll event

                if (sc.VerticalOffset == sc.ScrollableHeight)
                {   // Scroll bar is in bottom
                    // Set autoscroll mode
                    AutoScroll = true;
                    if (IsAmode)
                    {
                        IsAmode = false;
                        //_AModeTimer.Enabled = false;
                    }
                    Debug.WriteLine("Autoscroll=true");
                }
                else
                {   // Scroll bar isn't in bottom
                    // Unset autoscroll mode
                    AutoScroll = false;
                    if (!IsAmode)
                    {
                        //_AModeTimer.Enabled = true;
                        IsAmode = true;
                    }
                    else
                    {
                        //_AModeTimer.Enabled = false;
                        //IsAmode = false;
                    }
                    Debug.WriteLine("Autoscroll=false");
                }
            }

            // Content scroll event : autoscroll eventually
            if (AutoScroll && extentHeightChange != 0)
            {   // Content changed and autoscroll mode set
                // Autoscroll
                sc.ScrollToVerticalOffset(sc.ExtentHeight);
            }
        }
        private bool Test(ScrollChangedEventArgs e)
        {
            return e.ViewportHeightChange > 0 || e.ExtentHeightChange > 0 || e.ViewportHeightChange < 0 || e.ExtentHeightChange < 0;
        }
    }
}
