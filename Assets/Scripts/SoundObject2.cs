using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundObject2 : MonoBehaviour
{
    public LibPdInstance patch;
    float ramp;
    float t;
    int[] pitchArr;
    void Start()
    {
        pitchArr = ControlFunctions.PitchArray(0, new Vector2Int(72, 84), new int[] { 2, 1, 2, 2, 2, 1 });
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        float lfo = ControlFunctions.Sin(t, 4*0.1522f, 0);
       
        int pitch_ind = Mathf.RoundToInt((lfo * 0.5f + 0.5f) * (pitchArr.Length-1));
        Debug.Log(pitchArr[pitch_ind]);
        bool trig = ramp > ((ramp + Time.deltaTime) % 1);
        ramp = (ramp + Time.deltaTime) % 1;
        if(trig)
        {
            patch.SendMidiNoteOn(0, pitchArr[pitch_ind], 60);
        }
        
        patch.SendList("ADSR", 500, 100, 50, 50, 300);
        transform.position = new Vector3(0, lfo * 4, 0);
    }
}
