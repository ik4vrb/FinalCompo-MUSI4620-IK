using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequencerDrum : MonoBehaviour
{
    public LibPdInstance patch;
    float ramp;
    float t;
    int[] mode;
    int count = 0;

    [SerializeField]
    List<bool> Steps;
    GameObject[] StepsObjs;
    Material[] mat;
    System.Random rndPos = new System.Random();

    [SerializeField]
    List<bool> kick;
    [SerializeField]
    List<bool> snare;
    [SerializeField]
    List<bool> sticks;
    public List<AudioClip> sounds;
    string[] drum_type = new string[] { "Kick", "Snare", "Sticks" };
    List<float> envelopes = new List<float>();
    List<bool>[] gates = new List<bool>[3];
    Vector4 adsr_params;

    void Start()
    {
        StepsObjs = new GameObject[Steps.Count];
        mat = new Material[Steps.Count];
        for (int i = 0; i < Steps.Count; i++)
        {
            float normalized_ind = (float)i / StepsObjs.Length;
            StepsObjs[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            StepsObjs[i].transform.position = new Vector3(MathF.Cos(normalized_ind*MathF.PI*2),
                MathF.Sin(normalized_ind * MathF.PI * 2), 0);
            StepsObjs[i].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            mat[i] = StepsObjs[i].GetComponent<Renderer>().material;
        }

        for (int i = 0; i < sounds.Count; i++)
        {
            //send sound files names to patch
            //add .wav
            //drum type is both the name of receive obj 
            //and of Drums folder subdirectory for sound
            string name = sounds[i].name + ".wav";
            patch.SendSymbol(drum_type[i], name);
            //build list of envelopes
            envelopes.Add(0);
        }
        gates[0] = kick;
        gates[1] = snare;
        gates[2] = sticks;
        adsr_params = new Vector4(100, 50, 0.4f, 200);
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        bool trig = ramp > (ramp + Time.deltaTime) % 1;
        ramp = (ramp + Time.deltaTime) % 1;

        if (trig)
        {
            if (kick[count])
            {
                patch.SendBang("kick_bang");
                double randSize = (rndPos.Next(5)/5.0 - 0.5) * 2;
                StepsObjs[count].transform.localScale = new Vector3((float)randSize,
                (float)randSize, (float)randSize);
            }
            if (snare[count])
            {
                patch.SendBang("snare_bang");
                int numX = rndPos.Next(3);
                int numY = rndPos.Next(3);
                int numZ = rndPos.Next(3);
                mat[count].color = new Color(numX, numY, numZ);
            }
            if (sticks[count])
            {
                patch.SendBang("sticks_bang");
                double randomZ = rndPos.Next(2)/1.0 - 1;
                float normalized_ind = (float)count / StepsObjs.Length;
                StepsObjs[count].transform.position = new Vector3(MathF.Cos(normalized_ind * MathF.PI * 2),
                MathF.Sin(normalized_ind * MathF.PI * 2), (float)randomZ);
            }
            int numR = rndPos.Next(3);
            int numG = rndPos.Next(3);
            int numB = rndPos.Next(3);
            mat[count].color = new Color(numR, numG, numB);
            count = (count + 1) % kick.Count;
        }

        for (int i = 0; i < sounds.Count; i++)
        {
            envelopes[i] = ControlFunctions.ADSR(ramp/1000, gates[i][count], adsr_params);
        }
    }
}
