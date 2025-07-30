using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace KC.Framework.Base
{
    [Serializable]
    [DataContract(IsReference = true)]
    public abstract class TreeNode<T> : Entity, ICloneable where T : EntityBase
    {
        public TreeNode()
        {
            ChildNodes = new List<T>();
        }

        /// <summary>
        /// 子节点Id
        /// </summary>
        [Key] //主键
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] //数据库生成默认值
        public int Id { get; set; }

        /// <summary>
        /// 父节点Id
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [MaxLength(512)]
        public string Name { get; set; }

        /// <summary>
        /// 标识树形结构的编码:
        /// 一级树节点Id-二级树节点Id-三级树节点Id-四级树节点Id
        /// 1-1-1-1 ~~ 999-999-999-999
        /// </summary>
        [MaxLength(512)]
        public string TreeCode { get; set; }

        /// <summary>
        /// 是否叶节点
        /// </summary>
        public bool Leaf { get; set; }

        /// <summary>
        /// 节点深度
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Index { get; set; }

        [ForeignKey("ParentId")]
        public virtual T ParentNode { get; set; }

        public virtual List<T> ChildNodes { get; set; }

        public override object Clone()
        {
            base.MemberwiseClone();
            return this.MemberwiseClone();
        }
    }

    public class TreeNodeEquality<T> : EqualityComparer<TreeNode<T>> where T : EntityBase
    {
        public override bool Equals(TreeNode<T> x, TreeNode<T> y)
        {
            return x.Id == y.Id;
        }

        public override int GetHashCode(TreeNode<T> obj)
        {
            return obj.GetHashCode();
        }
    }
}
