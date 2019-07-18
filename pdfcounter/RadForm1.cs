using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using iTextSharp.text.xml;
using iTextSharp.text.pdf;


namespace pdfcounter
{
	public partial class RadForm1 : Telerik.WinControls.UI.RadForm
	{
		public RadForm1()
		{
			InitializeComponent();
		}

		private void RadForm1_Load(object sender, EventArgs e)
		{
			string dir= @"C:\COMPARTIDO\Cobranza\";
			LoadDirectory(dir);
		}
		private void LlenarGrid()
		{

		}
		public void LoadDirectory(string Dir)
		{
			DirectoryInfo di = new DirectoryInfo(Dir);
			//Setting ProgressBar Maximum Value  
			TreeNode tds = treeView1.Nodes.Add(di.Name);
			tds.Tag = di.FullName;
			//tds.StateImageIndex = 0;
			//tds.StateImageIndex = 6;
			//tds.ImageIndex = 6;
			//LoadFiles(Dir, tds);
			DirectoryInfo dir = new DirectoryInfo(Dir);
			
			LoadSubDirectories(dir, tds);
			
		}
	
		private void LoadSubDirectories(DirectoryInfo dir, TreeNode td)
		{
			// Get all subdirectories  
			DirectoryInfo[] subdirectoryEntries = dir.GetDirectories();
			// Loop through them to see if they have any other subdirectories  
			int driveImage;
			foreach (DirectoryInfo subdirectory in subdirectoryEntries)
			{
				DriveInfo dr = new DriveInfo(subdirectory.FullName);
				switch (dr.DriveType)    //set the drive's icon
				{
					case DriveType.CDRom:
						driveImage = 3;
						break;
					case DriveType.Network:
						driveImage = 6;
						break;
					case DriveType.NoRootDirectory:
						driveImage = 8;
						break;
					case DriveType.Unknown:
						driveImage = 8;
						break;
					default:
						driveImage = 2;
						break;
				}
				
				DirectoryInfo di = new DirectoryInfo(subdirectory.FullName);
				TreeNode tds = td.Nodes.Add(di.Name, di.Name, driveImage);
				
				
				
				tds.ImageIndex = driveImage;
				tds.SelectedImageIndex = driveImage;
				tds.ImageKey = driveImage.ToString();
				tds.SelectedImageKey = driveImage.ToString();
				tds.StateImageIndex = driveImage;
				tds.Tag = di.FullName;
				//LoadFiles(subdirectory, tds);
				LoadSubDirectories(subdirectory, tds);

			}
		}
		private void LoadFiles(string dir, TreeNode td)
		{
			string[] Files = Directory.GetFiles(dir, "*.*");

			// Loop through them to see files  
			foreach (string file in Files)
			{
				FileInfo fi = new FileInfo(file);
				TreeNode tds = td.Nodes.Add(fi.Name);
				tds.Tag = fi.FullName;
				
			}
		}

		private void treeView1_MouseMove(object sender, MouseEventArgs e)
		{
			// Get the node at the current mouse pointer location.  
			TreeNode theNode = this.treeView1.GetNodeAt(e.X, e.Y);

			// Set a ToolTip only if the mouse pointer is actually paused on a node.  
			if (theNode != null && theNode.Tag != null)
			{
				// Change the ToolTip only if the pointer moved to a new node.  
				if (theNode.Tag.ToString() != this.toolTip1.GetToolTip(this.treeView1))
					this.toolTip1.SetToolTip(this.treeView1, theNode.Tag.ToString());

			}
			else     // Pointer is not over a node so clear the ToolTip.  
			{
				this.toolTip1.SetToolTip(this.treeView1, "");
			}
		}

		private void treeView1_Click(object sender, EventArgs e)
		{
			
			
		}

		private void button1_Click(object sender, EventArgs e)
		{
			dataGridView1.DataSource = null;
			dataGridView1.Update();
			lbl_paginas.Text = "";
			TreeNode tree = new TreeNode();
			tree = treeView1.SelectedNode;
			DirectoryInfo df = new DirectoryInfo(@"C:\COMPARTIDO\" + tree.FullPath);
			DirectoryInfo[] fo = df.GetDirectories();
			FileInfo[] archivos=null;
			FileInfo[] fi;
			List<FileInfo> filepdf= new List<FileInfo>();
			if (fo.Count()>0)
			{
				foreach(DirectoryInfo dy in fo)
				{
					foreach(FileInfo inf in dy.GetFiles("*.pdf*"))
					{
						filepdf.Add(inf);
					}
					
				}
				archivos = filepdf.ToArray();
			}
			else
			{
				archivos = df.GetFiles("*.pdf*");
				
			}
			fi = archivos;
			string path = "";
			int numberOfPages = 0;
			DataTable table = new DataTable();
			DataColumn fNameColumn = new DataColumn();
			fNameColumn.DataType = System.Type.GetType("System.String");
			fNameColumn.ColumnName = "Archivo";

			table.Columns.Add(fNameColumn);
			fNameColumn = new DataColumn();
			fNameColumn.DataType = System.Type.GetType("System.String");
			fNameColumn.ColumnName = "Paginas";
			table.Columns.Add(fNameColumn);
			foreach (FileInfo file in fi)
			{
				path = file.FullName;
				PdfReader pdfReader = new PdfReader(path);
				numberOfPages = pdfReader.NumberOfPages;

				DataRow row;
				row = table.NewRow();
				row["Archivo"] = file.Name;
				row["Paginas"] = numberOfPages;
				table.Rows.Add(row);

			}
			dataGridView1.DataSource = table;
			dataGridView1.Columns[0].Width = 250;
			dataGridView1.Update();
			int sumapaginas = 0;
			foreach (DataGridViewRow row in dataGridView1.Rows)
			{
				sumapaginas += int.Parse(row.Cells[1].Value.ToString());
			}
			lbl_paginas.Text = sumapaginas.ToString();

		}
	}

}
