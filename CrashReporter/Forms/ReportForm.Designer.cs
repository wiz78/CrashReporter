namespace info.tellini.CrashReporter.Forms
{
	partial class ReportForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if( disposing && ( components != null ) ) {
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( ReportForm ) );
			this.label1 = new System.Windows.Forms.Label();
			this.tbName = new System.Windows.Forms.TextBox();
			this.lbEMail = new System.Windows.Forms.Label();
			this.tbEMail = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.tbComments = new System.Windows.Forms.TextBox();
			this.btSend = new System.Windows.Forms.Button();
			this.btCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			resources.ApplyResources( this.label1, "label1" );
			this.label1.Name = "label1";
			// 
			// tbName
			// 
			resources.ApplyResources( this.tbName, "tbName" );
			this.tbName.Name = "tbName";
			// 
			// lbEMail
			// 
			resources.ApplyResources( this.lbEMail, "lbEMail" );
			this.lbEMail.Name = "lbEMail";
			// 
			// tbEMail
			// 
			resources.ApplyResources( this.tbEMail, "tbEMail" );
			this.tbEMail.Name = "tbEMail";
			// 
			// label2
			// 
			resources.ApplyResources( this.label2, "label2" );
			this.label2.Name = "label2";
			// 
			// label3
			// 
			resources.ApplyResources( this.label3, "label3" );
			this.label3.Name = "label3";
			// 
			// tbComments
			// 
			this.tbComments.AcceptsReturn = true;
			this.tbComments.AcceptsTab = true;
			resources.ApplyResources( this.tbComments, "tbComments" );
			this.tbComments.Name = "tbComments";
			// 
			// btSend
			// 
			resources.ApplyResources( this.btSend, "btSend" );
			this.btSend.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btSend.Name = "btSend";
			this.btSend.UseVisualStyleBackColor = true;
			// 
			// btCancel
			// 
			resources.ApplyResources( this.btCancel, "btCancel" );
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Name = "btCancel";
			this.btCancel.UseVisualStyleBackColor = true;
			// 
			// ReportForm
			// 
			this.AcceptButton = this.btSend;
			resources.ApplyResources( this, "$this" );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btCancel;
			this.Controls.Add( this.btCancel );
			this.Controls.Add( this.btSend );
			this.Controls.Add( this.tbComments );
			this.Controls.Add( this.label3 );
			this.Controls.Add( this.label2 );
			this.Controls.Add( this.tbEMail );
			this.Controls.Add( this.lbEMail );
			this.Controls.Add( this.tbName );
			this.Controls.Add( this.label1 );
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ReportForm";
			this.ShowIcon = false;
			this.ResumeLayout( false );
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbName;
		private System.Windows.Forms.Label lbEMail;
		private System.Windows.Forms.TextBox tbEMail;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox tbComments;
		private System.Windows.Forms.Button btSend;
		private System.Windows.Forms.Button btCancel;
	}
}