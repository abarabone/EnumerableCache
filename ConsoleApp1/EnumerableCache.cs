using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System;

namespace Abss.UtilityOther
{
	// List<T> とかならオペレータをキャッシュして、中身を変えれば使いまわせる。
	// ただし、列挙ソースを変更したいときは、工夫が必要となる。
	
	// 例えば
	//	static Func<IEnumerable<int>, IEnumerable<int>>
	//		calcLengths = xs => xs
	//			.Select( model => model.BoneLength * model.ModelInstanceCount )
	//			;
	// みたいなのを持っておけばいいかな？と思ったけど、これだと calcLengths( source ) とした時に
	// はじめて Linq のオペレータオブジェクトが生成されるので、やっぱりキャッシュオブジェクトは必要。

	// しかし、Linq の各オペレータのイテレータブロックで毎回 class enumerator が生成されるだろうから、
	// ここまでする意味はないのかも（気にするなら Linq 使うな、っていう）

	
	/// <summary>
	/// ＬＩＮＱオブジェクトをキャッシュする。
	/// </summary>
	public class EnumerableCache<Tsrc, Tdst> : IEnumerable<Tsrc>
	{
		
		IEnumerable<Tsrc>			enumerableSource;
		
		internal IEnumerable<Tdst>	query;
		
		
		IEnumerator<Tsrc> IEnumerable<Tsrc>.GetEnumerator() => enumerableSource.GetEnumerator();
		
		IEnumerator IEnumerable.GetEnumerator() => enumerableSource.GetEnumerator();


		/// <summary>
		/// 列挙ソースを決定し、ＬＩＮＱオペレータを返す。
		/// </summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public IEnumerable<Tdst> Query( IEnumerable<Tsrc> source )
		{
			this.enumerableSource = source;

			return this.query;
		}
		
		/// <summary>
		/// ＬＩＮＱオペレータをセットする。
		/// </summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public EnumerableCache<Tsrc, Tdst> SetQuery
			( Func<EnumerableCache<Tsrc, Tdst>, IEnumerable<Tdst>> setOperationFunc )
		{
			this.query = setOperationFunc( this );

			return this;
		}
		
	}
	
	
	static public partial class LateBindEnumerableExtension
	{

		/// <summary>
		/// 列挙ソースにキャッシュオブジェクトを渡す形でセットし、ＬＩＮＱオペレータを返す。
		/// </summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		static public IEnumerable<Tdst> QueryWith<Tsrc, Tdst>
			( this IEnumerable<Tsrc> enumerableSource, EnumerableCache<Tsrc, Tdst> enumerableCache )
		{
			return enumerableCache.Query( enumerableSource );
		}

	}
}


