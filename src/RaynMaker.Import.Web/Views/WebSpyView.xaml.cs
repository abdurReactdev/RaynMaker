﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Blade.Data;
using RaynMaker.Import.Core;
using RaynMaker.Import.Spec;
using RaynMaker.Import.Web.ViewModels;
using System.Linq;
using Blade;
using Plainion;
using RaynMaker.Import.Html;
using System.ComponentModel;
using Microsoft.Practices.Prism.Mvvm;

namespace RaynMaker.Import.Web.Views
{
    [Export]
    public partial class WebSpyView : UserControl, IBrowser
    {
        private WebSpyViewModel myViewModel;
        private MarkupDocument myMarkupDocument = null;
        private LegacyDocumentBrowser myDocumentBrowser = null;

        [ImportingConstructor]
        internal WebSpyView( WebSpyViewModel viewModel )
        {
            myViewModel = viewModel;

            InitializeComponent();

            myDocumentBrowser = new LegacyDocumentBrowser( myBrowser );
            myDocumentBrowser.Browser.Navigating += myBrowser_Navigating;
            myDocumentBrowser.Browser.DocumentCompleted += myBrowser_DocumentCompleted;

            // disable links
            // TODO: we cannot use this, it disables navigation in general (Navigate() too)
            //myBrowser.AllowNavigation = false;

            // TODO: how to disable images in browser

            myMarkupDocument = new MarkupDocument();
            myMarkupDocument.ValidationChanged += SeriesName_ValidationChanged;

            DataContext = viewModel;

            Loaded += WebSpyView_Loaded;

            PropertyChangedEventManager.AddHandler( myViewModel.DataFormat, OnSelectedDimensionChanged, PropertySupport.ExtractPropertyName( () => myViewModel.DataFormat.SelectedDimension ) );
            PropertyChangedEventManager.AddHandler( myViewModel.DataFormat, OnSeriesNameChanged, PropertySupport.ExtractPropertyName( () => myViewModel.DataFormat.SeriesName ) );
            PropertyChangedEventManager.AddHandler( myViewModel.DataFormat, OnRowHeaderColumnChanged, PropertySupport.ExtractPropertyName( () => myViewModel.DataFormat.RowHeaderColumn ) );
            PropertyChangedEventManager.AddHandler( myViewModel.DataFormat, OnSkipRowsChanged, PropertySupport.ExtractPropertyName( () => myViewModel.DataFormat.SkipRows ) );
        }

        private void WebSpyView_Loaded( object sender, RoutedEventArgs e )
        {
            myViewModel.Browser = this;
        }

        private void myBrowser_Navigating( object sender, System.Windows.Forms.WebBrowserNavigatingEventArgs e )
        {
            if( myViewModel.Navigation.IsCapturing )
            {
                myViewModel.Navigation.NavigationUrls += new NavigatorUrl( UriType.Request, e.Url ).ToString();
                myViewModel.Navigation.NavigationUrls += Environment.NewLine;
            }
        }

        private void myBrowser_DocumentCompleted( object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e )
        {
            myViewModel.DataFormat.Path = "";
            myViewModel.DataFormat.Value = "";

            if( myMarkupDocument.Document != null )
            {
                myMarkupDocument.Document.Click -= HtmlDocument_Click;
            }

            myMarkupDocument.Document = myDocumentBrowser.Document;
            myMarkupDocument.Document.Click += HtmlDocument_Click;

            myViewModel.AddressBar.Url = myMarkupDocument.Document.Url.ToString();

            if( myViewModel.Navigation.IsCapturing )
            {
                myViewModel.Navigation.NavigationUrls += new NavigatorUrl( UriType.Response, myDocumentBrowser.Browser.Document.Url ).ToString();
                myViewModel.Navigation.NavigationUrls += Environment.NewLine;
            }
        }

        private void HtmlDocument_Click( object sender, System.Windows.Forms.HtmlElementEventArgs e )
        {
            myViewModel.DataFormat.Path = myMarkupDocument.SelectedElement.GetPath().ToString();
            myViewModel.DataFormat.Value = myMarkupDocument.SelectedElement.InnerText;
        }

        private void mySearchPath_Click( object sender, RoutedEventArgs e )
        {
            myMarkupDocument.Anchor = myViewModel.DataFormat.Path;

            if( myMarkupDocument.SelectedElement != null )
            {
                myViewModel.DataFormat.Value = myMarkupDocument.SelectedElement.InnerText;
            }
        }

        //protected override void OnHandleDestroyed( EventArgs e )
        //{
        //    myMarkupDocument.Dispose();
        //    myMarkupDocument = null;

        //    myDocumentBrowser.Browser.DocumentCompleted -= myBrowser_DocumentCompleted;
        //    myDocumentBrowser.Dispose();
        //    myBrowser.Dispose();
        //    myBrowser = null;

        //    base.OnHandleDestroyed( e );
        //}

        private void OnSelectedDimensionChanged( object sender, PropertyChangedEventArgs e )
        {
            myMarkupDocument.Dimension = myViewModel.DataFormat.SelectedDimension;
        }

        private void OnSkipRowsChanged( object sender, PropertyChangedEventArgs eventArgs )
        {
            SkipElements( myViewModel.DataFormat.SkipRows, x => myMarkupDocument.SkipRows = x );
        }

        private void mySkipColumns_TextChanged( object sender, RoutedEventArgs eventArgs )
        {
            SkipElements( mySkipColumns.Text, x => myMarkupDocument.SkipColumns = x );
        }

        private void OnRowHeaderColumnChanged( object sender, PropertyChangedEventArgs e )
        {
            MarkHeader( myViewModel.DataFormat.RowHeaderColumn, x => myMarkupDocument.RowHeader = x );
        }

        private void myColumnHeader_TextChanged( object sender, RoutedEventArgs e )
        {
            MarkHeader( myColumnHeader.Text, x => myMarkupDocument.ColumnHeader = x );
        }

        private void OnSeriesNameChanged( object sender, PropertyChangedEventArgs e )
        {
            myMarkupDocument.SeriesName = myViewModel.DataFormat.SeriesName;
        }

        private void myReset_Click( object sender, RoutedEventArgs e )
        {
            myViewModel.DataFormat.Path = "";
            myViewModel.DataFormat.Value = "";

            mySkipColumns.Text = string.Empty;
            myViewModel.DataFormat.SkipRows = string.Empty;
            myViewModel.DataFormat.RowHeaderColumn = string.Empty;
            myColumnHeader.Text = string.Empty;
            myViewModel.DataFormat.SeriesName = string.Empty;

            myMarkupDocument.Reset();
        }

        private void SeriesName_ValidationChanged( bool isValid )
        {
            myViewModel.DataFormat.IsValid = isValid;
        }

        private void MarkHeader( string text, Action<int> UpdateTemplate )
        {
            string str = text.TrimOrNull();
            if( string.IsNullOrEmpty( str ) )
            {
                UpdateTemplate( -1 );
                return;
            }

            try
            {
                UpdateTemplate( Convert.ToInt32( str ) );
            }
            catch
            {
                //errorProvider1.SetError( config, "Must be: <number> [, <number> ]*" );
            }
        }

        private void SkipElements( string text, Action<int[]> UpdateTemplate )
        {
            if( text.IsNullOrTrimmedEmpty() )
            {
                UpdateTemplate( null );
                return;
            }

            string[] tokens = text.Split( ',' );

            try
            {
                var positions = from t in tokens
                                where !t.IsNullOrTrimmedEmpty()
                                select Convert.ToInt32( t );

                UpdateTemplate( positions.ToArray() );
            }
            catch
            {
                //errorProvider1.SetError( config, "Must be: <number> [, <number> ]*" );
            }
        }

        public void Navigate( string url )
        {
            myDocumentBrowser.Navigate( url );
        }

        public void LoadDocument( IEnumerable<NavigatorUrl> urls )
        {
            myDocumentBrowser.LoadDocument( urls );
        }
    }
}
