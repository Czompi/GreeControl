# Pow: power state of the device

0: off
1: on
# Mod: mode of operation

0: auto
1: cool
2: dry
3: fan
4: heat
# "SetTem" and "TemUn": set temperature and temperature unit

if TemUn = 0, SetTem is the set temperature in Celsius
if TemUn = 1, SetTem is the set temperature is Fahrenheit
# WdSpd: fan speed

0: auto
1: low
2: medium-low (not available on 3-speed units)
3: medium
4: medium-high (not available on 3-speed units)
5: high
# Air: controls the state of the fresh air valve (not available on all units)

0: off
1: on
# Blo: "Blow" or "X-Fan", this function keeps the fan running for a while after shutting down. Only usable in Dry and Cool mode

# Health: controls Health ("Cold plasma") mode, only for devices equipped with "anion generator", which absorbs dust and kills bacteria

0: off
1: on
# SwhSlp: sleep mode, which gradually changes the temperature in Cool, Heat and Dry mode

0: off
1: on
# Lig: turns all indicators and the display on the unit on or off

0: off
1: on
# SwingLfRig: controls the swing mode of the horizontal air blades (available on limited number of devices, e.g. some Cooper & Hunter units - thanks to mvmn)

0: default
1: full swing
2-6: fixed position from leftmost to rightmost
Full swing, like for SwUpDn is not supported
# SwUpDn: controls the swing mode of the vertical air blades

0: default
1: swing in full range
2: fixed in the upmost position (1/5)
3: fixed in the middle-up position (2/5)
4: fixed in the middle position (3/5)
5: fixed in the middle-low position (4/5)
6: fixed in the lowest position (5/5)
7: swing in the downmost region (5/5)
8: swing in the middle-low region (4/5)
9: swing in the middle region (3/5)
10: swing in the middle-up region (2/5)
11: swing in the upmost region (1/5)
# Quiet: controls the Quiet mode which slows down the fan to its most quiet speed. Not available in Dry and Fan mode.

0: off
1: on
# Tur: sets fan speed to the maximum. Fan speed cannot be changed while active and only available in Dry and Cool mode.

0: off
1: on
# StHt: maintain the room temperature steadily at 8°C and prevent the room from freezing by heating operation when nobody is at home for long in severe winter (from http://www.gree.ca/en/features)

# HeatCoolType: unknown

# TemRec: this bit is used to distinguish between two Fahrenheit values (see Setting the temperature using Fahrenheit section below)

# SvSt: energy saving mode

0: off
1: on
