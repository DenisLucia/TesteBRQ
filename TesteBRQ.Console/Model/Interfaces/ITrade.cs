using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesteBRQ.Console.Model.Interfaces
{
	public interface ITrade
	{
		double Value { get; }
		string ClientSector { get; }
		DateTime NextPaymentDate { get; }
	}
}
