using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System;

namespace Abss.UtilityOther
{
	// List<T> のような、struct enumerator の場合はどうやって扱っていいのだろうか…
	// と思ったが、おそらくＬＩＮＱに IEnumerable<T> として渡される以上、不可能。

	// でも結局、ＬＩＮＱ使用すると内部で class enumerator がオペレータ分生成されるんだろうから、
	// その中の一つだけ struct enumerator になっても大差ない気がする…。

	
	/// <summary>
	/// ＬＩＮＱオブジェクトをキャッシュする。struct enumerator を扱う。
	/// ＬＩＮＱのソースになった時点で、struct enumerator が無意味になることがわかったので、意義がなくなった。
	/// （ IEnumerable<T> として渡される以上、インターフェースメソッドが呼ばれるため）
	/// </summary>
	public class EnumerableCache<Tsrc, Tdst, TEnumerator> : IEnumerable<Tsrc>
		where TEnumerator : struct, IEnumerator<Tsrc>
	{
		
		Func<TEnumerator>			getEnumeratorFunc;	// どうやっても struct 版の GetEnumerator() を使用させる方法が
														// 思いつかなかったため…
		public IEnumerable<Tdst>	query;

		
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public TEnumerator GetEnumerator() => getEnumeratorFunc();// 結局ＬＩＮＱからはこいつが呼ばれてくれない
		
		IEnumerator<Tsrc> IEnumerable<Tsrc>.GetEnumerator() => getEnumeratorFunc();
		
		IEnumerator IEnumerable.GetEnumerator() => getEnumeratorFunc();

		
		/// <summary>
		/// 列挙ソースを決定し、ＬＩＮＱオペレータを返す。
		/// </summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public IEnumerable<Tdst> Query( Func<TEnumerator> getEnumerator )
		{
			this.getEnumeratorFunc = getEnumerator;

			return this.query;
		}
		
	}
	
	
	static public partial class LateBindEnumerableExtension
	{
		
		/// <summary>
		/// 列挙ソースにキャッシュオブジェクトを渡す形でセットし、ＬＩＮＱオペレータを返す。
		/// </summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		static public IEnumerable<Tdst> QueryWith<Tsrc, Tdst, TEnumerator>
			( this Func<TEnumerator> getEnumerator, EnumerableCache<Tsrc, Tdst, TEnumerator> enumerableCache )
			where TEnumerator : struct, IEnumerator<Tsrc>
		{
			return enumerableCache.Query( getEnumerator );
		}
		
		/// <summary>
		/// ＬＩＮＱオペレータを、渡されたキャッシュオブジェクトにセットする。
		/// </summary>
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		static public void ToCache<Tsrc, Tdst, TEnumerator>
			( this IEnumerable<Tdst> query, EnumerableCache<Tsrc, Tdst, TEnumerator> cache )
			where TEnumerator : struct, IEnumerator<Tsrc>
		{
			cache.query = query;
		}

	}
}


