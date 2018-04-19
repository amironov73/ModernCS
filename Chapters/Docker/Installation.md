### Установка Docker на Windows

Docker for Windows может запускать как контейнеры Linux, так и контейнеры Windows, но не те и другие одновременно. В любой момент можно переключиться, но придётся остановить все выполняющиеся контейнеры.

Docker for Windows требует 64-битную Windows 10 Pro, Enterprise или Education (1607 Anniversary Update, Build 14393 или выше) с Hyper-V. В BIOS/UEFI должна быть включена виртуализация.

Контейнеры и образы разделяются между всеми аккаунтами машины, на которой они созданы.

Сценарий рекурсивной виртуализации (когда Docker запускается внутри виртуальной машины VMWare) не поддерживается, но может (не обязан!) работать.

Docker for Windows устанавливает следующие компоненты: Docker Engine, Docker CLI client, Docker Compose, Docker Machine и Kitematic.
