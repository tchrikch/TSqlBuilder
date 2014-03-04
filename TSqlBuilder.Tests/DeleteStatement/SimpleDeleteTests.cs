using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace TSqlBuilder.Tests.DeleteStatement
{
    [TestFixture]
    internal sealed class SimpleDeleteTests
    {
        [Test]
        public void T1()
        {
            var statement = Builder.Delete.From("Test").Where("Id>0").And("Id<4").Or("Id=5").Build();

            var expectedStatement = "DELETE FROM [Test] WHERE Id>0 AND Id<4 OR Id=5";
            Assert.That(statement, Is.EqualTo(expectedStatement));
        }
    }
}
