using JacksonVeroneze.NET.Cache.Interfaces;
using JacksonVeroneze.NET.Cache.Services;
using JacksonVeroneze.NET.Cache.Util;
using JacksonVeroneze.NET.Cache.Util.Builders;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace JacksonVeroneze.NET.Cache.UnitTests.Services;

[ExcludeFromCodeCoverage]
public class CacheServiceTests
{
    private readonly Mock<ILogger<CacheService>> _mockLogger;
    private readonly Mock<IDistributedCache> _mockDistributedCache;

    private readonly CacheService _service;

    public CacheServiceTests()
    {
        _mockLogger = new Mock<ILogger<CacheService>>();

        _mockLogger
            .Setup(mock => mock.IsEnabled(LogLevel.Debug))
            .Returns(true);

        _mockDistributedCache = new Mock<IDistributedCache>();

        _service = new CacheService(
            _mockLogger.Object,
            _mockDistributedCache.Object);

        _service.WithPrefixKey("prefix");
    }

    #region WithPrefixKey

    [Theory(DisplayName = nameof(CacheService)
                          + nameof(CacheService.WithPrefixKey)
                          + "must return success")]
    [InlineData("a")]
    [InlineData("_prefix_")]
    [InlineData("_prefix_1_")]
    public void WithPrefixKey_ReturnSuccess(string prefix)
    {
        // -------------------------------------------------------
        // Arrange && Act
        // -------------------------------------------------------
        ICacheService result = _service.WithPrefixKey(prefix);

        // -------------------------------------------------------
        // Assert
        // -------------------------------------------------------
        result.Should()
            .NotBeNull();
    }

    [Theory(DisplayName = nameof(CacheService)
                          + nameof(CacheService.WithPrefixKey)
                          + "EmptyOrNull Throw ArgumentException")]
    [InlineData("")]
    [InlineData(null)]
    public void WithPrefixKey_EmptyOrNull_ThrowArgumentException(
        string prefix)
    {
        // -------------------------------------------------------
        // Arrange && Act
        // -------------------------------------------------------
        Action action = () => _service
            .WithPrefixKey(prefix);

        // -------------------------------------------------------
        // Assert
        // -------------------------------------------------------
        action.Should()
            .Throw<ArgumentException>();
    }

    #endregion

    #region GetAsync

    [Theory(DisplayName = nameof(CacheService)
                          + nameof(CacheService.GetAsync)
                          + "Argument EmptyOrNull - Throw ArgumentException")]
    [InlineData("")]
    [InlineData(null)]
    public async Task GetAsync_ArgumentEmptyOrNull_ThrowArgumentException(
        string key)
    {
        // -------------------------------------------------------
        // Arrange && Act
        // -------------------------------------------------------
        Func<Task> action = async () =>
            await _service.GetAsync<User>(key);

        // -------------------------------------------------------
        // Assert
        // -------------------------------------------------------
        await action.Should()
            .ThrowAsync<ArgumentException>();
    }

    [Fact(DisplayName = nameof(CacheService)
                        + nameof(CacheService.GetAsync)
                        + "not found in cache - return null")]
    public async Task GetAsync_NotFound_ReturnNull()
    {
        // -------------------------------------------------------
        // Arrange
        // -------------------------------------------------------
        const string key = "cache_key";

        byte[]? expected = null as byte[];

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
        User? result = await _service.GetAsync<User>(key);

        // -------------------------------------------------------
        // Assert
        // -------------------------------------------------------
        result.Should()
            .BeNull();

        _mockDistributedCache.Verify(mock =>
            mock.GetAsync(It.IsAny<string>(),
                It.IsAny<CancellationToken>()), Times.Once);

        _mockLogger.Verify(nameof(CacheService.GetAsync),
            times: Times.Once, expectedLogLevel: LogLevel.Debug);
    }

    [Fact(DisplayName = nameof(CacheService)
                        + nameof(CacheService.GetAsync)
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
        User? result = await _service.GetAsync<User>(key);

        // -------------------------------------------------------
        // Assert
        // -------------------------------------------------------
        result.Should()
            .NotBeNull()
            .And.BeEquivalentTo(user);

        _mockDistributedCache.Verify(mock =>
            mock.GetAsync(It.IsAny<string>(),
                It.IsAny<CancellationToken>()), Times.Once);

        _mockLogger.Verify(nameof(CacheService.GetAsync),
            times: Times.Once, expectedLogLevel: LogLevel.Debug);
    }

    #endregion

    #region GetOrCreateAsync

    [Fact(DisplayName = nameof(CacheService)
                        + nameof(CacheService.GetOrCreateAsync)
                        + "found in cache - return data")]
    public async Task GetOrCreateAsync_Found_ReturnData()
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

        Func<DistributedCacheEntryOptions, Task<User>> func = options =>
        {
            options.SlidingExpiration = TimeSpan.FromSeconds(5);

            return Task.FromResult(user);
        };

        // -------------------------------------------------------
        // Act
        // -------------------------------------------------------
        User? result = await _service.GetOrCreateAsync(key, func);

        // -------------------------------------------------------
        // Assert
        // -------------------------------------------------------
        result.Should()
            .NotBeNull()
            .And.BeEquivalentTo(user);

        _mockDistributedCache.Verify(mock =>
            mock.GetAsync(It.IsAny<string>(),
                It.IsAny<CancellationToken>()), Times.Once);

        _mockDistributedCache.Verify(mock =>
            mock.SetAsync(It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()), Times.Never);

        _mockLogger.Verify(nameof(CacheService.GetOrCreateAsync),
            times: Times.Once, expectedLogLevel: LogLevel.Debug);
    }

    [Fact(DisplayName = nameof(CacheService)
                        + nameof(CacheService.GetOrCreateAsync)
                        + "not found in cache - retrieve data")]
    public async Task GetOrCreateAsync_NotFoundInCache_RetrieveData()
    {
        // -------------------------------------------------------
        // Arrange
        // -------------------------------------------------------
        const string key = "cache_key";

        User user = UserBuilder.BuildSingle();

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

        Func<DistributedCacheEntryOptions, Task<User>> func = options =>
        {
            options.SlidingExpiration = TimeSpan.FromSeconds(5);

            return Task.FromResult(user);
        };

        // -------------------------------------------------------
        // Act
        // -------------------------------------------------------
        User? result = await _service.GetOrCreateAsync(key, func);

        // -------------------------------------------------------
        // Assert
        // -------------------------------------------------------
        result.Should()
            .NotBeNull()
            .And.BeEquivalentTo(user);

        _mockDistributedCache.Verify(mock =>
            mock.GetAsync(It.IsAny<string>(),
                It.IsAny<CancellationToken>()), Times.Once);

        _mockDistributedCache.Verify(mock =>
            mock.SetAsync(It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);

        _mockLogger.Verify(nameof(CacheService.GetOrCreateAsync),
            times: Times.Once, expectedLogLevel: LogLevel.Debug);
    }

    #endregion

    #region RemoveAsync

    [Theory(DisplayName = nameof(CacheService)
                          + nameof(CacheService.RemoveAsync)
                          + "Argument EmptyOrNull - Throw ArgumentException")]
    [InlineData("")]
    [InlineData(null)]
    public async Task RemoveAsync_ArgumentEmptyOrNull_ThrowArgumentException(
        string key)
    {
        // -------------------------------------------------------
        // Arrange && Act
        // -------------------------------------------------------
        Func<Task> action = async () =>
            await _service.GetAsync<User>(key);

        // -------------------------------------------------------
        // Assert
        // -------------------------------------------------------
        await action.Should()
            .ThrowAsync<ArgumentException>();
    }

    [Fact(DisplayName = nameof(CacheService)
                        + nameof(CacheService.RemoveAsync)
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
        await _service.RemoveAsync(key);

        // -------------------------------------------------------
        // Assert
        // -------------------------------------------------------
        _mockDistributedCache.Verify(mock =>
            mock.RemoveAsync(It.IsAny<string>(),
                It.IsAny<CancellationToken>()), Times.Once);

        _mockLogger.Verify(nameof(CacheService.RemoveAsync),
            times: Times.Once, expectedLogLevel: LogLevel.Debug);
    }

    #endregion

    #region SetAsync

    [Fact(DisplayName = nameof(CacheService)
                        + nameof(CacheService.SetAsync)
                        + "set success")]
    public async Task SetAsync_SetSuccess()
    {
        // -------------------------------------------------------
        // Arrange
        // -------------------------------------------------------
        const string key = "cache_key";

        User user = UserBuilder.BuildSingle();

        Action<DistributedCacheEntryOptions> action = options =>
        {
            options.AbsoluteExpirationRelativeToNow =
                TimeSpan.FromSeconds(10);
        };

        // -------------------------------------------------------
        // Act
        // -------------------------------------------------------
        await _service.SetAsync(key, user, action);

        // -------------------------------------------------------
        // Assert
        // -------------------------------------------------------
        _mockDistributedCache.Verify(mock =>
            mock.SetAsync(It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);

        _mockLogger.Verify(nameof(CacheService.SetAsync),
            times: Times.Once, expectedLogLevel: LogLevel.Debug);
    }

    #endregion
}