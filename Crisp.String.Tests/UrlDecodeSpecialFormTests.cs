﻿using System.Collections.Generic;
using System.Web;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using Ploeh.AutoFixture;

using Crisp.Enums;
using Crisp.Interfaces;
using Crisp.Interfaces.Evaluation;
using Crisp.Interfaces.Types;
using Crisp.Types;

namespace Crisp.String.Tests
{
    [TestClass]
    public class UrlDecodeSpecialFormTests
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
        public void UrlDecodeSpecialFormShouldProduceCorrectResult()
        {
            /**
             * Description: The url-decode special form should return a a string atom that contains the given string
             * atom URL-decoded. This test ensures that this is the case.
             */

            // Special form to test.
            var function = new UrlDecodeSpecialForm();

            // Compute answer.
            var x = new StringAtom("hello%3dworld%26+foo%2fbar");
            var args = new List<ISymbolicExpression> { x }.ToProperList();
            var ans = function.Apply(args, _mockEvaluator);

            // We should have the correct string atom as a result.
            Assert.AreEqual(ans.Type, SymbolicExpressionType.String);
            Assert.AreEqual(HttpUtility.UrlDecode(x.Value), ans.AsString().Value);
        }

        [TestMethod]
        public void UrlDecodeSpecialFormShouldTakeCorrectNumberOfParameters()
        {
            /**
             * Description: This special form only takes a certain number of parameters, this test ensures that an 
             * exception is thrown in case more than that amount is given.
             */

            var function = new UrlDecodeSpecialForm();

            var arg = _fixture.Create<StringAtom>();
            var correct = new List<ISymbolicExpression> { arg }.ToProperList();
            var incorrect = new List<ISymbolicExpression> { arg, arg, arg }.ToProperList();

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
