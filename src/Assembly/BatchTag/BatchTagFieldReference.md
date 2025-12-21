# BatchTag Field Type Reference Guide

This document provides examples for modifying each supported field type in BatchTag templates.

## Template File Format

```
TagGroup|TagName|FieldDefinition
```

Where `FieldDefinition` follows the format: `type:line:name:value(s)`

**?? Critical:** ALL field types require:
1. **type** - The field type (e.g., `float32`, `tagref`, `int16`)
2. **line** - The plugin line number (find this in Assembly's meta editor)
3. **name** - The exact field name (case-sensitive, must match plugin)
4. **value(s)** - One or more values depending on the field type

**Example:**
```
weap|assault_rifle|float32:120:Maximum Forward Speed:25.5
          ?          ?     ?   ?                        ?
       TagGroup   TagName type line     name            value
```

---

## Basic Numeric Types

### float32 - Single Float Value
**Format:** `float32:line:name:value`

```
veh!|warthog|float32:120:Maximum Forward Speed:25.5
```

### int8 - Signed 8-bit Integer
**Format:** `int8:line:name:value`

```
weap|assault_rifle|int8:45:Damage Modifier:-5
```

### int16 - Signed 16-bit Integer
**Format:** `int16:line:name:value`

```
weap|assault_rifle|int16:67:Magazine Size:32
```

### int32 - Signed 32-bit Integer
**Format:** `int32:line:name:value`

```
weap|assault_rifle|int32:89:Total Ammunition:300
```

### uint8 - Unsigned 8-bit Integer
**Format:** `uint8:line:name:value`

```
weap|assault_rifle|uint8:102:Fire Rate:8
```

### uint16 - Unsigned 16-bit Integer
**Format:** `uint16:line:name:value`

```
weap|assault_rifle|uint16:115:Damage Per Shot:500
```

### uint32 - Unsigned 32-bit Integer
**Format:** `uint32:line:name:value`

```
weap|assault_rifle|uint32:128:Hash Value:4294967295
```

### int64 - Signed 64-bit Integer
**Format:** `int64:line:name:value`

```
globals|globals|int64:200:Large Counter:9223372036854775807
```

### uint64 - Unsigned 64-bit Integer
**Format:** `uint64:line:name:value`

```
globals|globals|uint64:215:Very Large Value:18446744073709551615
```

---

## Angle Types

### degree - Single Angle in Degrees
**Format:** `degree:line:name:value`

```
veh!|warthog|degree:150:Turret Rotation Angle:45.0
```

### degree2 - Two Angles (Pitch/Yaw)
**Format:** `degree2:line:name:x:y`

```
veh!|ghost|degree2:175:Camera Angles:30.0:60.0
```

### degree3 - Three Angles (Pitch/Yaw/Roll)
**Format:** `degree3:line:name:x:y:z`

```
veh!|banshee|degree3:200:Default Orientation:15.0:30.0:0.0
```

---

## Vector & Point Types

### vector2 - 2D Float Vector
**Format:** `vector2:line:name:x:y`

```
weap|assault_rifle|vector2:225:Recoil Direction:1.0:0.5
```

### vector3 - 3D Float Vector
**Format:** `vector3:line:name:x:y:z`

```
veh!|warthog|vector3:250:Velocity:10.0:0.0:2.5
```

### vector4 - 4D Float Vector
**Format:** `vector4:line:name:x:y:z:w`

```
bitm|texture|vector4:275:RGBA Multiplier:1.0:1.0:1.0:0.5
```

### point2 - 2D Point
**Format:** `point2:line:name:x:y`

```
ui|menu_screen|point2:300:Button Position:100.0:200.0
```

### point3 - 3D Point
**Format:** `point3:line:name:x:y:z`

```
scen|multiplayer_map|point3:325:Spawn Location:50.0:25.0:10.0
```

---

## Range Types

### rangef (rangeFloat32) - Float Range
**Format:** `rangef:line:name:min:max`

```
weap|assault_rifle|rangef:350:Damage Range:10.5:50.0
```

### ranged (rangeDegree) - Degree Range
**Format:** `ranged:line:name:min:max`

```
veh!|warthog|ranged:375:Turret Rotation Range:-45.0:45.0
```

### rangeint16 - Short Integer Range
**Format:** `rangeint16:line:name:min:max`

```
weap|assault_rifle|rangeint16:400:Random Damage:-5:10
```

---

## Enumeration & Flag Types

### enum8 - 8-bit Enumeration
**Format:** `enum8:line:name:value`

```
weap|assault_rifle|enum8:425:Weapon Type:2
```

### enum16 - 16-bit Enumeration
**Format:** `enum16:line:name:value`

```
weap|assault_rifle|enum16:450:Fire Mode:1
```

**Note:** Both `enum8` and `enum16` use the same internal data type (`EnumData`) but with different sizes. The value should be the numeric index of the option you want to select.

### flags32 - 32-bit Flags
**Format:** `flags32:line:name:+bitindex` (set) or `flags32:line:name:-bitindex` (clear)

```
weap|assault_rifle|flags32:475:Weapon Flags:+3
weap|assault_rifle|flags32:475:Weapon Flags:-7
```

**Note:** Use `+` prefix to set a flag bit, `-` prefix to clear it.

---

## String & ID Types

### stringid - String ID
**Format:** `stringid:line:name:value`

```
weap|assault_rifle|stringid:500:Display Name:assault_rifle_name
```

---

## Color Types

### colorf - RGB Color (Hex Format)
**Format:** `colorf:line:name:RRGGBB`

```
ligh|default_light|colorf:525:Light Color:FF8040
ligh|blue_light|colorf:525:Light Color:#0080FF
```

**Note:** The `#` prefix is optional.

---

## Reference Types

### tagref - Tag Reference
**Format:** `tagref:line:name:taggroup:path/to/tag`

**Example:**
```
weap|assault_rifle|tagref:550:First Person Model:mode:objects\weapons\rifle\fp\fp_assault_rifle
weap|assault_rifle|tagref:575:Projectile:proj:objects\weapons\rifle\bullet
hlmt|storm_masterchief|tagref:45:Base Animation Graph:jmad:objects\characters\storm_masterchief\storm_masterchief
```

**To clear a tag reference:**
```
weap|assault_rifle|tagref:550:First Person Model:mode:null
```

**?? Important:** 
- `line` = The plugin line number where this field is defined (check in Assembly's meta editor)
- `name` = The exact field name as it appears in the plugin (case-sensitive)
- `taggroup` = The 4-character tag group magic (e.g., `jmad`, `mode`, `bitm`)
- `path` = The full path to the tag WITHOUT the file extension

---

## Advanced Examples

### Combining Multiple Field Modifications

```
# Modify multiple fields in the same tag
weap|assault_rifle|int16:67:Magazine Size:32
weap|assault_rifle|int32:89:Total Ammunition:300
weap|assault_rifle|float32:120:Rate of Fire:15.0
weap|assault_rifle|vector3:250:Recoil:0.5:1.0:0.2
weap|assault_rifle|tagref:550:First Person Model:mode:objects\weapons\rifle\fp\fp_rifle

# Using wildcards to match multiple tags
weap|weapons\covenant\*|float32:120:Damage Multiplier:1.5
```

### Clone Fields from Another Entry

```
weap|custom_rifle|*weapons\rifle\assault_rifle
```

**Note:** This clones all field definitions from `weapons\rifle\assault_rifle` to `custom_rifle`.

---