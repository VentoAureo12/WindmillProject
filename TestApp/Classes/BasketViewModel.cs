using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.ComponentModel;

namespace TestApp.Classes
{
    /// <summary>
    /// ViewModel для отдельного товара в корзине
    /// </summary>
    public class BasketItemViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Свойство для хранения изображения товара
        /// </summary>
        public byte[] Изображение { get; set; }

        /// <summary>
        /// Свойство для хранения наименования товара
        /// </summary>
        public string Наименование { get; set; }

        /// <summary>
        /// Свойство для хранения цены товара
        /// </summary>
        public int Цена { get; set; }

        /// <summary>
        /// Свойство для хранения идентификатора товара
        /// </summary>
        public int Product { get; set; }

        private int _quantity;

        /// <summary>
        /// Свойство для хранения количества товара в корзине
        /// </summary>
        public int Quantity
        {
            get { return _quantity; }
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged(nameof(Quantity));
                    QuantityChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Команда для увеличения количества товара в корзине
        /// </summary>
        public RelayCommand AddItemCommand { get; private set; }

        /// <summary>
        /// Команда для уменьшения количества товара в корзине
        /// </summary>
        public RelayCommand RemoveItemCommand { get; private set; }

        /// <summary>
        /// Событие, возникающее при изменении количества товара в корзине
        /// </summary>
        public event EventHandler QuantityChanged;

        /// <summary>
        /// Событие, возникающее при изменении свойства
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Конструктор класса BasketItemViewModel
        /// </summary>
        public BasketItemViewModel()
        {
            // Инициализация команд
            AddItemCommand = new RelayCommand(() => AddItem(this), () => CanAddItem(this));
            RemoveItemCommand = new RelayCommand(() => RemoveItem(this), () => CanRemoveItem(this));
        }

        /// <summary>
        /// Метод для вызова события при изменении свойства
        /// </summary>
        /// <param name="propertyName">Имя измененного свойства</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Метод для увеличения количества товара в корзине
        /// </summary>
        /// <param name="basketItem">Текущий товар в корзине</param>
        private void AddItem(BasketItemViewModel basketItem)
        {
            basketItem.Quantity++;
        }

        /// <summary>
        /// Метод для уменьшения количества товара в корзине
        /// </summary>
        /// <param name="basketItem">Текущий товар в корзине</param>
        private void RemoveItem(BasketItemViewModel basketItem)
        {
            if (basketItem.Quantity > 1)
            {
                basketItem.Quantity--;
            }
        }

        /// <summary>
        /// Метод для проверки возможности увеличения количества товара в корзине
        /// </summary>
        /// <param name="basketItem">Текущий товар в корзине</param>
        /// <returns>Всегда возвращает true</returns>
        private bool CanAddItem(BasketItemViewModel basketItem)
        {
            return true;
        }

        /// <summary>
        /// Метод для проверки возможности уменьшения количества товара в корзине
        /// </summary>
        /// <param name="basketItem">Текущий товар в корзине</param>
        /// <returns>Возвращает true, если количество товара в корзине больше 1</returns>
        private bool CanRemoveItem(BasketItemViewModel basketItem)
        {
            return basketItem.Quantity > 1;
        }
    }
}