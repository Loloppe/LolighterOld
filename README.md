# Lolighter 
By **Loloppe#6435**

[Download Here](https://github.com/Loloppe/Lolighter/releases/latest)

**Light?** This program use "_notes" "_time" from your .dat file to generate "_events" corresponding to your Beatmap. A new file called "ExpertPlusStandard.dat" will be created in the same location of the program. Color swap speed: The color swap between blue and red every X beat (default 4). Color swap offset: Where the first color should start (default first note + 0). Use Strobe: Unticking this will remove all the "Off" events. Use Fade: Unticking this will change all "Fade" into "On" events. Spin/Zoom: Unticking this will remove all "Spin" and "Zoom" events. Spin Speed: Range allowed for the randomizer to pick from during left/right laser placement.

**Automapper?** Can convert osu!mania (or osu!taiko/osu but I wouldn't recommend) maps into notes and patterns randomly generated with an algorithm and some other features. To use the osu(!mania) converter, just need to open file and select a .osz or .zip file. Be patient, it take a while to read file. Nice for map and light generation.

**Sliders?** Convert most notes into sliders. Limiter: Minimum distance required between notes of the same color, or ignored. Checkbox: Untick to allow "Loloppe notes" to be slidified, can cause issue.

**Inverted?** Invert most notes while keeping the exact same flow. Limiter: Minimum distance required between notes of the same color, or ignored. Checkbox: Untick to allow "Loloppe notes" to be inverted, can cause issue.

**Bomb?** Add bombs around notes.

**Loloppe?** Add additional note around notes.

**Downscaler?** Remove stacks, windows, sliders, tower. Flatten map. Downmap most by half. Fix flow.

Any bugs or ideas? Message me on Discord.

Enjoy :)
___
## Changelog
### Version 2.5
Fixed a crash caused by white space at end of folder path.
### Version 2.4
Removed some of the commented-out codes to clean up a bit. Tried to fix the offset issue with osu and osu!mania chart (they don't have the same timing for some reason).
Fixed the RemoveExcessNotes() method.
### Version 2.1 - 2.3
Fixed some crash. Added new features to the converter. Renamed some stuff.
Removed gallops generation when "Double" is activated.
You can now select a specific .osu difficulty file.
### Version 2.0
Added a modified version of Osu2Saber (https://github.com/tmokmss/Osu2Saber) to Lolighter. Only some parts of the program is used (similar to a library). LGPL-3.0 license and whole source-code included.
Added osuBMParser https://github.com/N3bby/osuBMParser (which is necessary to use Osu2Saber). MIT license and whole source-code included.
___
## TODO
My main goal would be to remake from scratch a better Automapper, but the current one work very well so I'm not sure about it.
I would like to make my own converter in Lolighter, but merging Osu2Saber was just less work overall (It's less efficient tho).
All the program is missing right now is a way for the user to add/modify/delete pattern and choose what to use during the creation of the map (and some slight modification in the logic). It would then create maps good enough to be called hand-made.
My algorithm was made for osu!mania. It does work with osu and taiko but it's just stream then.
By using Melodyne to generate MIDI file and then converting them into osu!mania chart with Automap-chan (https://github.com/dudehacker/Automap-chan) it's possible to generate high quality map off mp3 audio file.
Or just use a decent osu!mania chart.
___
## Osu2Saber LGPL-3.0
TLDR of changes made in the program (I would like to write it in detail but it been more than a year since I've made those changes):
Heavily modified ConvertAlgorithm.cs. The main method "Convert" follow the same procedure as before but I've added some new method that can be used if they are selected in the MainWindow + ConfigPanel.
Var added in ConfigPanelViewModel.cs to link the Window with the var in ConvertAlgorithm.cs.
I've also modified the older method in ConvertAlgorithm.cs to fit my "requirement" for the MapReader that I've added in ConvertAlgorithm.
ConvertAlgorithm now include MakeLightEffect (Light with note time value), MapReader (Automapper note cut direction and placement using hard-coded pattern and an algorithm), LogicChecker (Algorithm that check flow, notes placement, etc), FixOffset (for the audio delay), BottomDisplacement (To make all the notes bottom), UpDown (Only up and down).
I modified the MainWindow and ConfigPanel to allow user to select the new feature.
I turned off the audio converter as it was causing more offset issue, crash and slowing down the whole process (Commented out in BatchProcessor.cs).
Added a global SaberBeatmap var in Osu2BsConverter.cs called "map" to grab the notes off Osu2Saber after they are generated and transfer them in Lolighter.
Disabled the "output" folder creator since Lolighter already deal with that (Commented out).
Also in Osu2BsConverter.cs, everything after the GenerateMap in ConvertBeatmap is disabled (Commented out) since we just want to grab the generated notes.
Added a new class file, called Pattern.cs, used by MapReader in ConvertAlgorithm.cs to get pattern loop for the automapper.
Changed some default variable value in SaberBeatmap.cs and SaberInfo.cs (They are not really used anymore since the merge).
Added a way to select a specific beatmap from a .osz pack.
99% of the changes were made in ConvertAlgorithm.cs.
