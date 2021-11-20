using System;
using System.Linq;
using FluentAssertions;
using VaraniumSharp.WinUI.ExtensionMethods;
using VaraniumSharp.WinUI.Tests.Fixtures;
using Xunit;

namespace VaraniumSharp.WinUI.Tests.ExtensionMethods
{
    public class ObjectExtensionMethodsTests
    {
        [Fact]
        public void RetrievingNestedPropertyValueReturnsTheExpectedValue()
        {
            // arrange
            const string testString = "This is a test";
            const string propertyPath = "NestedProperty.MyProperty";
            var fixture = new PropertyHelperClass
            {
                NestedProperty = new()
                {
                    MyProperty = testString
                }
            };

            // act
            var result = fixture.GetNestedPropertyValue(propertyPath);

            // assert
            result.Should().Be(testString);
        }

        [Fact]
        public void RetrievingPropertyInfoForNestedTypeReturnsTheExpectedValue()
        {
            // arrange
            const string testString = "This is a test";
            const string propertyPath = "NestedProperty.MyProperty";
            var fixture = new PropertyHelperClass
            {
                NestedProperty = new()
                {
                    MyProperty = testString
                }
            };

            // act
            var result = fixture.GetPropertyInfo(new []{ propertyPath});

            // assert
            result.Count.Should().Be(1);
            result.First().Key.Should().Be(propertyPath);
            result.First().Value.Name.Should().Be(nameof(NestedClass.MyProperty));
        }

        [Fact]
        public void AttemptingToRetrieveANestedPropertyThatDoesNotExistThrowsAnException()
        {
            // arrange
            const string testString = "This is a test";
            const string propertyPath = "NestedProperty.NoProperty";
            var fixture = new PropertyHelperClass
            {
                NestedProperty = new()
                {
                    MyProperty = testString
                }
            };

            var act = new Action(() => fixture.GetPropertyInfo(new[] {propertyPath}));

            // act
            // assert
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void RetrievingNonNestedPropertyInfoReturnsTheExpectedValue()
        {
            // arrange
            const string testString = "This is a test";
            const string propertyPath = "TopProperty";
            var fixture = new PropertyHelperClass
            {
                NestedProperty = new()
                {
                    MyProperty = testString
                }
            };

            // act
            var result = fixture.GetPropertyInfo(new[] { propertyPath });

            // assert
            result.Count.Should().Be(1);
            result.First().Key.Should().Be(propertyPath);
            result.First().Value.Name.Should().Be(nameof(PropertyHelperClass.TopProperty));
        }

        [Fact]
        public void AttemptingToRetrieveAPropertyThatDoesNotExistThrowsAnException()
        {
            // arrange
            const string testString = "This is a test";
            const string propertyPath = "NoProperty";
            var fixture = new PropertyHelperClass
            {
                NestedProperty = new()
                {
                    MyProperty = testString
                }
            };

            var act = new Action(() => fixture.GetPropertyInfo(new[] {propertyPath}));

            // act
            // assert
            act.Should().Throw<InvalidOperationException>();
        }
    }
}