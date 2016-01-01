﻿using System.Runtime.Serialization;
using NUnit.Framework;
using RaynMaker.Modules.Import.Spec;
using RaynMaker.Modules.Import.Spec.v2.Extraction;

namespace RaynMaker.Modules.Import.UnitTests.Spec.Extraction
{
    [TestFixture]
    public class TableDescriptorBaseTests
    {
        [DataContract( Namespace = "https://github.com/bg0jr/RaynMaker/Import/Spec", Name = "DummyFormat" )]
        private class DummyFormat : TableDescriptorBase
        {
            public DummyFormat( params FormatColumn[] columns )
                : base( "dummy", columns )
            {
            }
        }

        [Test]
        public void SkipRowsIsImmutable()
        {
            var format = new DummyFormat();

            var skipRows = new int[] { 1, 2, 3 };
            format.SkipRows = skipRows;

            skipRows[ 1 ] = 42;

            Assert.AreEqual( 1, format.SkipRows[ 0 ] );
            Assert.AreEqual( 2, format.SkipRows[ 1 ] );
            Assert.AreEqual( 3, format.SkipRows[ 2 ] );
        }

        [Test]
        public void SkipColumnsIsImmutable()
        {
            var format = new DummyFormat();

            var skipColumns = new int[] { 1, 2, 3 };
            format.SkipColumns = skipColumns;

            skipColumns[ 1 ] = 42;

            Assert.AreEqual( 1, format.SkipColumns[ 0 ] );
            Assert.AreEqual( 2, format.SkipColumns[ 1 ] );
            Assert.AreEqual( 3, format.SkipColumns[ 2 ] );
        }
        
        [Test]
        public void Clone_WhenCalled_AllMembersAreCloned()
        {
            var format = new DummyFormat(
                new FormatColumn( "c1", typeof( double ), "0.00" ),
                new FormatColumn( "c2", typeof( string ), "" ) );
            format.SkipRows = new[] { 5, 9 };
            format.SkipColumns = new[] { 11, 88 };

            var clone = FormatFactory.Clone( format );

            Assert.That( clone.Columns[ 0 ].Name, Is.EqualTo( "c1" ) );
            Assert.That( clone.Columns[ 0 ].Type, Is.EqualTo( typeof( double ) ) );
            Assert.That( clone.Columns[ 0 ].Format, Is.EqualTo( "0.00" ) );

            Assert.That( clone.Columns[ 1 ].Name, Is.EqualTo( "c2" ) );
            Assert.That( clone.Columns[ 1 ].Type, Is.EqualTo( typeof( string ) ) );
            Assert.That( clone.Columns[ 1 ].Format, Is.EqualTo( "" ) );
        
            Assert.That( clone.SkipRows, Is.EquivalentTo( format.SkipRows ) );
            Assert.That( clone.SkipColumns, Is.EquivalentTo( format.SkipColumns ) );
        }
    }
}
