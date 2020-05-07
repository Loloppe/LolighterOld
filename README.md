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
### Version 3.3
Massive cleanup of unused codes and dependencies.
Pattern and algorithm cleanup.
Fixed most of the bugs with the automapper and a very big bug from the converter.
Deleted a few unused files from Osu2Saber.
More...
___
## TODO
All the program is missing right now is a way for the user to add/modify/delete pattern and choose what to use during the creation of the map. It would then create maps good enough to be called hand-made.
My algorithm was made for osu!mania. It does work with osu and taiko but it's just stream then.
By using Melodyne to generate MIDI file and then converting them into osu!mania chart with Automap-chan (https://github.com/dudehacker/Automap-chan) it's possible to generate high quality map off mp3 audio file.
..Or just use a decent osu!mania chart.
___
## Osu2Saber LGPL-3.0
Majority of the files, dependencies, codes that were unused by lolighter were deleted.
Mp3toOggConverter.cs, ThumbnailGenerator.cs, MainWindow.xaml, MainWindowViewModel.cs removed.
Removed BindableBase, PropertyChange, ReportProgress, ProgressLock and majority of the method that weren't used for the conversion.
GongSolutions.Wpf.DragDrop, NAudio, OggVorbisEncoder, Prism dependencies removed.
Removed everything in ConvertAlgorithm.cs that Lolighter isn't using (Pretty much everything but the timing converter to create base note).
ConfigPanel modified to be used with Lolighter with new features.
Removed the output folder (there's no output anyway).
Modified the OszProcessor.cs so it only read a single difficulty you can select.
Added Pattern.cs for the automapper in ConvertAlgorithm.cs.
Added a lot of code in ConvertAlgorithm.cs, mostly an automapper, a different way to handle notes cleanup, some fixes and new features for the notes output.

Everything else is kept the same to not break something.
