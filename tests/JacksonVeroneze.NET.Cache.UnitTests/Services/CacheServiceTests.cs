using JacksonVeroneze.NET.Cache.Interfaces;
using JacksonVeroneze.NET.Cache.Models;
using JacksonVeroneze.NET.Cache.Services;
using JacksonVeroneze.NET.Cache.Util;
using JacksonVeroneze.NET.Cache.Util.Builders;
using Microsoft.Extensions.Logging;

namespace JacksonVeroneze.NET.Cache.UnitTests.Services;

[ExcludeFromCodeCoverage]
public class CacheServiceTests
{
    private readonly Mock<ILogger<CacheService>> _mockLogger;

    // private readonly Mock<IDistributedCache> _mockDistributedCache;
    private readonly Mock<ICacheAdapter> _mockCacheAdapter;

    private readonly CacheService _service;

    public CacheServiceTests()
    {
        _mockLogger = new Mock<ILogger<CacheService>>();

        _mockLogger
            .Setup(mock => mock.IsEnabled(LogLevel.Debug))
            .Returns(true);

        _mockCacheAdapter = new Mock<ICacheAdapter>();

        _service = new CacheService(
            _mockLogger.Object,
            _mockCacheAdapter.Object);

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
                        + "GetAsync - PrimitiveType - not found in cache - return null")]
    public async Task GetAsync_PrimitiveType_NotFound_ReturnNull()
    {
        // -------------------------------------------------------
        // Arrange
        // -------------------------------------------------------
        const string key = "cache_key";

        bool? expected = null;

        _mockCacheAdapter.Setup(mock =>
                mock.GetAsync<bool?>(
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
        bool? result = await _service.GetAsync<bool?>(key);

        // -------------------------------------------------------
        // Assert
        // -------------------------------------------------------
        result.Should()
            .BeNull();

        _mockCacheAdapter.Verify(mock =>
            mock.GetAsync<bool?>(It.IsAny<string>(),
                It.IsAny<CancellationToken>()), Times.Once);

        _mockLogger.Verify(nameof(CacheService.GetAsync),
            times: Times.Once, expectedLogLevel: LogLevel.Debug);
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

        User? expected = null;

        _mockCacheAdapter.Setup(mock =>
                mock.GetAsync<User>(
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

        _mockCacheAdapter.Verify(mock =>
            mock.GetAsync<User>(It.IsAny<string>(),
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

        User expected = UserBuilder.BuildSingle();

        _mockCacheAdapter.Setup(mock =>
                mock.GetAsync<User>(
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
            .And.BeEquivalentTo(expected);

        _mockCacheAdapter.Verify(mock =>
            mock.GetAsync<User>(It.IsAny<string>(),
                It.IsAny<CancellationToken>()), Times.Once);

        _mockLogger.Verify(nameof(CacheService.GetAsync),
            times: Times.Once, expectedLogLevel: LogLevel.Debug);
    }

    #endregion

    #region GetOrCreateAsync

    [Fact(DisplayName = nameof(CacheService)
                        + nameof(CacheService.GetOrCreateAsync)
                        + "GetOrCreateAsync - PrimitiveType - not found in cache - return null")]
    public async Task GetOrCreateAsync_PrimitiveType_NotFound_ReturnNull()
    {
        // -------------------------------------------------------
        // Arrange
        // -------------------------------------------------------
        const string key = "cache_key";

        bool? expected = null;

        Func<CacheEntryOptions, Task<bool?>> func = options =>
        {
            options.SlidingExpiration = TimeSpan.FromSeconds(5);

            return Task.FromResult(expected);
        };

        _mockCacheAdapter.Setup(mock =>
                mock.GetAsync<bool?>(
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
        bool? result = await _service.GetOrCreateAsync(key, func);

        // -------------------------------------------------------
        // Assert
        // -------------------------------------------------------
        result.Should()
            .BeNull();

        _mockCacheAdapter.Verify(mock =>
            mock.GetAsync<bool?>(It.IsAny<string>(),
                It.IsAny<CancellationToken>()), Times.Once);

        _mockCacheAdapter.Verify(mock =>
            mock.SetAsync(It.IsAny<string>(),
                It.IsAny<bool?>(),
                It.IsAny<CacheEntryOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);

        _mockLogger.Verify(nameof(CacheService.GetOrCreateAsync),
            times: Times.Once, expectedLogLevel: LogLevel.Debug);
    }

    [Fact(DisplayName = nameof(CacheService)
                        + nameof(CacheService.GetOrCreateAsync)
                        + "found in cache - return data")]
    public async Task GetOrCreateAsync_Found_ReturnData()
    {
        // -------------------------------------------------------
        // Arrange
        // -------------------------------------------------------
        const string key = "cache_key";

        User expected = UserBuilder.BuildSingle();

        _mockCacheAdapter.Setup(mock =>
                mock.GetAsync<User>(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .Callback((string keyCb, CancellationToken _) =>
            {
                keyCb.Should()
                    .NotBeEmpty()
                    .And.Contain(key);
            })
            .ReturnsAsync(expected);

        Func<CacheEntryOptions, Task<User>> func = options =>
        {
            options.SlidingExpiration = TimeSpan.FromSeconds(5);

            return Task.FromResult(expected);
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
            .And.BeEquivalentTo(expected);

        _mockCacheAdapter.Verify(mock =>
            mock.GetAsync<User>(It.IsAny<string>(),
                It.IsAny<CancellationToken>()), Times.Once);

        _mockCacheAdapter.Verify(mock =>
            mock.SetAsync(It.IsAny<string>(),
                It.IsAny<User>(),
                It.IsAny<CacheEntryOptions>(),
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
        User? expectedCache = null;

        _mockCacheAdapter.Setup(mock =>
                mock.GetAsync<User>(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .Callback((string keyCb, CancellationToken _) =>
            {
                keyCb.Should()
                    .NotBeEmpty()
                    .And.Contain(key);
            })
            .ReturnsAsync(expectedCache);

        Func<CacheEntryOptions, Task<User>> func = options =>
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

        _mockCacheAdapter.Verify(mock =>
            mock.GetAsync<User>(It.IsAny<string>(),
                It.IsAny<CancellationToken>()), Times.Once);

        _mockCacheAdapter.Verify(mock =>
            mock.SetAsync(It.IsAny<string>(),
                It.IsAny<User>(),
                It.IsAny<CacheEntryOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);

        _mockLogger.Verify(nameof(CacheService.GetOrCreateAsync),
            times: Times.Once, expectedLogLevel: LogLevel.Debug);
    }

    [Theory(DisplayName = nameof(CacheService)
                        + nameof(CacheService.GetOrCreateAsync)
                        + " not found in cache - retrieve data -"
                        + " AllowStoreNullValue option")]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetOrCreateAsync_NotFoundInCache_RetrieveData_AllowStoreNullValue(
        bool allowNullStore)
    {
        // -------------------------------------------------------
        // Arrange
        // -------------------------------------------------------
        const string key = "cache_key";

        User? user = null;
        User? expectedCache = null;

        _mockCacheAdapter.Setup(mock =>
                mock.GetAsync<User>(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .Callback((string keyCb, CancellationToken _) =>
            {
                keyCb.Should()
                    .NotBeEmpty()
                    .And.Contain(key);
            })
            .ReturnsAsync(expectedCache);

        Func<CacheEntryOptions, Task<User?>> func = options =>
        {
            options.AllowStoreNullValue = allowNullStore;
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
            .BeNull();

        _mockCacheAdapter.Verify(mock =>
            mock.GetAsync<User>(It.IsAny<string>(),
                It.IsAny<CancellationToken>()), Times.Once);

        Func<Times> times = allowNullStore ? Times.Once : Times.Never;
        
        _mockCacheAdapter.Verify(mock =>
            mock.SetAsync(It.IsAny<string>(),
                It.IsAny<User>(),
                It.IsAny<CacheEntryOptions>(),
                It.IsAny<CancellationToken>()), times);

        _mockLogger.Verify(nameof(CacheService.GetOrCreateAsync),
            times: times, expectedLogLevel: LogLevel.Debug);
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
        _mockCacheAdapter.Verify(mock =>
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

        Action<CacheEntryOptions> action = options =>
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
        _mockCacheAdapter.Verify(mock =>
            mock.SetAsync(It.IsAny<string>(),
                It.IsAny<User>(),
                It.IsAny<CacheEntryOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);

        _mockLogger.Verify(nameof(CacheService.SetAsync),
            times: Times.Once, expectedLogLevel: LogLevel.Debug);
    }

    #endregion
}