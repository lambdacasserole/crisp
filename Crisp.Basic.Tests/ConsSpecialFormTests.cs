﻿using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using Ploeh.AutoFixture;

using Crisp.Interfaces.Evaluation;
using Crisp.Interfaces.Types;
using Crisp.Types;

namespace Crisp.Basic.Tests
{
    [TestClass]
    public class ConsSpecialFormTests
    {
        private static Fixture _fixture;
        private static IEvaluator _mockEvaluator;

        [ClassInitialize] 
        public static void Initialize(TestContext context)
        {
            _fixture = new Fixture();

            // Use a dummy evaluator that just returns what it's been given.
            var mockEvaluator = new Mock<IEvaluator>();
            mockEvaluator.Setup(m => m.Evaluate(It.IsAny<ISymbolicExpression>()))
                .Returns((ISymbolicExpression s) => s);
            _mockEvaluator = mockEvaluator.Object;
        }

        [TestMethod]
        public void ConsSpecialFormShouldReturnCorrectPair()
        {
            /**
             * Description: The cons special form should return a pair with the given head and tail. This test ensures 
             * that this behavior is present.
             */

            // Special form to test.
            var function = new ConsSpecialForm();
            
            // Compute answer.
            var head = _fixture.Create<NumericAtom>();
            var tail = _fixture.Create<NumericAtom>();
            var args = new List<ISymbolicExpression> { head, tail }.ToProperList();
            var ans = function.Apply(args, _mockEvaluator).AsPair();
            
            // We should have a pair containing the head and the tail as a result.
            Assert.AreEqual(head, ans.Head);
            Assert.AreEqual(tail, ans.Tail);
        }

        [TestMethod]
        public void ConsSpecialFormShouldTakeCorrectNumberOfParameters()
        {
            /**
             * Description: This special form only takes a certain number of parameters, this test ensures that an 
             * exception is thrown in case more than that amount is given.
             */

            var function = new ConsSpecialForm();
            
            var arg = _fixture.Create<NumericAtom>();
            var correct = new List<ISymbolicExpression> {arg, arg}.ToProperList();
            var incorrect = new List<ISymbolicExpression> {arg, arg, arg}.ToProperList();

            function.Apply(correct, _mockEvaluator);

            try
            {
                function.Apply(incorrect, _mockEvaluator);

                // We should have failed.
                Assert.Fail("Exception should have been thrown for wrong number of arguments.");
            }
            catch (FunctionApplicationException) { }
        }
    }
}
