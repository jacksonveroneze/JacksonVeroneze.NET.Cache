using JacksonVeroneze.NET.BarrelCache.Adapters;
using JacksonVeroneze.NET.Cache.Interfaces;
using JacksonVeroneze.NET.Cache.Models;
using JacksonVeroneze.NET.Cache.Util;
using JacksonVeroneze.NET.Cache.Util.Builders;

namespace JacksonVeroneze.NET.Cache.BarrelCache.UnitTests.Adapters;

[ExcludeFromCodeCoverage]
public class BarrelAdapterTests
{
    private readonly ICacheAdapter _adapter;

    public BarrelAdapterTests()
    {
        _adapter = new BarrelAdapter();
    }

    #region GetAsync

    [Fact(DisplayName = nameof(BarrelAdapter)
                        + nameof(BarrelAdapter.GetAsync)
                        + "GetAsync - PrimitiveType - not found in cache - return null")]
    public async Task GetAsync_PrimitiveType_NotFound_ReturnNull()
    {
        // -------------------------------------------------------
        // Arrange
        // -------------------------------------------------------
        const string key = "cache_key";

        // -------------------------------------------------------
        // Act
        // -------------------------------------------------------
        bool? result = await _adapter.GetAsync<bool?>(key);

        // -------------------------------------------------------
        // Assert
        // -------------------------------------------------------
        result.Should()
            .BeNull();
    }

    [Fact(DisplayName = nameof(BarrelAdapter)
                        + nameof(BarrelAdapter.GetAsync)
                        + "not found in cache - return null")]
    public async Task GetAsync_NotFound_ReturnNull()
    {
        // -------------------------------------------------------
        // Arrange
        // -------------------------------------------------------
        const string key = "cache_key";

        // -------------------------------------------------------
        // Act
        // -------------------------------------------------------
        User? result = await _adapter.GetAsync<User>(key);

        // -------------------------------------------------------
        // Assert
        // -------------------------------------------------------
        result.Should()
            .BeNull();
    }

    [Fact(DisplayName = nameof(BarrelAdapter)
                        + nameof(BarrelAdapter.GetAsync)
                        + "found in cache - return data")]
    public async Task GetAsync_Found_ReturnData()
    {
        // -------------------------------------------------------
        // Arrange
        // -------------------------------------------------------
        const string key = "cache_key";

        User user = UserBuilder.BuildSingle();

        await _adapter.SetAsync(key, user, new CacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10)
        });

        // -------------------------------------------------------
        // Act
        // -------------------------------------------------------
        User? result = await _adapter.GetAsync<User>(key);

        // -------------------------------------------------------
        // Assert
        // -------------------------------------------------------
        result.Should()
            .NotBeNull()
            .And.BeEquivalentTo(user);
    }

    #endregion

    #region RemoveAsync

    [Fact(DisplayName = nameof(BarrelAdapter)
                        + nameof(BarrelAdapter.RemoveAsync)
                        + "remove success")]
    public async Task RemoveAsync_RemoveSuccess()
    {
        // -------------------------------------------------------
        // Arrange
        // -------------------------------------------------------
        const string key = "cache_key";

        User user = UserBuilder.BuildSingle();

        await _adapter.SetAsync(key, user, new CacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10)
        });

        // -------------------------------------------------------
        // Act
        // -------------------------------------------------------
        await _adapter.RemoveAsync(key);

        // -------------------------------------------------------
        // Assert
        // -------------------------------------------------------
        User? verifyUser = await _adapter.GetAsync<User>(key);

        verifyUser.Should().BeNull();
    }

    #endregion

    #region SetAsync

    [Fact(DisplayName = nameof(BarrelAdapter)
                        + nameof(BarrelAdapter.SetAsync)
                        + "set success")]
    public async Task SetAsync_SetSuccess()
    {
        // -------------------------------------------------------
        // Arrange
        // -------------------------------------------------------
        const string key = "cache_key";

        User user = UserBuilder.BuildSingle();

        CacheEntryOptions options = new()
        {
            AbsoluteExpirationRelativeToNow =
                TimeSpan.FromSeconds(10)
        };

        // -------------------------------------------------------
        // Act
        // -------------------------------------------------------
        await _adapter.SetAsync(key, user, options);

        // -------------------------------------------------------
        // Assert
        // -------------------------------------------------------
        User? verifyUser = await _adapter.GetAsync<User>(key);

        verifyUser.Should().NotBeNull();
    }

    #endregion
}