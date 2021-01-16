using System;
using System.Text;
using System.Reflection;
using System.Linq;
using System.IO;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Calculon.Types
{
    public class Config
    {
        public Config()
        {
            configuration = new ConfigurationBuilder()
                .AddIniFile("calculon.ini", optional: true, reloadOnChange: true)
                .Build();

            strings = StringTable.GetStringTable(Language);

            backupfile = Path.Combine(Environment.GetFolderPath(
                    System.Environment.SpecialFolder.MyDocuments), "calculon.sqlite");

            SqliteConnectionStringBuilder connectionStringBuilder = new SqliteConnectionStringBuilder(
                    "Data Source=InMemoryPack;Mode=Memory;Cache=Shared");
            memoryConnection = new SqliteConnection(connectionStringBuilder.ToString());
            memoryConnection.Open();
            if (!LoadConfigFromFile())
            {
                InitDb();
            }
        }

        ~Config()
        {
            if (memoryConnection != null)
            {
                memoryConnection.Close();
                memoryConnection = null;
            }
        }

        private static readonly Config _instance = new Config();
        public static Config handle { get { return _instance; } }

        public StringTable strings;

        private IConfiguration configuration;

        private string backupfile;

        public bool WriteConfigToFile()
        {
            if (AllowFilesystemWrites)
            {
                ICalculonType val = this["\"UseFile\""];
                if ((val.GetType() == typeof(Literal)) &&
                    (val.Display.ToLower() == "\"true\""))
                {
                    SqliteConnectionStringBuilder source =
                        new SqliteConnectionStringBuilder();
                    source.DataSource = backupfile;
                    using (SqliteConnection file = 
                        new SqliteConnection(source.ToString()))
                    {
                        file.Open();
                        memoryConnection.BackupDatabase(file);
                        file.Close();
                    }
                    return true;
                }
            }

            return false;
        }

        private bool LoadConfigFromFile()
        {
            if (AllowFilesystemWrites)
            {
                if (File.Exists(backupfile))
                {
                    SqliteConnectionStringBuilder source =
                        new SqliteConnectionStringBuilder();
                    source.DataSource = backupfile;

                    using (SqliteConnection src = 
                        new SqliteConnection(source.ToString()))
                    {
                        src.Open();
                        src.BackupDatabase(memoryConnection);
                        src.Close();
                    }
                    return true;
                }
            }

            return false;
        }

        public ICalculonType this[string key]
        {
            get
            {
                ICalculonType output = null;
                SqliteCommand cmd = memoryConnection.CreateCommand();
                cmd.CommandText = "SELECT type, value FROM storage WHERE key=@key;";
                cmd.Parameters.AddWithValue("@key", key);
                using (SqliteDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        Type t = Type.GetType(sdr["type"].ToString());
                        string val = sdr["value"].ToString();
                        object[] array = new[] { val };
                        output = (ICalculonType)Activator.CreateInstance(t, array);
                    }
                }

                //if we got here and didn't find anything
                if (output == null)
                {
                    output = new EmptyType();
                }
                
                return output;
            }

            set
            {
                SqliteCommand cmd = memoryConnection.CreateCommand();
                cmd.CommandText = "REPLACE INTO storage(key, value, type) VALUES (@key, @value, @type)";
                cmd.Parameters.AddWithValue("@key", key);
                cmd.Parameters.AddWithValue("@value", value.Display);
                cmd.Parameters.AddWithValue("@type", value.GetType().ToString());
                int status = cmd.ExecuteNonQuery();
                WriteConfigToFile();
            }
        }

        private void InitDb()
        {
            StringBuilder cmdTxt = new StringBuilder("CREATE TABLE IF NOT EXISTS storage");
            cmdTxt.Append("(key TEXT UNIQUE, ");
            cmdTxt.Append("value TEXT NOT NULL, type TEXT NOT NULL);");

            SqliteCommand cmd = memoryConnection.CreateCommand();
            cmd.CommandText = cmdTxt.ToString();
            cmd.ExecuteNonQuery();
        }

        public bool AllowFilesystemWrites
        {
            get
            {
                string value = configuration["AllowFilesystemWrites"];
                if (value is null)
                {
                    return false;
                }
                try
                {
                    bool answer = bool.Parse(value);
                    return answer;
                } catch (Exception)
                {
                    return false;
                }
            }
        }

        public string Language
        {
            get
            {
                string lang = configuration["Language"];
                if (lang is null)
                {
                    lang = "ENG";
                }

                return lang;
            }
        }

        private SqliteConnection memoryConnection;
    }

    public class StringTable : Dictionary<string, string>, IXmlSerializable
    {
        public static StringTable GetStringTable(string language)
        {
            StringTable output;
            string[] path = new string[3];
            path[0] = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location);
            path[1] = "lang";
            path[2] = language + ".xml";
            string xmlPath = Path.Combine(path);
            string xmlString = File.ReadAllText(xmlPath);
            XmlSerializer serializer = new XmlSerializer(typeof(StringTable));
            
            using (TextReader reader = new StringReader(xmlString))
            {
                output = (StringTable) serializer.Deserialize(reader);
            }

            return output;
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            bool wasEmpty = reader.IsEmptyElement;
            this.lang = reader.GetAttribute("lang");
            reader.Read();

            if (wasEmpty) { return; }

            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                string key = reader.GetAttribute("name");
                string value = reader.ReadInnerXml();
                this.Add(key, value);
            }

            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("lang", lang);
            foreach (string key in this.Keys)
            {
                writer.WriteStartElement("entry");
                writer.WriteAttributeString("name", key);
                string value = this[key];
                writer.WriteRaw(value);
                writer.WriteEndElement();
            }
        }

        private string lang;
        public string StringFile
        {
            get
            {
                string[] path = new string[3];
                path[0] = Path.GetDirectoryName(
                    Assembly.GetExecutingAssembly().Location);
                path[1] = "lang";
                path[2] = lang + ".json";
                return Path.Combine(path);
            }
        }
    }

    public class Store : IFunctionCog
    {
        public string[] FunctionName { get { return new string[] { "sto" }; } }

        public int NumArgs { get { return 2; } }

        public Type[][] AllowedTypes
        {
            get
            { 
                Type[][] output = new Type[1][];
                output[0] = new Type[] { typeof(Literal), typeof(AnyType) };
                return output;
            }
        }

        public ICalculonType Execute(ref ControllerState cs)
        {
            Literal l = (Literal)cs.stack.Pop();
            string key = l.Display;
            ICalculonType ict = cs.stack.Pop();
            
            cs.Config[key] = ict;
            return new EmptyType();
        }
    }

    public class Recall : IFunctionCog
    {
        public string[] FunctionName { get { return new string[] { "rcl" }; } }

        public int NumArgs { get { return 1; } }

        public Type[][] AllowedTypes
        {
            get
            {
                Type[][] output = new Type[1][];
                output[0] = new Type[] { typeof(Literal)};
                return output;
            }
        }

        public ICalculonType Execute(ref ControllerState cs)
        {
            Literal l = (Literal)cs.stack.Pop();
            string key = l.Display;
            ICalculonType output = cs.Config[key];
            return output;
        }
    }
}