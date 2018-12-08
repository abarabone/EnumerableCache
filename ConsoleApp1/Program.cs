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
			var n = Enumerable.Range(0,2).ToList();
			var m = Enumerable.Range(5,5).ToList();


			// なんとなく形にはなった？

			var e1 = new EnumerableCache<int, int>();
			e1.Select( x => x + 1 ).ToCache( e1 );
			
			foreach( var i in e1.Query(n) ) Console.WriteLine( i );
			foreach( var i in e1.Query(m) ) Console.WriteLine( i );


			// 以下は無意味（ struct enumerator を呼ぶことができなかった、というかやる意味なしかも）

			var e2 = new EnumerableCache<int,int,List<int>.Enumerator>();
			e2.Select( x => x+1 ).ToCache( e2 );

			foreach( var i in e2.Query(n.GetEnumerator) ) Console.WriteLine( i );
			foreach( var i in e2.Query(m.GetEnumerator) ) Console.WriteLine( i );


			Console.ReadKey();
		}
	}
	
}
