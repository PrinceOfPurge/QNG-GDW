
## About the Game:
Awakened is a monster shooter where the player is stuck in a car. The storyline will start with Sheriff going to investigate a missing persons report when they hear some breaking news on their radio. This broadcast will be about how monsters have taken over their town and to hide and take shelter. Hearing that the protagonist crashes into a barricade. The player has a basic pistol and has limited ammo to start. The player must open the door to get a better view and shoot the zombies. The added layer of difficulty is that the player can only look in one direction, left or right. Switching between sides has a short delay. 

## Group Members and tasks: 

  ### Christopher Shin (100974007):
    - Wave Manager Singleton
    - Custom button mapping command pattern design
    - Zombie Spawning Factory design 

    
  ### Bilal Rehman (100921929):
    - Audio Manager Singleton
    - Character Customization command
    - Trees factory

  ### Gomagan Prabagar (100944940):
    - Resource Manager Singleton
    - Collect / Use Resource Command
    - Resource Drop Factory

## How we created the group
The way we created this group was by seeing who in the class is someone that we have worked with in the past or someone we know. We also decided based on the projects we are working on, such as a GDW game. 



## What was Done: 

### Christopher Shin:

#### Singleton:
I built a WaveManager singleton where only one exists in the scene and manages and creates waves of enemies. This is done by creating a base singleton script that the WaveManager would inherit from. The base singleton checks if there are others like it and deletes them, and runs the Don't Destroy On Load function to make it persistent. The reason I made it this way is so that it's easier to manage enemy spawning. This would benefit the game, as there would be a lot of zombie enemies, and it would require a wave system. I think it would also be useful so that other scripts can access the function in the WaveManager. 

#### Command Design Pattern:
I followed this video https://www.youtube.com/watch?v=oLRINAn0cuw to go into more depth on how command design patterns work and are used in Unity. First, I created an abstract command class that has the abstract method Execute. Then I made a shootCommand script that inherits from it. This script executes the shoot function in playerShoot. The reason I implemented it this way is so you can track which shooting action the player has done. This is useful for this project as there could be different ways to shoot and guns that use different ammo. 

#### Factory:
I built a factory that can spawn different types of zombies or enemies. I first made a base Enemyspawner and a base enemy script, both are abstract, so the child has to inherit their functions. The base enemy has public and protected variables and functions that the child can inherit. The base EnemySpawner has an abstract spawn method that the child has to override to write their own implementation. I did it this way so that you can spawn entities without knowing what class they are, and it makes spawning enemies easier. This is great for this game as it is all about clearing hordes of enemies and fighting many different ones. 



### Gomagan Prabagar:

#### Singleton:
I built an Inventory Manager and named it "ResourceManager". How this works is that it tracks the three resources (Bullets, Grenades, MedPacks) and it sees how much of each the player is holding. It also has a Max value for each so you can only hold a limited amount of each resource. This script has two public methods. AddResource takes in a string value and checks if the resource hit its limit, and if to it refuses to take the item but if it didn't hit the limit, it adds one to the amount of that resource you have and returns true so that the script that calls this can delete the resource from the scene. The second method is called UseResource which also takes in a string value and sees if you have atleast 1 of this resource. If so, it will return true and then subtract 1 from its count. This singleton also features a public int called currentResourceCount which tracks the amount of resources in the scene that haven't been collected yet and is used by two different scripts to add and remove the resources from the scene.
<img width="973" height="1018" alt="image" src="https://github.com/user-attachments/assets/49d5bb8f-269f-4e9f-8f2f-c83656d1b05e" />


#### Command Design Pattern:
I also followed the video shared by Christopher Shin to see how exactly to implement the command design pattern in unity. I then created an interface called ICommand which has a barebones method called Execute(). I then made another script called CollectCommand and made it inherit from ICommand and has a public method called Execute which basically calls upon the spawned resource and calls .Add() which will add it to the inventory if you have space.

#### Factory:
I created a resource spawner factory called ResourceSpawner. It keeps track of 3 gameobject prefabs (bullet, grenade, and medpack) and all these prefabs also have the Resource script. The ResourceSpawner has a private method called SpawnRandomResource() which uses UnityEngine.Random to pick out of the 3 gameobjects and then Instantiate it. It also has another helper method that returns the right prefab when given in the string. 
<img width="700" height="883" alt="image" src="https://github.com/user-attachments/assets/05cd6998-cd10-4ff8-9d37-226dc9d3eac6" />


When this script first runs, it will call the SpawnRandomResource 5 times in a for loop. This script also has a value called MAXRESOURCE which is set to 5 so that there can only be 5 resources on the platform at a time and never more. There is also an Update method and if there are less than the MAXRESOURCE amount of resources in the platform, it will call SpawnRandomResource every second. The currentResourceCount exists in the ResourceManager singleton because another script also has to access it so it made sense to move that variable to the singleton. 

#### Resource:
This script is attached to all the resources and has an Init function that sets it's position to a random position on the platform. It also has a public method called Add() which the CollectCommand script calls so that the item can be collected. The Add method also calls the AddResource method in the ResourceManager singleton and if that returns true, it will subtracts 1 from currentResourceCount in the ResourceManager singleton so that the ResourceSpawner script can spawn one more resource to fill the 5 max resource on the platform thing, and also destroy itself from the platform. 

### Bilal;

Singleton:
The original game uses fmod so I thought that using a audio manager that uses fmod would be perfect for this project. and because fmod creates lots of changes regarding banks and whatnot using the singleton pattern is perfect for this specific use case. This audio manager plays the ambient rainfall sounds and can be used to add even more sounds and other music.  It has the framework to be further expanded on based on the needs of the game. It has different functions and methods to mess around with the audio. 

Command:
for the command design pattern I thought that it would be nice if the player could have a chance to customize the characters skin as it was just red before. And this fits in perfectly for command because the system doesn't need to know what colour they pick the system just has to change it. So the scripts show 5 options 3 skin tones, undo and redo buttons. the player simply clicks on the button and then that in return changes the mesh of the character to said colour. the player can then click redo or undo to compare colours. 

Factory:
The game scene is very boring and dry and not very plain so to add to the vibe of the game I had created a factory design to mass produce trees in the game. so there's two types of trees and what the code does is that it spawns in trees at random sizes around the spawn points. and this gives the effect of looking like a real forest and doesn't look like a cardboard cutout. and more trees can be spawned in by pressing the "t" key. This adds a level of realism because the trees are different and random just like in real life. 

## AI use disclosure

### Gomagan Prabagar
I've used AI, along with youtube videos and lecture slides for better help with understanding what the patterns are and ideas on what kind of what kind of systems I can build with the patterns, though I wrote all the code and architecture for these patterns myself without relying on AI. I also used AI to help me remember how to do Raycasting on unity for the collect feature. This was faster than searching up a video to remind me how to raycast on unity and raycasting wasn't a requirement that I had to do for the project so I believe this is fine. 

I did have a second accidental AI use where copilot automatically added the name to the README updates. I only realized its doing that to the README name after updating it for a few times. This wasn't Intentional and everything I wrote in the README itself was without any AI help. 

### Bilal Rehman 
I had a few issues with fmod not properly loading the audio assets so I have used AI to help me remove the errors and debug my code since I am not familiar with fmod. AI was used for fixing errors pertaining mismatching the naming of some functions as well as the GUI layout showing errors. I also made some logic errors in my code so AI was used to help with that. AI was only used to help me with the errors. 
