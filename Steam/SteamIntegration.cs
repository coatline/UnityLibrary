using System.Collections.Generic;
using UnityEngine;

public class SteamIntegration : Singleton<SteamIntegration>
{
    [SerializeField] bool clearData;

    bool initialized = false;

    private void OnValidate()
    {
        if (clearData)
        {
            ClearAllAchievements();
            clearData = false;
        }
    }

    protected override void Awake()
    {
        base.Awake();

        try
        {
            Steamworks.SteamClient.Init(3269830);
            print($"Welcome Steam User: {Steamworks.SteamClient.Name}");

            Steamworks.SteamFriends.OnGameOverlayActivated += OnOverlayActivated;
            initialized = true;

            TryUnlockAchievement("LOAD_GAME");
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
        }
    }

    private void OnOverlayActivated(bool isActive)
    {
        if (isActive)
            if (GameManager.I)
                PauseMenu.I.TogglePause(PlayerManager.I.GetDrivingPlayer);
    }

    public bool IsThisAchievementUnlocked(string id)
    {
        if (initialized == false)
            return false;

        Steamworks.Data.Achievement achievement = new Steamworks.Data.Achievement(id);
        return achievement.State;
    }

    public void TryUnlockAchievement(string id)
    {
        if (initialized == false)
            return;

        if (IsThisAchievementUnlocked(id))
            return;

        Steamworks.Data.Achievement achievement = new Steamworks.Data.Achievement(id);
        achievement.Trigger();

        Steamworks.SteamUserStats.StoreStats();

        Debug.Log($"Achievement {id} unlocked.");
    }

    public void ClearAchievementStatus(string id)
    {
        if (IsThisAchievementUnlocked(id) == false)
            return;

        Steamworks.Data.Achievement achievement = new Steamworks.Data.Achievement(id);
        achievement.Clear();

        Debug.Log($"Achievement {id} cleared.");
    }

    public void ClearAllAchievements()
    {
        Steamworks.SteamUserStats.ResetAll(true);
        Steamworks.SteamUserStats.StoreStats();
    }

    void Update()
    {
        if (initialized)
            Steamworks.SteamClient.RunCallbacks();
    }

    private void OnDestroy()
    {
        if (initialized)
            Steamworks.SteamFriends.OnGameOverlayActivated -= OnOverlayActivated;
    }

    private void OnApplicationQuit()
    {
        if (initialized)
            Steamworks.SteamClient.Shutdown();
    }
}