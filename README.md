# Lolighter 
By **Loloppe#6435**

[Download Here](https://github.com/Loloppe/Lolighter/releases/latest)

**Light?** This program use "_notes" "_time" from your .dat file to generate "_events" corresponding to your Beatmap. A new file called "ExpertPlusStandard.dat" will be created in the same location of the program. Color swap speed: The color swap between blue and red every X beat (default 4). Color swap offset: Where the first color should start (default first note + 0). Use Strobe: Unticking this will remove all the "Off" events. Use Fade: Unticking this will change all "Fade" into "On" events. Spin/Zoom: Unticking this will remove all "Spin" and "Zoom" events. Spin Speed: Range allowed for the randomizer to pick from during left/right laser placement. Nerf Strobes: Similar to Down Light.

**Down Light?** Tldr: Remove fast strobes, fast color swap, always keep one light on, reduce spin/zoom spam.

**Converter?** Can convert osu!mania, osu!taiko, osu, BS .dat, .mp3, .wav and .flac files.

**Sliders?** Convert most notes into sliders. Limiter: Minimum distance required between notes of the same color, or ignored. Checkbox: Untick to allow "Loloppe notes" to be slidified, can cause issue.

**Spacing?** Modify dots sliders spacing to a specific speed.

**Inverted?** Invert most notes while keeping the exact same flow. Limiter: Minimum distance required between notes of the same color, or ignored. Checkbox: Untick to allow "Loloppe notes" to be inverted, can cause issue.

**Bomb?** Add bombs around notes.

**Loloppe?** Add additional note around notes.

Any bugs or ideas? Message me on Discord.

Enjoy :)
___
## Changelog
### Version 4.8
Added onset-detection https://github.com/opcon/onset-detection with license and copyright.
Lolighter can now convert audio directly.
Added a simple DownLighter.
Fixed some stuff in the automapper.
Now show Onsets Amplitudes info after generation to have an easier time finding the sweet spot for double generation.
Added a new way to handle "Double" during generation (doesn't use Pack for them). If "Base Double" is activated, no need for "Double Fix". "Double Fix" is for fixing "Double" when they are generated from Pack.
Can now set a specific spacing for dots sliders. Modify the spacing at any time. Default: 1/16

### Version 4.2
Fixed variable BPM.
Added dual-pack for slower/faster section.
Added variable gallop and pack speed.
Cleaned up the automapper algorithm.
Cleaned up the Logic algorithm.
Fixed the .dat converter for the map generation.
You can now convert BS map into the automapper.
Flow handle.
Parity handle.
Better patterns loop.
Integrated logic inside the mapper to fix issues that come with "Double" generation.
Now forcefully exit the program if the attempt limit is reached (Anti-memory leaks).
Pack now have their own folder.
Added buttons to bring up or down notes during pack creation.
Double hitbox algorithm can be desactivated.
Fixed parity for DD, since we can create DD with the new limiter.
Better "double" handle. Fixed the issue it had for now.
More..?
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
Added Pack.cs for Pack/Pattern/Notes structure for the automapper.
Added PatternControl and PatternWindow as UI for the user to be able to create/modify pack.

Everything else is kept the same to not break something.
