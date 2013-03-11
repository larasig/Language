using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Ditw.App.EvtX.Cfg.Test
{
    [XmlRoot("attribute")]
    public class EvtXTestEventAttribute
    {
        [XmlElement("name")]
        public String Name
        {
            get;
            set;
        }

        [XmlElement("value")]
        public String Value
        {
            get;
            set;
        }

        [XmlElement("english")]
        public String EnglishTranslation
        {
            get;
            set;
        }
    }

    [XmlRoot("attributes")]
    public class EvtXTestEventAttributes
    {
        [XmlElement("attribute")]
        public EvtXTestEventAttribute[] Attributes
        {
            get;
            set;
        }
    }

    [XmlRoot("event")]
    public class EvtXTestEvent
    {
        [XmlElement("type")]
        public String Type
        {
            get;
            set;
        }

        [XmlElement("attributes")]
        public EvtXTestEventAttributes Attributes
        {
            get;
            set;
        }
    }

    [XmlRoot("events")]
    public class EvtXTestEvents
    {
        [XmlElement("event")]
        public EvtXTestEvent[] Events
        {
            get;
            set;
        }
    }

    [XmlRoot("test")]
    public class EvtXTest
    {
        [XmlElement("sentence")]
        public String Sentence
        {
            get;
            set;
        }

        [XmlElement("difficulty")]
        public String Difficulty
        {
            get;
            set;
        }

        [XmlElement("english")]
        public String EnglishTranslation
        {
            get;
            set;
        }

        [XmlElement("url")]
        public String Url
        {
            get;
            set;
        }

        [XmlElement("events")]
        public EvtXTestEvents Events
        {
            get;
            set;
        }
    }

    [XmlRoot("tests")]
    public class EvtXTests
    {
        [XmlElement("test")]
        public EvtXTest[] Tests
        {
            get;
            set;
        }
    }
}
