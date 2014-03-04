using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TSqlBuilder.Builders;

namespace TSqlBuilder.Tests.UpdateStatement
{
    [TestFixture]
    internal sealed class SimpleUpdateTests
    {
        [Test]
        public void ShouldGenerateUpdate_ForSimpleColumns()
        {
            var result = CommandBuilder.Update.Table("Test").Set("Id=3").Where("Id>3").Build();

            var expectedResult = "UPDATE [Test] SET Id=3 WHERE Id>3";
            Assert.That(result, Is.EqualTo(expectedResult));
        }
    }
}
