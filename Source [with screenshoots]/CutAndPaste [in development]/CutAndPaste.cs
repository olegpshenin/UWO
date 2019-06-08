using System.IO;
using UnityEditor;
using UnityEngine;

namespace PB
{
    public class CutAndPaste
    {
        private static string _path;
        private static Object[] _cuttedObjects;
        private static Object[] _targetObjects;

        [MenuItem("Assets/Cut")]
        private static void CopyAsset()
        {
            _path = "";

            _cuttedObjects = Selection.objects;
        }

        [MenuItem("Assets/Paste")]
        private static void PasteAsset()
        {
            _targetObjects = Selection.objects;
            if (_targetObjects.Length == 1)
            {
                var targetObject = _targetObjects[0];
                _path = AssetDatabase.GetAssetPath(targetObject.GetInstanceID());

                if (_path.Length > 0)
                {
                    if (Directory.Exists(_path))
                    {
                        foreach (var assetToPaste in _cuttedObjects)
                        {
                            var path = GetAssetPath(assetToPaste);
                            MoveAssetTo(path, _path);
                        }

                        _cuttedObjects = null;
                    }
                }
            }
        }

        private static string GetAssetPath(Object asset)
        {
            if (asset != null)
            {
                var path = AssetDatabase.GetAssetPath(asset.GetInstanceID());
                return path;
            }
            else
            {
                return "";
            }
        }

        private static void MoveAssetTo(string assetPath, string newPath)
        {
            var assetName = Path.GetFileName(assetPath);
            AssetDatabase.MoveAsset(assetPath, newPath + "/" + assetName);
        }

    }
}