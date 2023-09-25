using IA.FSM.Carriage;
using IA.FSM.Villager;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class VillagerAdmin : MonoBehaviour
{
    public System.Collections.Concurrent.ConcurrentBag<Villager> villagers = new System.Collections.Concurrent.ConcurrentBag<Villager>();
    public System.Collections.Concurrent.ConcurrentBag<Carriage> carriages = new System.Collections.Concurrent.ConcurrentBag<Carriage>();

    public List<Villager> villagersForBag;
    public List<Carriage> carriagesForBag;

    private void Start()
    {
        foreach(Villager villager in villagersForBag)
        {
            villagers.Add(villager);
        }

        foreach (Carriage carriage in carriagesForBag)
        {
            carriages.Add(carriage);

        }

        ParallelOptions options = new ParallelOptions { MaxDegreeOfParallelism = 8 };

        Parallel.ForEach(villagers, options, currentItem =>
        {
            currentItem.Update();
        });

        Parallel.ForEach(carriages, options, currentItem =>
        {
            currentItem.Update();
        });
    }
}
