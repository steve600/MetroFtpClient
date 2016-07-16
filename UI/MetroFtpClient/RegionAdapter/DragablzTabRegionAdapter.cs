using Dragablz;
using MetroFtpClient.Core.Base;
using MetroFtpClient.Model;
using Prism.Regions;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MetroFtpClient.RegionAdapter
{
    public class TabablzControlRegionAdapter : RegionAdapterBase<TabablzControl>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="TabablzControlRegionAdapter"/>.
        /// </summary>
        /// <param name="regionBehaviorFactory">The factory used to create the region behaviors to attach to the created regions.</param>
        public TabablzControlRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
            : base(regionBehaviorFactory)
        {
        }

        protected override void Adapt(IRegion region, TabablzControl regionTarget)
        {
            if (region == null)
                throw new ArgumentNullException(nameof(region));

            if (regionTarget == null)
                throw new ArgumentNullException(nameof(regionTarget));

            region.ActiveViews.CollectionChanged += (s, e) =>
            {
                switch (e.Action)
                {
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                        foreach (var t in e.NewItems)
                        {
                            //TabItem ti = new TabItem();
                            var iv = e.NewItems[0];
                            var vm = (((FrameworkElement)iv)?.DataContext) as ViewModelBase;

                            regionTarget.Items.Insert(regionTarget.Items.Count, new TabContent(vm?.Title, vm?.Icon, e.NewItems[0]));
                            regionTarget.SelectedIndex = regionTarget.Items.Count - 1;
                        }
                        break;

                    case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                        foreach (var t in e.OldItems)
                        {
                            for (var i = 0; i < regionTarget.Items.Count; i++)
                            {
                                var tab = (TabItem)regionTarget.Items[i];
                                if (tab.Content == e.OldItems[0])
                                {
                                    regionTarget.Items.Remove(tab);
                                }
                            }
                            regionTarget.SelectedIndex = regionTarget.Items.Count - 1;
                        }
                        break;
                }
            };
        }

        protected override IRegion CreateRegion()
        {
            return new AllActiveRegion();
        }
    }
}