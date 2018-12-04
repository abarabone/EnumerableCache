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
	
	// List<T> のような、struct enumerator の場合はどうやって扱っていいのだろうか
	// Linq でも結局ボクシングしてるのかな？
	
	/// <summary>
	/// 列挙ソースのレイトバインディングを可能にする。
	/// </summary>
	//public struct LateBindEnumerable<T, TEnumerator>
	//	where TEnumerator : struct, IEnumerator<T>, IDisposable, IEnumerator
	//{

	//	public IEnumerable<T> EnumerableSource;
		

	//	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	//	public TEnumerator GetEnumerator()
	//	{
	//		return (TEnumerator)EnumerableSource.GetEnumerator();
	//	}
	//}
	public struct LateBindEnumerable<T> : IEnumerable<T>
	{

		public IEnumerable<T> EnumerableSource;
		
		
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public IEnumerator<T> GetEnumerator()
		{
			return EnumerableSource.GetEnumerator();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return EnumerableSource.GetEnumerator();
		}
	}

	/// <summary>
	/// ＬＩＮＱオブジェクトをキャッシュする。
	/// </summary>
	public struct EnumerableCache<Tsrc, Tdst>
	{

		LateBindEnumerable<Tsrc>	enumerable;	// 列挙ソースを

		IEnumerable<Tdst>			query;		// オペレータ
		
		/// <summary>
		/// ソースを決定し、ＬＩＮＱクエリを実行する。
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IEnumerable<Tdst> Query( IEnumerable<Tsrc> source )
		{
			this.enumerable.EnumerableSource = source;
			return this.query;
		}
		
		/// <summary>
		/// オペレータを登録する。
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public EnumerableCache( Func<LateBindEnumerable<Tsrc>, IEnumerable<Tdst>> operatorExpressionOnLateBind )
		{
			this.enumerable = new LateBindEnumerable<Tsrc>();
			this.query		= operatorExpressionOnLateBind( this.enumerable );
		}
	}

	static public class LateBindEnumerableExtension
	{
		static public IEnumerable<Tdst> QueryWith<Tsrc, Tdst>
			( this IEnumerable<Tsrc> enumerableSource, EnumerableCache<Tsrc, Tdst> enumerableCache )
		{
			return enumerableCache.Query( enumerableSource );
		}
	}
}

