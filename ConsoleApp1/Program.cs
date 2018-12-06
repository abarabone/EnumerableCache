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
			var e = new EnumerableCache<int,int,List<int>.Enumerator>();
			e.query = e.enumerable.Select( x => x ).ToCache();

			var n = Enumerable.Range(0,2).ToList();
			foreach( var i in q.Query(e.GetEnumerator) ) Console.WriteLine( i );
			Console.ReadKey();
		}
	}
	
}
