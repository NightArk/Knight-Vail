using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "DialogData", menuName = "Dialog System/Dialog Data")]
public class DialogData : ScriptableObject
{
    [System.Serializable]
    public class CharacterData
    {
        public int idNumber;
        public int IdNumber { get { return idNumber; } } // Readonly property for ID number

        public string characterName;
        public Color color;

        // Constructor to set the idNumber
        public CharacterData(int id)
        {
            idNumber = id;
        }
    }

    public CharacterData[] characters;

    public CharacterData GetCharacterById(int id)
    {
        foreach (var character in characters)
        {
            if (character.idNumber == id)
            {
                return character;
            }
        }
        return null;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (characters != null)
        {
            for (int i = 0; i < characters.Length; i++)
            {
                if (characters[i] != null)
                {
                    characters[i].idNumber = i; // Set idNumber corresponding to index
                }
            }
        }
    }
#endif
}
