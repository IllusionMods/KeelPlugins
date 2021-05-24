# KeelPlugins

Various plugins for Illusion's Unity games like Koikatu, Honey Select, PlayHome and AI Syoujyo.  
Not all of these plugins exist or are even possible to make for all of the games.  
Configuration Manager is recommended to make changing the numerous settings from these plugins easier.

## How to install
1. Install the latest build of [BepInEx](https://github.com/BepInEx/BepInEx/releases)
2. Download the latest release for your game from [the releases page](../../releases)
3. Plugin dlls go to `bepinex/plugins` and patchers to `bepinex/patchers`

## Plugins

#### AltAutoMode [PH]
An alternative auto mode for H scenes.  
Edits the full auto mode so you can control the speed of the animation but not the pose.
There is also some added randomization to make the pose changing feel more natural.

#### AnimeAssAssistant [KK]
An assistant to help you manage your huge card collections.  
It provides a few shortcuts in maker that let you go through your cards quickly while deciding which cards to keep.  
Enable the plugin in the maker sidebar for the hotkeys to work.

<!--
#### BetterSceneLoader
An alternative scene loader for Studio.  
The subfolders of the scene folder will act as categories for your scenes.
-->

#### CameraFrameMask [KK]
Mask certain ugly frames caused by other plugins during character loading.

#### CharaStateX [KK]
Allows editing the state of multiple studio characters simultaneously.  
Normally only a few parameters such as animation speed/pattern can be edited for multiple characters at once,
but with this you can very easily load poses, change blinking state, switch animation, change clothing state, correct joints and so on.

Another feature in this plugin is H animation matching.  
By selecting a male and a female, and then clicking on an H anim in the list while holding the ctrl key, the plugin will automatically choose the right H animation based on their sex.

#### ClothingStateMenuX [KK]
Adds options to edit chara clothing states and current outfit to the maker sidebar.  
<sub><sup>Accessory options might come later.</sup></sub>

#### DefaultParamEditor [KK]
Allows editing default settings for character/scene parameters such as eye blinking or shadow density.  
This only affects parameters that make sense to be saved.

To use, set your preferred settings normally and then save them with the new buttons in the studio menus.  
Now when loading a character or starting the studio these settings will be used.

#### DefaultStudioScene [KK]
Load the scene specified in the config automatically when starting studio.

#### FreeHDefaults [KK]
Keeps freeh selection screen settings and freeh fov and aibu hand state saved.

#### ItemLayerEdit [KK]
Adds a hotkey that switches the currently selected objects layer between the character layer and the map layer.  
This allows for more in-depth editing of lighting in studio.

#### LightManager [KK]
Make studio spotlights automatically track characters.  
First select the lights you want to edit then add the character you want to track to the selection and hit apply in the light settings. You cannot select a character first because the light menu has to be open to edit the settings.

#### LockOnPlugin [KK]
A camera helper plugin that lets you forget the annoying camera controls and keeps the action right where it needs to be.
Select a character and press the hotkey to automatically keep the camera focus on the character.
In an H scene the closest character to the camera target will be selected.

#### MakerBridge [KK]
Press the hotkey to send the selected character from maker to studio and vice versa.  
To put it plainly, a temporary character card is created by the sending program which is then loaded by the receiving program when it is detected.

#### MaterialLink [KK]
Automatically sets the correct skin material on clothing that has the MaterialLink monobehaviour (for example, skindentation thighhighs).
To add this monobehaviour to your mod you can use a [stubbed MaterialLink dll](src/MaterialLinkStub.Koikatu/MaterialLink.Koikatu.dll) in unity or just copy the mb from another mod with SB3UGS. 

#### RealPOV [KK][PH]
First person mode for studio and H scenes.

#### TitleShortcuts [KK][HS][AI]
Title menu keyboard shortcuts to open different modes.  
For example, press F to open the female editor.

#### StudioAddonLite [KK]
A lite version of the StudioAddon from HS.  
Currently only has the object manipulation hotkeys for studio.

## Patchers

#### UnityLogFilter [Any]
A filter for Unity's own log messages.  
On first run the patcher will create `UnityLogFilter.txt` in the config folder and you can add your regexes there.
