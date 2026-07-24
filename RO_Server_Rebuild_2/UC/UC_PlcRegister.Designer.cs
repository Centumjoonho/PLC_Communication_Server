namespace RO_Server_Rebuild_2.UC
{
    partial class UC_PlcRegister
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnBack = new System.Windows.Forms.Button();
            this.inputPanel = new System.Windows.Forms.Panel();
            this.lblPlcCode = new System.Windows.Forms.Label();
            this.lblPlcName = new System.Windows.Forms.Label();
            this.lblPlcIp = new System.Windows.Forms.Label();
            this.lblPlcPort = new System.Windows.Forms.Label();
            this.lblMemoryAddress = new System.Windows.Forms.Label();
            this.txtPlcCode = new System.Windows.Forms.TextBox();
            this.txtPlcName = new System.Windows.Forms.TextBox();
            this.txtPlcIp = new System.Windows.Forms.TextBox();
            this.txtPlcPort = new System.Windows.Forms.TextBox();
            this.txtMemoryAddress = new System.Windows.Forms.TextBox();
            this.chkUseYn = new System.Windows.Forms.CheckBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.lblListTitle = new System.Windows.Forms.Label();
            this.gridPlcMaster = new System.Windows.Forms.DataGridView();
            this.headerPanel.SuspendLayout();
            this.inputPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridPlcMaster)).BeginInit();
            this.SuspendLayout();
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(48)))), ((int)(((byte)(67)))));
            this.headerPanel.Controls.Add(this.lblTitle);
            this.headerPanel.Controls.Add(this.btnBack);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(1560, 64);
            this.headerPanel.TabIndex = 1;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("맑은 고딕", 15F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(18, 17);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(141, 28);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "PLC 설비 등록";
            // 
            // btnBack
            // 
            this.btnBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBack.BackColor = System.Drawing.Color.LightSlateGray;
            this.btnBack.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(112)))), ((int)(((byte)(140)))));
            this.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBack.Font = new System.Drawing.Font("맑은 고딕", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnBack.ForeColor = System.Drawing.Color.White;
            this.btnBack.Location = new System.Drawing.Point(1421, 20);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(109, 28);
            this.btnBack.TabIndex = 1;
            this.btnBack.Text = "메인 화면";
            this.btnBack.UseVisualStyleBackColor = false;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // inputPanel
            // 
            this.inputPanel.BackColor = System.Drawing.Color.White;
            this.inputPanel.Controls.Add(this.lblPlcCode);
            this.inputPanel.Controls.Add(this.lblPlcName);
            this.inputPanel.Controls.Add(this.lblPlcIp);
            this.inputPanel.Controls.Add(this.lblPlcPort);
            this.inputPanel.Controls.Add(this.lblMemoryAddress);
            this.inputPanel.Controls.Add(this.txtPlcCode);
            this.inputPanel.Controls.Add(this.txtPlcName);
            this.inputPanel.Controls.Add(this.txtPlcIp);
            this.inputPanel.Controls.Add(this.txtPlcPort);
            this.inputPanel.Controls.Add(this.txtMemoryAddress);
            this.inputPanel.Controls.Add(this.chkUseYn);
            this.inputPanel.Controls.Add(this.btnClear);
            this.inputPanel.Controls.Add(this.btnSave);
            this.inputPanel.Controls.Add(this.btnDelete);
            this.inputPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.inputPanel.Location = new System.Drawing.Point(0, 64);
            this.inputPanel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.inputPanel.Name = "inputPanel";
            this.inputPanel.Size = new System.Drawing.Size(1560, 132);
            this.inputPanel.TabIndex = 2;
            // 
            // lblPlcCode
            // 
            this.lblPlcCode.AutoSize = true;
            this.lblPlcCode.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lblPlcCode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(92)))), ((int)(((byte)(104)))));
            this.lblPlcCode.Location = new System.Drawing.Point(24, 24);
            this.lblPlcCode.Name = "lblPlcCode";
            this.lblPlcCode.Size = new System.Drawing.Size(62, 15);
            this.lblPlcCode.TabIndex = 0;
            this.lblPlcCode.Text = "PLC Code";
            // 
            // lblPlcName
            // 
            this.lblPlcName.AutoSize = true;
            this.lblPlcName.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lblPlcName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(92)))), ((int)(((byte)(104)))));
            this.lblPlcName.Location = new System.Drawing.Point(224, 24);
            this.lblPlcName.Name = "lblPlcName";
            this.lblPlcName.Size = new System.Drawing.Size(67, 15);
            this.lblPlcName.TabIndex = 1;
            this.lblPlcName.Text = "PLC Name";
            // 
            // lblPlcIp
            // 
            this.lblPlcIp.AutoSize = true;
            this.lblPlcIp.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lblPlcIp.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(92)))), ((int)(((byte)(104)))));
            this.lblPlcIp.Location = new System.Drawing.Point(484, 24);
            this.lblPlcIp.Name = "lblPlcIp";
            this.lblPlcIp.Size = new System.Drawing.Size(18, 15);
            this.lblPlcIp.TabIndex = 2;
            this.lblPlcIp.Text = "IP";
            // 
            // lblPlcPort
            // 
            this.lblPlcPort.AutoSize = true;
            this.lblPlcPort.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lblPlcPort.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(92)))), ((int)(((byte)(104)))));
            this.lblPlcPort.Location = new System.Drawing.Point(704, 24);
            this.lblPlcPort.Name = "lblPlcPort";
            this.lblPlcPort.Size = new System.Drawing.Size(31, 15);
            this.lblPlcPort.TabIndex = 3;
            this.lblPlcPort.Text = "Port";
            // 
            // lblMemoryAddress
            // 
            this.lblMemoryAddress.AutoSize = true;
            this.lblMemoryAddress.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lblMemoryAddress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(92)))), ((int)(((byte)(104)))));
            this.lblMemoryAddress.Location = new System.Drawing.Point(844, 24);
            this.lblMemoryAddress.Name = "lblMemoryAddress";
            this.lblMemoryAddress.Size = new System.Drawing.Size(107, 15);
            this.lblMemoryAddress.TabIndex = 4;
            this.lblMemoryAddress.Text = "Memory Address";
            // 
            // txtPlcCode
            // 
            this.txtPlcCode.Font = new System.Drawing.Font("맑은 고딕", 9.5F);
            this.txtPlcCode.Location = new System.Drawing.Point(24, 48);
            this.txtPlcCode.Name = "txtPlcCode";
            this.txtPlcCode.Size = new System.Drawing.Size(170, 24);
            this.txtPlcCode.TabIndex = 5;
            // 
            // txtPlcName
            // 
            this.txtPlcName.Font = new System.Drawing.Font("맑은 고딕", 9.5F);
            this.txtPlcName.Location = new System.Drawing.Point(224, 48);
            this.txtPlcName.Name = "txtPlcName";
            this.txtPlcName.Size = new System.Drawing.Size(230, 24);
            this.txtPlcName.TabIndex = 6;
            // 
            // txtPlcIp
            // 
            this.txtPlcIp.Font = new System.Drawing.Font("맑은 고딕", 9.5F);
            this.txtPlcIp.Location = new System.Drawing.Point(484, 48);
            this.txtPlcIp.Name = "txtPlcIp";
            this.txtPlcIp.Size = new System.Drawing.Size(190, 24);
            this.txtPlcIp.TabIndex = 7;
            // 
            // txtPlcPort
            // 
            this.txtPlcPort.Font = new System.Drawing.Font("맑은 고딕", 9.5F);
            this.txtPlcPort.Location = new System.Drawing.Point(704, 48);
            this.txtPlcPort.Name = "txtPlcPort";
            this.txtPlcPort.Size = new System.Drawing.Size(110, 24);
            this.txtPlcPort.TabIndex = 8;
            this.txtPlcPort.Text = "502";
            // 
            // txtMemoryAddress
            // 
            this.txtMemoryAddress.Font = new System.Drawing.Font("맑은 고딕", 9.5F);
            this.txtMemoryAddress.Location = new System.Drawing.Point(844, 48);
            this.txtMemoryAddress.Name = "txtMemoryAddress";
            this.txtMemoryAddress.Size = new System.Drawing.Size(140, 24);
            this.txtMemoryAddress.TabIndex = 9;
            this.txtMemoryAddress.Text = "30";
            // 
            // chkUseYn
            // 
            this.chkUseYn.AutoSize = true;
            this.chkUseYn.Checked = true;
            this.chkUseYn.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUseYn.Font = new System.Drawing.Font("맑은 고딕", 9.5F, System.Drawing.FontStyle.Bold);
            this.chkUseYn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(48)))), ((int)(((byte)(67)))));
            this.chkUseYn.Location = new System.Drawing.Point(1020, 50);
            this.chkUseYn.Name = "chkUseYn";
            this.chkUseYn.Size = new System.Drawing.Size(53, 21);
            this.chkUseYn.TabIndex = 10;
            this.chkUseYn.Text = "사용";
            this.chkUseYn.UseVisualStyleBackColor = true;
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(78)))), ((int)(((byte)(121)))));
            this.btnClear.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClear.Font = new System.Drawing.Font("맑은 고딕", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnClear.ForeColor = System.Drawing.Color.White;
            this.btnClear.Location = new System.Drawing.Point(1240, 41);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(90, 36);
            this.btnClear.TabIndex = 11;
            this.btnClear.Text = "신규";
            this.btnClear.UseVisualStyleBackColor = false;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(78)))), ((int)(((byte)(121)))));
            this.btnSave.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("맑은 고딕", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(1341, 41);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(90, 36);
            this.btnSave.TabIndex = 12;
            this.btnSave.Text = "저장";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(78)))), ((int)(((byte)(121)))));
            this.btnDelete.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Font = new System.Drawing.Font("맑은 고딕", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnDelete.ForeColor = System.Drawing.Color.White;
            this.btnDelete.Location = new System.Drawing.Point(1440, 41);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(90, 36);
            this.btnDelete.TabIndex = 13;
            this.btnDelete.Text = "삭제";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // lblListTitle
            // 
            this.lblListTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblListTitle.Font = new System.Drawing.Font("맑은 고딕", 10F, System.Drawing.FontStyle.Bold);
            this.lblListTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(48)))), ((int)(((byte)(67)))));
            this.lblListTitle.Location = new System.Drawing.Point(0, 196);
            this.lblListTitle.Margin = new System.Windows.Forms.Padding(0);
            this.lblListTitle.Name = "lblListTitle";
            this.lblListTitle.Size = new System.Drawing.Size(1560, 40);
            this.lblListTitle.TabIndex = 3;
            this.lblListTitle.Text = "등록된 PLC 설비 목록";
            this.lblListTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // gridPlcMaster
            // 
            this.gridPlcMaster.AllowUserToAddRows = false;
            this.gridPlcMaster.AllowUserToDeleteRows = false;
            this.gridPlcMaster.AllowUserToResizeRows = false;
            this.gridPlcMaster.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridPlcMaster.BackgroundColor = System.Drawing.Color.White;
            this.gridPlcMaster.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(68)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("맑은 고딕", 9.5F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(49)))), ((int)(((byte)(68)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridPlcMaster.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridPlcMaster.ColumnHeadersHeight = 44;
            this.gridPlcMaster.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("맑은 고딕", 9.5F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(242)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(40)))), ((int)(((byte)(54)))));
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridPlcMaster.DefaultCellStyle = dataGridViewCellStyle2;
            this.gridPlcMaster.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridPlcMaster.EnableHeadersVisualStyles = false;
            this.gridPlcMaster.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(229)))), ((int)(((byte)(235)))));
            this.gridPlcMaster.Location = new System.Drawing.Point(0, 236);
            this.gridPlcMaster.Margin = new System.Windows.Forms.Padding(0);
            this.gridPlcMaster.MultiSelect = false;
            this.gridPlcMaster.Name = "gridPlcMaster";
            this.gridPlcMaster.ReadOnly = true;
            this.gridPlcMaster.RowHeadersVisible = false;
            this.gridPlcMaster.RowTemplate.Height = 34;
            this.gridPlcMaster.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridPlcMaster.Size = new System.Drawing.Size(1560, 272);
            this.gridPlcMaster.TabIndex = 4;
            this.gridPlcMaster.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridPlcMaster_CellClick);
            // 
            // UC_PlcRegister
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridPlcMaster);
            this.Controls.Add(this.lblListTitle);
            this.Controls.Add(this.inputPanel);
            this.Controls.Add(this.headerPanel);
            this.Name = "UC_PlcRegister";
            this.Size = new System.Drawing.Size(1560, 508);
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.inputPanel.ResumeLayout(false);
            this.inputPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridPlcMaster)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Panel inputPanel;
        private System.Windows.Forms.Label lblPlcCode;
        private System.Windows.Forms.Label lblPlcName;
        private System.Windows.Forms.Label lblPlcIp;
        private System.Windows.Forms.Label lblPlcPort;
        private System.Windows.Forms.Label lblMemoryAddress;
        private System.Windows.Forms.TextBox txtPlcCode;
        private System.Windows.Forms.TextBox txtPlcName;
        private System.Windows.Forms.TextBox txtPlcIp;
        private System.Windows.Forms.TextBox txtPlcPort;
        private System.Windows.Forms.TextBox txtMemoryAddress;
        private System.Windows.Forms.CheckBox chkUseYn;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Label lblListTitle;
        private System.Windows.Forms.DataGridView gridPlcMaster;
    }
}
