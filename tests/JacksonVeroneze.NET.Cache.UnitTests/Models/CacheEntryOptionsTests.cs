using JacksonVeroneze.NET.Cache.Models;

namespace JacksonVeroneze.NET.Cache.UnitTests.Models;

[ExcludeFromCodeCoverage]
public class CacheEntryOptionsTests
{
    #region AbsoluteExpiration

    [Fact(DisplayName = nameof(CacheEntryOptions)
                        + nameof(CacheEntryOptions.AbsoluteExpiration)
                        + "ArgumentOutOfRange")]
    public void AbsoluteExpiration_ArgumentOutOfRange_ThrowException()
    {
        // -------------------------------------------------------
        // Arrange && Act
        // -------------------------------------------------------
        Func<CacheEntryOptions> action = () =>
            new CacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(-10)
            };

        // -------------------------------------------------------
        // Assert
        // -------------------------------------------------------
        action.Should()
            .ThrowExactly<ArgumentOutOfRangeException>();
    }

    #endregion

    #region AbsoluteExpirationRelativeToNow

    [Fact(DisplayName = nameof(CacheEntryOptions)
                        + nameof(CacheEntryOptions.AbsoluteExpiration)
                        + "ArgumentOutOfRange")]
    public void AbsoluteExpirationRelativeToNow_ArgumentOutOfRange_ThrowException()
    {
        // -------------------------------------------------------
        // Arrange && Act
        // -------------------------------------------------------
        Func<CacheEntryOptions> action = () =>
            new CacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.Zero
            };

        // -------------------------------------------------------
        // Assert
        // -------------------------------------------------------
        action.Should()
            .ThrowExactly<ArgumentOutOfRangeException>();
    }

    #endregion

    #region SlidingExpiration

    [Fact(DisplayName = nameof(CacheEntryOptions)
                        + nameof(CacheEntryOptions.AbsoluteExpiration)
                        + "ArgumentOutOfRange")]
    public void SlidingExpiration_ArgumentOutOfRange_ThrowException()
    {
        // -------------------------------------------------------
        // Arrange && Act
        // -------------------------------------------------------
        Func<CacheEntryOptions> action = () =>
            new CacheEntryOptions
            {
                SlidingExpiration = TimeSpan.Zero
            };

        // -------------------------------------------------------
        // Assert
        // -------------------------------------------------------
        action.Should()
            .ThrowExactly<ArgumentOutOfRangeException>();
    }

    #endregion
}