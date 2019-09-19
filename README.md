# KeelPlugins [![Build status](https://ci.appveyor.com/api/projects/status/j2aa50y5o9onu8b0/branch/master?svg=true&passingText=master%20-%20OK&failingText=master%20-%20Fail)](https://ci.appveyor.com/project/Keelhauled/keelplugins/branch/master/artifacts)

Various plugins for Illusion's Unity games like Koikatu, Honey select, PlayHome and AI Syoujyo.  
Not all of these plugins exist or are even possible to make for all of the games.

Configuration Manager is recommended to make managing and changing the numerous hotkeys from these plugins easier. Just press F1 if you have it installed.

## Installation
1. Install BepInEx 5
2. Download the latest artifact from the [Appveyor page](https://ci.appveyor.com/project/Keelhauled/keelplugins/build/artifacts)
3. Put the dll files you want in the `bepinex/plugins` folder

## Plugin descriptions

#### AltAutoMode
An alternative auto mode for H scenes.  
Edits the full auto mode so you can control the speed of the animation but not the pose.
There is also some added randomization to make the pose changing feel more natural.

#### AnimeAssAssistant
An assistant to help you manage your huge card collections.  
It provides a few shortcuts that let you go through your cards quickly while deciding which cards to keep.

#### CharaStateX
Allows editing the state of multiple studio characters simultaneously.  
Normally only a few parameters such as animation speed/pattern can be edited for multiple characters at once,
but with this you can very easily load poses, change blinking state, switch animation, change clothing state, correct joints and so on.

Another feature in this plugin is H animation matching.  
By selecting a male and a female, and then clicking on an H anim in the list while holding the ctrl key, the plugin will automatically choose the right H animation based on their sex.

#### DefaultParamEditor
Allows editing default settings for character/scene parameters such as eye blinking or shadow density.  
This only affects parameters that make sense to be saved.

To use, set your preferred settings normally and then save them with the new buttons in the studio menus.  
Now when loading a character or starting the studio these settings will be used.

#### DragAndDrop
Adds drag and drop support, making it possible to load characters, outfits, scenes and poses by dragging them into the game window.
Support for each card types depends on the game and the current scene of the game.

#### GraphicsSettings
Exposes the game's graphics settings and some other values for editing.  
The default settings on the plugin may be too heavy for some computers so remember to tweak them.

#### HideAllUI
Hide the UI with the same, adjustable hotkey in maker, studio and freeh.  
This plugin combines the old ui hiding plugins into one and requires you to delete them.

#### LockOnPlugin
A camera helper plugin that lets you forget the annoying camera controls and keeps the action right where it needs to be.  
It allows you to automatically keep the camera focus on the character.

#### MakerBridge
Press the hotkey to send the selected character from maker to studio and vice versa.  
To put it plainly, a temporary character card is created by the sending program which is then loaded by the receiving program when it is detected.

#### TitleShortcuts
Title menu keyboard shortcuts to open different modes.  
For example, press F to open the female editor.  
Also has a setting to start certain modes automatically during startup.  
Hold esc just before the title screen to cancel automatic startup.
