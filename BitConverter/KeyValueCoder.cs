using System;
using System.Reflection;

namespace Utils
{
	public static class KeyValueCoder
	{
		public static object TryGetFieldValue ( object obj, string path, out bool res ) {
			string [] pathNodes = path.Split ( '.' );
			object curObj = obj;
			Type curType = obj.GetType ();
			FieldInfo fi;
			res = false;

			foreach ( string node in pathNodes ) {
				string curNode = node;
				string [] arrIndexNodes = node.Split ( new char [] { '[', ']' } );
				int [] indices = null;

				if ( arrIndexNodes.Length > 1 ) {
					curNode = arrIndexNodes [0].TrimEnd ();

					string [] withoutEmptiness = new string [( arrIndexNodes.Length - 1 ) >> 1];

					for ( int i = 1 ; i < arrIndexNodes.Length ; i += 2 )
						withoutEmptiness [( i - 1 ) >> 1] = arrIndexNodes [i];

					arrIndexNodes = withoutEmptiness;
					indices = new int [arrIndexNodes.Length];

					for ( int i = 0 ; i < arrIndexNodes.Length ; i++ )
						indices [i] = Convert.ToInt32 ( arrIndexNodes [i] );
				}

				if ( null != ( fi = curType.GetField ( curNode ) ) ) {
					if ( ( indices != null ) != ( fi.FieldType.IsArray ) )
						return	null;

					if ( null == ( curObj = fi.GetValue ( curObj ) ) )
						return	null;
					else {
						if ( indices != null && fi.FieldType.IsArray ) {
							Array arr = ( Array ) curObj;

							try { curObj = arr.GetValue ( indices ); } catch { return	null; }
							if ( null == curObj ) return	null;
						}

						curType = curObj.GetType ();
					}
				}
			}

			res = true;

			return	curObj;
		}

		public static object TryGetFieldValue ( object obj, string path ) {
			bool res;
			return	TryGetFieldValue ( obj, path, out res );
		}

		public static bool TrySetFieldValue ( object obj, string path, object value ) {
			string [] pathNodes = path.Split ( '.' );
			object curObj = obj;
			Type curType = obj.GetType ();
			FieldInfo fi = null;
			int j = 1;
			int [] indices = null;

			foreach ( string node in pathNodes ) {
				string curNode = node;
				string [] arrIndexNodes = node.Split ( new char [] { '[', ']' } );
				indices = null;

				if ( arrIndexNodes.Length > 1 ) {
					curNode = arrIndexNodes [0].TrimEnd ();

					string [] withoutEmptiness = new string [( arrIndexNodes.Length - 1 ) >> 1];

					for ( int i = 1 ; i < arrIndexNodes.Length ; i += 2 )
						withoutEmptiness [( i - 1 ) >> 1] = arrIndexNodes [i];

					arrIndexNodes = withoutEmptiness;
					indices = new int [arrIndexNodes.Length];

					for ( int i = 0 ; i < arrIndexNodes.Length ; i++ )
						indices [i] = Convert.ToInt32 ( arrIndexNodes [i] );
				}

				if ( null != ( fi = curType.GetField ( curNode ) ) ) {
					if ( ( indices != null ) != ( fi.FieldType.IsArray ) )
						return	false;

					object prevObj = curObj;

					if ( null == ( curObj = fi.GetValue ( curObj ) ) )
						return	false;
					else {
						if ( j >= pathNodes.Length ) {
							if ( indices == null && !fi.FieldType.IsArray )
								curObj = prevObj;

							break;
						}

						if ( indices != null && fi.FieldType.IsArray ) {
							Array arr = ( Array ) curObj;

							try { curObj = arr.GetValue ( indices ); } catch { return	false; }
							if ( null == curObj ) return	false;
						}
						
						curType = curObj.GetType ();
					}
				}

				j++;
			}

			if ( indices != null && fi != null && fi.FieldType.IsArray ) {
				Array arr = ( Array ) curObj;

				try { arr.SetValue ( value, indices ); } catch { return	false; }
			} else
				fi.SetValue ( curObj, value );

			return	true;
		}
	}
}
