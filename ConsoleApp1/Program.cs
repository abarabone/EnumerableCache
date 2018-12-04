using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Abss.UtilityOther;

namespace ConsoleApp1
{
	class Program
	{
		static void Main( string[] args )
		{
			var q = new LateBindEnumerable<int>();

			q.EnumerableSource = Enumerable.Range( 0, 10 );

			foreach( var i in q ) Console.WriteLine( i );
			Console.ReadKey();
		}

		void aaa()
		{
			var e = new List<int>().GetEnumerator();
		}
	}
}
