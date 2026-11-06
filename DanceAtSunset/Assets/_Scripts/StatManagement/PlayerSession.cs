public static class PlayerSession
{
    public static PlayerData PlayerData
    {
        get
        {
            if(_playerData == null)
                StartNewGame();

            return _playerData;
        }
        set
        {
            _playerData = value;
        }
    }
    private static PlayerData _playerData;

    // Called when starting a new game
    public static void StartNewGame()
    {
        _playerData = new PlayerData();
    }

    // Called when loading a save
    public static void LoadGame(GameSaveData save)
    {
        BaseStats stats = new BaseStats(
            save.PlayerStats.maxHealth,
            save.PlayerStats.currentHealth,
            save.PlayerStats.attack,
            save.PlayerStats.defense
        );

        PlayerData = new PlayerData(
            stats,
            save.PlayerStats.level,
            save.PlayerStats.exp,
            save.PlayerStats.totalPlayTime
        );
    }

    public static void Clear()
    {
        PlayerData = null;
    }
}
