﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace CinemaClient.Converters
{
	internal class StringToBitmapConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			try
			{
				var image = ConvertToBitmap((string)value);

				return image;
			}
			catch { return null; }
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private BitmapImage ConvertToBitmap(string img)
		{
			// Convert the image data to a byte array
			byte[] imageBytes = System.Convert.FromBase64String(img);

			// Convert the byte array to a BitmapImage and display it
			BitmapImage bitmapImage = new BitmapImage();
			using (MemoryStream stream = new MemoryStream(imageBytes))
			{
				bitmapImage.BeginInit();
				bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
				bitmapImage.StreamSource = stream;
				bitmapImage.EndInit();
			}

			return bitmapImage;
		}
	}
}
