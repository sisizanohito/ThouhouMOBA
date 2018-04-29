using System.Collections;
using System.Collections.Generic;
using UnityEngine.Video;
using UnityEngine;

public class VideoManager : SingletonMonoBehaviour<VideoManager>
{
    public VideoClip titleMovie;
    public VideoClip clearMovie;

    public bool IsPlay { get { return videoPlayer.isPlaying; } }
    [SerializeField] private VideoClip playVideoClip;


    [SerializeField] private VideoPlayer videoPlayer;


    protected override void Init()
    {
        base.Init();
        Debug.Log("videoManager");
    }



    public void SetVideoClip(string videoName)
    {
        if (videoName == "title")
            playVideoClip = titleMovie;
        else if (videoName == "clear")
        {
            playVideoClip = clearMovie;

        }
        videoPlayer.clip = playVideoClip;

    }

    public void SetMainCamToVideo()
    {
        videoPlayer = GameObject.FindWithTag("MainCamera").AddComponent<VideoPlayer>();
    }


    public void PlayVideo()
    {
        videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
        videoPlayer.Play();
    }

    public void PauseVideo()
    {
        videoPlayer.Pause();
    }

    public void ForwardVideoFlame()
    {
        videoPlayer.StepForward();
    }

}
