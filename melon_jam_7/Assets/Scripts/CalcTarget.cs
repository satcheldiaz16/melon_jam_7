using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Remember for future, player movement has give perception interface but no means to generate values it's passing in. 
// Need to add this interface to any object that will be generating light or noise as well
// Need to implement functions to determine perceived light and noise based on distance, will also have to account for obstacles with light
// 



#region Find Targets
public class CalcTarget : MonoBehaviour
{

    // called by enemy script, handles decision making and returning sorted target list
    public List<Target> GetTargets (List<Target> targets, GameObject enemy)
    {
        return null;
    }


    public Transform[] getVision(List<Target> targets)
    {
        Perceivables[] sorted = GetPerceived(targets);
        Transform[] transforms = new Transform[targets.Count];
        int index = 0;
        // Add func to find light relative to distance before passing in, also remove unecessary entries without any light 
        sorted = sorted.OrderBy(perceived => perceived.light).ToArray();
        foreach (Perceivables obj in sorted)
        {
            transforms[index] = obj.position;
            index++;
        }
        return transforms;
    }

    public Transform[] getHeard (List<Target> targets)
    {
        Perceivables[] sorted = GetPerceived(targets);
        Transform[] transforms = new Transform[targets.Count];
        int index = 0;
        // Add func to find noise relative to distance before passing in
        sorted = sorted.OrderBy(perceived => perceived.noise).ToArray();
        foreach (Perceivables obj in sorted)
        {
            transforms[index] = obj.position;
            index++;
        }
        return transforms;
    }

    // Makes a list of perceivedObjects which contains their transforms and noise + light
    private Perceivables[] GetPerceived (List<Target> targets)
    {
        Perceivables[] perceivedObjects = new Perceivables[targets.Count];
        int index = 0;
        foreach (Target target in targets)
        {
            GameObject noiseMaker = target.transform.root.gameObject;
            givePerception findObj = noiseMaker.GetComponent<givePerception>();
            perceivedObjects[index] = findObj.givePerception();
            perceivedObjects[index].addTransform(target.transform);
            index++;
        }

        return perceivedObjects;
    }

}
#endregion

#region Perception
public interface givePerception 
{
    Perceivables givePerception();
}

public class Perceivables
{
    public Transform position;
    public float noise;
    public float light;

    public Perceivables (float sound, float lit)
    {
        noise = sound;
        light = lit;
    }

    public void addTransform (Transform pos)
    {
        position = pos;
    }

}
#endregion