using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Web.Script.Serialization;
using System.Windows.Data;

namespace Auremo.DataModel
{
    // Convert string IDs to human readable strings in different languages.
    public class Translator
    {
        private string m_ResourceName = null;
        private IDictionary<string, string> m_Translations = new SortedDictionary<string, string>();

        public class TranslationFile
        {
            public class TranslationStanza
            {
                public string Id { get; set; }
                public string Description { get; set; }
                public string InEnglish { get; set; }
                public string Translation { get; set; }
            }

            public TranslationStanza[] Translations { get; set; }
        }

        public Translator(string language)
        {
            m_ResourceName = $"Auremo.Translations.{language}.json";
            Assembly assembly = Assembly.GetExecutingAssembly();
            string json = null;

            using (Stream stream = assembly.GetManifestResourceStream(m_ResourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    json = reader.ReadToEnd();
                }
            }

            JavaScriptSerializer deserializer = new JavaScriptSerializer();
            TranslationFile translationFile = deserializer.Deserialize<TranslationFile>(json);

            foreach (TranslationFile.TranslationStanza stanza in translationFile.Translations)
            {
                string key = stanza.Id;

                if (!key.StartsWith("example."))
                {
                    string value = stanza.Translation;

                    if (m_Translations.ContainsKey(key))
                    {
                        throw new Exception($"{m_ResourceName}: duplicate string id {key}");
                    }

                    m_Translations[key] = value;
                }
            }
        }

        public string Tranlate(string stringId)
        {
            if (m_Translations.ContainsKey(stringId))
            {
                return m_Translations[stringId];
            }
            else
            {
                return $"Not translated: {stringId}";
            }

            
        }

        public string FixMeMissingTranslation(string stringLiteral)
        {
            World.Instance.Log.LogMessage($"Application not translating missing string \"{stringLiteral}\"!");
            return stringLiteral;
        }
    }

    [ValueConversion(typeof(string), typeof(string))]
    public class LanguageTranslationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
#if DEBUG
            if (World.Instance != null)
            {
#endif
                return World.Instance.Translator.Tranlate(parameter as string);
#if DEBUG
            }
            else
            {
                return new Translator("English").Tranlate(parameter as string);
            }
#endif
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
