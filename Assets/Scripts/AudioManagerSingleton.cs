using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine.Android;

public class AudioManagerSingleton : Singleton<AudioManagerSingleton>
{
    [Header("FMOD Bus Paths")]
    [SerializeField] private string masterBusPath = "bus:/";
    [SerializeField] private string musicBusPath = "bus:/Music";
    [SerializeField] private string sfxBusPath = "bus:/Sfx";

    [Header("Default Volumes")]
    [Range(0f, 1f)][SerializeField] private float defaultMaster = 1f;
    [Range(0f, 1f)][SerializeField] private float defaultMusic = 0.9f;
    [Range(0f, 1f)][SerializeField] private float defaultSfx = 1f;

    private const string PrefMaster = "Audio_Master";
    private const string PrefMusic = "Audio_Music";
    private const string PrefSfx = "Audio_Sfx";

    private Bus _masterBus, _musicBus, _sfxBus;
    private bool _busesReady;

    private EventInstance _musicInstance;
    private EventReference _currentMusic;
    private bool _haveMusic;

    private float _master, _music, _sfx;

    public override void Awake()
    {
        base.Awake();
        if (Instance != this) return;
    }

    private IEnumerator Start()
    {
        if(Instance != this) yield break;
        yield return new WaitUntil(() => RuntimeManager.HaveAllBanksLoaded);
        _masterBus = RuntimeManager.GetBus(masterBusPath);
        _musicBus = RuntimeManager.GetBus(musicBusPath);
        _sfxBus = RuntimeManager.GetBus(sfxBusPath);
        _busesReady = true;
        _master = PlayerPrefs.GetFloat(PrefMaster,defaultMaster);
        _music = PlayerPrefs.GetFloat(PrefMusic,defaultMusic);
        _sfx = PlayerPrefs.GetFloat(PrefSfx,defaultSfx);
        ApplyVolumes();
    }
    private void OnDestroy()
    {
        if (_instanceIsThis()) ReleaseMusic(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
    private bool _instanceIsThis() => Instance == this;
    public void PlayOneShot(EventReference sound, Vector3 position = default)
    {
        if (sound.IsNull) return;
        RuntimeManager.PlayOneShot(sound, position);
    }
    public EventInstance CreateInstance(EventReference sound, GameObject attachTo = null)
    {
        EventInstance instance = RuntimeManager.CreateInstance(sound);
        if (attachTo == null) 
            RuntimeManager.AttachInstanceToGameObject(instance, attachTo);
        return instance;
    }
    public void StopInstance(EventInstance instance, bool allowFadeOut = true)
    {
        if (!instance.isValid()) return;
        instance.stop(allowFadeOut ? FMOD.Studio.STOP_MODE.ALLOWFADEOUT : FMOD.Studio.STOP_MODE.IMMEDIATE);
        instance.release();
    }
    public void PlayMusic(EventReference music)
    {
        if (music.IsNull) return;
        if (_haveMusic && _musicInstance.isValid() && _currentMusic.Guid.Equals(music.Guid))
            return;
        ReleaseMusic(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _musicInstance = RuntimeManager.CreateInstance(music);
        _musicInstance.start();
        _currentMusic = music;
        _haveMusic = true;
    }
    public void StopMusic(bool fadeOut = true)
    {
        ReleaseMusic(fadeOut ? FMOD.Studio.STOP_MODE.ALLOWFADEOUT : FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
    public void SetMusicParameter(string name, float value)
    {
        if (_haveMusic && _musicInstance.isValid()) _musicInstance.setParameterByName(name, value);
    }
    private void ReleaseMusic(FMOD.Studio.STOP_MODE mode)
    {
        if (_haveMusic && _musicInstance.isValid())
        {
            _musicInstance.stop(mode);
            _musicInstance.release();
        }
        _haveMusic = false;
    }
    public void SetGlobalParameter(string name, float value)
    {
        RuntimeManager.StudioSystem.setParameterByName(name, value);
    }
    public float MasterVolume { get => _master; set { _master = Mathf.Clamp01(value); SaveAndApply(PrefMaster, _master); } }
    public float MusicVolume { get => _music; set { _music = Mathf.Clamp01(value); SaveAndApply(PrefMusic, _music); } }
    public float SfxVolume { get => _sfx; set {  _sfx = Mathf.Clamp01(value); SaveAndApply(PrefSfx, _sfx); } }
    private void SaveAndApply(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
        ApplyVolumes();
    }
    private void ApplyVolumes()
    {
        if (!_busesReady) return;
        _masterBus.setVolume(_master);
        _musicBus.setVolume(_music);
        _sfxBus.setVolume(_sfx);
    }
    public void PauseAll(bool paused)
    {
        if(_busesReady) _masterBus.setPaused(paused);
    }
    public void PauseSfx(bool paused)
    {
        if(_busesReady) _sfxBus.setPaused(paused);
    }
    private void OnApplicationPause(bool pause)
    {
        if (_busesReady) _masterBus.setPaused(pause);
    }
}
