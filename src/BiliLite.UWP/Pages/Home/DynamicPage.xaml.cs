﻿using BiliLite.Extensions;
using BiliLite.Models.Common;
using BiliLite.Modules;
using BiliLite.Services;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace BiliLite.Pages.Home
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DynamicPage : Page
    {
        DynamicVM dynamicVM;
        public DynamicPage()
        {
            this.InitializeComponent();
            dynamicVM = new DynamicVM();
            dynamicVM.dynamicItemDataTemplateSelector.resource = this.Resources;
            this.DataContext = dynamicVM;
            if (SettingService.GetValue<bool>(SettingConstants.UI.CACHE_HOME, true))
            {
                this.NavigationCacheMode = NavigationCacheMode.Enabled;
            }
            else
            {
                this.NavigationCacheMode = NavigationCacheMode.Disabled;
            }
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == NavigationMode.New && dynamicVM.Items == null)
            {
                await dynamicVM.GetDynamicItems();
            }
        }

        private void RefreshContainer_RefreshRequested(Microsoft.UI.Xaml.Controls.RefreshContainer sender, Microsoft.UI.Xaml.Controls.RefreshRequestedEventArgs args)
        {
            dynamicVM.Refresh();
        }

        private void AdaptiveGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as DynamicItemModel;
            DynamicItemModelOpen(sender, item);
        }

        private void DynamicItemModelOpen(object sender, DynamicItemModel item, bool dontGoTo = false)
        {
            if (item == null) return;
            if (item.desc.type == 8)
            {
                MessageCenter.NavigateToPage(this, new NavigationInfo()
                {
                    icon = Symbol.Play,
                    page = typeof(VideoDetailPage),
                    parameters = item.video.aid,
                    title = item.video.title,
                    dontGoTo = dontGoTo
                });
            }
            else if (item.desc.type == 512)
            {
                MessageCenter.NavigateToPage(this, new NavigationInfo()
                {
                    icon = Symbol.Play,
                    page = typeof(SeasonDetailPage),
                    parameters = item.season.season.season_id,
                    title = item.season.season.title,
                    dontGoTo = dontGoTo
                });
            }
        }

        private void AdaptiveGridView_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (!e.IsMiddleButtonNewTap(sender)) return;
            var element = e.OriginalSource as FrameworkElement;
            var item = element.DataContext as DynamicItemModel;
            DynamicItemModelOpen(sender, item, true);
        }

        private void AddToWatchLater_Click(object sender, RoutedEventArgs e)
        {
            var data = (sender as MenuFlyoutItem).DataContext as DynamicItemModel;
            Modules.User.WatchLaterVM.Instance.AddToWatchlater(data.video.aid);
        }
    }
}
