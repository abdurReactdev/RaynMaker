﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using RaynMaker.Entities;
using RaynMaker.Modules.Import.Web.Services;
using RaynMaker.Modules.Import.Web.ViewModels;
using RaynMaker.Modules.Import.Web.Views;
using RaynMaker.Infrastructure.Services;

namespace RaynMaker.Modules.Import.Web
{
    [Export( typeof( IDataProvider ) )]
    class WebDatumProvider : IDataProvider
    {
        private StorageService myStorageService;
        private ILutService myLutService;

        // for usability reasons we remember the currency settings for the stock
        private CurrencyCache myCurrencyCache;

        private class CurrencyCache
        {
            public string Isin;
            public Currency Currency;

        }
        [ImportingConstructor]
        public WebDatumProvider( StorageService storageService, ILutService lutService )
        {
            myStorageService = storageService;
            myLutService = lutService;
            myCurrencyCache = new CurrencyCache();
        }

        public bool CanFetch( Type datum )
        {
            return myStorageService.Load()
                .Any( source => source.ExtractionSpec.Any( f => f.Figure == datum.Name ) );
        }

        public void Fetch( DataProviderRequest request, ICollection<IDatum> resultContainer )
        {
            var previewViewModel = new ImportPreviewModel( myStorageService, myLutService.CurrenciesLut )
            {
                Stock = request.Stock,
                From = request.From,
                To = request.To,
                Series = resultContainer
            };

            if( request.WithPreview )
            {
                if( myCurrencyCache.Isin == request.Stock.Isin )
                {
                    // take over last setting from user
                    previewViewModel.Currency = myCurrencyCache.Currency;
                }

                var preview = new ImportPreview( previewViewModel );

                previewViewModel.FinishAction = () =>
                {
                    preview.Close();

                    // remember last setting from user
                    if( previewViewModel.Currency != null )
                    {
                        myCurrencyCache.Isin = previewViewModel.Stock.Isin;
                        myCurrencyCache.Currency = previewViewModel.Currency;
                    }
                };
                preview.DataContext = previewViewModel;

                previewViewModel.Fetch( request.DatumType );

                preview.Top = 0;
                preview.Left = 0;
                preview.Show();
            }
            else
            {
                var preview = new ImportPreview( previewViewModel );
                previewViewModel.Browser = new WinForms.SafeWebBrowser();
                previewViewModel.Fetch( request.DatumType );
                previewViewModel.PublishData();
            }
        }
    }
}