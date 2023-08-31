using IA.FSM.Villager;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class VillagerAdmin : MonoBehaviour
{
    public System.Collections.Concurrent.ConcurrentBag<Villager> villagers = new System.Collections.Concurrent.ConcurrentBag<Villager>();

    public List<Villager> villagersForBag;

    private void Start()
    {
        foreach(Villager villager in villagersForBag)
        {
            villagers.Add(villager);
        }

        ParallelOptions options = new ParallelOptions { MaxDegreeOfParallelism = 4 };

        Parallel.ForEach(villagers, options, currentItem =>
        {
            currentItem.Update();
        });
    }
}
