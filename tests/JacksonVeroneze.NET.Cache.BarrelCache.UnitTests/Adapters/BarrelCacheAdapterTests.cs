using JacksonVeroneze.NET.BarrelCache.Adapters;
using JacksonVeroneze.NET.Cache.Models;
using JacksonVeroneze.NET.Cache.Util;
using JacksonVeroneze.NET.Cache.Util.Builders;
using MonkeyCache.FileStore;

namespace JacksonVeroneze.NET.Cache.BarrelCache.UnitTests.Adapters;

[ExcludeFromCodeCoverage]
public class BarrelCacheAdapterTests
{
    private readonly BarrelCacheAdapter _cacheAdapter;

    public BarrelCacheAdapterTests()
    {
        _cacheAdapter = new BarrelCacheAdapter();

        Barrel.Current.EmptyAll();
    }

    #region GetAsync

    [Fact(DisplayName = nameof(BarrelCacheAdapter)
                        + nameof(BarrelCacheAdapter.GetAsync)
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
        bool? result = await _cacheAdapter.GetAsync<bool?>(key);

        // -------------------------------------------------------
        // Assert
        // -------------------------------------------------------
        result.Should()
            .BeNull();
    }

    [Fact(DisplayName = nameof(BarrelCacheAdapter)
                        + nameof(BarrelCacheAdapter.GetAsync)
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
        User? result = await _cacheAdapter.GetAsync<User>(key);

        // -------------------------------------------------------
        // Assert
        // -------------------------------------------------------
        result.Should()
            .BeNull();
    }

    [Fact(DisplayName = nameof(BarrelCacheAdapter)
                        + nameof(BarrelCacheAdapter.GetAsync)
                        + "found in cache - return data")]
    public async Task GetAsync_Found_ReturnData()
    {
        // -------------------------------------------------------
        // Arrange
        // -------------------------------------------------------
        const string key = "cache_key";

        User user = UserBuilder.BuildSingle();

        await _cacheAdapter.SetAsync(key, user, new CacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10)
        });

        // -------------------------------------------------------
        // Act
        // -------------------------------------------------------
        User? result = await _cacheAdapter.GetAsync<User>(key);

        // -------------------------------------------------------
        // Assert
        // -------------------------------------------------------
        result.Should()
            .NotBeNull()
            .And.BeEquivalentTo(user);
    }

    #endregion

    #region RemoveAsync

    [Fact(DisplayName = nameof(BarrelCacheAdapter)
                        + nameof(BarrelCacheAdapter.RemoveAsync)
                        + "remove success")]
    public async Task RemoveAsync_RemoveSuccess()
    {
        // -------------------------------------------------------
        // Arrange
        // -------------------------------------------------------
        const string key = "cache_key";

        User user = UserBuilder.BuildSingle();

        await _cacheAdapter.SetAsync(key, user, new CacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10)
        });

        // -------------------------------------------------------
        // Act
        // -------------------------------------------------------
        await _cacheAdapter.RemoveAsync(key);

        // -------------------------------------------------------
        // Assert
        // -------------------------------------------------------
        User? verifyUser = await _cacheAdapter.GetAsync<User>(key);

        verifyUser.Should().BeNull();
    }

    #endregion

    #region SetAsync

    [Fact(DisplayName = nameof(BarrelCacheAdapter)
                        + nameof(BarrelCacheAdapter.SetAsync)
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
        await _cacheAdapter.SetAsync(key, user, options);

        // -------------------------------------------------------
        // Assert
        // -------------------------------------------------------
        User? verifyUser = await _cacheAdapter.GetAsync<User>(key);

        verifyUser.Should().NotBeNull();
    }

    #endregion
}