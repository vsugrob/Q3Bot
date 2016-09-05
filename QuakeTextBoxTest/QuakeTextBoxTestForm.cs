﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Controls;

namespace QuakeTextBoxTest
{
	public partial class QuakeTextBoxTestForm : Form
	{
		public QuakeTextBoxTestForm()
		{
			InitializeComponent();
		}

		private void btnQtf2Rtf_Click(object sender, EventArgs e)
		{
			qtfText.Qtf = txtQtfText.Text;
		}

		private void btnRtf2Qtf_Click(object sender, EventArgs e)
		{
			txtQtfText.Text = qtfText.Qtf;
		}

		private void btnIncrementRtf_Click(object sender, EventArgs e)
		{
			qtfText.Qtf += txtQtfText.Text;
		}
	}
}
