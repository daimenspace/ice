// **********************************************************************
//
// Copyright (c) 2003-2016 ZeroC, Inc. All rights reserved.
//
// This copy of Ice is licensed to you under the terms described in the
// ICE_LICENSE file included in this distribution.
//
// **********************************************************************

package IceInternal;

public class DictionaryPatcher<K, V> implements Ice.ReadObjectCallback
{
    // TBD: Remove _type?
    public DictionaryPatcher(java.util.Map<K, V> dict, Class<V> cls, String type, K key)
    {
        _dict = dict;
        _cls = cls;
        _type = type;
        _key = key;
    }

    public void objectReady(Ice.Object v)
    {
        if(v != null)
        {
            //
            // Raise ClassCastException if the element doesn't match the expected type.
            //
            if(!_cls.isInstance(v))
            {
                throw new ClassCastException("expected element of type " + _cls.getName() + " but received " +
                                             v.getClass().getName());
            }
        }

        _dict.put(_key, _cls.cast(v));
    }

    private java.util.Map<K, V> _dict;
    private Class<V> _cls;
    private String _type;
    private K _key;
}
