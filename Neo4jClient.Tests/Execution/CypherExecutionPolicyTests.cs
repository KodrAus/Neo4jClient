using System;
using System.Runtime.Remoting.Channels;
using Neo4jClient.ApiModels.Cypher;
using Neo4jClient.Cypher;
using Neo4jClient.Execution;
using Neo4jClient.Serialization;
using NSubstitute;
using NUnit.Framework;

namespace Neo4jClient.Test.Execution
{
    [TestFixture]
    public class CypherExecutionPolicyTests
    {
        [Test]
        public void SerializesInTransactionFormat_WhenUsingTransactionEndPoint()
        {
            var graphClient = Substitute.For<IRawGraphClient>();
            graphClient.IsUsingTransactionalEndpointForCypher.Returns(true);
            graphClient.Serializer.Returns(Substitute.For<ISerializer>());


            var cep = new CypherExecutionPolicy(graphClient);

            var cypherQuery = new CypherQuery("RETURN 1", null, CypherResultMode.Projection, CypherResultFormat.DependsOnEnvironment);

            cep.SerializeRequest(cypherQuery);
            graphClient.Serializer.Received().Serialize(Arg.Any<CypherStatementList>());
        }

        [Test]
        public void SerializesInCypherFormat_WhenNotUsingTransactionEndPoint()
        {
            var graphClient = Substitute.For<IRawGraphClient>();
            graphClient.Serializer.Returns(Substitute.For<ISerializer>());


            var cep = new CypherExecutionPolicy(graphClient);

            var cypherQuery = new CypherQuery("RETURN 1", null, CypherResultMode.Projection, CypherResultFormat.DependsOnEnvironment);

            cep.SerializeRequest(cypherQuery);
            graphClient.Serializer.Received().Serialize(Arg.Any<CypherApiQuery>());
        }

        [TestCase(null)]
        [TestCase("string")]
        [ExpectedException(typeof (InvalidOperationException))]
        public void ThrowsInvalidOperationException_WhenParameterIsNotACypherQuery(object toSerialize)
        {
            var graphClient = Substitute.For<IRawGraphClient>();
            var cep = new CypherExecutionPolicy(graphClient);

            cep.SerializeRequest(toSerialize);
        }
    }
}