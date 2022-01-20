using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GameController : MonoBehaviour
{
    // Singleton Pattern
    public static GameController Instance => gameController;
    private static GameController gameController;

    // Game Audio
    public AudioMixer MasterMixer => masterMixer;
    [SerializeField]
    private AudioMixer masterMixer;
    [Range(-50, 20)]
    public float initialGameVolume = 0;

    // Game Data
    private bool playerDead;
    private bool playerFinished;

    /* Level Controller contains the reference for the specific objects in each level.
     * In this regard, GameController will only act as an accessible interface for funcionality
     * that must be called in other scripts.
     */
    public LevelController LevelController
    {
        get
        {
            /*  GameController survives scene reloads, so after changing rooms, _roomController is not null. It is a reference to a destroyed UnityEngine.Object.
                (C# objects can't be explicitly destroyed, so the C++ object is destroyed and the C# object is marked as dead).
                However, UnityEngine.Object overrides the == operator to account for this, and makes it so that (destroyedObjectReference == null) is true.
                Casting to bool does the same thing. (!roomController) is the same as (roomController == null).

                Because of all that, the reference to the old room doesn't need to be cleared. 
                The object will eventually be garbaged collected after the reference is overwritten by a new room.
            */

            if (!levelController)
            {
                levelController = FindObjectOfType<LevelController>();
            }

            return levelController;
        }
    }
    private LevelController levelController;

    void Awake()
    {
        if (gameController != null && gameController != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            gameController = this;
            DontDestroyOnLoad(this.gameObject);
            OnFirstAwake();
        }

        /* As the destruction of non-singleton GameControllers is delayed until the end of the frame,
         * the code below will allways be called on load from the GameControllers found in each scene.
         * We make sure to contain only references to Instance inside it, so we update the permanent GameController object.
         */
        GameController.Instance.InitOnAwake();
    }

    void OnFirstAwake()
    {
        SetMasterVolume(initialGameVolume);
    }

    void InitOnAwake()
    {
        playerDead = false;
        playerFinished = false;
    }

    #region Audio Controls 
    public void SetMasterVolume(float volume)
    {
        masterMixer.SetFloat("masterVolume", volume);
    }

    public float GetMasterVolume()
    {
        float volume;
        MasterMixer.GetFloat("masterVolume", out volume);
        return volume;
    }
    #endregion

    #region Victory and Defeat Conditions
    public void FinishLevel()
    {
        playerFinished = true;
        MouseLook.ToggleCameraAndCursor(false);
        AudioController.ChangeLevelMusic(5);
        LevelController.OpenCanvas(LevelController.VICTORY_CANVAS_ID);
    }

    public void Death()
    {
        playerDead = true;
        MouseLook.ToggleCameraAndCursor(false);
        AudioController.ChangeLevelMusic(6);
        LevelController.OpenCanvas(LevelController.DEATH_CANVAS_ID);
    }

    public bool HasGameFinished()
    {
        return playerDead || playerFinished;
    }
    #endregion

    public void GoToNextStage()
    {
        LevelController.GoToNextStage();
    }
}
