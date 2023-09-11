using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISavableData
{
    void SaveData(ref GameData data);
    void LoadData(GameData data);
}
