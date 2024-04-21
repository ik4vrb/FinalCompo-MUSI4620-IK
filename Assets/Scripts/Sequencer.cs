using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequencer : MonoBehaviour
{
    public LibPdInstance patch;
    float ramp;
    float t;
    int[] pitchArr;
    int count = 0;
    [SerializeField]
    List<bool> Steps;
    GameObject[] StepsObjs;
    System.Random rndPos = new System.Random();

    // Start is called before the first frame update
    void Start()
    {
        StepsObjs = new GameObject[Steps.Count];
        int[] modeArray = new int[Steps.Count];
        for (int i = 0; i < Steps.Count; i++)
        {
            StepsObjs[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            int numX = rndPos.Next(3);
            int numY = rndPos.Next(3);
            int numZ = rndPos.Next(3);
            StepsObjs[i].transform.position = new Vector3(numX, numY, numZ);
            StepsObjs[i].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            modeArray[i] = rndPos.Next(1) + 1;
        }
        pitchArr = ControlFunctions.PitchArray(0, new Vector2Int(48, 60), modeArray);
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        float lfo = ControlFunctions.Sin(t, 0.1522f, 0);
        int pitch_ind = Mathf.RoundToInt((lfo * 0.5f + 0.5f) * (pitchArr.Length - 1));
        Debug.Log(pitchArr[pitch_ind]);
        bool trig = ramp > ((ramp + Time.deltaTime) % 1);
        ramp = (ramp + Time.deltaTime) % 1;
        patch.SendList("ADSR", 500, 100, 50, 50, 300);
        if (trig)
        {
            if (Steps[count])
            {
                int numX2 = rndPos.Next(3);
                int numY2 = rndPos.Next(3);
                int numZ2 = rndPos.Next(3);
                StepsObjs[count].transform.position = new Vector3(numX2, numY2, numZ2);
                StepsObjs[count].GetComponent<Renderer>().material.color = new Color(numX2, numY2, numZ2);
                patch.SendMidiNoteOn(0, pitchArr[pitch_ind], 60);
            }
            count = (count + 1) % Steps.Count;
            Debug.Log(count);
        }
    }
}
