using System;
using System.Collections.Generic;
using System.Text;

namespace ProjetoT.Security.Domain.Entities.OAuth
{
    public struct LimitCounter
    {
        public readonly DateTimeOffset FirstCallTimestamp;
        public readonly TimeSpan Window;
        public readonly int CallCount;
        public readonly int? Limit;

        public LimitCounter(DateTimeOffset firstCallTimestamp, TimeSpan window, int callCount, int? limit)
        {
            FirstCallTimestamp = firstCallTimestamp;
            CallCount = callCount;
            Limit = limit;
            Window = window;
        }
    }
}
