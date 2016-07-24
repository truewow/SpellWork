using System;

namespace SpellWork.DBC
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    sealed class DataStoreFileNameAttribute : Attribute
    {
        public DataStoreFileNameAttribute(string filename) 
        {
            FileName = filename;
        }
   
        public string FileName { get; }
    }
}
