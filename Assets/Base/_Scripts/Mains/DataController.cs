using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class DataController : MonoSing<DataController>
{
    [SerializeField] private BoardManager board;
    private string levelDataFileName = "/levels.json";
    private string stickmanDataFileName = "/stickmans.json";
    /// <summary>
    /// BoardManager Sınıfındaki _levelsInBoard Listesindeki Verileri levels.json Adında Bir Dosyaya Kaydeder - 
    /// Saves Data from _levelsInBoard List in BoardManager Class to a File Named levels.json
    /// </summary>
    public void SaveLevel()
    {
        string jsonLevel = JsonConvert.SerializeObject(board._levelsInBoard);
        try
        {
            if (File.Exists(Application.persistentDataPath + levelDataFileName))
            {
                File.Delete(Application.persistentDataPath + levelDataFileName);
                File.WriteAllText(Application.persistentDataPath + levelDataFileName, jsonLevel);
            }
            else
            {
                File.WriteAllText(Application.persistentDataPath + levelDataFileName, jsonLevel);
            }
        }
        catch (IOException e)
        {
            Debug.LogError($"An error occurred while deleting the file: {e.Message}");
        }
    }
    /// <summary>
    /// BoardManager Sınıfındaki _squaresFilled Listesindeki Verileri stickman.json Adında Bir Dosyaya Kaydeder - 
    /// Saves Data from _squaresFilled List in BoardManager Class to a File Named stickmans.json
    /// </summary>
    public void SaveStickman()
    {
        string stickmanLevel = JsonConvert.SerializeObject(board._squaresFilled);
        try
        {
            if (File.Exists(Application.persistentDataPath + stickmanDataFileName))
            {
                File.Delete(Application.persistentDataPath + stickmanDataFileName);
                File.WriteAllText(Application.persistentDataPath + stickmanDataFileName, stickmanLevel);
            }
            else
            {
                File.WriteAllText(Application.persistentDataPath + stickmanDataFileName, stickmanLevel);
            }
        }
        catch (IOException e)
        {
            Debug.LogError($"An error occurred while deleting the file: {e.Message}");
        }
    }
    /// <summary>
    /// levels.json Dizi Verilerinin Tutulduğu Dosyadaki Verileri BoardManager Sınıfındaki _levelsInBoard Listesine Aktarır - 
    /// levels.json Transfers Data from Array Data File to _levelsInBoard List in BoardManager Class
    /// </summary>
    public void LoadLevel()
    {
        if (File.Exists(Application.persistentDataPath + levelDataFileName))
        {
            string readLevelData = File.ReadAllText(Application.persistentDataPath + levelDataFileName);
            System.Collections.Generic.List<int> tempList = JsonConvert.DeserializeObject<System.Collections.Generic.List<int>>(readLevelData);
            if (tempList != null)
                board._levelsInBoard = tempList;
        }
    }
    /// <summary>
    /// stickmans.json Dizi Verilerinin Tutulduğu Dosyadaki Verileri BoardManager Sınıfındaki _squaresFilled Listesine Aktarır - 
    /// stickmans.json Transfers Data from Array Data File to _squaresFilled List in BoardManager Class
    /// </summary>
    public void LoadStickman()
    {
        if (File.Exists(Application.persistentDataPath + stickmanDataFileName))
        {
            string readStickmanData = File.ReadAllText(Application.persistentDataPath + stickmanDataFileName);
            System.Collections.Generic.List<int> tempList = JsonConvert.DeserializeObject<System.Collections.Generic.List<int>>(readStickmanData);
            if (tempList != null)
                board._squaresFilled = tempList;
        }
    }
    /// <summary>
    /// Sahne Yüklendiğinde Oluşucak Kayıtlı Board Formasyonunun Tutulduğu Veri Dosyalarını Siler - 
    /// Deletes the Data Files Keeping the Registered Board Formation that Will Be Created When the Scene is Loaded
    /// </summary>
    public void DeleteDataFiles()
    {
        if (File.Exists(Application.persistentDataPath + levelDataFileName))
            File.Delete(Application.persistentDataPath + levelDataFileName);
        if (File.Exists(Application.persistentDataPath + stickmanDataFileName))
            File.Delete(Application.persistentDataPath + stickmanDataFileName);
    }
}