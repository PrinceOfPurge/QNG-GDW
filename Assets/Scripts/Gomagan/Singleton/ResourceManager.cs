using System.Collections;
using System.Collections.Generic;
using Chapter.Singleton;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    private const int MAXBULLETS = 6;
    private const int MAXGRENADES = 5;
    private const int MAXMEDPACKS = 3;

    private int bullets = 6;
    private int grenades = 1;
    private int medPacks = 0;
    
    public int currentResourceCount = 0;

    public bool AddResource(string resource)
    {
        switch (resource)
        {
            case "Bullet":
                if (bullets >= MAXBULLETS)
                {
                    print("Max Bullets reached");
                    return false;
                }
                bullets++;
                print("Added 1 Bullet");
                return true;
            case "Grenade":
                if (grenades >= MAXGRENADES)
                {
                    print("Max Grenades reached");
                    return false;
                }
                grenades++;
                print("Added 1 Grenade");
                return true;
            case "MedPack":
                if (medPacks >= MAXMEDPACKS)
                {
                    print("Max Medpacks reached");
                    return false;
                }
                medPacks++;
                print("Added 1 MedPack");
                return true;
        }
    
        return false;
    }

    public bool UseResource(string resource)
    {
        switch (resource)
        {
            case "Bullet":
                if (bullets > 0)
                {
                    bullets--;
                    print("Used Bullet");
                    return true;
                }
                print("No Bullets left");
                return false;
            case "Grenade":
                if (grenades > 0)
                {
                    grenades--;
                    print("Used Grenade");
                    return true;
                }
                print("No Grenades left");
                return false;
            case "MedPack":
                if (medPacks > 0)
                {
                    medPacks--;
                    print("Used MedPack");
                    return true;
                }
                print("No MedPacks left");
                return false;
        }
    
        return false;
    }
}
