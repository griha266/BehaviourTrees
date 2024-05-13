using System;
using UnityEditor;
using UnityEngine;

namespace Shipico.BehaviourTrees.Editor
{
    public static class BehaviourTreeEditorUtils
    {
        public static string GetPackageRootPath()
        {
            var regularAssetPath = "Assets/Plugins/BehaviourTrees/";
            var packageAssetsPath = "Packages/com.shipico.behaviourtrees/";

            if (AssetDatabase.IsValidFolder(packageAssetsPath))
            {
                return packageAssetsPath;
            }
            
            if (AssetDatabase.IsValidFolder(regularAssetPath))
            {
                return regularAssetPath;
            }

            throw new Exception("Cannot find asset path");
        }
        
        public static TreeNode AddNodeAssetToTree(this BehaviourTree tree, Type type, Vector2 position)
        {
            if (!type.IsSubclassOf(typeof(TreeNode)))
            {
                Debug.LogError("Try to add unsupported type to tree", tree);
                return null;
            }

            var newNode = ScriptableObject.CreateInstance(type) as TreeNode;
            if (newNode == null)
            {
                Debug.LogError("Cannot cast object to TreeNode type");
                return null;
            }

            SaveNodeAssetToTree(newNode, tree, position);
            return newNode;
        }

        private static void SaveNodeAssetToTree<TTreeType>(TTreeType node, BehaviourTree tree, Vector2 position)
            where TTreeType : TreeNode
        {
            node.GrahpPosition = position;
            node.Id = GUID.Generate().ToString();
            node.name = node.Id;
            tree.Nodes.Add(node);
            AssetDatabase.AddObjectToAsset(node, tree);
            AssetDatabase.SaveAssets();
        }
        public static TTreeType AddNodeAssetToTree<TTreeType>(this BehaviourTree tree, Vector2 position)
            where TTreeType : TreeNode
        {
            var newNode = ScriptableObject.CreateInstance<TTreeType>();
            SaveNodeAssetToTree(newNode, tree, position);
            return newNode;
        }

        public static void DeleteNodeAsset(this BehaviourTree tree, TreeNode node)
        {
            tree.Nodes.Remove(node);
            AssetDatabase.RemoveObjectFromAsset(node);
            AssetDatabase.SaveAssets();
        }
    }
}