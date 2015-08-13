﻿using System.Collections.Generic;
using System.Linq;
using Plainion;
using RaynMaker.Blade.DataSheetSpec;
using RaynMaker.Blade.Entities;

namespace RaynMaker.Blade.Engine
{
    class MissingDataForPeriod : IFigureProviderFailure
    {
        public MissingDataForPeriod( string datum, IPeriod period, params IPeriod[] periods )
        {
            Contract.RequiresNotNullNotEmpty( datum, "datum" );
            Contract.RequiresNotNull( period, "period" );

            Datum = datum;

            var allPeriods = periods.ToList();
            allPeriods.Add( period );

            Period = allPeriods;
        }

        public string Datum { get; private set; }

        public IReadOnlyCollection<IPeriod> Period { get; private set; }

        public override string ToString()
        {
            return string.Format( "No data found for '{0}' in periods: {1}", Datum, string.Join( ",", Period ) );
        }
    }
}
