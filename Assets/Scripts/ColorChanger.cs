using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Collections;

// Character changer
public class ColorChanger : MonoBehaviour
{
	public List<Material> characters;
	public Text chrIdxDisplayTxt;
	[ReadOnly]
	public int chrIdx;

    // Start is called before the first frame update
    void Start()
    {
		chrIdx = Random.Range(0, characters.Count);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectNextCharacter(int nextHigher)
    {
		chrIdx += nextHigher;

		if( chrIdx >= characters.Count )
			chrIdx = 0;
		else if( chrIdx < 0 )
			chrIdx = characters.Count-1;

		if( chrIdxDisplayTxt != null )
			chrIdxDisplayTxt.text = (chrIdx+1).ToString("D2");

		Material newMaterial = characters[chrIdx];
        foreach(Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.material = newMaterial;
        }
    }
}
