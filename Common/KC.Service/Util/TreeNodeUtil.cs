using System;
using System.Collections.Generic;
using System.Linq;
using KC.Framework.Extension;
using KC.Service.Base;
using KC.Service.Message;
using KC.Framework.Base;
using KC.Framework.Util;
using KC.Service.DTO;

namespace KC.Service.Util
{
    public static class TreeNodeUtil
    {
        /// <summary>
        /// 树形选中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scoreList">所有节点</param>
        /// <param name="checkList">已选中节点的Id列表</param>
        /// <returns></returns>
        public static List<T> GetTreeNodeSimpleWithChildren<T>(List<T> scoreList, List<int> checkList) where T : TreeNodeSimpleDTO<T>
        {
            foreach (var dto in scoreList)
            {
                if (dto.Children.Any())
                {
                    if (checkList.Contains(dto.Id))
                        dto.@checked = true;
                    GetTreeNodeSimpleWithChildren(dto.Children, checkList);
                }
                else
                {
                    if (checkList.Contains(dto.Id))
                    {
                        dto.@checked = true;
                    }
                }
            }
            return scoreList;
        }
        /// <summary>
        /// 将List<TreeNode>对象转化为树形结构的List<TreeNode>（rootTreeNodes）使用范例：<br/>
        /// var rootTreeNodes = allTreeNodes.Where(m => m.ParentId == null).ToList();<br/>
        /// foreach(var level1 in rootTreeNodes)<br/>
        /// {<br/>
        ///     TreeNodeUtil.NestTreeNode(level1, allTreeNodes);<br/>
        /// }
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <param name="parent">根节点：ParentId==null</param>
        /// <param name="allTreeNodes">对象列表对象（非树结构）</param>
        /// <param name="predict">筛选节点条件</param>
        public static void NestTreeNode<T>(T parent, List<T> allTreeNodes, Func<T, bool> predict = null) where T : TreeNode<T>
        {
            if (predict == null)
            {
                var child = allTreeNodes.Where(m => m.ParentId.Equals(parent.Id)).ToList();
                parent.ChildNodes = child;
                foreach (var children in child.OrderBy(m => m.Index))
                {
                    NestTreeNode(children, allTreeNodes, null);
                }
            }
            else
            {
                var child = allTreeNodes.Where(m => m.ParentId.Equals(parent.Id)).Where(predict).OrderBy(m => m.Index).ToList();
                parent.ChildNodes = child.ToList();
                foreach (var children in child)
                {
                    NestTreeNode(children, allTreeNodes, predict);
                }
            }
        }

        /// <summary>
        /// 将List<TreeNode>对象转化为树形结构的List<TreeNode>（rootTreeNodes）使用范例：<br/>
        /// var rootTreeNodes = allTreeNodes.Where(m => m.ParentId == null).ToList();<br/>
        /// foreach(var level1 in rootTreeNodes)<br/>
        /// {<br/>
        ///     TreeNodeUtil.NestTreeNode(level1, allTreeNodes);<br/>
        /// }
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <param name="parent">根节点：ParentId==null</param>
        /// <param name="allTreeNodes">对象列表对象（非树结构）</param>
        /// <param name="checkList">已选中节点的Id列表</param>
        /// <param name="predict">筛选节点条件</param>
        public static void NestTreeNodeDTO<T>(T parent, List<T> allTreeNodes, List<int> checkList = null, Func<T, bool> predict = null) where T : TreeNodeDTO<T>
        {
            if (checkList != null && checkList.Contains(parent.Id))
                parent.@checked = true;
            if (predict == null)
            {
                var child = allTreeNodes.Where(m => m.ParentId.Equals(parent.Id)).ToList();
                parent.Children = child;
                foreach (var children in child.OrderBy(m => m.Index))
                {
                    NestTreeNodeDTO(children, allTreeNodes, checkList, null);
                }
            }
            else
            {
                var child = allTreeNodes.Where(m => m.ParentId.Equals(parent.Id)).Where(predict).OrderBy(m => m.Index).ToList();
                parent.Children = child.ToList();
                foreach (var children in child)
                {
                    NestTreeNodeDTO(children, allTreeNodes, checkList, predict);
                }
            }
        }

        /// <summary>
        /// 将List<TreeNode>对象转化为树形结构的List<TreeNode>（rootTreeNodes）使用范例：<br/>
        /// var rootTreeNodes = allTreeNodes.Where(m => m.ParentId == null).ToList();<br/>
        /// foreach(var level1 in rootTreeNodes)<br/>
        /// {<br/>
        ///     TreeNodeUtil.NestTreeNode(level1, allTreeNodes);<br/>
        /// }
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <param name="parent">根节点：ParentId==null</param>
        /// <param name="allTreeNodes">对象列表对象（非树结构）</param>
        /// <param name="checkList">已选中节点的Id列表</param>
        /// <param name="predict">筛选节点条件</param>
        public static void NestSimpleTreeNodeDTO<T>(T parent, List<T> allTreeNodes, List<int> checkList = null, Func<T, bool> predict = null) where T : TreeNodeSimpleDTO<T>
        {
            if (checkList != null && checkList.Contains(parent.Id))
                parent.@checked = true;
            if (predict == null)
            {
                var child = allTreeNodes.Where(m => m.ParentId.Equals(parent.Id)).ToList();
                parent.Children = child;
                foreach (var children in child.OrderBy(m => m.Index))
                {
                    NestSimpleTreeNodeDTO(children, allTreeNodes, null, null);
                }
            }
            else
            {
                var child = allTreeNodes.Where(m => m.ParentId.Equals(parent.Id)).Where(predict).OrderBy(m => m.Index).ToList();
                parent.Children = child.ToList();
                foreach (var children in child)
                {
                    NestSimpleTreeNodeDTO(children, allTreeNodes, null, predict);
                }
            }
                
        }

        /// <summary>
        /// 排除需要删除的树节点后，只保留{maxLevel}级树节点；
        /// </summary>
        /// <param name="tree">需要筛选的树结构数据</param>
        /// <param name="maxLevel">最大的节点深度</param>
        /// <param name="excludeIds">需要排除的树Id列表</param>
        /// <returns></returns>
        public static T GetNeedLevelTreeNode<T>(T tree, int maxLevel, List<int> excludeIds = null) where T : TreeNode<T>
        {
            if (excludeIds != null 
                && tree.ChildNodes != null 
                && tree.ChildNodes.Any(o => excludeIds.Contains(o.Id)))
                tree.ChildNodes.RemoveAll(o => excludeIds.Contains(o.Id));
            if (tree.Level >= maxLevel)
                tree.ChildNodes = null;

            if (tree.ChildNodes != null)
            {
                foreach (var child in tree.ChildNodes)
                {
                    GetNeedLevelTreeNode(child, maxLevel, excludeIds);
                }
            }
            return tree;
        }

        /// <summary>
        /// 排除需要删除的树节点后，只保留{maxLevel}级树节点；
        /// </summary>
        /// <param name="tree">需要筛选的树结构数据</param>
        /// <param name="maxLevel">最大的节点深度</param>
        /// <param name="excludeIds">需要排除的树Id列表</param>
        /// <param name="checkList">已选中节点的Id列表</param>
        /// <returns></returns>
        public static T GetNeedLevelTreeNodeDTO<T>(T tree, int maxLevel, List<int> excludeIds = null, List<int> checkList = null) where T : TreeNodeDTO<T>
        {
            if (excludeIds != null 
                && tree.Children != null 
                && tree.Children.Any(o => excludeIds.Contains(o.Id)))
                tree.Children.RemoveAll(o => excludeIds.Contains(o.Id));
            if (tree.Level >= maxLevel)
                tree.Children = null;
            if (checkList != null && checkList.Contains(tree.Id))
                tree.@checked = true;
            if (tree.Children != null)
            {
                foreach (var child in tree.Children)
                {
                    GetNeedLevelTreeNodeDTO(child, maxLevel, excludeIds);
                }
            }
            return tree;
        }

        /// <summary>
        /// 排除需要删除的树节点后，只保留{maxLevel}级树节点；
        /// </summary>
        /// <param name="tree">需要筛选的树结构数据</param>
        /// <param name="maxLevel">最大的节点深度</param>
        /// <param name="excludeIds">需要排除的树Id列表</param>
        /// <param name="checkList">已选中节点的Id列表</param>
        /// <returns></returns>
        public static T GetNeedLevelSimpleTreeNodeDTO<T>(T tree, int maxLevel, List<int> excludeIds = null, List<int> checkList = null) where T : TreeNodeSimpleDTO<T>
        {
            if (excludeIds != null 
                && tree.Children != null 
                && tree.Children.Any(o => excludeIds.Contains(o.Id)))
                tree.Children.RemoveAll(o => excludeIds.Contains(o.Id));
            if (tree.Level >= maxLevel)
                tree.Children = null;
            if (checkList != null
                && checkList.Contains(tree.Id))
                tree.@checked = true;
            if (tree.Children != null)
            {
                foreach (var child in tree.Children)
                {
                    GetNeedLevelSimpleTreeNodeDTO(child, maxLevel, excludeIds);
                }
            }
            return tree;
        }

        /// <summary>
        /// 排除需要删除的树节点后，只保留{maxLevel}级树节点；
        /// </summary>
        /// <param name="tree">需要筛选的树结构数据</param>
        /// <param name="maxLevel">最大的节点深度</param>
        /// <param name="excludeIds">需要排除的树Id列表</param>
        /// <returns></returns>
        public static List<T> LoadNeedLevelTreeNode<T>(List<T> treeList, int maxLevel, List<int> excludeIds = null) where T : TreeNode<T>
        {
            foreach (var child in treeList)
            {
                GetNeedLevelTreeNode(child, maxLevel, excludeIds);
            }
            return treeList;
        }

        /// <summary>
        /// 排除需要删除的树节点后，只保留{maxLevel}级树节点；
        /// </summary>
        /// <param name="tree">需要筛选的树结构数据</param>
        /// <param name="maxLevel">最大的节点深度</param>
        /// <param name="excludeIds">需要排除的树Id列表</param>
        /// <param name="checkList">已选中节点的Id列表</param>
        /// <returns></returns>
        public static List<T> LoadNeedLevelTreeNodeDTO<T>(List<T> treeList, int maxLevel, List<int> excludeIds = null, List<int> checkList = null) where T : TreeNodeDTO<T>
        {
            foreach (var child in treeList)
            {
                GetNeedLevelTreeNodeDTO(child, maxLevel, excludeIds, checkList);
            }
            return treeList;
        }

        /// <summary>
        /// 排除需要删除的树节点后，只保留{maxLevel}级树节点；
        /// </summary>
        /// <param name="tree">需要筛选的树结构数据</param>
        /// <param name="maxLevel">最大的节点深度</param>
        /// <param name="excludeIds">需要排除的树Id列表</param>
        /// <param name="checkList">已选中节点的Id列表</param>
        /// <returns></returns>
        public static List<T> LoadNeedLevelSimpleTreeNodeDTO<T>(List<T> treeList, int maxLevel, List<int> excludeIds = null, List<int> checkList = null) where T : TreeNodeSimpleDTO<T>
        {
            foreach (var child in treeList)
            {
                GetNeedLevelSimpleTreeNodeDTO(child, maxLevel, excludeIds, checkList);
            }
            return treeList;
        }
    }
}
