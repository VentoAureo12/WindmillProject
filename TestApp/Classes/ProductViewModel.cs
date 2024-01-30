using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TestApp.Classes
{
    public class ProductViewModel : INotifyPropertyChanged
    {
        public ICommand AddItemCommand { get; }
        public ICommand RemoveItemCommand { get; }

        private int _quantity;
        public Товар Product { get; set; }

        public int Quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
                OnPropertyChanged(nameof(Quantity));
            }
        }

     

        public ProductViewModel(Товар product)
        {
            Product = product;
            Quantity = 1;

            AddItemCommand = new RelayCommand(AddItem, CanAddItem);
            RemoveItemCommand = new RelayCommand(RemoveItem, CanRemoveItem);
        }

        private bool CanAddItem()
        {
            // Реализуйте логику для проверки, можно ли добавить товар
            return true;
        }

        private void AddItem()
        {
            Quantity++;
        }

        private bool CanRemoveItem()
        {
            // Реализуйте логику для проверки, можно ли убрать товар
            return Quantity > 1;
        }

        private void RemoveItem()
        {
            Quantity--;
        }


        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
