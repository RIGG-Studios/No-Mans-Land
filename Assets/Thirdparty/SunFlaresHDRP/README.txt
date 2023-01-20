********************************
*      SUN FLARES FOR HDRP     *
*      Created by Kronnect     *   
*          README FILE         *
********************************

Requirements & Setup
--------------------
1) This version of Sun Flares only works with High Definition Rendering Pipeline (HDRP 7.1.8 or later).
2) Make sure you have High Definition RP package imported in the project before importing Sun Flares to avoid compilation errors.
3) Make sure you have a High Definition RP asset assigned to Project Settings / Graphics. There's a HDRP sample asset in Demo folder.
4) Assign Sun Flares to the "After Post Process" list in Project Settings / HDRP Default Settings.

Setup video: https://youtu.be/LK-Zvj52sBo


Demo Scene
----------
There's a demo scene which lets you quickly check if Sun Flares is working correctly in your project.


Support
-------
* Support: contact@kronnect.com
* Website-Forum: https://kronnect.com/support
* Twitter: @Kronnect


Future updates
--------------
All our assets follow an incremental development process by which a few beta releases are published on our support forum (kronnect.com).
We encourage you to signup and engage our forum. The forum is the primary support and feature discussions medium.

Of course, all updates of Sun Flares HDRP will be eventually available on the Asset Store.


Version history
---------------

v1.7
- Added "Raycast based Occlusion" option

v1.6
- Added "Clouds Occlusion" option

v1.5.2
- [Fix] Fixed compatibility issue with Unity 2021.2 beta

v1.5.1
- Fixes and improvements to the occlusion system

v1.5
- Added "Occlusion Speed" parameter

v1.4.2 11/4/2021
- Prevents an issue with gradient evaluation on negative Sun rotation angle

v1.4.1 28/10/2020
- [Fix] Fixed occlusion issue when Sun center is exactly at the edges of screen

v1.4.0 22/9/2020
- Added "Rotation Dead Zone Threshold" parameter to customize the rotation behaviour
- Added option to specify the directional light (add SunFlaresLight component to desired light)

v1.3.1 10/8/2020
- Gradient Tint color now can work properly with Physically Based Sky when using custom planetary position
- General fixes and improvements

v1.3 2/8/2020
- Added Gradient Color option (tints color based on Sun altitude where 0 = horizon, 1 = zenith)

v1.2 26/07/2020
- Added pentagonal and octagonal shapes
- Added ghost sharpness setting
- [Fix] Fixed a graphic glitch in Editor when SceneView and GameView are visible at same time

v1.1 25/06/2020
- Added hexagonal shapes

v1.0 June / 2020
First release
