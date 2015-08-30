﻿using System.ComponentModel.Composition;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Regions;
using RaynMaker.Data.ViewModels;

namespace RaynMaker.Data.Views
{
    [Export]
    [ViewSortHint( "100" )]
    public partial class FactsContentPage : UserControl
    {
        [ImportingConstructor]
        internal FactsContentPage( FactsContentPageModel viewModel )
        {
            InitializeComponent();

            DataContext = viewModel;
        }
    }
}