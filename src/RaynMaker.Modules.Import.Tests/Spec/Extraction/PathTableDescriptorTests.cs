﻿using System;
using System.ComponentModel.DataAnnotations;
using NUnit.Framework;
using Plainion.Validation;
using RaynMaker.Modules.Import.Spec;
using RaynMaker.Modules.Import.Spec.v2.Extraction;

namespace RaynMaker.Modules.Import.UnitTests.Spec.Extraction
{
    [TestFixture]
    public class PathTableDescriptorTests
    {
        [Test]
        public void Clone_WhenCalled_AllMembersAreCloned()
        {
            var descriptor = new PathTableDescriptor( );
            descriptor.Columns.Add( new FormatColumn( "c1", typeof( double ) ) );

            var clone = FigureDescriptorFactory.Clone( descriptor );

            Assert.That( clone.Columns[ 0 ].Name, Is.EqualTo( "c1" ) );
        }

        [Test]
        public void Validate_IsValid_DoesNotThrows()
        {
            var descriptor = new PathTableDescriptor();
            descriptor.Columns.Add( new FormatColumn( "c1", typeof( double ) ) );
            descriptor.Path = "123";

            RecursiveValidator.Validate( descriptor );
        }

        [Test]
        public void Validate_InvalidPath_Throws( [Values( null, "" )]string path )
        {
            var descriptor = new PathTableDescriptor();
            descriptor.Columns.Add( new FormatColumn( "c1", typeof( double ) ) );
            descriptor.Path = path;

            var ex = Assert.Throws<ValidationException>( () => RecursiveValidator.Validate( descriptor ) );
            Assert.That( ex.Message, Is.StringContaining( "The Path field is required" ) );
        }
    }
}