using System;

namespace BaselessJumping.Internals
{
    public class SettableKeyValuePair<TKey, TValue>
    {
        public TKey Key { get; set; }
        public TValue Value { get; set; }

        public SettableKeyValuePair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }
}