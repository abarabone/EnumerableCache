﻿using System.Collections;
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
	public struct LateBindEnumerable<T, TEnumerator> : IEnumerable<T>
		where TEnumerator : struct, IEnumerator<T>, IDisposable
	{
		
		public IEnumerable<T> EnumerableSource;


		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public TEnumerator GetEnumerator()
		{
			return (TEnumerator)EnumerableSource.GetEnumerator();
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return EnumerableSource.GetEnumerator();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return EnumerableSource.GetEnumerator();
		}
	}
	public struct LateBindEnumerable<T> : IEnumerable<T>
	{

		public IEnumerable<T> EnumerableSource;
		
		
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return GetEnumeratorFunc();
		}
		
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumeratorFunc();
		}
	}


	/// <summary>
	/// ＬＩＮＱオブジェクトをキャッシュする。
	/// </summary>
	public struct EnumerableCache<Tsrc, Tdst, TEnumerator>
		where TEnumerator : IEnumerator<Tsrc>
	{

		public LateBindEnumerable<Tsrc, TEnumerator> enumerable;	// 列挙ソースを

		//public Func<LateBindEnumerable<Tsrc, TEnumerator>, IEnumerable<Tdst>>	query;
		public IEnumerable<Tdst> query;// オペレータ


		/// <summary>
		/// ソースを決定し、ＬＩＮＱクエリを実行する。
		/// </summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public IEnumerable<Tdst> Query( Func<TEnumerator> getEnumerator )
		{
			this.enumerable.GetEnumeratorFunc = getEnumerator;

			return this.query;
		}
	}

	public struct EnumerableCache<Tsrc, Tdst>
	{

		LateBindEnumerable<Tsrc, IEnumerator<Tsrc>> enumerable;	// 列挙ソースを

		public IEnumerable<Tdst> query;							// オペレータ


		/// <summary>
		/// ソースを決定し、ＬＩＮＱクエリを実行する。
		/// </summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public IEnumerable<Tdst> Query( IEnumerable<Tsrc> source )
		{
			this.enumerable.GetEnumeratorFunc = source.GetEnumerator;

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

		static public List<T>.Enumerator GetEnumerator2<T>( this List<T> source )
		{
			return source.GetEnumerator();
		}
		static public TEnumerator GetEnumerator<T, TEnumerator>( IEnumerable<T> source )
		{
			return (TEnumerator)source.GetEnumerator();
		}
		static public EnumerableCache<T> CreateEnumerableCache<T, TEnumerator>( this IEnumerable<T> source )
		{
			return new EnumerableCache<T,TEnumerator>();
		}
	}
}


