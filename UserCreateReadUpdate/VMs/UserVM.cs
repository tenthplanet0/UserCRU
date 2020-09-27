using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserCreateReadUpdate.VMs
{
	class UserVM : User
	{
		public string Emails => GetEmailsInOneString();
		public string Contacts => GetContactsInOneString();

		private string GetEmailsInOneString()
		{
			StringBuilder emails = new StringBuilder();
			if (Email1 != null && Email1.Length > 0)
				emails.Append(Email1);
			AppendComma(ref emails);

			if (Email2 != null && Email2.Length > 0)
				emails.Append(Email2);
			AppendComma(ref emails);

			if (Email3 != null && Email3.Length > 0)
				emails.Append(Email3);
			RemoveComma(ref emails);

			return emails.ToString();
		}
		private string GetContactsInOneString()
		{
			StringBuilder contacts = new StringBuilder();
			if (Contact1 != null && Contact1.Length > 0)
				contacts.Append(Contact1);

			AppendComma(ref contacts);

			if (Contact2!=null && Contact2.Length > 0)
				contacts.Append(Contact2);

			AppendComma(ref contacts);

			if (Contact3 != null && Contact3.Length > 0)
				contacts.Append(Contact3);

			RemoveComma(ref contacts);

			return contacts.ToString();
		}

		private void AppendComma(ref StringBuilder str)
		{
			if (str.Length > 0)
				str.Append(", ");
		}

		private void RemoveComma(ref StringBuilder str)
		{
			if (str[str.Length-2] == ',')
				str = str.Remove(str.Length-2, 2);
		}
	}
	
}
