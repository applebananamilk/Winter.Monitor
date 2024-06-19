using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Winter.Monitor.HealthChecks;

public class HealthCheckHelper_Tests
{
    public static IEnumerable<object?[]> ToTimeSpanTestData()
    {
        yield return new object?[] { null, null };
        yield return new object?[] { 1, TimeSpan.FromMilliseconds(1) };
        yield return new object?[] { -1, null };
    }

    [Theory]
    [MemberData(nameof(ToTimeSpanTestData))]
    public void ToTimeSpan_Tests(int? milliseconds, TimeSpan? timeSpan)
    {
        timeSpan.ShouldBe(HealthCheckHelper.ToTimeSpan(milliseconds));
    }

    [Theory]
    [InlineData("group1", "name1", "group1@name1")]
    [InlineData("group", "@name", "group@@name")]
    public void CalculateName_ShouldReturnExpectedResult(string groupName, string name, string expectedResult)
    {
        string actualResult = HealthCheckHelper.CalculateHealthCheckName(groupName, name);
        Assert.Equal(expectedResult, actualResult);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void CalculateName_ShouldThrowException_WhenAnyArgumentIsNullOrEmpty(string? nullOrEmpty)
    {
        Assert.Throws<ArgumentException>(() => HealthCheckHelper.CalculateHealthCheckName(nullOrEmpty!, "name"));
        Assert.Throws<ArgumentException>(() => HealthCheckHelper.CalculateHealthCheckName("group", nullOrEmpty!));
    }

    [Theory]
    [InlineData("group1@name1", "group1", "name1")]
    [InlineData("name3", null, "name3")]
    [InlineData("group4@", "group4", "")]
    [InlineData("@name5", "", "name5")]
    public void ResolveHealthCheckName_ShouldReturnCorrectValues(string healthCheckName, string? expectedGroupName, string expectedName)
    {
        // Act
        (string? actualGroupName, string actualName) = HealthCheckHelper.ResolveHealthCheckName(healthCheckName);
        // Assert
        Assert.Equal(expectedGroupName, actualGroupName);
        Assert.Equal(expectedName, actualName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void ResolveHealthCheckName_ShouldThrowException_WhenHealthCheckNameIsNullOrEmpty(string? nullOrEmpty)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => HealthCheckHelper.ResolveHealthCheckName(nullOrEmpty!));
    }
}
