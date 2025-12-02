# Структура проекта PickMe

## Обзор
Проект разделен на основные системы согласно техническому заданию:
- **Characters** - Система персонажей
- **Combat** - Боевая система
- **City** - Система города
- **Managers** - Менеджеры игровых систем
- **Data** - Конфигурационные данные
- **UI** - Пользовательский интерфейс

---

## Scripts/

### Scripts/Characters/
Система персонажей:
- **Summon/** - Логика призыва персонажей (Зал призыва)
- **Collection/** - Система коллекции персонажей
- **Models/** - Модели данных персонажей (CharacterData, CharacterClass и т.д.)

### Scripts/Combat/
Боевая система:
- **Tower/** - Логика башни (этажи, награды, переходы)
- **Enemies/** - Поведение и логика противников
- **Abilities/** - Система способностей персонажей
- **AI/** - ИИ для персонажей (выбор целей, движение, атака)

### Scripts/City/
Система города:
- **Buildings/** - Логика зданий (Дом, Зал призыва, Башня и т.д.)
- **Construction/** - Система строительства зданий

### Scripts/Managers/
Менеджеры игровых систем:
- GameManager
- CharacterManager
- CombatManager
- CityManager
- ConfigManager (загрузка конфигов из Google Sheets)
- SaveManager

### Scripts/Data/
Модели данных:
- CharacterData
- EnemyData
- AbilityData
- TowerFloorData
- BuildingData
- PlayerData

### Scripts/UI/
Пользовательский интерфейс:
- **Common/** - Общие UI компоненты
- **Characters/** - UI для персонажей (коллекция, карточки)
- **Combat/** - UI боевой системы (HP бары, результаты боя)
- **City/** - UI города (здания, строительство)

---

## Data/

### Data/Config/
Конфигурационные файлы (будут загружаться из Google Sheets):
- summon.csv / summon.json
- ability_list.csv / ability_list.json
- tower_floors.csv / tower_floors.json
- enemy.csv / enemy.json

### Data/Characters/
Данные персонажей:
- CharacterClasses.json
- CharacterNames.json (10 унисекс имен)

### Data/Combat/
Данные боевой системы:
- EnemyTypes.json
- TowerFloors.json

### Data/Abilities/
Данные способностей:
- Abilities.json

### Data/Names/
Список имен для персонажей (10 унисекс имен)

---

## Prefabs/

### Prefabs/Characters/
Префабы персонажей:
- CharacterCard.prefab
- CharacterPortrait.prefab

### Prefabs/Combat/
Префабы боевой системы:
- CharacterCombat.prefab
- EnemyCombat.prefab
- HPBar.prefab
- CombatResultWindow.prefab

### Prefabs/City/
Префабы города:
- Building.prefab
- BuildingRuins.prefab (площадка под строительство)

### Prefabs/UI/
UI префабы:
- **Characters/** - UI коллекции, карточки персонажей
- **Combat/** - UI боя, результаты
- **City/** - UI зданий, строительства

---

## Art/

### Art/Characters/
Арты персонажей:
- Портреты
- Карточки

### Art/Combat/
Арты боевой системы:
- Арена
- Эффекты способностей

### Art/City/
Арты города:
- Здания
- Фон города

### Art/UI/
Арты интерфейса:
- Кнопки
- Панели
- Иконки

---

## Audio/

### Audio/Music/
Музыка:
- CityTheme
- CombatTheme

### Audio/SFX/
Звуковые эффекты:
- Призыв персонажа
- Атака
- Способности
- UI клики

---

## Scenes/

Сцены игры:
- City.unity - Главная сцена города
- TowerFloor_01.unity - Этажи башни (будут создаваться по мере необходимости)
- SummonHall.unity - Сцена зала призыва (или UI overlay)

---

## Основные системы

### 1. Система персонажей
- Призыв за билеты (3 персонажа за 1 билет)
- Хранение в коллекции
- Отображение карточек с параметрами
- Использование в отряде

### 2. Боевая система
- Башня с этажами
- Автономный бой
- Поведение персонажей по классам
- Способности (Кровавая жертва для Воина)

### 3. Система города
- Главный экран/лобби
- Здания (Дом, Зал призыва, Башня, Портал, Мердж)
- Строительство зданий
- Переходы между системами

