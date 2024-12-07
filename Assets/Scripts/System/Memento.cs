using System;
using UnityEngine;

namespace Santa
{
    [System.Serializable]
    public struct Memento
    {
        [NonSerialized] public IMemento owner;
        public MementoData data;

        public Memento(IMemento owner)
        {
            this.owner = owner;
            this.data = default;
        }
    }

    public interface IMemento
    {
        public GameObject gameObject { get; }
        public int UUID
        {
            get
            {
                unchecked
                {
                    int hash = gameObject.name.GetHashCode();
                    var t = gameObject.transform;
                    if (t.parent)
                        hash = hash * 23 + t.parent.gameObject.name.GetHashCode();
                    hash = hash * 97 + t.GetSiblingIndex();
                    return hash;
                }
            }
        }

        public void SetData(MementoData data);
        public MementoData GetData();
    }

    [Serializable]
    public struct MementoData
    {
        [NonSerialized] public bool active;
        public int a;
        public int b;

        public MementoData(int a, int b)
        {
            this.a = a;
            this.b = b;
            active = true;
        }

        public MementoData(int a)
        {
            this.a = a;
            this.b = 0;
            active = true;
        }

        public MementoData(bool a, bool b)
        {
            this.a = a ? 1 : 0;
            this.b = b ? 1 : 0;
            active = true;
        }

        public MementoData(bool a)
        {
            this.a = a?1:0;
            this.b = 0;
            active = true;
        }
    }
}
