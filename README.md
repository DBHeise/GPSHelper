# GPSHelper
A GPS Helper mod for Space Engineers
GPS Helper is a set of chat commands that make management of your GPS points easier. Most commands take an optional parameter which is used to match against either the name or description.

## Commands

### gpsx
"/gpsx" - creates a GPS point from the player's current position. Adds which planet it's from in the description. Without any parameters it will create a GPS waypoint using the player's name and an "autoID" (base36), if the more text is supplied it will use that as the GPS point name (followed by the autoID)
#### EXAMPLES:
- "/gpsx" - creates a gps point named "{playername} {autoid}" where playername is the player's name and autoID is the next number in the autoid sequence
- "/gpsx Ice Astroid" - creates a gps point named "Ice Astroid {autoid}" 
- "/gpsx foo bar baz" - creates a gps point named "foo bar baz {autoid}" 
If the above 3 examples are used in squence, the playerid is "Player2" and the autoID is starting at the beginning then the three GPS points would be "Player2 001", "Ice Astroid 002", and "foo bar baz 003" respectively


### gpsx_reset
"/gpsx_reset" - resets the autoID to zero. Does not change existing GPS points
#### EXAMPLES:
- "/gpsx_reset" - resets the autoID to zero (so the next one issued would be 001)

### gps_export
"/gps_export" - exports GPS points to the player's clipboard. If no extra parameter is given it will export ALL GPS points to the player's local clipboard. if an extra parameter is given it will only export those gps points that contain the given parameter in either the name or description
#### EXAMPLES:
- "/gpsx_export foo" - copies all the gps points with either foo in their name or description to the user's clipboard

### gps_toggle
"/gps_toggle" - toggles the gps points show on hud
#### EXAMPLES:
- "/gpsx_toggle" - turns all the gps points currently not showing in the HUD on, and all the ones showing to off
- "/gps_toggle Ice" - toggles the show on HUD for all the gps points with "Ice" in the name or description

### gps_off
"/gps_off" - turns off the gps points show on hud
#### EXAMPLES:
- "/gpsx_off" - turns off all player gps points from being shown on the HUD
- "/gpsx_off Astroid" - turns off all player gps points with "Astroid" in the name or description

### gps_on
"/gps_on" - turns on the gps points show on hud
#### EXAMPLES:
- "/gpsx_on" - turns on all player gps points to be shown on the HUD
- "/gpsx_on Gold" - turns on all player gps points with "Gold" in the name or description

### gps_remove
"/gps_remove" - deletes the gps points
#### EXAMPLES:
- "/gpsx_remove" - deletes all player gps points
- "/gpxs_remove TEMP Foo bAr" - deletes all player gps points with "TEMP Foo bAr" in the name or description
