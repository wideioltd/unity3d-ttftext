using UnityEngine;
using System.Collections;

[AddComponentMenu("Text/TTFText DemoScenes Helpers/Tut1 Set Anim")]
public class Tut1SetAnim : MonoBehaviour
{
    TTFText tm;
    TTFSubtext st;
    public AnimationCurve ac1;


    // Use this for initialization
    void Start()
    {
        st = GetComponent<TTFSubtext>();
        tm = transform.parent.GetComponent<TTFText>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
