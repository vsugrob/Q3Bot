using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Threading;

namespace Controls
{
	public partial class QuakeTextBox : UserControl
	{
		private Dictionary <Regex, string> qtf2rtf = new Dictionary <Regex,string> ();
		private Dictionary <Regex, string> rtf2qtf = new Dictionary <Regex,string> ();
		private string [] colorTable = new string [] {
			@"",
			@"\red255\green0\blue0",
			@"\red0\green255\blue0",
			@"\red255\green255\blue0",
			@"\red0\green0\blue255",
			@"\red0\green255\blue255",
			@"\red255\green0\blue255",
			@"\red255\green255\blue255"
		};
		private string rtfPrefix = @"{\rtf1\ansi\ansicpg1251\deff0\deflang1049" +
								   @"{\fonttbl{\f0\fswiss\fcharset0 Courier New;}}";
		private string rtfPostfix = @"}";
		private bool insertingColor = false;
		private bool colorInserted  = false;
		private AutoResetEvent qtfEvent = new AutoResetEvent ( true );

		public string Qtf {
			get {
				qtfEvent.WaitOne ();
				string rtf = this.rtfText.Rtf;
				int st = rtf.IndexOf ( @"\fs20" ) + 5;
				int len = rtf.Length - 3 - st;
				string qtf = rtf.Substring ( st, len );

				if ( rtf.Contains ( "colortbl" ) ) {
					st = rtf.IndexOf ( "\r\n" ) + 13;
					len = rtf.IndexOf ( "\r\n", st ) - st - 1;
					string [] rtfColorTbl = rtf.Substring ( st, len ).Split ( ';' );
					string [] qtfColorEquivalents = new string [rtfColorTbl.Length - 1];

					for ( int i = 0 ; i < rtfColorTbl.Length - 1 ; i++ )
						for ( int j = 0 ; j < colorTable.Length ; j++ )
							if ( colorTable [j] == rtfColorTbl [i] ) {
								qtfColorEquivalents [i] = j.ToString ();
								break;
							}
					
					st = rtf.IndexOf ( @"\viewkind4\uc1\pard" ) + 19;
					string startColor = rtf.Substring ( st, 4 );
					startColor = startColor.Contains ( @"\cf" ) ? startColor : @"\cf0";
					qtf = startColor + ( qtf.StartsWith ( "\\par\r\n" ) ? " " : "" ) + qtf;

					for ( int i = 0 ; i < qtfColorEquivalents.Length ; i++ )
						qtf = Regex.Replace ( qtf, @"(\\cf" + i.ToString () + ") ", "^" + qtfColorEquivalents [i] );
				}

				foreach ( KeyValuePair <Regex, string> keyVal in rtf2qtf )
					qtf = keyVal.Key.Replace ( qtf, keyVal.Value );

				qtfEvent.Set ();
				
				return	qtf;
			}

			set {
				qtfEvent.WaitOne ();

				if ( value != "\r\n" ) {
					foreach ( KeyValuePair <Regex, string> keyVal in qtf2rtf )
						value = keyVal.Key.Replace ( value, keyVal.Value );
				} else
					value = "";

				this.rtfText.Rtf = rtfPrefix + value + rtfPostfix;

				if ( this.rtfText.Lines.Length > 100 )
					this.rtfText.Text = "  ";

				qtfEvent.Set ();
			}
		}

		public int SelectionStart {
			get { return	rtfText.SelectionStart; }
			set { rtfText.SelectionStart = value; }
		}

		public override string Text {
			get { return	rtfText.Text; }
			set { rtfText.Text = value; }
		}

		public void ScrollToCaret () {
			rtfText.ScrollToCaret ();
		}

		public QuakeTextBox()
		{
			InitializeComponent();

			rtfPrefix += @"{\colortbl " + string.Join ( ";", colorTable ) + ";}" +
						 @"\viewkind4\uc1\pard\lang1033\f0\fs20";

			qtf2rtf.Add ( new Regex ( @"\\" ), @"\\" );
			qtf2rtf.Add ( new Regex ( @"\^(\d)" ), @"\cf$1 " );
			qtf2rtf.Add ( new Regex ( @"\^([a-z])", RegexOptions.IgnoreCase ), @"\cf0 " );
			qtf2rtf.Add ( new Regex ( @"\r\n", RegexOptions.Multiline ), @"\par " );
			rtf2qtf.Add ( new Regex ( @"\\\\" ), @"\" );
			rtf2qtf.Add ( new Regex ( @"\\par.?\r\n" ), "\r\n" );
		}

		private void rtfText_KeyUp(object sender, KeyEventArgs e)
		{
			if ( colorInserted ) {
				int caret = rtfText.SelectionStart;
				this.Qtf = this.Qtf;
				rtfText.SelectionStart = caret - 2;
				colorInserted = false;
			}
		}

		private void rtfText_KeyDown(object sender, KeyEventArgs e)
		{
			if ( insertingColor ) {
				if ( e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9 )
					colorInserted = true;
				else
					rtfText.Undo ();
				insertingColor = false;
			} else if ( e.Shift && e.KeyValue == 54 ) {
				insertingColor = true;
			}
		}
	}
}
