using System;
using FluentAssertions;
using VaraniumSharp.WinUI.ExtensionMethods;
using Xunit;

namespace VaraniumSharp.WinUI.Tests.ExtensionMethods
{
    public class PredicateExtensionTests
    {
        #region Public Methods

        [Theory]
        [InlineData(true, false, false)]
        [InlineData(true, true, true)]
        public void AndAlsoCorrectlyAndTheResultsOfPassedPredicates(bool f1, bool f2, bool expectedResult)
        {
            // arrange
            var func1 = new Func<object?, bool>((o) => f1);
            var func2 = new Func<object?, bool>((o) => f2);

            var finalFunc = func1.AndAlso(func2);

            // act
            var result = finalFunc.Invoke(null);

            // assert
            result.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData(true, false, true)]
        [InlineData(true, true, true)]
        public void OrElseCorrectlyOrTheResultsOfPassedPredicates(bool f1, bool f2, bool expectedResult)
        {
            // arrange
            var func1 = new Func<object?, bool>((o) => f1);
            var func2 = new Func<object?, bool>((o) => f2);

            var finalFunc = func1.OrElse(func2);

            // act
            var result = finalFunc.Invoke(null);

            // assert
            result.Should().Be(expectedResult);
        }

        #endregion
    }
}