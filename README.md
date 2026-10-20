# PlateUp-HungarianTransylvanianCooking

A PlateUp! mod adding Hungarian and Transylvanian dishes. Built on [KitchenLib](https://steamcommunity.com/sharedfiles/filedetails/?id=2898069883), which must also be installed.

## Implemented
- **Lángos** (starting dish): combine flour + oil, knead on a counter, fry in a frying pan on a hob, plate.
- **Savoury topping cards**, any 3 combined on one lángos:
  - **Sajtos**: grated cheese
  - **Fokhagymás**: garlic paste (chop garlic, new provider)
  - **Tejfölös**: sour cream (new provider)
  - **Hagymás**: chopped onion
  - **Savanyúságos**: pickles
  - **Sonkás**: chopped Hungarian smoked ham (new provider)
- **Édes Lángos** (main card): melted chocolate and/or chopped nuts. Sweet and savoury toppings never mix.

Models are placeholder primitives built in code (`src/Utils/Prefabs.cs`), so no Unity project is needed yet.

## Building
Needs the .NET SDK and the game's DLLs in a git-ignored `lib/` folder (subfolders are fine):

- everything in `PlateUp/PlateUp_Data/Managed/`
- `KitchenLib-Workshop.dll` from the KitchenLib workshop folder (`steamapps/workshop/content/1599600/2898069883/`)

```
dotnet build -c Release
```

The build writes `HungarianTransylvanianCooking-Workshop.dll` to `bin/<Configuration>/net472/`.

# Ideas

## Starters
- Bread (Multi) & Vinete (Multi)
- Bread (Multi) & Liptauer (Multi)

## Mains
- Lángos (Topping Variants)
- Mazăre cu porc
- Paprikás
- Gulyásleves (Multi)
- Mititei
- Töltött Káposzta/Sarmale (Multi)
- Ardei umpluți (Multi)

## Sides
- Lecsó (Multi)
- Cabbage Salad (Multi)

## Dessert
- Kürtőskalács (Multi)
- Kajszibarackos gombóc
- Dobos (Multi)
