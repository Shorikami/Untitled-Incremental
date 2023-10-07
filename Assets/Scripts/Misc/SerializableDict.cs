using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableDict<TKey, TVal> : Dictionary<TKey, TVal>, ISerializationCallbackReceiver
{
    [SerializeField] private List<TKey> m_Keys = new List<TKey>();
    [SerializeField] private List<TVal> m_Values = new List<TVal>();

    public void OnBeforeSerialize()
    {
        m_Keys.Clear();
        m_Values.Clear();

        foreach (KeyValuePair<TKey, TVal> pair in this)
        {
            m_Keys.Add(pair.Key);
            m_Values.Add(pair.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        Clear();

        if (m_Keys.Count != m_Values.Count)
            Debug.LogError("Key count in SerializableDict does not equal value count!");

        for (int i = 0; i < m_Keys.Count; ++i)
        {
            Add(m_Keys[i], m_Values[i]);
        }
    }
}
