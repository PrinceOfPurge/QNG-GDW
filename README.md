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



Command Design Pattern:
I followed this video https://www.youtube.com/watch?v=oLRINAn0cuw to go into more depth on how command design patterns work and are used in Unity. First, I created an abstract command class that has the abstract method Execute. Then I made a shootCommand script that inherits from it. This script executes the shoot function in playerShoot. The reason I implemented it this way is so you can track which shooting action the player has done. This is useful for this project as there could be different ways to shoot and guns that use different ammo. 
  


About the Game:

  Awakened is a monster shooter where the player is stuck in a car. The storyline will start with Sheriff going to investigate a missing persons report when they hear some breaking news on their radio. This broadcast will be about how monsters have taken over their town and to hide and take shelter. Hearing that the protagonist crashes into a barricade. The player has a basic pistol and has limited ammo to start. The player must open the door to get a better view and shoot the zombies. The added layer of difficulty is that the player can only look in one direction, left or right. Switching between sides has a short delay. 
