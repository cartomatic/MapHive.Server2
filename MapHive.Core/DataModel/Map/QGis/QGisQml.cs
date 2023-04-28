using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace MapHive.Core.DataModel.Map.QGis
{
    /// <summary>
    /// A simplified model fo QGIS Qml files used for style extraction
    /// </summary>
    [XmlRoot("qgis")]
    public class QGisQml
    {
        [XmlAttribute("version")]
        public string Version { get; set; }

        [XmlAttribute("minimumScale")]
        public double MinimumScale { get; set; }

        [XmlAttribute("maximumScale")]
        public double MaximumScale { get; set; }

        [XmlElement("renderer-v2")]
        public QGisRendererV2 RendererV2 { get; set; }



        /// <summary>
        /// Reads a QGIS style from a QML file
        /// </summary>
        /// <param name="fPath"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static QGisQml Read(string fPath)
        {
            if (!File.Exists(fPath))
                throw new ArgumentException($"File {fPath} does not exist");

            var xml = File.ReadAllText(fPath);

            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(xml));

            var xmlReaderSettings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Parse,
                //so xml DoS called million of loughs is addressed: https://stackoverflow.com/questions/3451203/how-does-the-billion-laughs-xml-dos-attack-work
                MaxCharactersFromEntities = 1024
            };
            var xmlReader = XmlReader.Create(ms, xmlReaderSettings);
            var xmlSerializer = new XmlSerializer(typeof(QGisQml));

            return (QGisQml) xmlSerializer.Deserialize(xmlReader);
        }
    }

    public class QGisRendererV2
    {

        /// <summary>
        /// column to be styled???
        /// </summary>
        [XmlAttribute("attr")]
        public string Attr { get; set; }

        [XmlAttribute("forceraster")]
        public int ForceRaster { get; set; }

        [XmlAttribute("symbollevels")]
        public int SymbolLevels { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("graduatedMethod")]
        public string GraduatedMethod { get; set; }

        [XmlAttribute("enableorderby")]
        public int EnableOrderBy { get; set; }

        [XmlArray("ranges")]
        [XmlArrayItem("range")]
        public List<QGisRendererV2Range> Ranges { get; set; }

        [XmlArray("symbols")]
        [XmlArrayItem("symbol")]
        public List<QGisRendererV2Symbol> Symbols { get; set; }

    }

    public class QGisRendererV2Range
    {
        [XmlAttribute("render")]
        public bool Render { get; set; }

        /// <summary>
        /// Points to symbol
        /// </summary>
        [XmlAttribute("symbol")]
        public int Symbol { get; set; }

        [XmlAttribute("lower")]
        public double Lower { get; set; }

        [XmlAttribute("upper")]
        public double Upper { get; set; }

        [XmlAttribute("label")]
        public string Label { get; set; }
    }

    public class QGisRendererV2Symbol
    {
        [XmlAttribute("alpha")]
        public int Alpha { get; set; }

        [XmlAttribute("clip_to_extent")]
        public int ClipToExtent { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        /// <summary>
        /// Pointer to range???
        /// </summary>
        [XmlAttribute("name")]
        public int Name { get; set; }

        [XmlElement("layer")]
        public QGisRendererV2SymbolLayer Layer { get; set; }
    }

    public class QGisRendererV2SymbolLayer
    {
        [XmlElement("pass")]
        public int Pass { get; set; }

        [XmlElement("class")]
        public string Class { get; set; }

        [XmlElement("locked")]
        public int Locked { get; set; }

        [XmlElement("prop")]
        public List<QGisRendererV2SymbolLayerProperty> Properties { get; set; }
    }

    public class QGisRendererV2SymbolLayerProperty
    {
        [XmlAttribute("k")]
        public string Key { get; set; }

        [XmlAttribute("v")]
        public string Value { get; set; }
    }
}
