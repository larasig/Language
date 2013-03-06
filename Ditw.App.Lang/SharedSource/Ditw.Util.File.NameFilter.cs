using System;

namespace Ditw.Util.File
{
    public struct FilterItem
    {
        public String Extension
        {
            get;
            set;
        }

        public String Description
        {
            get;
            set;
        }

        private const String _filterFormat = "{0} ({1})|{1}";

        //public String FilterFormat
        //{
        //    get { return _filterFormat; }
        //}

        public String Filter
        {
            get
            {
                return String.Format(_filterFormat, Description, Extension);
            }
        }
    }

    public static class FileNameFilter
    {
        public static readonly FilterItem XmlFilter = new FilterItem()
        {
            Extension = "*.xml",
            Description = "Xml Files"
        };

    }
}
