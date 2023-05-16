using JacksonVeroneze.NET.Cache.Interfaces;
using JacksonVeroneze.NET.Cache.Models;
using JacksonVeroneze.NET.Cache.Util;
using JacksonVeroneze.NET.Cache.Util.Builders;
using JacksonVeroneze.NET.DistributedCache.Adapters;
using Microsoft.Extensions.Caching.Distributed;

namespace JacksonVeroneze.NET.Cache.DistributedCache.UnitTests.Adapters;

[ExcludeFromCodeCoverage]
public class DistributedCacheAdapterTests
{
    private readonly Mock<IDistributedCache> _mockDistributedCache;

    private readonly ICacheAdapter _adapter;

    public DistributedCacheAdapterTests()
    {
        _mockDistributedCache = new Mock<IDistributedCache>();

        _adapter = new DistributedCacheAdapter(
            _mockDistributedCache.Object);
    }

    #region GetAsync

    [Fact(DisplayName = nameof(DistributedCacheAdapter)
                        + nameof(DistributedCacheAdapter.GetAsync)
                        + "GetAsync - PrimitiveType - not found in cache - return null")]
    public async Task GetAsync_PrimitiveType_NotFound_ReturnNull()
    {
        // -------------------------------------------------------
        // Arrange
        // -------------------------------------------------------
        const string key = "cache_key";

        byte[]? expected = null;

        _mockDistributedCache.Setup(mock =>
                mock.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .Callback((string keyCb, CancellationToken _) =>
            {
                keyCb.Should()
                    .NotBeEmpty()
                    .And.Contain(key);
            })
            .ReturnsAsync(expected);

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

    [Fact(DisplayName = nameof(DistributedCacheAdapter)
                        + nameof(DistributedCacheAdapter.GetAsync)
                        + "not found in cache - return null")]
    public async Task GetAsync_NotFound_ReturnNull()
    {
        // -------------------------------------------------------
        // Arrange
        // -------------------------------------------------------
        const string key = "cache_key";

        byte[]? expected = null;

        _mockDistributedCache.Setup(mock =>
                mock.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .Callback((string keyCb, CancellationToken _) =>
            {
                keyCb.Should()
                    .NotBeEmpty()
                    .And.Contain(key);
            })
            .ReturnsAsync(expected);

        // -------------------------------------------------------
        // Act
        // -------------------------------------------------------
        User? result = await _adapter.GetAsync<User>(key);

        // -------------------------------------------------------
        // Assert
        // -------------------------------------------------------
        result.Should()
            .BeNull();

        _mockDistributedCache.Verify(mock =>
            mock.GetAsync(It.IsAny<string>(),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = nameof(DistributedCacheAdapter)
                        + nameof(DistributedCacheAdapter.GetAsync)
                        + "found in cache - return data")]
    public async Task GetAsync_Found_ReturnData()
    {
        // -------------------------------------------------------
        // Arrange
        // -------------------------------------------------------
        const string key = "cache_key";

        User user = UserBuilder.BuildSingle();
        byte[] expected = UserDataBuilder.BuildSingle(user);

        _mockDistributedCache.Setup(mock =>
                mock.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .Callback((string keyCb, CancellationToken _) =>
            {
                keyCb.Should()
                    .NotBeEmpty()
                    .And.Contain(key);
            })
            .ReturnsAsync(expected);

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

        _mockDistributedCache.Verify(mock =>
            mock.GetAsync(It.IsAny<string>(),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region RemoveAsync

    [Fact(DisplayName = nameof(DistributedCacheAdapter)
                        + nameof(DistributedCacheAdapter.RemoveAsync)
                        + "remove success")]
    public async Task RemoveAsync_RemoveSuccess()
    {
        // -------------------------------------------------------
        // Arrange
        // -------------------------------------------------------
        const string key = "cache_key";

        // -------------------------------------------------------
        // Act
        // -------------------------------------------------------
        await _adapter.RemoveAsync(key);

        // -------------------------------------------------------
        // Assert
        // -------------------------------------------------------
        _mockDistributedCache.Verify(mock =>
            mock.RemoveAsync(It.IsAny<string>(),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region SetAsync

    [Fact(DisplayName = nameof(DistributedCacheAdapter)
                        + nameof(DistributedCacheAdapter.SetAsync)
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
        _mockDistributedCache.Verify(mock =>
            mock.SetAsync(It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion
}