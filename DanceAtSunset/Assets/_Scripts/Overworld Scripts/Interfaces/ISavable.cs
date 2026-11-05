using UnityEngine;

public interface ISavable
{
    void CaptureSaveData(GameSaveData data);
    void RestoreSaveData(GameSaveData data);
}
