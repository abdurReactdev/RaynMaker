﻿using System;
using System.Linq;
using RaynMaker.Modules.Import.Design;
using RaynMaker.Modules.Import.Parsers.Html;
using RaynMaker.Modules.Import.Spec;
using RaynMaker.Modules.Import.Spec.v2.Extraction;

namespace RaynMaker.Modules.Import.Web.ViewModels
{
    class PathSeriesFormatViewModel : FormatViewModelBase<PathSeriesDescriptor,HtmlTableMarker>
    {
        private string myPath;
        private string myValue;
        private SeriesOrientation mySelectedDimension;
        private bool myIsValid;
        private string myRowHeaderColumn;
        private string myColumnHeaderRow;
        private string mySkipValues;

        public PathSeriesFormatViewModel( PathSeriesDescriptor format )
            : base( format, new HtmlTableMarker() )
        {
            IsValid = true;

            Value = "";

            // first set properties without side-effects to others
            SelectedDatum = Datums.FirstOrDefault( d => d.Name == Format.Figure );
            Path = Format.Path;
            SkipValues = string.Join( ",", format.Excludes );
            TimeFormat = Format.TimeFormat ?? new FormatColumn( "time", typeof( int ) );
            ValueFormat = Format.ValueFormat ?? new FormatColumn( "value", typeof( double ) );

            ColumnHeaderRow = ( format.Orientation == SeriesOrientation.Row ? Format.TimesLocator.HeaderSeriesPosition : Format.ValuesLocator.HeaderSeriesPosition ).ToString();
            RowHeaderColumn = ( format.Orientation == SeriesOrientation.Row ? Format.ValuesLocator.HeaderSeriesPosition : Format.TimesLocator.HeaderSeriesPosition ).ToString();

            // needs to be AFTER RowHeaderColumn and ColumnHeaderRow
            SelectedDimension = Format.Orientation;
        }

        protected override void OnSelectionChanged()
        {
            if( MarkupBehavior.SelectedElement != null )
            {
                Path = MarkupBehavior.PathToSelectedElement;
                Value = MarkupBehavior.SelectedElement.InnerText;
            }
        }

        protected override void OnDocumentChanged()
        {
            MarkupBehavior.PathToSelectedElement = Path;
        }
        
        public string Path
        {
            get { return myPath; }
            set
            {
                if( SetProperty( ref myPath, value ) )
                {
                    Format.Path = myPath;

                    if( !string.IsNullOrWhiteSpace( myPath ) )
                    {
                        MarkupBehavior.PathToSelectedElement = myPath;

                        if( MarkupBehavior.SelectedElement != null )
                        {
                            Value = MarkupBehavior.SelectedElement.InnerText;
                        }
                    }
                }
            }
        }

        public string Value
        {
            get { return myValue; }
            set { SetProperty( ref myValue, value ); }
        }

        public SeriesOrientation SelectedDimension
        {
            get { return mySelectedDimension; }
            set
            {
                if( SetProperty( ref mySelectedDimension, value ) )
                {
                    if( SelectedDimension == SeriesOrientation.Row )
                    {
                        MarkupBehavior.Marker.ExpandColumn = false;
                        MarkupBehavior.Marker.ExpandRow = true;
                    }
                    else if( SelectedDimension == SeriesOrientation.Column )
                    {
                        MarkupBehavior.Marker.ExpandColumn = true;
                        MarkupBehavior.Marker.ExpandRow = false;
                    }
                }
            }
        }

        //public string SeriesName
        //{
        //    get { return mySeriesName; }
        //    set
        //    {
        //        if( SetProperty( ref mySeriesName, value ) )
        //        {
        //            UpdateAnchor();

        //            Format.SeriesName = mySeriesName;

        //            MarkupDocument.SeriesName = mySeriesName;
        //        }
        //    }
        //}

        public bool IsValid
        {
            get { return myIsValid; }
            set { SetProperty( ref myIsValid, value ); }
        }

        public string RowHeaderColumn
        {
            get { return myRowHeaderColumn; }
            set
            {
                if( SetProperty( ref myRowHeaderColumn, value ) )
                {
                    MarkHeader( myRowHeaderColumn, x => MarkupBehavior.Marker.RowHeaderColumn = x );
                }
            }
        }

        private void MarkHeader( string text, Action<int> UpdateTemplate )
        {
            string str = text != null ? text.Trim() : null;
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

        public string ColumnHeaderRow
        {
            get { return myColumnHeaderRow; }
            set
            {
                if( SetProperty( ref myColumnHeaderRow, value ) )
                {
                    MarkHeader( myColumnHeaderRow, x => MarkupBehavior.Marker.ColumnHeaderRow = x );
                }
            }
        }

        public string SkipValues
        {
            get { return mySkipValues; }
            set
            {
                if( SetProperty( ref mySkipValues, value ) )
                {
                    Format.Excludes.Clear();
                    Format.Excludes.AddRange( GetIntArray( mySkipValues ) );
                    MarkupBehavior.Marker.SkipRows = Format.Excludes.ToArray();
                }
            }
        }

        private int[] GetIntArray( string value )
        {
            if( string.IsNullOrWhiteSpace( value ) )
            {
                return null;
            }

            try
            {
                return value.Split( ',' )
                    .Where( t => !string.IsNullOrWhiteSpace( t ) )
                    .Select( t => Convert.ToInt32( t ) )
                    .ToArray();
            }
            catch
            {
                //errorProvider1.SetError( config, "Must be: <number> [, <number> ]*" );
            }

            return null;
        }

        public FormatColumn TimeFormat { get; private set; }

        public FormatColumn ValueFormat { get; private set; }
    }
}
