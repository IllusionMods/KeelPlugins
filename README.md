# KeelPlugins

Various plugins for Illusion's Unity games like Koikatu, Honey select, PlayHome and AI Syoujyo.

## Installation
1. Install BepInEx 5.
2. Download the latest release from the releases tab.
3. Put dll files to the `bepinex/plugins` folder.

## Plugin descriptions

#### AltAutoMode
An alternative auto mode for H scenes.  
Edits the full auto mode so you can control the speed of the animation but not the pose.
There is also some added randomization to make the pose changing feel more natural.

#### AnimeAssAssistant
An assistant to help you manage your huge card collections.  
It provides a few shortcuts that let you go through your cards quickly, while deciding which cards to keep.

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
