﻿using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using Ploeh.AutoFixture;

using Crisp.Core;
using Crisp.Core.Evaluation;
using Crisp.Core.Types;

namespace Crisp.Basic.Tests
{
    [TestClass]
    public class CdrSpecialFormTests
    {
        private static Fixture _fixture;
        private static IEvaluator _mockEvaluator;

        [ClassInitialize] 
        public static void Initialize(TestContext context)
        {
            _fixture = new Fixture();
            _fixture.Register<SymbolicExpression>(() => _fixture.Create<NumericAtom>());

            // Use a dummy evaluator that just returns what it's been given.
            var mockEvaluator = new Mock<IEvaluator>();
            mockEvaluator.Setup(m => m.Evaluate(It.IsAny<SymbolicExpression>()))
                .Returns((SymbolicExpression s) => s);
            _mockEvaluator = mockEvaluator.Object;
        }

        [TestMethod]
        public void CdrSpecialFormShouldReturnListTail()
        {
            /**
             * Description: The cdr special form should return the tail of the pair it is given as an argument. This 
             test ensures that this behavior is present.
             */

            // Special form to test.
            var function = new CdrSpecialForm();
            
            // Compute answer.
            var head = _fixture.Create<NumericAtom>();
            var tail = _fixture.Create<NumericAtom>();
            var args = new List<SymbolicExpression> { new Pair(head, tail) }.ToProperList();
            var ans = function.Apply(args, _mockEvaluator);
            
            // We should have the tail of the pair as a result.
            Assert.AreEqual(tail, ans);
        }
        
        [TestMethod]
        public void CdrSpecialFormShouldTakeCorrectNumberOfParameters()
        {
            /**
             * Description: This special form only takes a certain number of parameters, this test ensures that an 
             * exception is thrown in case more than that amount is given.
             */

            var function = new CdrSpecialForm();
            
            var arg = _fixture.Create<Pair>();
            var correct = new List<SymbolicExpression> {arg}.ToProperList();
            var incorrect = new List<SymbolicExpression> {arg, arg, arg}.ToProperList();

            function.Apply(correct, _mockEvaluator);

            try
            {
                function.Apply(incorrect, _mockEvaluator);

                // We should have failed.
                Assert.Fail("Exception should have been thrown for wrong number of arguments.");
            }
            catch (RuntimeException) { }
        }
    }
}
