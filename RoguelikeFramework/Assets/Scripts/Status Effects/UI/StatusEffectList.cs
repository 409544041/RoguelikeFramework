﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/* A custom object that should, for all intents and purposes, match the way that a List<StatusEffect>
 * works. This is necessary over a list because StatusEffects need some help with their setup, especially
 * when they get created or destroyed by a list. Status Effects are actually just references to SO's in the
 * background, and so the list just duplicates the last one when you add new elements, and leaves the ref
 * hanging when you delete them. This is super annoying to deal with. A property drawer for type
 * List<StatusEffect> would solve the problem, but Unity doesn't let you do that. Hence, we need an 
 * intermediary type in order to get the property drawer to do what we want. 
 * 
 * TLDR: StatusEffects should never be copied, or just deleted. This is what List<T> does for elements,
 * and we can't override the UI in the convenient way, so we need to use this object to do it for us.
 */

[Serializable]
public class StatusEffectList //: ICollection
{
    public List<StatusEffect> list;

    public StatusEffect this[int index]
    {
        get { return list[index]; }
        set { list[index] = value; }
    }

    public void Add(StatusEffect item)
    {
        list.Add(item);
    }

    public void Remove(StatusEffect item)
    {
        list.Remove(item);
    }

    public void RemoveAt(int index)
    {
        list.RemoveAt(index);
    }

    public int Count
    {
        get { return list.Count; }
    }
    /*
    void ICollection.CopyTo(Array array, int index)
    {
        int count = index;
        foreach (StatusEffect stat in list)
        {
            array.SetValue(stat, count);
            count++;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return list.GetEnumerator();
    }

    bool ICollection.IsSynchronized
    {
        get
        {
            return false;
        }
    }

    object ICollection.SyncRoot
    {
        get
        {
            return this;
        }
    }

    // The Count read-only property returns the number
    // of items in the collection. - Microsoft docs
    //https://i.kym-cdn.com/entries/icons/original/000/031/254/cover3.jpg
    int ICollection.Count
    {
        get
        {
            return list.Count;
        }
    }*/
}

//This works great in code, doesn't jibe with the editor. Sadge.

/*public class StatusEffectList : List<StatusEffect>
{

}*/