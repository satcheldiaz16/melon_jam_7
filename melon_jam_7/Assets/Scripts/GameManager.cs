using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] AudioSource pursuit_sfx;
    public List<TestCreature> creatures_pursuing_player = new();
    void Awake()
    {
        instance = this;
    }
    public void AddToPursuingPlayer(TestCreature c)
    {
        if(creatures_pursuing_player.Contains(c)) return;

        
        creatures_pursuing_player.Add(c);
        if(!pursuit_sfx.isPlaying) pursuit_sfx.Play();
    }
    public void RemoveFromPursuingPlayer(TestCreature c)
    {
        if(!creatures_pursuing_player.Contains(c)) return;

        Debug.Log("removed");
        creatures_pursuing_player.Remove(c);
        if(creatures_pursuing_player.Count < 1) pursuit_sfx.Stop();
    }
}
