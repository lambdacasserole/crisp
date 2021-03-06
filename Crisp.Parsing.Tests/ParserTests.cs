﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

using Newtonsoft.Json;

using Crisp.Testing.Common;

namespace Crisp.Parsing.Tests
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void ParserBuildsProperLists()
        {
            /**
             * Description: The parser should build proper lists that end with a pointer to nil. This test checks that 
             * lists are built in this way.
             */
            
            // Parse and serialize expression tree.
            var subject = new Parser(MockTokenListSourceFactory.GetProperListTokenListSource(), 
                MockTokenFilterFactory.GetMockTokenFilter());
            var actual = JsonConvert.SerializeObject(subject.Get());
            
            // Check accuracy of expression tree.
            Assert.AreEqual(Properties.Resources.ParserBuildsProperLists_Expected, actual);
        }

        [TestMethod]
        public void ParserShouldUseDotNotation()
        {
            /**
             * Description: Crisp supports dot notation for creating improper lists. This test checks that dot notation 
             * is supported as required.
             */

            // Parse and serialize expression tree.
            var subject = new Parser(MockTokenListSourceFactory.GetImproperListTokenListSource(),
                MockTokenFilterFactory.GetMockTokenFilter());
            var actual = JsonConvert.SerializeObject(subject.Get());

            // Check accuracy of expression tree.
            Assert.AreEqual(Properties.Resources.ParserShouldUseDotNotation_Expected, actual);
        }

        [TestMethod]
        public void ParserShouldFailOnMismatchedBrackets()
        {
            /**
             * Description: Mismatched brackets should cause the parser to fail for obvious reasons. This test checks 
             * that a failure takes place as expected.
             */

            var subject = new Parser(MockTokenListSourceFactory.GetMismatchedBracketsTokenListSource(),
                MockTokenFilterFactory.GetMockTokenFilter());
            try
            {
                subject.Get();

                // We should have failed.
                Assert.Fail("Parser should have detected mismatched brackets.");
            }
            catch (ParsingException ex)
            {
                // Error position should be reported correctly.
                Assert.AreEqual(1, ex.Line);
                Assert.AreEqual(1, ex.Column);
            }
        }

        [TestMethod]
        public void ParserShouldTurnEmptyBracketsIntoNil()
        {
            /**
             * Description: Empty brackets are analogous to nil, so they are substituted out for the nil during 
             * parsing. This test checks that substitution has taken place.
             */

            // Parse and serialize expression tree.
            var subject = new Parser(MockTokenListSourceFactory.GetNilBracketsTokenListSource(),
                MockTokenFilterFactory.GetMockTokenFilter());
            var actual = JsonConvert.SerializeObject(subject.Get());

            // Check accuracy of expression tree.
            Assert.AreEqual(Properties.Resources.ParserShouldTurnEmptyBracketsIntoNil_Expected, actual);
        }

        [TestMethod]
        public void ParserShouldFailOnUnexpectedLeadingDot()
        {
            /**
             * Description: If the head part of a dotted pair is omitted, parsing should fail. This test checks for 
             * that failure.
             */

            var subject = new Parser(MockTokenListSourceFactory.GetHeadlessPairTokenListSource(),
                MockTokenFilterFactory.GetMockTokenFilter());
            try
            {
                subject.Get();

                // We should have failed.
                Assert.Fail("Parser should have detected invalid leading dot.");
            }
            catch (ParsingException ex)
            {
                // Error position should be reported correctly.
                Assert.AreEqual(1, ex.Line);
                Assert.AreEqual(2, ex.Column);
            }
        }

        [TestMethod]
        public void ParserShouldFailOnTooManyElementsInDottedPair()
        {
            /**
             * Description: If the parser encounters a 'dotted pair' with more than two elements, it should fail. This 
             * test checks for that failure.
             */

            var subject = new Parser(MockTokenListSourceFactory.GetDottedTripletTokenListSource(),
                MockTokenFilterFactory.GetMockTokenFilter());
            try
            {
                subject.Get();

                // We should have failed.
                Assert.Fail("Parser should have detected too many elements in dotted pair.");
            }
            catch (ParsingException ex)
            {
                // Error position should be reported correctly.
                Assert.AreEqual(1, ex.Line);
                Assert.AreEqual(8, ex.Column);
            }
        }

        [TestMethod]
        public void ParserShouldFailOnFreestandingExpressions()
        {
            /**
             * Description: If the parser is provided with a naked array of expressions with no enclosing brackets, 
             * this can't be evaluated and parsing should fail. This test checks for that failure.
             */

            var subject = new Parser(MockTokenListSourceFactory.GetFreestandingExpressionsTokenListSource(),
                MockTokenFilterFactory.GetMockTokenFilter());
            try
            {
                subject.Get();

                // We should have failed.
                Assert.Fail("Parser should have detected freestanding expressions.");
            }
            catch (ParsingException ex)
            {
                // Error position should be reported correctly.
                Assert.AreEqual(1, ex.Line);
                Assert.AreEqual(1, ex.Column);
            }
        }

        [TestMethod]
        public void ParserShouldNotFailOnImplicitDottedPair()
        {
            /**
             * Description: Crisp permits a dot between the last two elements in a list literal to form an improper 
             * list. This unit test ensures that the parser doesn't choke.
             */

            // Parse and serialize expression tree.
            var subject = new Parser(MockTokenListSourceFactory.GetImplicitDottedPairTokenListSource(),
                MockTokenFilterFactory.GetMockTokenFilter());
            var actual = JsonConvert.SerializeObject(subject.Get());

            // Check accuracy of expression tree.
            Assert.AreEqual(Properties.Resources.ParserShouldNotFailOnImplicitDottedPair_Expected, actual);
        }
    }
}
