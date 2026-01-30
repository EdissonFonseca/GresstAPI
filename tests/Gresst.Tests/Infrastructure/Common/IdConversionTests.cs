using FluentAssertions;
using Gresst.Infrastructure.Common;
using Xunit;

namespace Gresst.Tests.Infrastructure.Common;

/// <summary>
/// Unit tests for IdConversion utility class (string and long for domain/DB IDs).
/// </summary>
public class IdConversionTests
{
    [Fact]
    public void ToLongFromString_WhenIdIsNull_ReturnsZero()
    {
        var result = IdConversion.ToLongFromString(null);
        result.Should().Be(0);
    }

    [Fact]
    public void ToLongFromString_WhenIdIsEmpty_ReturnsZero()
    {
        var result = IdConversion.ToLongFromString(string.Empty);
        result.Should().Be(0);
    }

    [Fact]
    public void ToLongFromString_WhenIdIsNumeric_ReturnsParsedLong()
    {
        var result = IdConversion.ToLongFromString("123456789");
        result.Should().Be(123456789L);
    }

    [Fact]
    public void ToLongFromString_WhenIdIsValidGuid_ReturnsPositiveLong()
    {
        var guidString = Guid.NewGuid().ToString();
        var result = IdConversion.ToLongFromString(guidString);
        result.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public void ToStringFromLong_WhenIdIsZero_ReturnsZeroString()
    {
        var result = IdConversion.ToStringFromLong(0);
        result.Should().Be("0");
    }

    [Fact]
    public void ToStringFromLong_WhenIdIsPositive_ReturnsString()
    {
        var result = IdConversion.ToStringFromLong(123456789L);
        result.Should().Be("123456789");
    }

    [Fact]
    public void ToStringFromLong_ThenToLongFromString_RoundTrips()
    {
        var original = 987654321L;
        var str = IdConversion.ToStringFromLong(original);
        var back = IdConversion.ToLongFromString(str);
        back.Should().Be(original);
    }

    [Fact]
    public void ToLongFromString_WhenIdIsNumeric_ThenToStringFromLong_RoundTrips()
    {
        var original = "42";
        var l = IdConversion.ToLongFromString(original);
        var str = IdConversion.ToStringFromLong(l);
        str.Should().Be(original);
    }
}
