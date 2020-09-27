using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UserCreateReadUpdate.VMs
{
	public class ViewModelBase : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string propertyName)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public bool IsItemValid1
		{
			get
			{
				PropertyInfo[] properties = this.GetType().GetProperties();
				foreach (PropertyInfo property in properties)
				{
					string propertyName = property.Name;
					if (!string.IsNullOrEmpty(propertyName))
						return false;
				}
				return true;
			}
		}
	}
}
