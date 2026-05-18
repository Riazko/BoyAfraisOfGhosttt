using System;

namespace BoyAfraidOfGhosts.Helpers
{
    public struct Optional<T>
    {
        private readonly T value;

        public bool HasValue { get; }

        public T Value
        {
            get
            {
                if (!HasValue)
                    throw new InvalidOperationException("Optional does not contain a value.");
                return value;
            }
        }

        private Optional(T value)
        {
            this.value = value;
            HasValue = true;
        }

        public static Optional<T> Some(T value)
        {
            if (value == null)
                return None();
            return new Optional<T>(value);
        }

        public static Optional<T> None()
        {
            return new Optional<T>();
        }

        public T GetValueOrDefault(T defaultValue)
        {
            return HasValue ? value : defaultValue;
        }

        public Optional<TResult> Map<TResult>(Func<T, TResult> mapper)
        {
            if (!HasValue)
                return Optional<TResult>.None();
            return Optional<TResult>.Some(mapper(value));
        }

        public TResult Match<TResult>(Func<T, TResult> some, Func<TResult> none)
        {
            return HasValue ? some(value) : none();
        }

        public void IfSome(Action<T> action)
        {
            if (HasValue)
                action(value);
        }
    }
}