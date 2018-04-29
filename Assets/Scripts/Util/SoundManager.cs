using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<AudioClip> SEList;
    protected override void Init()
    {
        base.Init();
        Debug.Log("サウンドマネージャー");
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySE(SEenum se)
    {
        audioSource.PlayOneShot(SEList[(int)se]);
    }

    public void PlaySE(SEenum se,float volume)
    {
        audioSource.PlayOneShot(SEList[(int)se], volume);
    }
    public void PlaySE(SEenum se, float volume, float time)
    {
        StartCoroutine(DeleySE(time, se, volume));
    }
    private IEnumerator DeleySE(float time, SEenum se, float volume)
    {
        yield return new WaitForSeconds(time);  //停止
        audioSource.PlayOneShot(SEList[(int)se], volume);

    }

    public enum SEenum
    {
        PlayerWalk,
        PlayerResporn,
        PlayerDead,
        GoalEffect,
        EnemyPatrol,
        EnemyChase,
        LiquidsBubbling
    }
}
