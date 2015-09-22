﻿using System.ComponentModel.Composition;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using RaynMaker.Infrastructure;

namespace RaynMaker.Notes
{
    [Export]
    class StorageService
    {
        private IProjectHost myProjectHost;

        [ImportingConstructor]
        public StorageService( IProjectHost projectHost )
        {
            myProjectHost = projectHost;
        }

        public void Load( FlowDocument target )
        {
            var file = Path.Combine( myProjectHost.Project.StorageRoot, "Notes.rtf" );
            if( !File.Exists( file ) )
            {
                return;
            }

            using( var stream = new FileStream( file, FileMode.Open, FileAccess.Read ) )
            {
                var range = new TextRange( target.ContentStart, target.ContentEnd );
                range.Load( stream, DataFormats.Rtf );
            }
        }

        public void Store( FlowDocument content )
        {
            var file = Path.Combine( myProjectHost.Project.StorageRoot, "Notes.rtf" );
            using( var stream = new FileStream( file, FileMode.Create, FileAccess.Write ) )
            {
                var range = new TextRange( content.ContentStart, content.ContentEnd );
                range.Save( stream, DataFormats.Rtf );
            }
        }
    }
}