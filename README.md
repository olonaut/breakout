# Breakout
Simple Breakout Game made in C# using the MonoGame SDK

So I was trying to learn MonoGame/C# and got the Idea to try making a simple Breakout game.
If anyone wants to help me with that, feel free. But if you push code, please document what you are doing, becuase I am quite a noob. 
The Game is meant to be played in windows mode, on a fixed resolution. This may change in the future, yet it is the current state to save time drawing out Hi-Res textures.

Planned are:
##Basic Gameplay mechanics:
* (Implemented) Collision with Platform
* (Implemented) Collision with Walls
* (Implemented) Collision with destructable objects (hereby referred to as blocks)
* (Needs work) Ball bounces off the plattform, angle determined by point on the platform the ball impacts on.
* (Implemented) Blocks destroyed __AFTER__ ball bounces off them.
* (Missing) Three lives

##"Maybe if I have time" modular features:
* (Implemented) Controller Support (Optimised for Xbox 360)
* (Missnig) Controller Rumbles a bit when Ball hits platform
* (Missnig) Sound
* (Missnig) Powerups maybe?