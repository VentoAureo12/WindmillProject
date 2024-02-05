using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.ComponentModel;

namespace TestApp.Classes
{
    public class BasketItemViewModel : INotifyPropertyChanged
    {
        public byte[] Изображение { get; set; }
        public string Наименование { get; set; }
        public int Цена { get; set; }
        public int Product { get; set; }

        private int _quantity;

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

        public RelayCommand AddItemCommand { get; private set; }
        public RelayCommand RemoveItemCommand { get; private set; }


        public event EventHandler QuantityChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public BasketItemViewModel()
        {
            AddItemCommand = new RelayCommand(() => AddItem(this), () => CanAddItem(this));
            RemoveItemCommand = new RelayCommand(() => RemoveItem(this), () => CanRemoveItem(this));
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void AddItem(BasketItemViewModel basketItem)
        {
            basketItem.Quantity++;
        }

        private void RemoveItem(BasketItemViewModel basketItem)
        {
            if (basketItem.Quantity > 1)
            {
                basketItem.Quantity--;
            }
        }

        private bool CanAddItem(BasketItemViewModel basketItem)
        {
            // Реализуйте логику для проверки, можно ли добавить товар
            return true;
        }

        private bool CanRemoveItem(BasketItemViewModel basketItem)
        {
            // Реализуйте логику для проверки, можно ли убрать товар
            return basketItem.Quantity > 1;
        }
    }
}
