using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System;

namespace Abss.UtilityOther
{
	// ＬＩＮＱオペレータオブジェクトを使いまわしたいと思ったとき、
	//		static Func<IEnumerable<int>, IEnumerable<int>>
	//			calcLengths = xs => xs
	//				.Select( model => model.BoneLength * model.ModelInstanceCount )
	//				;
	// みたいなのを持っておけばいいかな？と思ったけど、これだと calcLengths( source ) とした時に
	// はじめて Linq のオペレータオブジェクトが生成されるので、やっぱりキャッシュオブジェクトは必要。

	// List<T> とかならオペレータをキャッシュして、中身だけ変えればいい。

	// List<T> のような、struct enumerator の場合はどうやって扱っていいのだろうか
	// Linq でも結局ボクシングしてるのかな？

	
	/// <summary>
	/// 列挙ソースのレイトバインディングを可能にする。
	/// </summary>
	public struct LateBindEnumerable<T, TEnumerator> : IEnumerable<T>
		where TEnumerator : IEnumerator<T>
	{

		public Func<TEnumerator>	GetEnumeratorFunc;

		
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public TEnumerator GetEnumerator() => GetEnumeratorFunc();
		
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumeratorFunc();
		
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		IEnumerator IEnumerable.GetEnumerator() => GetEnumeratorFunc();

	}

	

	/// <summary>
	/// ＬＩＮＱオブジェクトをキャッシュする。
	/// </summary>
	public struct EnumerableCache<Tsrc, Tdst, TEnumerator> : IEnumerable<Tsrc>
		where TEnumerator : IEnumerator<Tsrc>
	{
		
		Func<TEnumerator>			getEnumeratorFunc;
		
		public IEnumerable<Tdst>	query;

		
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public TEnumerator GetEnumerator() => getEnumeratorFunc();
		
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		IEnumerator<Tsrc> IEnumerable<Tsrc>.GetEnumerator() => getEnumeratorFunc();
		
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		IEnumerator IEnumerable.GetEnumerator() => getEnumeratorFunc();

		
		/// <summary>
		/// ソースを決定し、ＬＩＮＱクエリを実行する。
		/// </summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public IEnumerable<Tdst> Query( Func<TEnumerator> getEnumerator )
		{
			this.getEnumeratorFunc = getEnumerator;

			return this.query;
		}
		
	}

	public struct EnumerableCache<Tsrc, Tdst> : IEnumerable<Tsrc>
	{
		
		Func<IEnumerator<Tsrc>>		getEnumeratorFunc;
		
		public IEnumerable<Tdst>	query;
		
		
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		IEnumerator<Tsrc> IEnumerable<Tsrc>.GetEnumerator() => getEnumeratorFunc();
		
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		IEnumerator IEnumerable.GetEnumerator() => getEnumeratorFunc();


		/// <summary>
		/// ソースを決定し、ＬＩＮＱクエリを実行する。
		/// </summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public IEnumerable<Tdst> Query( IEnumerable<Tsrc> source )
		{
			this.getEnumeratorFunc = source.GetEnumerator;

			return this.query;
		}
	}

	
	static public class LateBindEnumerableExtension
	{
		/// <summary>
		/// 
		/// </summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		static public IEnumerable<Tdst> QueryWith<Tsrc, Tdst>
			( this IEnumerable<Tsrc> enumerableSource, EnumerableCache<Tsrc, Tdst> enumerableCache )
		{
			return enumerableCache.Query( enumerableSource );
		}

		/// <summary>
		/// 
		/// </summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		static public IEnumerable<Tdst> QueryWith<Tsrc, Tdst, TEnumerator>
			( this Func<TEnumerator> getEnumerator, EnumerableCache<Tsrc, Tdst, TEnumerator> enumerableCache )
			where TEnumerator : struct, IEnumerator<Tsrc>
		{
			return enumerableCache.Query( getEnumerator );
		}

		static public void ToCache<Tsrc, Tdst, TEnumerator>
			( this IEnumerable<Tdst> query, ref EnumerableCache<Tsrc, Tdst, TEnumerator> cache )
			where TEnumerator : struct, IEnumerator<Tsrc>
		{
			cache.query = query;
		}

		static public void ToCache<Tsrc, Tdst>
			( this IEnumerable<Tdst> query, ref EnumerableCache<Tsrc, Tdst> cache )
		{
			cache.query = query;
		}

		//static public EnumerableCache<Tsrc, Tdst> ToCache<Tsrc, Tdst>( this IEnumerable<Tdst> expression )
		//{
		//	var cache	= new EnumerableCache<Tsrc, Tdst>();
		//	cache.query	= expression;
		//	return cache;
		//}

		//static public EnumerableCache<Tsrc, Tdst, TEnumerator> ToCache<Tsrc, Tdst, TEnumerator>
		//	( this IEnumerable<Tdst> expression )
		//	where TEnumerator : struct, IEnumerator<Tsrc>
		//{
		//	var cache	= new EnumerableCache<Tsrc, Tdst, TEnumerator>();
		//	cache.query	= expression;
		//	return cache;
		//}

	}
}


