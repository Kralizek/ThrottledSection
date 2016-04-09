using System;

namespace Kralizek.ThrottledSection
{
    public interface IClock
    {
        DateTimeOffset Now { get; }
    }

    internal class StandardClock : IClock
    {
        public DateTimeOffset Now => DateTimeOffset.Now;

        public static readonly IClock Default = new StandardClock();
    }
}