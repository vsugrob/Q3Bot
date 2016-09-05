using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Drawing;
using Ionic.Zip;

namespace Q3Renderer
{
	public static class Q3FileSystem
	{
		#region Properties
		public static Dictionary <string, ZipEntry> resources = new Dictionary <string, ZipEntry> ();
		public static Dictionary <string, ZipEntry> maps = new Dictionary <string, ZipEntry> ();
		public static Dictionary <string, ZipEntry> models = new Dictionary <string, ZipEntry> ();
		public static Dictionary <string, ZipEntry> levelshots = new Dictionary <string, ZipEntry> ();
		public static Dictionary <string, Dictionary <string, string>> arenas = new Dictionary <string, Dictionary <string, string>> ();
		public static Dictionary <string, Dictionary <string, string>> bots = new Dictionary <string, Dictionary <string, string>> ();
		private static string baseDir;

		public static string BaseDirectory { get { return	baseDir; } }
		#endregion Properties

		#region Q3FileSystem Methods
		public static bool InitWithDirectory ( string dir ) {
			if ( !Directory.Exists ( dir ) )
				return	false;

			ScanDir ( dir );
			baseDir = dir;
			
			return	true;
		}

		private static void ScanDir ( string dir ) {
			string [] files = Directory.GetFiles ( dir, "*.pk3" );

			foreach ( string filename in files ) {
				ZipFile zip;

				try {
					zip = ZipFile.Read ( filename );
				} catch {
					Console.WriteLine ( @"Error while reading {0}", filename );
					continue;
				}

				foreach ( ZipEntry entry in zip.Entries ) {
					string entryFileName = entry.FileName.ToLower ();

					if ( !entry.IsDirectory && !resources.ContainsKey ( entryFileName ) ) {
						resources.Add ( entryFileName, entry );
						//Console.WriteLine ( entryFileName );

						if ( entryFileName.EndsWith ( ".bsp" ) && !maps.ContainsKey ( entryFileName ) )
							maps.Add ( entryFileName, entry );
						else if ( entryFileName.EndsWith ( ".md3" ) && !models.ContainsKey ( entryFileName ) )
							models.Add ( entryFileName, entry );
						else if ( entryFileName.StartsWith ( "scripts/" ) ) {
							if ( entryFileName.EndsWith ( ".shader" ) ) {
							} else {
								// May be arena(s)/bot(s) description so parse
								MemoryStream ms = new MemoryStream ();
								WriteResourceToStream ( entryFileName, ms );
								string contents = Encoding.ASCII.GetString ( ms.GetBuffer (), 0, ( int ) ms.Length );
								
								// Assume file contains only flat scopes
								string [] scopes = contents.Split ( new char [] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries );

								foreach ( string scope in scopes ) {
									if ( scope.Trim ().Length == 0 )
										continue;

									string [] props = scope.Split ( new string [] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries );
									Dictionary <string, string> keyVals = new Dictionary <string, string> ( props.Length );
									string map = null;	// for arenas
									string name = null;	// for bots

									foreach ( string p in props ) {
										Match m = Regex.Match ( p, @"(\w+)\s+(?:""?)(.*?)(?:""?)$" );

										if ( m.Success ) {
											string key = m.Groups [1].Value;
											string val = m.Groups [2].Value;

											keyVals [key] = val;

											if ( key == "map" )
												map = val;
											else if ( key == "name" )
												name = val;
										}
									}

									if ( map != null )
										arenas [map] = keyVals;
									else if ( name != null )
										bots [name] = keyVals;
								}
							}
						} else if ( entryFileName.StartsWith ( "levelshots/" ) ) {
							int lastSlashPos = entryFileName.LastIndexOf ( '/' );
							int lastDotPos = entryFileName.LastIndexOf ( '.' );
							string shotname = entryFileName.Substring ( lastSlashPos + 1, lastDotPos - lastSlashPos - 1 );
							levelshots [shotname] = entry;
						}
					}
				}
			}

			string [] subdirs = Directory.GetDirectories ( dir );

			foreach ( string dirname in subdirs )
				ScanDir ( dirname );
		}

		public static bool WriteResourceToStream ( string path, Stream stream ) {
			ZipEntry entry;

			if ( !resources.TryGetValue ( path, out entry ) )
				return	false;

			stream.SetLength ( entry.UncompressedSize );
			stream.Position = 0;
			entry.Extract ( stream );
			stream.Position = 0;

			return	true;
		}

		public static bool WriteResourceToStream ( ZipEntry entry, Stream stream ) {
			stream.SetLength ( entry.UncompressedSize );
			stream.Position = 0;
			entry.Extract ( stream );
			stream.Position = 0;

			return	true;
		}

		public static Image GetLevelShot ( string shotname ) {
			ZipEntry shotentry;

			if ( levelshots.TryGetValue ( shotname, out shotentry ) )
				return	GetLevelShot ( shotentry );
			else
				return	null;
		}

		public static Image GetLevelShot ( ZipEntry shotentry ) {
			MemoryStream ms = new MemoryStream ();
			WriteResourceToStream ( shotentry, ms );
			Image shot = null;
			
			try { shot = Image.FromStream ( ms ); }
			catch {}

			return	shot;
		}

		public static Image ResourceAsImage ( string path ) {
			ZipEntry imageEntry;

			if ( resources.TryGetValue ( path, out imageEntry ) ) {
				MemoryStream ms = new MemoryStream ();
				WriteResourceToStream ( imageEntry, ms );
				Image img = null;
				
				try { img = Image.FromStream ( ms ); }
				catch {}

				return	img;
			} else
				return	null;
		}
		#endregion Q3FileSystem Methods
	}
}
