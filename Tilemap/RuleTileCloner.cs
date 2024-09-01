using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class RuleTileCloner : EditorWindow
{
    private RuleTile referenceTile;
    private Texture2D sprite;

    [MenuItem("Tools/RuleTile Cloner")]
    public static void ShowTool()
    {
        GetWindow<RuleTileCloner>().Show();
    }

    public void OnGUI()
    {
        referenceTile = EditorGUILayout.ObjectField("Reference Tile", referenceTile, typeof(RuleTile), false) as RuleTile;
        sprite = EditorGUILayout.ObjectField("Sprite", sprite, typeof(Texture2D), false) as Texture2D;

        if (GUILayout.Button("Clone"))
        {
            CloneTile();
        }
    }

    private void CloneTile()
    {
        if (referenceTile == null || sprite == null)
        {
            Debug.LogError("Reference Tile and Sprite must be assigned.");
            return;
        }

        string origPath = AssetDatabase.GetAssetPath(referenceTile);
        string spritePath = AssetDatabase.GetAssetPath(sprite);
        string targetPath = $"{origPath.Substring(0, origPath.LastIndexOf('/'))}/{char.ToUpper(sprite.name[0]) + sprite.name.Substring(1)}.asset";

        Debug.Log($"Copying asset to: {targetPath}");

        AssetDatabase.CopyAsset(origPath, targetPath);
        RuleTile newTile = AssetDatabase.LoadAssetAtPath<RuleTile>(targetPath);

        if (newTile != null)
        {
            CloneBySpriteIndex(spritePath, newTile, referenceTile);
        }
        else
        {
            Debug.LogError("Failed to load the new tile from the target path.");
            return;
        }

        EditorUtility.SetDirty(newTile);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void CloneBySpriteIndex(string spritePath, RuleTile newTile, RuleTile refTile)
    {
        Sprite[] refSprites = LoadAndSortSprites(refTile.m_TilingRules[0].m_Sprites[0].texture);
        Sprite[] newSprites = LoadAndSortSprites(spritePath);

        if (newSprites.Length != refSprites.Length)
        {
            Debug.LogError("The number of new sprites does not match the number of reference sprites.");
            return;
        }

        for (int i = 0; i < newTile.m_TilingRules.Count; i++)
        {
            RuleTile.TilingRule rule = newTile.m_TilingRules[i];

            for (int j = 0; j < rule.m_Sprites.Length; j++)
            {
                int refIndex = FindIndex(refSprites, rule.m_Sprites[j]);
                if (refIndex >= 0 && refIndex < newSprites.Length)
                {
                    rule.m_Sprites[j] = newSprites[refIndex];
                }
                else
                {
                    Debug.LogError("Sprite index out of bounds.");
                }
            }
        }

        if (refTile.m_DefaultSprite != null)
        {
            int defaultIndex = FindIndex(refSprites, refTile.m_DefaultSprite);
            if (defaultIndex >= 0 && defaultIndex < newSprites.Length)
            {
                newTile.m_DefaultSprite = newSprites[defaultIndex];
            }
            else
            {
                Debug.LogError("Default sprite index out of bounds.");
            }
        }

        EditorUtility.SetDirty(newTile);
    }

    private Sprite[] LoadAndSortSprites(Texture2D texture)
    {
        string path = AssetDatabase.GetAssetPath(texture);
        Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().ToArray();
        return SortSpritesByNumberInName(sprites);
    }

    private Sprite[] LoadAndSortSprites(string path)
    {
        Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().ToArray();
        return SortSpritesByNumberInName(sprites);
    }

    private int FindIndex(Sprite[] spriteArray, Sprite sprite)
    {
        for (int i = 0; i < spriteArray.Length; i++)
        {
            if (spriteArray[i] == sprite)
                return i;
        }

        return -1;
    }

    private Sprite[] SortSpritesByNumberInName(Sprite[] sprites)
    {
        return sprites.OrderBy(s => ExtractNumberFromName(s.name)).ToArray();
    }

    private int ExtractNumberFromName(string name)
    {
        int i = name.Length - 1;
        while (i >= 0 && char.IsDigit(name[i]))
        {
            i--;
        }

        string numberPart = name.Substring(i + 1);

        if (int.TryParse(numberPart, out int number))
        {
            return number;
        }
        else
        {
            Debug.LogError($"Failed to extract number from sprite name: {name}");
            return int.MaxValue;
        }
    }
}




//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;
//using System.Linq;

//public class RuleTileCloner : EditorWindow
//{
//    private RuleTile referenceTile;
//    private Texture2D sprite;

//    [MenuItem("Tools/RuleTile Cloner")]
//    public static void ShowTool()
//    {
//        GetWindow<RuleTileCloner>().Show();
//    }

//    public void OnGUI()
//    {
//        referenceTile = EditorGUILayout.ObjectField(referenceTile, typeof(RuleTile), false) as RuleTile;
//        sprite = EditorGUILayout.ObjectField(sprite, typeof(Texture2D), false) as Texture2D;

//        if (GUILayout.Button("Clone"))
//        {
//            string origPath = AssetDatabase.GetAssetPath(referenceTile);
//            string spritePath = AssetDatabase.GetAssetPath(sprite);
//            string targetPath = $"{origPath.Substring(0, origPath.LastIndexOf('/'))}/{char.ToUpper(sprite.name[0]) + sprite.name.Substring(1)}.asset";

//            Debug.Log($"Copied to: {targetPath}. Originalpath: {origPath}");

//            AssetDatabase.CopyAsset(origPath, targetPath);
//            RuleTile oldTile = AssetDatabase.LoadAssetAtPath<RuleTile>(origPath);
//            RuleTile newTile = AssetDatabase.LoadAssetAtPath<RuleTile>(targetPath);

//            if (newTile != null)
//            {
//                CloneBySpriteIndex(spritePath, newTile, oldTile);
//            }

//            AssetDatabase.SaveAssets();
//        }
//    }

//    private void CloneBySpriteIndex(string spritePath, RuleTile newTile, RuleTile refTile)
//    {
//        // First load in the data for the reference tile.
//        Texture2D refTex = refTile.m_TilingRules[0].m_Sprites[0].texture;
//        string refPath = AssetDatabase.GetAssetPath(refTex);
//        Sprite[] refSprites = AssetDatabase.LoadAllAssetsAtPath(refPath).OfType<Sprite>().ToArray();
//        refSprites = SortSpritesByNumberInName(refSprites);

//        // New rule tile created, now to swap out the sprites.
//        Sprite[] newSprites = AssetDatabase.LoadAllAssetsAtPath(spritePath).OfType<Sprite>().ToArray();
//        newSprites = SortSpritesByNumberInName(newSprites);

//        for (int i = 0; i < newTile.m_TilingRules.Count; i++)
//        {
//            RuleTile.TilingRule rule = newTile.m_TilingRules[i];

//            for (int j = 0; j < rule.m_Sprites.Length; j++)
//            {
//                int refIndex = FindIndex(refSprites, rule.m_Sprites[j]);
//                Debug.Log(refIndex);
//                rule.m_Sprites[j] = newSprites[refIndex];
//            }
//        }

//        if (referenceTile.m_DefaultSprite != null)
//        {
//            newTile.m_DefaultSprite = newSprites[15];
//        }

//        AssetDatabase.SaveAssets();
//    }

//    private int FindIndex(Sprite[] spriteArray, Sprite sprite)
//    {
//        for (int i = 0; i < spriteArray.Length; i++)
//        {
//            Debug.Log(spriteArray[i].name);
//            if (spriteArray[i] == sprite)
//                return i;
//        }

//        return -1;
//    }

//    Sprite[] SortSpritesByNumberInName(Sprite[] sprites)
//    {
//        return sprites.OrderBy(s => ExtractNumberFromName(s.name)).ToArray();
//    }

//    int ExtractNumberFromName(string name)
//    {
//        // Find the start of the number by iterating backwards from the end of the name
//        int i = name.Length - 1;
//        while (i >= 0 && char.IsDigit(name[i]))
//        {
//            i--;
//        }

//        // Extract the number part of the name
//        string numberPart = name.Substring(i + 1);

//        // Convert the number part to an integer
//        if (int.TryParse(numberPart, out int number))
//        {
//            return number;
//        }
//        else
//        {
//            Debug.LogError($"Failed to extract number from sprite name: {name}");
//            return int.MaxValue; // Return a high number to handle names without numbers
//        }
//    }
//}