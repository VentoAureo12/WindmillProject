//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TestApp
{
    using System;
    using System.Collections.Generic;
    
    public partial class Заказ
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Заказ()
        {
            this.Заказ_Товар = new HashSet<Заказ_Товар>();
        }
    
        public int ID { get; set; }
        public Nullable<int> ID_пользователя { get; set; }
        public Nullable<int> ID_пункта_выдачи { get; set; }
        public Nullable<int> Статус_заказа { get; set; }
        public Nullable<int> Сумма_заказа { get; set; }
    
        public virtual Пользователь Пользователь { get; set; }
        public virtual Пункт_Выдачи Пункт_Выдачи { get; set; }
        public virtual Статус_заказа Статус_заказа1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Заказ_Товар> Заказ_Товар { get; set; }
    }
}
