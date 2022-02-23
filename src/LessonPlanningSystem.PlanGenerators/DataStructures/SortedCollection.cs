using System.Collections;

namespace LessonPlanningSystem.PlanGenerators.DataStructures;

public class SortedCollection<TKey, TValue> : ICollection<KeyValuePair<TKey,TValue>> where TKey : notnull
{
    private readonly SortedDictionary<TKey, List<TValue>> _sortedDictionary = new();
    
    public ICollection<TKey> Keys => _sortedDictionary.Keys;
    public ICollection<TValue> Values => _sortedDictionary.Values.SelectMany(x => x).ToList();
    public int Count => ValueCount;
    public int KeyCount => _sortedDictionary.Count;
    public int ValueCount => _sortedDictionary.Values.Sum(x => x.Count);
    public bool IsReadOnly => false;
    
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public void Add(TKey key, TValue value)
    {
        if (_sortedDictionary.ContainsKey(key)) _sortedDictionary[key].Add(value);
        else _sortedDictionary.Add(key, new List<TValue> { value });
    }

    public bool ContainsKey(TKey key)
    {
        return _sortedDictionary.ContainsKey(key);
    }

    public bool Remove(TKey key)
    {
        return _sortedDictionary.Remove(key);
    }

    public bool TryGetValue(TKey key, out List<TValue> value)
    {
        return _sortedDictionary.TryGetValue(key, out value);
    }

    public List<TValue> this[TKey key] {
        get => _sortedDictionary[key];
        set => _sortedDictionary[key] = value;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    
    public void Add(KeyValuePair<TKey, TValue> item)
    {
        throw new NotImplementedException();
    }

    public void Clear()
    {
        _sortedDictionary.Clear();
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        throw new NotImplementedException();
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        throw new NotImplementedException();
    }
}
