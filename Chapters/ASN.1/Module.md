### Определение модуля

#### Шаблон модуля

```
MY-MODULE ( <oid> )
    DEFINITIONS
    AUTOMATIC TAGS ::=
    BEGIN
    EXPORTS <exports clause>;
    IMPORTS <import clause>;
    <Type and value assignments>
    END
```

#### Типы и значения

```
<TypeReferenceName> ::= <TypeDefinition>
```

например:

```
Age          ::= INTEGER
Country-name ::= UTF8String
Hex-string   ::= OCTET STRING
```

```
<valueReferenceName> <TypeOfValue> ::= <ValueNotation>
```

например:

```
twenty-one-today Age ::= 21
spain Country-name ::= "ESPANA"
```

#### Определение структуры

```
My-sequence ::= SEQUENCE {
    first   BOOLEAN,
    second  INTEGER      OPTIONAL,
    third   INTEGER      DEFAULT 129,
    fourth  BOOLEAN      DEFAULT TRUE,
    fifth   REAL         DEFAULT 0.629,
                 -- or   DEFAULT 62.9E-2
    sixth   UTF8String   DEFAULT "нет значения",
    seventh IA5String    DEFAULT "James Morrison",
    eigth   BIT STRING   DEFAULT '101100011'B,
    ninth   OCTET STRING DEFAULT '89AEF764'H
}
```

#### Еще определения типов

VersionsSupported ::= BIT STRING {
    version1 (0),
    version2 (1),
    version3 (2)
}

Message ::= SEQUENCE {
    ...,
    version-bit-map VersionsSupported DEFAULT { version1 }
}

Color ::= INTEGER {
    red    (10),
    orange (20),
    yellow (30),
    green  (40),
    blue   (50),
    indigo (60),
    violet (70)

} (0..80)

Codes ::= ENUMERATED {
    code1 (0),
    code2 (1),
    code3 (2)
}

#### Ограничения типов

```
INTEGER (0..MAX) -- только неотрицательные значения
INTEGER (-6..3 | 10..30) -- от -6 до 3 включительно либо от 10 до 30
INTEGER (ALL EXCEPT 0) -- ноль запрещен
SEQUENCE (SIZE (0..10)) OF INTEGER
IA5String (SIZE (1..25)) (FROM ("A" .. "Z"))
```

#### Экспорт и импорт

```
EXPORTS TypeA, TypeB, valueC;

IMPORTS TypeA, TypeB, valueC FROM MODULE-A { ... }
        TypeD, TypeE FROM MODULE-B { ... };
```
