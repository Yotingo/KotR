using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudioSFX : MonoBehaviour
{
	public AudioSource arrow1;
	public AudioSource arrow2;
	public AudioSource type;
	public AudioSource joinLaugh;
	public AudioSource joinLaughRare;
	public AudioSource openClose;
	
	public void playArrowSound()
	{
		int r = Random.Range(0, 2);
		if( r > 0 )
		{
			arrow1.PlayOneShot(arrow1.clip);
		}
		else
		{
			arrow2.PlayOneShot(arrow2.clip);
		}
	}

	public void playJoinLaugh()
	{
		float jr = Random.Range(0f, 1f);
		if( jr < .9f )
		{
			joinLaugh.PlayOneShot(joinLaugh.clip);
		}
		else
		{
			joinLaughRare.PlayOneShot(joinLaughRare.clip);
		}
	}
}
