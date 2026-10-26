Group Members: 

  Christopher Shin:
    - Wave Manager/Game Manager Singleton
    - Zombie Spawning Factory design 
    - Custom button mapping command pattern design
    
  Bilal Rehman:
    - Audio Manager Singleton
    - Character Customization command
    - Trees factory

  Gomagan Prabagar:
    - Resource Manager
    - World Editing
    - Ill add third later


The way we created this group is by seeing who in the class is someone that we have worked with in the past. We also decided based on the projects we are working on, such as a GDW game. 




What was Done: 

Christopher:

Singleton:
I built a GameManager singleton where only one exists in the scene and manages waves of enemies. This is done by creating a base singleton script that the GameManager would inherit from. The base singleton checks if there are others like it and deletes them, and runs the Don't Destroy On Load function to make it persistent. The reason I made it this way is so that it's easier to manage enemy spawning. This would benefit the game, as there would be a lot of zombie enemies, and it would require a wave system. I think it would also be useful so that other scripts can access the function in the GameManager. 

Command Design Pattern:
I followed this video https://www.youtube.com/watch?v=oLRINAn0cuw to go into more depth on how command design patterns work and are used in Unity. First, I created an abstract command class that has the abstract method Execute. Then I made a shootCommand script that inherits from it. This script executes the shoot function in playerShoot. The reason I implemented it this way is so you can track which shooting action the player has done. This is useful for this project as there could be different ways to shoot and guns that use different ammo. 


Factory:
I built a factory that can spawn different types of zombies or enemies. I first made a base Enemyspawner and a base enemy script, both are abstract, so the child has to inherit their functions. The base enemy has public and protected variables and functions that the child can inherit. The base EnemySpawner has an abstract spawn method that the child has to override to write their own implementation. I did it this way so that you can spawn entities without knowing what class they are, and it makes spawning enemies easier. This is great for this game as it is all about clearing hordes of enemies and fighting many different ones. 


About the Game:

  Awakened is a monster shooter where the player is stuck in a car. The storyline will start with Sheriff going to investigate a missing persons report when they hear some breaking news on their radio. This broadcast will be about how monsters have taken over their town and to hide and take shelter. Hearing that the protagonist crashes into a barricade. The player has a basic pistol and has limited ammo to start. The player must open the door to get a better view and shoot the zombies. The added layer of difficulty is that the player can only look in one direction, left or right. Switching between sides has a short delay. 
