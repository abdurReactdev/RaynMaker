﻿using System.Collections.Generic;
using RaynMaker.Entities;

namespace RaynMaker.Modules.Analysis.Engine
{
    static class FigureProviderContextExtensions
    {
        public static IDatumSeries GetDatumSeries<T>( this IFigureProviderContext self )
        {
            return self.GetSeries( typeof( T ).Name );
        }
    }
}