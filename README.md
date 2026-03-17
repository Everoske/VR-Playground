# VR Shooting Gallery

## Description
This is a shooting gallery game largely inspired by Resident Evil 4's Shooting Range but in Virtual Reality and built in Unity. You have game modes for different weapons or mix of weapons where you shoot at Lovecraftian creatures and attempt to beat your high score. Things like accuracy and number of targets hit adds to your final score. Currently, there are game modes for semi-auto pistol and bolt-action rifle.

## Implemented Features
### Main Game Loop
- Ability to select and start a shoot gallery mode.
- Moving and stationary targets that increase the player's score.
- Moving and stationary decoys that decrease the player's score.
- Accuracy tracking.
- Final score calculation and high score tracking.
### VR
- Realistic pistol mechanics including the ability to fire, pull back on the slide, and reload.
- Realistic bolt-action rifle mechanics including the ability to load bullets into the chamber, the ability to use the bolt, and the ability to fire and reload.
- UI for ammo count for each weapon which can be toggled on and off.
- The ability to switch between locomotion and rotation types for VR movement.
- Pushable VR buttons.
- VR collision handling.
- Hand menu with multiple options for in-game settings.
### IO
- Saving VR settings.
- Saving high score for each game mode. 

## Project Breakdown
- /Assets/Scripts: All project files.
- /Assets/Prefabs: Prefab objects.
- /Assets/Shaders: Unity shaders.
- /Assets/Tests: Game and editor tests using Unity's Testing Framework.