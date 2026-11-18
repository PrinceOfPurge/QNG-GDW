# Game Engine Design and Implementation

**Name:** Bilal Rehman  
**Student Number:** 100921929  

---

## Project Overview & Gameplay Loop
**Awakened** is a 3D interactive arcade/action game built in Unity where you play as a sheriff stuck in her car and must shoot and kill monsters trying to eat you.

**Gameplay Loop:**  
You constantly check your blind spots and must shoot and kill all the monsters while conserving ammo 
---

## Profiler Results

### Before Optimization 
<img width="2536" height="1525" alt="image" src="https://github.com/user-attachments/assets/aa895123-b717-4af3-993a-20dfe4ceb701" />



### After Optimization 
<img width="1024" height="615" alt="image" src="https://github.com/user-attachments/assets/420f63cf-b274-47f7-aeb4-2f78626a39ba" />


---

## Reflection Questions

### 1. What did you notice was impacting the performance of your project?
The thing that was impacting my performance the most was textures of different game objects in the game.
### 2. Which pattern(s) did you implement, and what behavior did they replace?
I implemented the Object Pooling and Factory and that replaced the enemy spawner. 
### 3. Do your patterns / technique implementations improve performance? Why or why not?
It did improve performance but by a little. not much difference between the first screenshot and the second screenshot. the game is already so basic so trying to make it more optimized is hard.
### 4. Can you demonstrate what the profiling process revealed?
Player loop execution time decreased by 13%. CPU frame stability is more flat and so is memory stabilization. 
