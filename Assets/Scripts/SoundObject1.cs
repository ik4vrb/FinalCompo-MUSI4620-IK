using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundObject1 : MonoBehaviour
{
    public LibPdInstance patch;
    float ramp;
    float t;
    [Range(0f, 4f)]
    [SerializeField] float lfo_freq;
    [SerializeField] float beat_sec;
    [Range(0f, 1f)]
    [SerializeField] float reverb;
    [SerializeField] Vector4 ADSR;
    [SerializeField] int gate;
    int[] pitchArr;

    void Start()
    {
        pitchArr = ControlFunctions.PitchArray(0, new Vector2Int(50, 80), new int[] { 2, 1, 2, 2, 2, 1 });
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        float lfo = ControlFunctions.Sin(t, lfo_freq, 0);
       
        int pitch_ind = Mathf.RoundToInt((lfo * 0.5f + 0.5f) * (pitchArr.Length-1));
        Debug.Log(pitchArr[pitch_ind]);
        bool trig = ramp > ((ramp + Time.deltaTime) % beat_sec);
        ramp = (ramp + Time.deltaTime) % beat_sec;
        if(trig)
        {
            patch.SendMidiNoteOn(0, pitchArr[pitch_ind], 60);
        }
        
        patch.SendList("ADSR", gate, ADSR.x, ADSR.y, ADSR.z, ADSR.w);
        transform.position = new Vector3(-2, lfo * 4, 0);
    }
}
