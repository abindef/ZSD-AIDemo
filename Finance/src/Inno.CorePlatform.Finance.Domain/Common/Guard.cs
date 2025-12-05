namespace Inno.CorePlatform.Finance.Domain.Common;

/// <summary>
/// 参数校验守卫
/// </summary>
public static class Guard
{
    public static class Against
    {
        public static T Null<T>(T? value, string parameterName) where T : class
        {
            if (value is null)
                throw new ArgumentNullException(parameterName);
            return value;
        }

        public static T? NullValue<T>(T? value, string parameterName) where T : struct
        {
            if (!value.HasValue)
                throw new ArgumentNullException(parameterName);
            return value;
        }

        public static string NullOrEmpty(string? value, string parameterName)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("Value cannot be null or empty.", parameterName);
            return value;
        }

        public static string NullOrWhiteSpace(string? value, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be null or whitespace.", parameterName);
            return value;
        }

        public static decimal Negative(decimal value, string parameterName)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(parameterName, "Value cannot be negative.");
            return value;
        }

        public static int Negative(int value, string parameterName)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(parameterName, "Value cannot be negative.");
            return value;
        }

        public static int OutOfRange(int value, string parameterName, int min, int max)
        {
            if (value < min || value > max)
                throw new ArgumentOutOfRangeException(parameterName, $"Value must be between {min} and {max}.");
            return value;
        }

        public static decimal OutOfRange(decimal value, string parameterName, decimal min, decimal max)
        {
            if (value < min || value > max)
                throw new ArgumentOutOfRangeException(parameterName, $"Value must be between {min} and {max}.");
            return value;
        }
    }
}
