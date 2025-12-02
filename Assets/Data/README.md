# Данные и конфигурация

## Структура папок

### Config/
Конфигурационные файлы из Google Sheets:
- **summon.csv/json** - Конфиг призыва персонажей
  - Параметры классов (base_hp, base_atk, atk_range и т.д.)
  - ability_chance для каждого класса
  
- **ability_list.csv/json** - Список способностей
  - ability_id, ability_name
  - activate (условие активации)
  - ability_effect (эффект)
  - ability_duration, ability_cooldown
  
- **tower_floors.csv/json** - Этажи башни
  - level (номер этажа)
  - enemy (список врагов: ВРАГ:КОЛИЧЕСТВО)
  - rewards (награды: НАГРАДА:КОЛИЧЕСТВО)
  
- **enemy.csv/json** - Конфиг врагов
  - Параметры врагов (hp, atk, range и т.д.)

### Characters/
Данные персонажей:
- CharacterClasses.json - Классы и их параметры
- CharacterNames.json - Список из 10 унисекс имен

### Combat/
Данные боевой системы:
- EnemyTypes.json - Типы врагов
- TowerFloors.json - Данные этажей

### Abilities/
Данные способностей:
- Abilities.json - Все способности персонажей

### Names/
Список имен:
- names.json - 10 унисекс имен для рандомизации

## Формат данных

Все конфиги будут загружаться из Google Sheets через ConfigManager.
Локальные JSON файлы используются как fallback или для разработки.

