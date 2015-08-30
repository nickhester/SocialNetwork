using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionTrail {

    public List<KeyValuePair<int, bool>> trail = new List<KeyValuePair<int,bool>>();
    
    public ActionTrail(int person, bool action)
    {
        AddItem(person, action);
    }
    
    public ActionTrail(bool isNewTrail)
    {
    }

    public ActionTrail(string _recordedTrail)
    {
		string[] justData = _recordedTrail.Split(':');
        string[] tokens = justData[1].Split('-');
        foreach (string token in tokens)
        {
            if (token.Length > 0)
            {
                int num;
                int.TryParse(token.Substring(0, 1), out num);
                bool choice = true;
                string visibleString = token.Substring(1, 1);
                if (token.Substring(1, 1) == "F")
                {
                    choice = false;
                }
                KeyValuePair<int, bool> oneStep = new KeyValuePair<int, bool>(num, choice);
                trail.Add(oneStep);
            }
        }
    }
    
    public void AddItem(int person, bool action)
    {
        KeyValuePair<int, bool> tempkvp = new KeyValuePair<int, bool>(person, action);
        trail.Add(tempkvp);
    }
    
    public override string ToString()
    {
        string returnValue = "";
        if (trail != null && trail.Count > 0)
        {
            foreach (KeyValuePair<int, bool> step in trail)
            {
                returnValue += step.Key;
                if (step.Value == true) { returnValue += "T"; }
                else { returnValue += "F"; }
                returnValue += "-";
            }
        }
        return returnValue;
    }
}
