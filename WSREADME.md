This adds batch editing features to Assembly.
All original code credits go to palesius for the Walking Sim mod and batch editing code for Assembly.

To apply a template:
1) Open the maps to edit
2) On the toolbar select BATCH.
3) You can either apply a single file to all open maps, or 
4) You can apply an entire folder of .txt templates to all open maps

Template Files:
These are named based on the game engine with a .txt extension (e.g. halo1.txt halo3odst.txt). You may also create a level specific file that will overlay the base template for that game engine.

Here are some representative lines for halo1:
matg|globals\globals|float32:429:Airborne Acceleration:5
bipd|characters\cyborg\cyborg|flags32:733:Flags:+7
weap|*|int16:742:Rounds Total Initial:32000

They are pipe delimited tokens are as follows:
1) tag type it applies to
2) entity it applies to, the wildcards here can only be used at the end of the entity name, and will apply to anything with the same beginning you entered. (e.g. * applies to all entities with that tag, objects\characters\dervish\* would applied to all entities starting with objects\characters\dervish\ )
3) tag identification and value. This is colon separated
	a) data type as per the assembly template
	b) line number in the assembly template. This is the line number of the tag, not of the enum value or flag.
	c) tag name in assembly template
	d) value to change to. For flags a - or + in front of it will unset or set a particular flag respectively.

