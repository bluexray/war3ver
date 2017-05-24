using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace War3Ver
{
    public class IniHelper
    {
        //public string Path;
        //public IniHelper(string path)
        //{
        //    Path = path;
        //}

        #region 声明读写INI文件的API函数 

        /// <summary>
        /// 读取ini配置文件
        /// </summary>
        /// <param name="sectionName">要读取的section名,如果传入null获取ini文件所有的section名</param>
        /// <param name="key">要读取的key,如果传入null获取此section名下的所有key</param>
        /// <param name="defaultValue">读取异常的情况下的缺省值</param>
        /// <param name="returnBuffer">key所对应的值，如果该key不存在则返回defaultValue</param>
        /// <param name="size">值允许的大小</param>
        /// <param name="filePath">ini文件的完整路径和文件名</param>
        /// <see cref="https://msdn.microsoft.com/zh-cn/library/ms724353.aspx"/>
        /// <returns></returns>
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string sectionName, string key, string defaultValue, byte[] returnBuffer, int size, string filePath);

        /// <summary>
        /// 写入ini配置文件
        /// </summary>
        /// <param name="sectionName">要写入的section名</param>
        /// <param name="key">要写入的key，如果传入为null，整个sectionName被清除</param>
        /// <param name="value">key所对应的值，如果传入为null，此key将被清除</param>
        /// <param name="filePath">ini文件的完整路径和文件名</param>
        /// <see cref="https://msdn.microsoft.com/zh-cn/library/ms725501.aspx"/>
        /// <returns></returns>
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string sectionName, string key, string value, string filePath);


        #endregion


        /// <summary>
        /// 根据key读取Value
        /// </summary>
        /// <param name="sectionName">section名称</param>
        /// <param name="key">key的名称</param>
        /// <param name="filePath">文件路径</param>
        public static string GetValue(string sectionName, string key, string filePath)
        {
            byte[] buffer = new byte[2048];
            int length = GetPrivateProfileString(sectionName, key, "发生错误", buffer, 999, filePath);
            string rs = System.Text.UTF8Encoding.Default.GetString(buffer, 0, length);
            return rs;
        }

        /// <summary>
        /// 获取ini文件内所有的section名称
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>返回一个包含section名称的集合</returns>
        public static List<string> GetSectionNames(string filePath)
        {
            byte[] buffer = new byte[2048];
            int length = GetPrivateProfileString(null, "", "", buffer, 999, filePath);
            String[] rs = System.Text.UTF8Encoding.Default.GetString(buffer, 0, length).Split(new string[] { "\0" }, StringSplitOptions.RemoveEmptyEntries);
            return rs.ToList();
        }

        /// <summary>
        /// 获取指定section内的所有key
        /// </summary>
        /// <param name="sectionName">section名称</param>
        /// <param name="filePath">文件路径</param>
        /// <returns>返回一个包含key名称的集合</returns>
        public static List<string> GetKeys(string sectionName, string filePath)
        {
            byte[] buffer = new byte[2048];
            int length = GetPrivateProfileString(sectionName, null, "", buffer, 999, filePath);
            String[] rs = System.Text.UTF8Encoding.Default.GetString(buffer, 0, length).Split(new string[] { "\0" }, StringSplitOptions.RemoveEmptyEntries);
            return rs.ToList();
        }

        /// <summary>
        /// 保存内容到ini文件
        /// <para>若存在相同的key，就覆盖，否则就增加</para>
        /// </summary>
        /// <param name="sectionName">section名称</param>
        /// <param name="key">key的名称</param>
        /// <param name="value">存储的值</param>
        /// <param name="filePath">文件路径</param>
        public static bool SetValue(string sectionName, string key, string value, string filePath)
        {
            try
            {
                int rs = (int)WritePrivateProfileString(sectionName, key, value, filePath);
                return rs > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 移除指定的section
        /// </summary>
        /// <param name="sectionName">section名称</param>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static bool RemoveSection(string sectionName, string filePath)
        {
            try
            {
                int rs = (int)WritePrivateProfileString(sectionName, null, "", filePath);
                return rs > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 移除指定的key
        /// </summary>
        /// <param name="sectionName">section名称</param>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static bool Removekey(string sectionName, string key, string filePath)
        {
            try
            {
                int rs = (int)WritePrivateProfileString(sectionName, key, null, filePath);
                return rs > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
