### О порядке доступа к элементам массива

Пусть у нас есть такой код:

```c#
public class C {
    
    int[] _array = { 1, 2, 3 };
    
    public void M() {
        _array[2] = 1;
        _array[1] = 1;
        _array[0] = 1;
    }

    public void N() {
        _array[0] = 1;
        _array[1] = 1;
        _array[2] = 1;
    }
}
```

Внезапно оказывается, что метод <code>N</code> будет работать чуть-чуть медленнее только из-за того, что JIT-компилятор вставит на две проверки выхода за границы массива больше:

```
; Core CLR v5.0.721.25508 on x86

C.M()
    L0000: mov eax, [ecx+4]
    L0003: mov edx, eax
    L0005: mov ecx, [edx+4]
    L0008: cmp ecx, 2
    L000b: jbe short L0025
    L000d: mov dword ptr [edx+0x10], 1
    L0014: mov edx, eax
    L0016: mov dword ptr [edx+0xc], 1
    L001d: mov dword ptr [eax+8], 1
    L0024: ret
    L0025: call 0x72311070
    L002a: int3

C.N()
    L0000: mov eax, [ecx+4]
    L0003: mov edx, eax
    L0005: mov ecx, [edx+4]
    L0008: cmp ecx, 0
    L000b: jbe short L002f
    L000d: mov dword ptr [edx+8], 1
    L0014: mov edx, eax
    L0016: cmp ecx, 1
    L0019: jbe short L002f
    L001b: mov dword ptr [edx+0xc], 1
    L0022: cmp ecx, 2
    L0025: jbe short L002f
    L0027: mov dword ptr [eax+0x10], 1
    L002e: ret
    L002f: call 0x72311070
    L0034: int3
```
