﻿# ASN.1

ASN.1 (англ. Abstract Syntax Notation One) — в области телекоммуникаций и компьютерных сетей язык для описания абстрактного синтаксиса данных (ASN.1), используемый OSI. Стандарт записи, описывающий структуры данных для представления, кодирования, передачи и декодирования данных. Он обеспечивает набор формальных правил для описания структуры объектов, которые не зависят от конкретной машины. ASN.1 является ISO- и ITU-T-совместимым стандартом, первоначально был определён в 1984 году в рамках CCITT X.409:1984. Из-за широкого применения ASN.1 в 1988 году перешёл в свой собственный стандарт X.208. Начиная с 1995 года, существенно пересмотренный ASN.1 описывается стандартом X.680.

В России ASN.1 стандартизирован по ГОСТ Р ИСО/МЭК 8824-1-2001 и ГОСТ Р ИСО/МЭК 8825-93

ASN.1 предоставляет несколько предопределенных примитивных типов данных:

* INTEGER - целое (произвольной длины);
* BOOLEAN - логическое;
* BITSTRING - битовые строки;
* строки символов (IA5String, UTF8String, UniversalString и т. д.)

и несколько способов конструирования составных типов:

* SEQUENCE - структура, содержащая минимум одно поле данных;
* SEQUENCE OF - список, содержащий произвольное количество элементов (в т. ч. 0);
* CHOICE - выбор между несколькими альтернативами.

Кроме того, имеется возможность задания подтипов путём наложения ограничений на базовые типы.
