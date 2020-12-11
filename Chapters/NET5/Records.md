### Записи

#### Под капотом

Вбиваем в https://sharplab.io следующую программку (не забыть переключиться на последнюю версию Roslyn!):

```c#
public record C (string FirstName, string LastName) 
{
    public string FullName => FirstName + " " + LastName;
}
 
namespace System.Runtime.CompilerServices
{
    public class IsExternalInit{}
}
```

и вот что нам выдаёт компилятор:

```c#
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Permissions;
using System.Text;
 
[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.Default 
   | DebuggableAttribute.DebuggingModes.DisableOptimizations 
   | DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints 
   | DebuggableAttribute.DebuggingModes.EnableEditAndContinue)]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
[assembly: AssemblyVersion("0.0.0.0")]
[module: UnverifiableCode]
namespace Microsoft.CodeAnalysis
{
    [CompilerGenerated]
    [Embedded]
    internal sealed class EmbeddedAttribute : Attribute
    {
    }
}
 
namespace System.Runtime.CompilerServices
{
    [CompilerGenerated]
    [Microsoft.CodeAnalysis.Embedded]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property 
      | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Parameter 
      | AttributeTargets.ReturnValue | AttributeTargets.GenericParameter, 
      AllowMultiple = false, Inherited = false)]
    internal sealed class NullableAttribute : Attribute
    {
        public readonly byte[] NullableFlags;
 
        public NullableAttribute(byte P_0)
        {
            byte[] array = new byte[1];
            array[0] = P_0;
            NullableFlags = array;
        }
 
        public NullableAttribute(byte[] P_0)
        {
            NullableFlags = P_0;
        }
    }
    [CompilerGenerated]
    [Microsoft.CodeAnalysis.Embedded]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct 
      | AttributeTargets.Method | AttributeTargets.Interface 
      | AttributeTargets.Delegate, AllowMultiple = false, Inherited = false)]
    internal sealed class NullableContextAttribute : Attribute
    {
        public readonly byte Flag;
 
        public NullableContextAttribute(byte P_0)
        {
            Flag = P_0;
        }
    }
}
 
public class C : IEquatable<C>
{
    [CompilerGenerated]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly string <FirstName>k__BackingField;
 
    [CompilerGenerated]
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly string <LastName>k__BackingField;
 
    [System.Runtime.CompilerServices.Nullable(1)]
    protected virtual Type EqualityContract
    {
        [System.Runtime.CompilerServices.NullableContext(1)]
        [CompilerGenerated]
        get
        {
            return typeof(C);
        }
    }
 
    public string FirstName
    {
        [CompilerGenerated]
        get
        {
            return <FirstName>k__BackingField;
        }
        [CompilerGenerated]
        set
        {
            <FirstName>k__BackingField = value;
        }
    }
 
    public string LastName
    {
        [CompilerGenerated]
        get
        {
            return <LastName>k__BackingField;
        }
        [CompilerGenerated]
        set
        {
            <LastName>k__BackingField = value;
        }
    }
 
    public string FullName
    {
        get
        {
            return FirstName + " " + LastName;
        }
    }
 
    public C(string FirstName, string LastName)
    {
        <FirstName>k__BackingField = FirstName;
        <LastName>k__BackingField = LastName;
        base..ctor();
    }
 
    public override string ToString()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("C");
        stringBuilder.Append(" { ");
        if (PrintMembers(stringBuilder))
        {
            stringBuilder.Append(" ");
        }
        stringBuilder.Append("}");
        return stringBuilder.ToString();
    }
 
    [System.Runtime.CompilerServices.NullableContext(1)]
    protected virtual bool PrintMembers(StringBuilder builder)
    {
        builder.Append("FirstName");
        builder.Append(" = ");
        builder.Append((object)FirstName);
        builder.Append(", ");
        builder.Append("LastName");
        builder.Append(" = ");
        builder.Append((object)LastName);
        builder.Append(", ");
        builder.Append("FullName");
        builder.Append(" = ");
        builder.Append((object)FullName);
        return true;
    }
 
    [System.Runtime.CompilerServices.NullableContext(2)]
    public static bool operator !=(C r1, C r2)
    {
        return !(r1 == r2);
    }
 
    [System.Runtime.CompilerServices.NullableContext(2)]
    public static bool operator ==(C r1, C r2)
    {
        return (object)r1 == r2 || ((object)r1 != null && r1.Equals(r2));
    }
 
    public override int GetHashCode()
    {
        return (EqualityComparer<Type>.Default.GetHashCode(EqualityContract) * -1521134295 
          + EqualityComparer<string>.Default.GetHashCode(<FirstName>k__BackingField)) * -1521134295 
          + EqualityComparer<string>.Default.GetHashCode(<LastName>k__BackingField);
    }
 
    [System.Runtime.CompilerServices.NullableContext(2)]
    public override bool Equals(object obj)
    {
        return Equals(obj as C);
    }
 
    [System.Runtime.CompilerServices.NullableContext(2)]
    public virtual bool Equals(C other)
    {
        return (object)other != null && EqualityContract == other.EqualityContract 
          && EqualityComparer<string>.Default.Equals(<FirstName>k__BackingField, 
            other.<FirstName>k__BackingField) 
          && EqualityComparer<string>.Default.Equals(<LastName>k__BackingField, 
            other.<LastName>k__BackingField);
    }
 
    [System.Runtime.CompilerServices.NullableContext(1)]
    public virtual C <Clone>$()
    {
        return new C(this);
    }
 
    protected C([System.Runtime.CompilerServices.Nullable(1)] C original)
    {
        <FirstName>k__BackingField = original.<FirstName>k__BackingField;
        <LastName>k__BackingField = original.<LastName>k__BackingField;
    }
 
    public void Deconstruct(out string FirstName, out string LastName)
    {
        FirstName = this.FirstName;
        LastName = this.LastName;
    }
}
 
namespace System.Runtime.CompilerServices
{
    public class IsExternalInit
    {
    }
}
```

