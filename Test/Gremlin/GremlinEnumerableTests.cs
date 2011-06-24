﻿using System.Collections.Specialized;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Neo4jClient.Gremlin;

namespace Neo4jClient.Test.Gremlin
{
    [TestFixture]
    public class GremlinEnumerableTests
    {
        [Test]
        [ExpectedException(typeof(DetachedNodeException))]
        public void GetEnumeratorShouldThrowDetachedNodeExceptionWhenClientNotSet()
        {
            // ReSharper disable ReturnValueOfPureMethodIsNotUsed
            var enumerable = new GremlinNodeEnumerable<object>(null, "abc");
            enumerable.ToArray();
            // ReSharper restore ReturnValueOfPureMethodIsNotUsed
        }

        [Test]
        public void GetEnumeratorShouldExecuteQueryAgainstClient()
        {
            // Arrange
            var expectedResults = new[]
            {
                new Node<object>(new object(), new NodeReference<object>(123)),
                new Node<object>(new object(), new NodeReference<object>(456)),
                new Node<object>(new object(), new NodeReference<object>(789))
            };
            var client = Substitute.For<IGraphClient>();
            client
                .ExecuteGetAllNodesGremlin<object>("abc")
                .Returns(expectedResults);

            // Act
            var enumerable = new GremlinNodeEnumerable<object>(client, "abc");
            var results = enumerable.ToArray();

            // Assert
            Assert.AreEqual(expectedResults, results);
        }
    }
}