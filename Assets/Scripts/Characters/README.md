# Система персонажей

## Структура папок

### Summon/
Логика призыва персонажей:
- SummonManager.cs - Менеджер призыва
- SummonController.cs - Контроллер экрана зала призыва
- SummonLogic.cs - Логика рандомизации персонажей

### Collection/
Система коллекции:
- CollectionManager.cs - Менеджер коллекции
- CollectionUI.cs - UI коллекции персонажей
- CharacterCardUI.cs - UI карточки персонажа

### Models/
Модели данных:
- CharacterData.cs - Структура данных персонажа
- CharacterClass.cs - Enum классов (warrior, scout, tank, mage)
- CharacterRarity.cs - Enum редкости (если понадобится)

## Параметры персонажа
- id - уникальный идентификатор
- class_tag - класс (warrior, scout, tank, mage)
- base_hp - базовое здоровье
- base_atk - базовая атака
- atk_range - радиус атаки
- atk_speed - скорость атаки
- move_speed - скорость передвижения
- ability_chance - вероятность получения способности
- has_ability - есть ли способность
- is_dead - статус (жив/повержен)
- ch_name - имя персонажа (рандомится из списка)

