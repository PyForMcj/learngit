using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLZ.OldSite.DB.MongoDB.Model
{
    /// <summary>
    /// 系统硬盘上下文
    /// </summary>
    public class SystemHddContext
    {
        #region Data
        private string _PartitionName;
        private double _FreeSpace;
        private double _SumSpace;
        #endregion 

        #region Properties
       
        /// <summary>
        /// 空余大小
        /// </summary>
        public double FreeSpace
        {
            get { return _FreeSpace; }
            set { this._FreeSpace = value; }
        }

        /// <summary>
        /// 使用空间
        /// </summary>
        public double UseSpace
        {
            get { return _SumSpace - _FreeSpace; }
        }

        /// <summary>
        /// 总空间
        /// </summary>
        public double SumSpace
        {
            get { return _SumSpace; }
            set { this._SumSpace = value; }
        }
        
        /// <summary>
        /// 分区名称
        /// </summary>
        public string PartitionName
        {
            get { return _PartitionName; }
            set { this._PartitionName = value; }
        }
        
        /// <summary>
        /// 是否主分区
        /// </summary>
        public bool IsPrimary
        {
            get
            {
                //判断是否为系统安装分区
                if (System.Environment.GetEnvironmentVariable("windir").Remove(2) == this._PartitionName)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion
    }
}
