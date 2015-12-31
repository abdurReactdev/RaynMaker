﻿using NUnit.Framework;
using RaynMaker.Import.Spec.v2.Locating;

namespace RaynMaker.Import.Tests.Spec.Locating
{
    [TestFixture]
    public class RequestTests
    {
        [Test]
        public void Clone_WhenCalled_AllMembersAreCloned()
        {
            var navi = new Request( "http://test1.org" );

            var clone = FormatFactory.Clone( navi );

            Assert.That( clone.UrlString, Is.EqualTo( "http://test1.org" ) );
        }
    }
}
