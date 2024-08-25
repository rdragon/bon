using System.Collections.Generic;

namespace Bon.SourceGeneration
{
    /// <summary>
    /// Keeps track of two chains of objects that are in the process of being compared for equality.
    /// </summary>
    public readonly struct AncestorCollection
    {
        private readonly Dictionary<Ancestor, int> _ancestors;

        public AncestorCollection(int capacity)
        {
            _ancestors = new Dictionary<Ancestor, int>(capacity, AncestorEqualityComparer.Instance);
        }

        /// <summary>
        /// Checks if <paramref name="x"/> can be found in the left chain (by comparing by reference), and if so, returns
        /// whether <paramref name="y"/> is found in the right chain at the same index.
        /// Returns null if <paramref name="x"/> is not found.
        /// </summary>
        public bool? FirstCheck(object x, object y)
        {
            if (_ancestors.TryGetValue(new Ancestor(x, true), out var leftIndex))
            {
                return _ancestors.TryGetValue(new Ancestor(y, false), out var rightIndex) && leftIndex == rightIndex;
            }

            return null;
        }

        /// <summary>
        /// Adds <paramref name="x"/> to the left chain and <paramref name="y"/> to the right chain.
        /// </summary>
        public void Add(object x, object y)
        {
            var index = _ancestors.Count;
            _ancestors[new Ancestor(x, true)] = index;
            _ancestors[new Ancestor(y, false)] = index;
        }

        /// <summary>
        /// Adds <paramref name="x"/> to the left chain.
        /// </summary>
        public void Add(object x)
        {
            _ancestors[new Ancestor(x, true)] = _ancestors.Count;
        }

        /// <summary>
        /// Removes <paramref name="x"/> from the left chain and <paramref name="y"/> from the right chain.
        /// </summary>
        public void Remove(object x, object y)
        {
            _ancestors.Remove(new Ancestor(x, true));
            _ancestors.Remove(new Ancestor(y, false));
        }

        /// <summary>
        /// Removes <paramref name="x"/> from the left chain.
        /// </summary>
        public void Remove(object x)
        {
            _ancestors.Remove(new Ancestor(x, true));
        }

        /// <summary>
        /// If <paramref name="x"/> is found in the left chain, appends the hash code of the corresponding index to
        /// <paramref name="hashCode"/> and returns true.
        /// Returns false if <paramref name="x"/> is not found.
        /// </summary>
        public bool TryAppendHashCode(object x, ref int hashCode)
        {
            if (_ancestors.TryGetValue(new Ancestor(x, true), out var index))
            {
                hashCode = hashCode.AddHashOf(index);

                return true;
            }

            return false;
        }
    }

    public readonly struct Ancestor
    {
        public object Object { get; }
        public bool LeftChain { get; }

        public Ancestor(object obj, bool leftChain)
        {
            Object = obj;
            LeftChain = leftChain;
        }
    }

    public sealed class AncestorEqualityComparer : IEqualityComparer<Ancestor>
    {
        public static AncestorEqualityComparer Instance { get; } = new AncestorEqualityComparer();

        public bool Equals(Ancestor x, Ancestor y) => ReferenceEquals(x.Object, y.Object) && x.LeftChain == y.LeftChain;

        public int GetHashCode(Ancestor obj) => ReferenceEqualityComparer.Instance.GetHashCode(obj.Object) ^ (obj.LeftChain ? 1 : 0);
    }
}
