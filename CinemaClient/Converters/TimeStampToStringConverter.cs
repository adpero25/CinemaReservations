using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using System.Windows.Data;
using System.Xml;

namespace CinemaClient.Converters
{
	internal class TimeStampToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var duration = ParseDuration((string)value);

			return new string(string.Format("{0,2}h {1,2}min", duration.Hours, duration.Minutes));
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}


		static TimeSpan ParseDuration(string durationString)
		{
			// Create an XML element from the duration string
			XElement durationElement = XElement.Parse($"<root>{durationString}</root>");

			// Get the duration value
			string durationValue = durationElement.Value;

			// Parse the duration value
			TimeSpan duration = XmlConvert.ToTimeSpan(durationValue);

			return duration;
		}
	}
}
