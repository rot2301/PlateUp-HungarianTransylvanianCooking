# PlateUp-HungarianTransylvanianCooking

A PlateUp! mod adding Hungarian and Transylvanian dishes. Built on [KitchenLib](https://steamcommunity.com/sharedfiles/filedetails/?id=2898069883), which must also be installed.

## Implemented
- **Lángos** (starting dish): combine flour + oil, knead on a counter, fry in a pot of oil on a hob, plate.
- **Sajtos Lángos** (topping card): customers may ask for grated cheese on top.

Models are placeholder primitives built in code (`src/Utils/Prefabs.cs`), so no Unity project is needed yet.

## Building
Needs the .NET SDK and a PlateUp install (the build references the game's DLLs). PlateUp is Windows-only, so build on the machine that has the game:

```
dotnet build -c Release
```

The build finds PlateUp through Steam to reference the game's DLLs and writes `HungarianTransylvanianCooking-Workshop.dll` to `bin/<Configuration>/net472/`. If it can't find the game, pass `-p:GamePath="<folder containing PlateUp.exe>"`.

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
