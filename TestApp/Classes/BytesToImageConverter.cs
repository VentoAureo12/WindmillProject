using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace TestApp
{
    // Класс-конвертер для преобразования массива байт в изображение (BitmapImage)
    public class BytesToImageConverter : IValueConverter
    {
        // Метод Convert для преобразования массива байт в BitmapImage
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Проверка, что значение является массивом байт
            if (value is byte[] bytes)
            {
                // Создание нового объекта BitmapImage
                BitmapImage image = new BitmapImage();

                // Использование MemoryStream для чтения массива байт и установки его в качестве источника изображения
                using (MemoryStream stream = new MemoryStream(bytes))
                {
                    // Инициализация BitmapImage
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = stream;
                    image.EndInit();
                }

                // Возвращение объекта BitmapImage
                return image;
            }

            // Возвращение null, если значение не является массивом байт
            return null;
        }

        // Метод ConvertBack, необходим для обратного преобразования
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // В данном случае выбрасываем исключение, так как обратное преобразование не поддерживается
            throw new NotImplementedException();
        }
    }
}
