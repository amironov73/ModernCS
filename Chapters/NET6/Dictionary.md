### Улучшения в Dictionary

A new *unsafe* api — `CollectionsMarshal.GetValueRefOrNullRef` — has been added that makes updating struct values in Dictionaries faster. The new API is intended for high performance scenarios, not for general purpose use. It returns a `ref` to the struct value which can then be updated in place with typical techniques.

Prior to this change, updating `struct` dictionary values can be expensive for high-performance scenarios, requiring a dictionary lookup and a copy to stack of the `struct`; then after changing the `struct`, it needs to be assigned to the dictionary key again resulting in another look up and copy operation. This improvement reduces the key hashing to 1 (from 2) and removes all the struct copy operations.

Example:

```c#
ref MyStruct value = ref CollectionsMarshal.GetValueRefOrNullRef(dictionary, key);

// Returns Unsafe.NullRef<TValue>() if it doesn't exist; check using Unsafe.IsNullRef(ref value)
if (!Unsafe.IsNullRef(ref value))
{
    // Mutate in-place
    value.MyInt++;
}
```

Credit to Ben Adams.
