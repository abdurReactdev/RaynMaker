﻿using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Regions;
using RaynMaker.Analyzer.Services;
using RaynMaker.Infrastructure;

namespace RaynMaker.Analyzer.ViewModels
{
    [Export]
    class AssetMasterPageModel : BindableBase, INavigationAware
    {
        private string myHeader;
        private IAssetNavigation myNavigation;
        private IRegionManager myRegionManager;

        [ImportingConstructor]
        public AssetMasterPageModel( IAssetNavigation navigation, IRegionManager regionManager )
        {
            myNavigation = navigation;
            myRegionManager = regionManager;

            OkCommand = new DelegateCommand( OnOk );
            CancelCommand = new DelegateCommand( OnCancel );
        }

        public string Header
        {
            get { return myHeader; }
            private set { SetProperty( ref myHeader, value ); }
        }

        public bool IsNavigationTarget( NavigationContext navigationContext )
        {
            return true;
        }

        public void OnNavigatedFrom( NavigationContext navigationContext )
        {
        }

        public void OnNavigatedTo( NavigationContext navigationContext )
        {
            var args = new AssetNavigationParameters( navigationContext.Parameters );
            Header = args.Stock.Company.Name;
        }

        public ICommand OkCommand { get; private set; }

        private void OnOk()
        {
            foreach( var contentPage in GetContentPages() )
            {
                contentPage.Complete();
            }

            myNavigation.ClosePage( this );
        }

        private IEnumerable<IContentPage> GetContentPages()
        {
            if( !myRegionManager.Regions.ContainsRegionWithName( RegionNames.AssetContentPages ) )
            {
                return Enumerable.Empty<IContentPage>();
            }

            var region = myRegionManager.Regions[ RegionNames.AssetContentPages ];

            return region.Views
                .OfType<FrameworkElement>()
                .Select( view => view.DataContext )
                .OfType<IContentPage>();
        }

        public ICommand CancelCommand { get; private set; }

        private void OnCancel()
        {
            foreach( var contentPage in GetContentPages() )
            {
                contentPage.Cancel();
            }
            
            myNavigation.ClosePage( this );
        }
    }
}