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
			e.Select( x => x ).ToCache( ref e );

			var n = Enumerable.Range(0,2).ToList();
			var m = Enumerable.Range(5,5).ToList();
			foreach( var i in e.Query(n.GetEnumerator) ) Console.WriteLine( i );
			foreach( var i in e.Query(m.GetEnumerator) ) Console.WriteLine( i );
			Console.ReadKey();
		}
	}
	
}
