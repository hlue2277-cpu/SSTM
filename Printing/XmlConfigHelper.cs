using System.IO;
using System.Xml.Serialization;

namespace Printing
{
	public static class XmlCofigHelper
	{
		public static T Load<T>(string configurationFile) where T : class
		{
			if (File.Exists(configurationFile))
			{
				StreamReader streamReader = new StreamReader(configurationFile);
				try
				{
					XmlSerializer serializer = new XmlSerializer(typeof(T));
					T result = serializer.Deserialize(streamReader) as T;
					return result;
				}
				catch
				{
					//if load failed, will recreate the file.
				}
				finally
				{
					streamReader.Close();
				}
			}
			//if the file doesn't exist, or error when read the file
			return null;
		}
	}
}
