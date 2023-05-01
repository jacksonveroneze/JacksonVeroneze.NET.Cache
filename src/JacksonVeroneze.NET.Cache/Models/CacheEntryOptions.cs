namespace JacksonVeroneze.NET.Cache.Models;

public class CacheEntryOptions
{
    private DateTimeOffset? _absoluteExpiration;
    private TimeSpan? _absoluteExpirationRelativeToNow;
    private TimeSpan? _slidingExpiration;

    public DateTimeOffset? AbsoluteExpiration
    {
        get => _absoluteExpiration;
        set
        {
            if (value <= DateTimeOffset.Now)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(AbsoluteExpiration),
                    value,
                    "The absolute expiration value must be greater than the moment.");
            }

            _absoluteExpiration = value;
        }
    }

    public TimeSpan? AbsoluteExpirationRelativeToNow
    {
        get => _absoluteExpirationRelativeToNow;
        set
        {
            if (value <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(AbsoluteExpirationRelativeToNow),
                    value,
                    "The relative expiration value must be positive.");
            }

            _absoluteExpirationRelativeToNow = value;
        }
    }

    public TimeSpan? SlidingExpiration
    {
        get => _slidingExpiration;
        set
        {
            if (value <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(SlidingExpiration),
                    value,
                    "The sliding expiration value must be positive.");
            }

            _slidingExpiration = value;
        }
    }
}