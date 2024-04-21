using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumShakes : MonoBehaviour
{
    [SerializeField] int beat;
    float t;
    public LibPdInstance pdPatch;
    float ramp;
    int count = 0;
    [SerializeField]
    List<bool> kick;
    [SerializeField]
    List<bool> snare;
    public List<AudioClip> sounds;
    string[] drum_type = new string[] { "Kick", "Snare" };
    List<float> envelopes;
    List<bool>[] gates = new List<bool>[2];
    Vector4 adsr_params;
    Vector4 shakes_params;
    [HideInInspector]
    public float cameraShake;
    bool is_shake;
    void Start()
    {
        envelopes = new List<float>();
        for (int i = 0; i < sounds.Count; i++)
        {
            string name = sounds[i].name + ".wav";
            pdPatch.SendSymbol(drum_type[i], name);
            envelopes.Add(0);

        }

        gates[0] = kick;
        gates[1] = snare;
        adsr_params = new Vector4(100, 150, .8f, 500);
        shakes_params = new Vector4(100, 10, .8f, 500);
        
    }


    void Update()
    {
        t += Time.deltaTime;
        int dMs = Mathf.RoundToInt(Time.deltaTime * 1000);
        bool trig = ramp > ((ramp + dMs) % beat);
        ramp = (ramp + dMs) % beat;
        

        for (int i = 0; i < sounds.Count; i++)
        {
            envelopes[i] = ControlFunctions.ADSR(ramp/1000, gates[i][count], adsr_params);
        }
        if (is_shake)
        {
            cameraShake = ControlFunctions.ADSR(ramp / 1000, kick[count], shakes_params);
        }
        if (trig)
        {
            if (kick[count])
            {
                pdPatch.SendBang("kick_bang");
                is_shake = true;
            }
            else is_shake = false;
            if (snare[count])
            {
                pdPatch.SendBang("snare_bang");
            }
            count = (count + 1) % kick.Count;

        }
        


    }
}
