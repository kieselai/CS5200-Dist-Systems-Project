using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyUtilities
{
    static public class LinqUpdateCollection {

        public static void Update<T, T2>( this IDictionary<T, T2> collection, Action<T2> action ) {
            foreach( var key in collection.Keys ) {
                action( collection[key] );
            }
        }

       public static void Update<T> (this IList<T> arr, Action<T> action  ) {
            for( var i = 0; i < arr.Count; i++ ) {
                action(arr[i]);
            }
        }

        public static void Update<T> (this T[] arr, Action<T> action  ) {
            for( var i = 0; i < arr.Length; i++ ) {
                action(arr[i]);
            }
        }

        public static void Update<T> (this IEnumerable<T> collection, Action<T> action  ) {
            var iteratingCollection = collection;
            for( var i = 0; i < collection.Count(); i++ ) {
                action( iteratingCollection.Take(1).Single() );
                iteratingCollection = iteratingCollection.Skip(1);
            }
        }
    }
}
