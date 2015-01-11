Welcome to the Little Heroe's Life!

The two packages that are included contain a demo scene and prefabs that will get your little heroes up and running in no time.

Each package contains the following:
1. Little heroes prefabs already setup for the Motion Controller
2. Mechanim Animator specific to the little heroes
3. Demo scene to see the little heoroes in action
4. Support scripts for the little heroes AI

------------------------------------

Both packages require the following:

1. Redhead Robot's GIANT Cartoon Hero Pack: 
https://www.assetstore.unity3d.com/en/#!/content/16209

2. ootii's Motion Controller: 
https://www.assetstore.unity3d.com/en/#!/content/15672

3. ootii's Adventure Camera & Rig (optional)
https://www.assetstore.unity3d.com/en/#!/content/13768

------------------------------------

LittleHeroesLife_MC.package
This package uses the standard Motion Controller follow camera.

LittleHeroesLife_MC_AC.package
This package is nearly identical to the previous one. However, if you also own ootii's Adventure Camera & Rig, this package will support the adventure camera as well.

Note: You only need to import one of these.

------------------------------------

To run the demo, simply follow these steps:

1. Start a new Unity project
2. Import Redhead Robot's GIANT Cartoon Hero Pack
3. Import ootii's Motion Controller
4. Import ootii's Adventure Camera & Rig (optional)
5. Import LittleHeroesLife_MC (or LittleHeroesLife_MC_AC)
6. Open the LittleHeroesLife_MC scene (or LittleHeroesLife_MC_AC scene)
7. Press play

------------------------------------

To use a prefab in your own scene, simply follow these steps:

1. Start a new Unity project
2. Import Redhead Robot's GIANT Cartoon Hero Pack
3. Import ootii's Motion Controller
4. Import ootii's Adventure Camera & Rig (optional)
5. Import LittleHeroesLife_MC (or LittleHeroesLife_MC_AC)
6. Create your own scene
7. Setup the project inputs per the Motion Controller instructions
8. Setup the scene's camera per the Motion Controller instruction
9. Drag your little hero prefab into the scene

10. On the little hero's Motion Controller component (nested one layer deep), the 'Camera Transform' will empty. This is because prefabs can't hold scene objects and the camera is a scene object. Simply drag the 'Main Camera' you created in step 8 into this slot.

11. If you're using this hero as your player, ensure the Motion Controller's 'Use Input' checkbox is checked. If he's an NPC, uncheck it.

12. If you're using the Adventure Camera, remember to uncheck the Casual Idle Motion's 'Rotate with View' checkbox.

