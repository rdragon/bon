using Microsoft.CodeAnalysis;

namespace Bon.SourceGeneration
{
    internal sealed class Maybe<T>
    {
        public bool HasData { get; }

        public T Data { get; }

        public Diagnostic Diagnostic { get; }

        public Maybe(T data)
        {
            HasData = true;
            Data = data;
        }

        public Maybe(Diagnostic diagnostic)
        {
            HasData = false;
            Diagnostic = diagnostic;
        }

        public override bool Equals(object obj) =>
            HasData &&
            obj is Maybe<T> other &&
            other.HasData &&
            Data.Equals(other.Data);

        public override int GetHashCode() => HasData ? Data.GetHashCode() : 0;
    }
}
