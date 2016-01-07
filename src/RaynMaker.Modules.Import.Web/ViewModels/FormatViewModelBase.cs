﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Prism.Mvvm;
using Plainion;
using RaynMaker.Entities;
using RaynMaker.Modules.Import.Design;
using RaynMaker.Modules.Import.Documents.WinForms;
using RaynMaker.Modules.Import.Spec.v2.Extraction;

namespace RaynMaker.Modules.Import.Web.ViewModels
{
    class FormatViewModelBase<TFigureDescriptor, TMarker> : BindableBase, IDescriptorViewModel
        where TFigureDescriptor : IFigureDescriptor
        where TMarker : IHtmlMarker
    {
        private Type mySelectedDatum;

        protected FormatViewModelBase( TFigureDescriptor descriptor, TMarker marker )
            : this( descriptor, new HtmlMarkupBehavior<TMarker>( marker ) )
        {
        }

        protected FormatViewModelBase( TFigureDescriptor descriptor, IHtmlMarkupBehavior<TMarker> markupBehavior )
        {
            Contract.RequiresNotNull( descriptor, "descriptor" );
            Contract.RequiresNotNull( markupBehavior, "markupBehavior" );

            Format = descriptor;

            Datums = Dynamics.AllDatums
                .OrderBy( d => d.Name )
                .ToList();

            MarkupBehavior = markupBehavior;
            MarkupBehavior.SelectionChanged += OnSelectionChanged;
        }

        public TFigureDescriptor Format { get; private set; }

        IFigureDescriptor IDescriptorViewModel.Format { get { return this.Format; } }

        // TODO: we do not support add, remove and edit of datums as they are currently fixed by entities model.
        // TODO: "Standing" datums also exists
        public IEnumerable<Type> Datums { get; private set; }

        public Type SelectedDatum
        {
            get { return mySelectedDatum; }
            set
            {
                if( SetProperty( ref mySelectedDatum, value ) )
                {
                    Format.Figure = mySelectedDatum.Name;
                }
            }
        }

        protected IHtmlMarkupBehavior<TMarker> MarkupBehavior { get; private set; }

        private void OnSelectionChanged( object sender, EventArgs e )
        {
            OnSelectionChanged();
        }

        protected virtual void OnSelectionChanged()
        {
        }

        public IDocument Document
        {
            set
            {
                // always force update because the document reference does NOT change!
                //if( myMarkupDocument.Document == value )
                //{
                //    return;
                //}

                if( value == null )
                {
                    MarkupBehavior.Detach();
                    return;
                }

                MarkupBehavior.AttachTo( ( HtmlDocumentAdapter )value );

                OnDocumentChanged();
            }
        }

        /// <summary>
        /// Can be used to reapply the path or SelectedElement. 
        /// When toggeling between FigureDescriptor ViewModels we have to detach and reattach the document. Use this hook to update the path or selected element
        /// so that everything gets applied to the new document automatically.
        /// </summary>
        protected virtual void OnDocumentChanged()
        {
        }

        public void Unmark()
        {
            MarkupBehavior.Marker.Unmark();
        }
    }
}
