﻿using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Xaml;
using Plainion;
using RaynMaker.Blade.Model;
using RaynMaker.Entities;

namespace RaynMaker.Blade.AnalysisSpec
{
    public sealed class CurrencyConverter : System.ComponentModel.TypeConverter
    {
        internal static Project Sheet { get; set; }

        public override bool CanConvertFrom( ITypeDescriptorContext context, Type sourceType )
        {
            return sourceType == typeof( string ) || base.CanConvertFrom( context, sourceType );
        }

        public override object ConvertFrom( ITypeDescriptorContext context, CultureInfo culture, object value )
        {
            if( value == null )
            {
                throw base.GetConvertFromException( value );
            }

            var rootProvider = ( IRootObjectProvider )context.GetService( typeof( IRootObjectProvider ) );
            var root = rootProvider.RootObject;

            string text = value as string;
            if( text != null )
            {
                return Parse( text );
            }

            return base.ConvertFrom( context, culture, value );
        }

        private static Currency Parse( string name )
        {
            Contract.Invariant( Sheet != null, "Currencies sheet not yet initialized" );

            var currency = Sheet.Currencies
                .FirstOrDefault( c => c.Name == name );

            Contract.Requires( currency != null, "No currency found with name: " + name );

            return currency;
        }

        public override bool CanConvertTo( ITypeDescriptorContext context, Type destinationType )
        {
            throw new NotImplementedException();
        }

        public override object ConvertTo( ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType )
        {
            throw new NotImplementedException();
        }
    }
}
