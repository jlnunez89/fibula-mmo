# How to convert from .mon files:

> Using `warlock.mon` as an example.

|Field|Convert as|Example|
|--|--|--|
|Name| same | `{ "name": "warlock" }` |
|Article| same | `{ "article": "a" }` |
|Experience| the `experienceYield` field | `{ "experienceYield": 4000 }` |
|Corpse| same | `{ "corpse": "4240" }` |
|Blood| one of `enum_bloodTypes` values | `{ "blood": "blood" }` |

This means the following:

```
RaceNumber    = 10
Name          = "warlock"
Article       = "a"
Outfit        = (130, 0-52-128-95)
Corpse        = 4240
Blood         = Blood
Experience    = 4000
SummonCost    = 0
FleeThreshold = 1000
Attack        = 40
Defend        = 50
Armor         = 32
Poison        = 0
LoseTarget    = 50
Strategy      = (100, 0, 0, 0)
```
gets converted to:

```
{
  "article": "a",
  "name": "warlock",
  "blood": "blood",
  "experienceYield": 4000,
  "corpse": "4240"
}
```
---

# Ignored fields:
The following fields **do not need to be migrated**:
```
RaceNumber    = 10
...
SummonCost    = 0
...
Poison        = 0
```

---

# Outfit 

The `Outfit` field we'll convert into one of the following schemas:

|Format|Convert as|
|--|--|
|(130, 0-52-128-95) | `{ "type": "outfit", "id": 130, "head": 0, "body": 52, "legs": 128, "feet": 95 }` |
|(36, 0-0-0-0)| `{ "type": "race", "id": 130 }` |
|(0, 0)| `{ "type": "invisible" }` |
|(0, 4240)| `{ "type": "item", "id": 4240 }` |

---

#  Attack, Defend and Armor values

|Field|Convert to|
|--|--|
|Attack|combat.baseAttackPower|
|Defend|combat.baseDefensePower|
|Armor|combat.baseArmor|

For example:
```
"combat": {
	...
	"baseAttackPower": 40,
	"baseDefensePower": 50,
	"baseArmor": 32,
	...
}
```
---
# Strategy, FleeThreshold and LoseTarget

`Strategy` is broken into 4 values under `combat` (which must add to 100):

|Strategy value| Converts as|
|--|--|
|Strategy = (`x`, x, x, x)| combat.strategy.closest |
|Strategy = ( x, `x`, x, x)| combat.strategy.weakest |
|Strategy = ( x, x, `x`, x)| combat.strategy.strongest |
|Strategy = ( x, x, x, `x`)| combat.strategy.random |

`FleeThreshold` is converted as `combat.strategy.flee.hitpointThreshold`, such as:

`LoseTarget` is converted as `combat.strategy.changeTarget.chance`, such as:

```
"combat": {
  	...
    "strategy": {
      "closest": 100,
      "weakest": 0,
      "strongest": 0,
      "random": 0,
      "flee": {
        "hitpointThreshold": 6
      },
      "changeTarget": {
        "chance": 0.5
      }
    }
	...
  }
```

> Note that the `LoseTarget` value is converted from the `integer` range `[0, 100]` to `decimal` probability range `[0.00, 1.00]`.

---

# Flags conversion

|Flag| Converts as|
|--|--|
|Unpushable | CannotBePushed|
|KickBoxes | CanPushItems|
|KickCreatures | CanPushCreatures|
|SeeInvisible | CanSeeInvisible|

These specific flags go into the `flags` property (which are values of `enum_flags`):

```
{
  ...
  "flags": [
    "CannotBePushed",
    "CanPushItems",
    "CanPushCreatures",
    "CanSeeInvisible"
  ],
  ...
}
```

However these other flags:

|Flag| Converts as|
|--|--|
|DistanceFighting | `combat.distance = 4`|
|NoBurning | `combat.immunities.poison = 1.00`|
|NoPoison |` combat.immunities.fire = 1.00`|
|NoEnergy | `combat.immunities.energy = 1.00`|
|NoParalyze | `combat.immunities.paralysis = 1.00`|
|NoLifeDrain | `combat.immunities.lifeDrain = 1.00`|

end up being like this:

```
"combat": {
    ...
    "distance": 4,
    "immunities": {
      "poison": 1.00,
      "fire": 1.00,
      "energy": 1.00,
      "lifeDrain": 1.00,
      "paralysis": 1.00
    },
    ...
  },
```

# Skills

These 3 skills convert one to one (using the first number):
|Skill| Converts as|
|--|--|
| HitPoints | `stats.hitpoints` | 
| GoStrength | `stats.baseSpeed` |
| CarryStrength | `stats.carryStrength` |

But `FistFighting` becomes `combat.skills.Fist`, and is **split** into:

|Element| Converts as | Remarks |
|--|--|--|
|(FistFighting, `50`, 50, 50, 100, 1100, 2)|combat.skills.Fist.level| |
|(FistFighting, 50, 50, 50, `100`, 1100, 2)|combat.skills.Fist.targetCount| |
|(FistFighting, 50, 50, 50, 100, `1100`, 2)|combat.skills.Fist.factor| normalized by dividing by 1000 |
|(FistFighting, 50, 50, 50, 100, 1100, `2`)|combat.skills.Fist.increase| |

---

# Inventory

For this one it's easier if you convert from the XML files in Nostalrius format. For simplicity here are only 2 of the items in the inventory/loot element for the warlock:

```
<item id="3031" countmax="80" chance="300" /> <!-- a gold coin -->
<item id="3360" countmax="1" chance="3" /> <!-- a golden armor -->
```

We'll: 

1) Carry over names from Nostalrius files into the NAME property, stripping `'a'` / `'an'` prefixes.
1) Convert chance by dividing by 1000. e.g:  

   `<item id="3360" countmax="1" chance="3" /> <!-- a golden armor -->`

   becomes
	
   `{ "id": "3360", "name": "golden armor", "maximumCount": 1, "chance": 0.003 }`

> Also note that `countmax` converts to `maximumCount`, and that `id` is a string.

---
# Talk/Voices
	
You may just copy as-is from the .mon file. Alternatively, on the XML format, any text tagged `YELL` will need to be prefixed with `#Y`.

Here's an example:

```
Talk          = {"Learn the secret of our magic! YOUR death!",
                 "Even a rat is a better mage than you.",
                 "We don't like intruders!"}
```
becomes
```
{
	...
	"speech": [
		"Learn the secret of our magic! YOUR death!",
		"Even a rat is a better mage than you.",
		"We don't like intruders!"
	]
	...
}
```

# Spells

## This is the complicated part...
... because you actually have to think.

Spells all become an entry under `combat.abilities`, and they are broken down for each different type.

Here's what a Warlock has:

```
Spells        = {Actor (13) -> Healing (80, 20) : 4,
                 Actor (13) -> Outfit ((0, 0), 20) : 10,
                 Victim (7, 5, 0) -> Damage (1, 75, 30) : 2,
                 Victim (7, 0, 0) -> Damage (512, 55, 20) : 6,
                 Victim (7, 0, 14) -> Speed (-80, 20, 40) : 9,
                 Origin (0, 13) -> Summon (67, 1) : 10,
                 Destination (7, 4, 2, 7) -> Damage (4, 130, 40) : 3,
                 Destination (7, 4, 0, 0) -> Field (1) : 7,
                 Destination (7, 4, 1, 0) -> Field (1) : 5,
                 Angle (0, 8, 12) -> Damage (8, 175, 30) : 8}
```

We'll dissect them all in the general form:

`CastType`(`params`) -> `Action`(`params`) : `CHANCE`

Then, assume we can mix and match, any `Action`(`params`) that we want, so forget about that part. 
We're only left with `CastType`(`params`) and `CHANCE` to worry about:

For `CHANCE` there is a simple conversion rule: do 1/`CHANCE` and round to 3 decimals:

(back to the warlock example)
|Spell|Chance|Converted Chance|
|--|--|--|
|Actor (13) -> Healing (80, 20) : 4 | 6 | 1/6 = `0.167`|
|Victim (7, 5, 0) -> Damage (1, 75, 30) : 2 | 2 | 1/2 = `0.5`|
|Destination (7, 4, 2, 7) -> Damage (4, 130, 40) : 3 | 3 | 1/3 = `0.333`|
|Victim (7, 0, 14) -> Speed (-80, 20, 40) : 9 | 9 | 1/9 = `0.111`|
|Angle (0, 8, 12) -> Damage (8, 175, 30) : 8 | 8 | 1/8 = `0.125`|

---

Now, let's focus on the `CastType`(`params`) part:

## `Actor(int castEffect)` -> {action} : chance

converts to the form:
```
{ 
	"type": "self",
	"casterEffect": "SparklesBlue",
	"actions": [
		...
	],
	"chance": 0.1
},		
```
where 

|Param|Type|Remarks|
|--|--|--|
|type|string| always `"self"`|
|castEffect|`enum_animatedEffect`||
|actions|array||
|chance|number|bounded in the range `[0.000, 1.000]` (with precision `3`).|

--- 

## `Origin(int radius, int areaEffect)` -> {action} : chance

```
      { 
        "type": "selfArea",
        "radius": 0,
        "areaEffect": "SparklesBlue",
        "actions": [
          ...
        ],
        "chance": 0.1
      },
```
where 

|Param|Type|Remarks|
|--|--|--|
|type|string| always `"selfArea"`|
|radius|integer||
|areaEffect|`enum_animatedEffect`||
|actions|array||
|chance|number|bounded in the range `[0.000, 1.000]` (with precision `3`).|

--- 

## `Victim(int range, int projectileEffect, int targetEffect)` -> {action} : chance

```
{ 
	"type": "target",
	"range": 7,
	"projectileEffect": "None",
	"targetEffect": "SparklesRed",
	"actions": [
		...
	],
	"chance": 0.111
},

```
where 

|Param|Type|Remarks|
|--|--|--|
|type|string| always `"target"`|
|range|integer||
|projectileEffect|`enum_projectileEffects`||
|targetEffect|`enum_animatedEffect`||
|actions|array||
|chance|number|bounded in the range `[0.000, 1.000]` (with precision `3`).|

--- 

##	`Destination(int range, int projectileEffect, int radius, int areaEffect)` -> {action} : chance

```
{
	"type": "targetArea",
	"range": 7,
	"casterEffect": "None",
	"projectileEffect": "Fire",
	"areaEffect": "AreaFlame",
	"radius": 2,
	"actions": [
		...
	],
	"chance": 0.333
},
```
where 

|Param|Type|Remarks|
|--|--|--|
|type|string| always `"targetArea"`|
|range|integer||
|casterEffect|`enum_animatedEffect`||
|projectileEffect|`enum_projectileEffects`||
|areaEffect|`enum_animatedEffect`||
|radius|integer||
|actions|array||
|chance|number|bounded in the range `[0.000, 1.000]` (with precision `3`).|

---

## `Angle(int spread, int length, int areaEffect)` -> {action} : chance

```
{ 
	"type": "targetDirection",
	"length": 8,
	"spread": 0,
	"areaEffect": "DamageEnergy",
	"actions": [
		...
	],
	"chance": 0.125
}
```
where 

|Param|Type|Remarks|
|--|--|--|
|type|string| always `"targetDirection"`|
|spread|integer||
|length|integer||
|areaEffect|`enum_animatedEffect`||
|actions|array||
|chance|number|bounded in the range `[0.000, 1.000]` (with precision `3`).|

---

# Actions:

Finally, let's get back to the general form:

`CastType`(`params`) -> `Action`(`params`) : `CHANCE`

And focus on the `Action`(`params`) part.
The spells/abilities can have any of the following actions:

## `Healing (int base, int variation)`

Which is converted to:
```
{
	"type": "heal",
	"base": 80,
	"variation": 20
}
```

## `Damage (int damageKind, int base, int variation)`

```					
{
	"type": "damage",
	"kind": "Physical",
	"base": 75,
	"variation": 30
}
```
where `kind` is a `enum_damageKinds`.


## `Summon (int raceId, int maxCount)`

```
{
	"type": "summon",
	"monsterFile": "stonegolem",
	"maximumCount": 1
}
```

where `monsterFile` is the actual name (without extension) of the monster file (which you'll be converting too).

## `Field (int magicFieldKind)`
```
{
	"type": "magicField",
	"kind": "Fire"
}
```

where `kind` is a `enum_magicFieldKinds`.

## `Speed (int base, int variation, int duration)`

```
{
	"type": "speedChange",
	"base": -70,
	"variation": 20,
	"duration": 30000		
}
```

where `duration` is in milliseconds instead of seconds, so multiply by 1000.

## `Drunken (int strength, int effect, int duration) `

```
{
	"type": "makeDrunk",
	"strength": 6,
	"duration": 60000
}
```
where `duration` is in milliseconds instead of seconds, so multiply by 1000.

## `Outfit ((0, 0), int duration)` for invisible

```
{
	"type": "lookChange",
	"targetLook": {
		"type": "invisible"
	},
	"duration": 20000
}
```
where `duration` is in milliseconds instead of seconds, so multiply by 1000.

## `Outfit ((0, int itemdId), int duration)` for item

```
{
	"type": "lookChange",
	"targetLook": {
		"type": "item"
		"id": "4240"
	},
	"duration": 20000
}
```
where `duration` is in milliseconds instead of seconds, so multiply by 1000.

## `Outfit ((int id, 0-0-0-0), int duration)` for monster race

```
{
	"type": "lookChange",
	"targetLook": {
		"type": "race",
		"id": 122
	},
	"duration": 6000
}
```

where `duration` is in milliseconds instead of seconds, so multiply by 1000.

## `Outfit ((int id, head-body-legs-feet), int duration)` for a full outfit

```
{
	"type": "lookChange",
	"targetLook": {
		"type": "outfit",
		"id": 129,
		"head": 95,
		"body": 116,
		"legs": 120,
		"feet": 115
	},
	"duration": 6000
}
```
where `duration` is in milliseconds instead of seconds, so multiply by 1000.

