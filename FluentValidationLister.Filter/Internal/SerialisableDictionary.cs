namespace FluentValidationLister.Filter.Internal
{
    using FluentValidationLister.Filter.Meta;
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [XmlRoot("dictionary")]
    public class SerialisableDictionary<TKey, TValue>
        : Dictionary<TKey, TValue>, IXmlSerializable
    {
        public SerialisableDictionary() { }
        public SerialisableDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary) { }
        public SerialisableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(dictionary, comparer) { }
        public SerialisableDictionary(IEqualityComparer<TKey> comparer) : base(comparer) { }
        public SerialisableDictionary(int capacity) : base(capacity) { }
        public SerialisableDictionary(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer) { }

        #region IXmlSerializable Members
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();

            if (wasEmpty)
                return;

            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                reader.ReadStartElement("item");

                reader.ReadStartElement("key");
                TKey key = (TKey)keySerializer.Deserialize(reader);
                reader.ReadEndElement();

                reader.ReadStartElement("value");
                TValue value = (TValue)valueSerializer.Deserialize(reader);
                reader.ReadEndElement();

                this.Add(key, value);

                reader.ReadEndElement();
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            Type[] supportedSubTypes = new Type[] { typeof(FromToValues), typeof(MinMaxValues) };

            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue), supportedSubTypes);

            foreach (TKey key in this.Keys)
            {
                writer.WriteStartElement("item");

                writer.WriteStartElement("key");
                keySerializer.Serialize(writer, key);
                writer.WriteEndElement();

                writer.WriteStartElement("value");
                TValue value = this[key];
                valueSerializer.Serialize(writer, value);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }
        }
        #endregion
    }
}
