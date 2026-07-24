namespace RO_Server_Rebuild_2.UC
{
    partial class UC_AdminSettings
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
            this.rootLayout = new System.Windows.Forms.TableLayoutPanel();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSubTitle = new System.Windows.Forms.Label();
            this.btnBack = new System.Windows.Forms.Button();
            this.dbPanel = new System.Windows.Forms.Panel();
            this.lblDbSectionTitle = new System.Windows.Forms.Label();
            this.lblDbDescription = new System.Windows.Forms.Label();
            this.lblDbServer = new System.Windows.Forms.Label();
            this.lblDbPort = new System.Windows.Forms.Label();
            this.lblDbName = new System.Windows.Forms.Label();
            this.lblDbUserId = new System.Windows.Forms.Label();
            this.lblDbPassword = new System.Windows.Forms.Label();
            this.txtDbServer = new System.Windows.Forms.TextBox();
            this.txtDbPort = new System.Windows.Forms.TextBox();
            this.txtDbName = new System.Windows.Forms.TextBox();
            this.txtDbUserId = new System.Windows.Forms.TextBox();
            this.txtDbPassword = new System.Windows.Forms.TextBox();
            this.btnDbTest = new System.Windows.Forms.Button();
            this.btnDbApply = new System.Windows.Forms.Button();
            this.apiPanel = new System.Windows.Forms.Panel();
            this.lblApiSectionTitle = new System.Windows.Forms.Label();
            this.lblApiDescription = new System.Windows.Forms.Label();
            this.lblApiPort = new System.Windows.Forms.Label();
            this.lblApiKey = new System.Windows.Forms.Label();
            this.txtApiPort = new System.Windows.Forms.TextBox();
            this.txtApiKey = new System.Windows.Forms.TextBox();
            this.btnApiTest = new System.Windows.Forms.Button();
            this.btnApiApply = new System.Windows.Forms.Button();
            this.guidePanel = new System.Windows.Forms.Panel();
            this.lblGuideTitle = new System.Windows.Forms.Label();
            this.lblGuide = new System.Windows.Forms.Label();
            this.rootLayout.SuspendLayout();
            this.headerPanel.SuspendLayout();
            this.dbPanel.SuspendLayout();
            this.apiPanel.SuspendLayout();
            this.guidePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // rootLayout
            // 
            this.rootLayout.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(247)))), ((int)(((byte)(249)))));
            this.rootLayout.ColumnCount = 1;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Controls.Add(this.headerPanel, 0, 0);
            this.rootLayout.Controls.Add(this.dbPanel, 0, 1);
            this.rootLayout.Controls.Add(this.apiPanel, 0, 2);
            this.rootLayout.Controls.Add(this.guidePanel, 0, 3);
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.Location = new System.Drawing.Point(0, 0);
            this.rootLayout.Margin = new System.Windows.Forms.Padding(0);
            this.rootLayout.Name = "rootLayout";
            this.rootLayout.RowCount = 4;
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 64F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 174F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 142F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.Size = new System.Drawing.Size(1560, 508);
            this.rootLayout.TabIndex = 0;
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(48)))), ((int)(((byte)(67)))));
            this.headerPanel.Controls.Add(this.lblTitle);
            this.headerPanel.Controls.Add(this.lblSubTitle);
            this.headerPanel.Controls.Add(this.btnBack);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Margin = new System.Windows.Forms.Padding(0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(1560, 64);
            this.headerPanel.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("맑은 고딕", 15F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(18, 10);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(119, 28);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "관리자 설정";
            // 
            // lblSubTitle
            // 
            this.lblSubTitle.AutoSize = true;
            this.lblSubTitle.Font = new System.Drawing.Font("맑은 고딕", 8.5F);
            this.lblSubTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(205)))), ((int)(((byte)(218)))));
            this.lblSubTitle.Location = new System.Drawing.Point(21, 40);
            this.lblSubTitle.Name = "lblSubTitle";
            this.lblSubTitle.Size = new System.Drawing.Size(288, 15);
            this.lblSubTitle.TabIndex = 1;
            this.lblSubTitle.Text = "데이터베이스와 API 서버의 연결 정보를 관리합니다.";
            // 
            // btnBack
            // 
            this.btnBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBack.BackColor = System.Drawing.Color.LightSlateGray;
            this.btnBack.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(112)))), ((int)(((byte)(140)))));
            this.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBack.Font = new System.Drawing.Font("맑은 고딕", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnBack.ForeColor = System.Drawing.Color.White;
            this.btnBack.Location = new System.Drawing.Point(1421, 18);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(109, 30);
            this.btnBack.TabIndex = 2;
            this.btnBack.Text = "메인 화면";
            this.btnBack.UseVisualStyleBackColor = false;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // dbPanel
            // 
            this.dbPanel.BackColor = System.Drawing.Color.White;
            this.dbPanel.Controls.Add(this.lblDbSectionTitle);
            this.dbPanel.Controls.Add(this.lblDbDescription);
            this.dbPanel.Controls.Add(this.lblDbServer);
            this.dbPanel.Controls.Add(this.lblDbPort);
            this.dbPanel.Controls.Add(this.lblDbName);
            this.dbPanel.Controls.Add(this.lblDbUserId);
            this.dbPanel.Controls.Add(this.lblDbPassword);
            this.dbPanel.Controls.Add(this.txtDbServer);
            this.dbPanel.Controls.Add(this.txtDbPort);
            this.dbPanel.Controls.Add(this.txtDbName);
            this.dbPanel.Controls.Add(this.txtDbUserId);
            this.dbPanel.Controls.Add(this.txtDbPassword);
            this.dbPanel.Controls.Add(this.btnDbTest);
            this.dbPanel.Controls.Add(this.btnDbApply);
            this.dbPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dbPanel.Location = new System.Drawing.Point(18, 76);
            this.dbPanel.Margin = new System.Windows.Forms.Padding(18, 12, 18, 8);
            this.dbPanel.Name = "dbPanel";
            this.dbPanel.Size = new System.Drawing.Size(1524, 154);
            this.dbPanel.TabIndex = 1;
            // 
            // lblDbSectionTitle
            // 
            this.lblDbSectionTitle.AutoSize = true;
            this.lblDbSectionTitle.Font = new System.Drawing.Font("맑은 고딕", 11F, System.Drawing.FontStyle.Bold);
            this.lblDbSectionTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(48)))), ((int)(((byte)(67)))));
            this.lblDbSectionTitle.Location = new System.Drawing.Point(18, 13);
            this.lblDbSectionTitle.Name = "lblDbSectionTitle";
            this.lblDbSectionTitle.Size = new System.Drawing.Size(169, 20);
            this.lblDbSectionTitle.TabIndex = 0;
            this.lblDbSectionTitle.Text = "데이터베이스 연결 설정";
            // 
            // lblDbDescription
            // 
            this.lblDbDescription.AutoSize = true;
            this.lblDbDescription.Font = new System.Drawing.Font("맑은 고딕", 8.5F);
            this.lblDbDescription.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(105)))), ((int)(((byte)(114)))), ((int)(((byte)(126)))));
            this.lblDbDescription.Location = new System.Drawing.Point(19, 39);
            this.lblDbDescription.Name = "lblDbDescription";
            this.lblDbDescription.Size = new System.Drawing.Size(350, 15);
            this.lblDbDescription.TabIndex = 1;
            this.lblDbDescription.Text = "PLC 기준정보와 수집 데이터를 저장할 MySQL 서버 정보입니다.";
            // 
            // lblDbServer
            // 
            this.lblDbServer.AutoSize = true;
            this.lblDbServer.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lblDbServer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(92)))), ((int)(((byte)(104)))));
            this.lblDbServer.Location = new System.Drawing.Point(18, 75);
            this.lblDbServer.Name = "lblDbServer";
            this.lblDbServer.Size = new System.Drawing.Size(65, 15);
            this.lblDbServer.TabIndex = 2;
            this.lblDbServer.Text = "DB Server";
            // 
            // lblDbPort
            // 
            this.lblDbPort.AutoSize = true;
            this.lblDbPort.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lblDbPort.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(92)))), ((int)(((byte)(104)))));
            this.lblDbPort.Location = new System.Drawing.Point(274, 75);
            this.lblDbPort.Name = "lblDbPort";
            this.lblDbPort.Size = new System.Drawing.Size(52, 15);
            this.lblDbPort.TabIndex = 3;
            this.lblDbPort.Text = "DB Port";
            // 
            // lblDbName
            // 
            this.lblDbName.AutoSize = true;
            this.lblDbName.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lblDbName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(92)))), ((int)(((byte)(104)))));
            this.lblDbName.Location = new System.Drawing.Point(380, 75);
            this.lblDbName.Name = "lblDbName";
            this.lblDbName.Size = new System.Drawing.Size(63, 15);
            this.lblDbName.TabIndex = 4;
            this.lblDbName.Text = "DB Name";
            // 
            // lblDbUserId
            // 
            this.lblDbUserId.AutoSize = true;
            this.lblDbUserId.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lblDbUserId.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(92)))), ((int)(((byte)(104)))));
            this.lblDbUserId.Location = new System.Drawing.Point(596, 75);
            this.lblDbUserId.Name = "lblDbUserId";
            this.lblDbUserId.Size = new System.Drawing.Size(51, 15);
            this.lblDbUserId.TabIndex = 5;
            this.lblDbUserId.Text = "User ID";
            // 
            // lblDbPassword
            // 
            this.lblDbPassword.AutoSize = true;
            this.lblDbPassword.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lblDbPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(92)))), ((int)(((byte)(104)))));
            this.lblDbPassword.Location = new System.Drawing.Point(772, 75);
            this.lblDbPassword.Name = "lblDbPassword";
            this.lblDbPassword.Size = new System.Drawing.Size(63, 15);
            this.lblDbPassword.TabIndex = 6;
            this.lblDbPassword.Text = "Password";
            // 
            // txtDbServer
            // 
            this.txtDbServer.Font = new System.Drawing.Font("맑은 고딕", 9.5F);
            this.txtDbServer.Location = new System.Drawing.Point(18, 98);
            this.txtDbServer.Name = "txtDbServer";
            this.txtDbServer.Size = new System.Drawing.Size(240, 24);
            this.txtDbServer.TabIndex = 7;
            this.txtDbServer.Text = "192.168.0.249";
            // 
            // txtDbPort
            // 
            this.txtDbPort.Font = new System.Drawing.Font("맑은 고딕", 9.5F);
            this.txtDbPort.Location = new System.Drawing.Point(274, 98);
            this.txtDbPort.Name = "txtDbPort";
            this.txtDbPort.Size = new System.Drawing.Size(90, 24);
            this.txtDbPort.TabIndex = 8;
            this.txtDbPort.Text = "3306";
            // 
            // txtDbName
            // 
            this.txtDbName.Font = new System.Drawing.Font("맑은 고딕", 9.5F);
            this.txtDbName.Location = new System.Drawing.Point(380, 98);
            this.txtDbName.Name = "txtDbName";
            this.txtDbName.Size = new System.Drawing.Size(200, 24);
            this.txtDbName.TabIndex = 9;
            this.txtDbName.Text = "test";
            // 
            // txtDbUserId
            // 
            this.txtDbUserId.Font = new System.Drawing.Font("맑은 고딕", 9.5F);
            this.txtDbUserId.Location = new System.Drawing.Point(596, 98);
            this.txtDbUserId.Name = "txtDbUserId";
            this.txtDbUserId.Size = new System.Drawing.Size(160, 24);
            this.txtDbUserId.TabIndex = 10;
            this.txtDbUserId.Text = "hdrbdbtest";
            // 
            // txtDbPassword
            // 
            this.txtDbPassword.Font = new System.Drawing.Font("맑은 고딕", 9.5F);
            this.txtDbPassword.Location = new System.Drawing.Point(772, 98);
            this.txtDbPassword.Name = "txtDbPassword";
            this.txtDbPassword.Size = new System.Drawing.Size(220, 24);
            this.txtDbPassword.TabIndex = 11;
            this.txtDbPassword.Text = "hdrbdbtest1234!";
            this.txtDbPassword.UseSystemPasswordChar = true;
            // 
            // btnDbTest
            // 
            this.btnDbTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDbTest.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(112)))), ((int)(((byte)(140)))));
            this.btnDbTest.FlatAppearance.BorderSize = 0;
            this.btnDbTest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDbTest.Font = new System.Drawing.Font("맑은 고딕", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnDbTest.ForeColor = System.Drawing.Color.White;
            this.btnDbTest.Location = new System.Drawing.Point(1274, 89);
            this.btnDbTest.Name = "btnDbTest";
            this.btnDbTest.Size = new System.Drawing.Size(110, 36);
            this.btnDbTest.TabIndex = 12;
            this.btnDbTest.Text = "연결 테스트";
            this.btnDbTest.UseVisualStyleBackColor = false;
            this.btnDbTest.Click += new System.EventHandler(this.btnDbTest_Click);
            // 
            // btnDbApply
            // 
            this.btnDbApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDbApply.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(78)))), ((int)(((byte)(121)))));
            this.btnDbApply.FlatAppearance.BorderSize = 0;
            this.btnDbApply.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDbApply.Font = new System.Drawing.Font("맑은 고딕", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnDbApply.ForeColor = System.Drawing.Color.White;
            this.btnDbApply.Location = new System.Drawing.Point(1394, 89);
            this.btnDbApply.Name = "btnDbApply";
            this.btnDbApply.Size = new System.Drawing.Size(110, 36);
            this.btnDbApply.TabIndex = 13;
            this.btnDbApply.Text = "설정 적용";
            this.btnDbApply.UseVisualStyleBackColor = false;
            this.btnDbApply.Click += new System.EventHandler(this.btnDbApply_Click);
            // 
            // apiPanel
            // 
            this.apiPanel.BackColor = System.Drawing.Color.White;
            this.apiPanel.Controls.Add(this.lblApiSectionTitle);
            this.apiPanel.Controls.Add(this.lblApiDescription);
            this.apiPanel.Controls.Add(this.lblApiPort);
            this.apiPanel.Controls.Add(this.lblApiKey);
            this.apiPanel.Controls.Add(this.txtApiPort);
            this.apiPanel.Controls.Add(this.txtApiKey);
            this.apiPanel.Controls.Add(this.btnApiTest);
            this.apiPanel.Controls.Add(this.btnApiApply);
            this.apiPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.apiPanel.Location = new System.Drawing.Point(18, 244);
            this.apiPanel.Margin = new System.Windows.Forms.Padding(18, 6, 18, 6);
            this.apiPanel.Name = "apiPanel";
            this.apiPanel.Size = new System.Drawing.Size(1524, 130);
            this.apiPanel.TabIndex = 2;
            // 
            // lblApiSectionTitle
            // 
            this.lblApiSectionTitle.AutoSize = true;
            this.lblApiSectionTitle.Font = new System.Drawing.Font("맑은 고딕", 11F, System.Drawing.FontStyle.Bold);
            this.lblApiSectionTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(48)))), ((int)(((byte)(67)))));
            this.lblApiSectionTitle.Location = new System.Drawing.Point(18, 12);
            this.lblApiSectionTitle.Name = "lblApiSectionTitle";
            this.lblApiSectionTitle.Size = new System.Drawing.Size(104, 20);
            this.lblApiSectionTitle.TabIndex = 0;
            this.lblApiSectionTitle.Text = "API 서버 설정";
            // 
            // lblApiDescription
            // 
            this.lblApiDescription.AutoSize = true;
            this.lblApiDescription.Font = new System.Drawing.Font("맑은 고딕", 8.5F);
            this.lblApiDescription.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(105)))), ((int)(((byte)(114)))), ((int)(((byte)(126)))));
            this.lblApiDescription.Location = new System.Drawing.Point(19, 37);
            this.lblApiDescription.Name = "lblApiDescription";
            this.lblApiDescription.Size = new System.Drawing.Size(339, 15);
            this.lblApiDescription.TabIndex = 1;
            this.lblApiDescription.Text = "모니터링 클라이언트의 요청을 수신할 Port와 인증 Key입니다.";
            // 
            // lblApiPort
            // 
            this.lblApiPort.AutoSize = true;
            this.lblApiPort.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lblApiPort.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(92)))), ((int)(((byte)(104)))));
            this.lblApiPort.Location = new System.Drawing.Point(18, 66);
            this.lblApiPort.Name = "lblApiPort";
            this.lblApiPort.Size = new System.Drawing.Size(54, 15);
            this.lblApiPort.TabIndex = 2;
            this.lblApiPort.Text = "API Port";
            // 
            // lblApiKey
            // 
            this.lblApiKey.AutoSize = true;
            this.lblApiKey.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold);
            this.lblApiKey.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(92)))), ((int)(((byte)(104)))));
            this.lblApiKey.Location = new System.Drawing.Point(142, 66);
            this.lblApiKey.Name = "lblApiKey";
            this.lblApiKey.Size = new System.Drawing.Size(51, 15);
            this.lblApiKey.TabIndex = 3;
            this.lblApiKey.Text = "API Key";
            // 
            // txtApiPort
            // 
            this.txtApiPort.Font = new System.Drawing.Font("맑은 고딕", 9.5F);
            this.txtApiPort.Location = new System.Drawing.Point(18, 88);
            this.txtApiPort.Name = "txtApiPort";
            this.txtApiPort.Size = new System.Drawing.Size(100, 24);
            this.txtApiPort.TabIndex = 4;
            this.txtApiPort.Text = "3410";
            // 
            // txtApiKey
            // 
            this.txtApiKey.Font = new System.Drawing.Font("맑은 고딕", 9.5F);
            this.txtApiKey.Location = new System.Drawing.Point(142, 88);
            this.txtApiKey.Name = "txtApiKey";
            this.txtApiKey.Size = new System.Drawing.Size(420, 24);
            this.txtApiKey.TabIndex = 5;
            this.txtApiKey.Text = "HYUNDAI_RB_RO_2026";
            this.txtApiKey.UseSystemPasswordChar = true;
            // 
            // btnApiTest
            // 
            this.btnApiTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApiTest.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(112)))), ((int)(((byte)(140)))));
            this.btnApiTest.FlatAppearance.BorderSize = 0;
            this.btnApiTest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApiTest.Font = new System.Drawing.Font("맑은 고딕", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnApiTest.ForeColor = System.Drawing.Color.White;
            this.btnApiTest.Location = new System.Drawing.Point(1274, 80);
            this.btnApiTest.Name = "btnApiTest";
            this.btnApiTest.Size = new System.Drawing.Size(110, 36);
            this.btnApiTest.TabIndex = 6;
            this.btnApiTest.Text = "입력 확인";
            this.btnApiTest.UseVisualStyleBackColor = false;
            this.btnApiTest.Click += new System.EventHandler(this.btnApiTest_Click);
            // 
            // btnApiApply
            // 
            this.btnApiApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApiApply.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(78)))), ((int)(((byte)(121)))));
            this.btnApiApply.FlatAppearance.BorderSize = 0;
            this.btnApiApply.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApiApply.Font = new System.Drawing.Font("맑은 고딕", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnApiApply.ForeColor = System.Drawing.Color.White;
            this.btnApiApply.Location = new System.Drawing.Point(1394, 80);
            this.btnApiApply.Name = "btnApiApply";
            this.btnApiApply.Size = new System.Drawing.Size(110, 36);
            this.btnApiApply.TabIndex = 7;
            this.btnApiApply.Text = "설정 적용";
            this.btnApiApply.UseVisualStyleBackColor = false;
            this.btnApiApply.Click += new System.EventHandler(this.btnApiApply_Click);
            // 
            // guidePanel
            // 
            this.guidePanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(238)))), ((int)(((byte)(246)))));
            this.guidePanel.Controls.Add(this.lblGuideTitle);
            this.guidePanel.Controls.Add(this.lblGuide);
            this.guidePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.guidePanel.Location = new System.Drawing.Point(18, 386);
            this.guidePanel.Margin = new System.Windows.Forms.Padding(18, 6, 18, 14);
            this.guidePanel.Name = "guidePanel";
            this.guidePanel.Size = new System.Drawing.Size(1524, 64);
            this.guidePanel.TabIndex = 3;
            // 
            // lblGuideTitle
            // 
            this.lblGuideTitle.AutoSize = true;
            this.lblGuideTitle.Font = new System.Drawing.Font("맑은 고딕", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblGuideTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(78)))), ((int)(((byte)(121)))));
            this.lblGuideTitle.Location = new System.Drawing.Point(18, 12);
            this.lblGuideTitle.Name = "lblGuideTitle";
            this.lblGuideTitle.Size = new System.Drawing.Size(65, 17);
            this.lblGuideTitle.TabIndex = 0;
            this.lblGuideTitle.Text = "적용 안내";
            // 
            // lblGuide
            // 
            this.lblGuide.AutoSize = true;
            this.lblGuide.Font = new System.Drawing.Font("맑은 고딕", 9F);
            this.lblGuide.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(78)))), ((int)(((byte)(96)))));
            this.lblGuide.Location = new System.Drawing.Point(18, 36);
            this.lblGuide.Name = "lblGuide";
            this.lblGuide.Size = new System.Drawing.Size(545, 15);
            this.lblGuide.TabIndex = 1;
            this.lblGuide.Text = "연결 테스트 또는 입력 확인이 성공한 설정만 적용하며, 적용 결과는 통합 로그 화면에서 확인합니다.";
            // 
            // UC_AdminSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(247)))), ((int)(((byte)(249)))));
            this.Controls.Add(this.rootLayout);
            this.Name = "UC_AdminSettings";
            this.Size = new System.Drawing.Size(1560, 508);
            this.rootLayout.ResumeLayout(false);
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.dbPanel.ResumeLayout(false);
            this.dbPanel.PerformLayout();
            this.apiPanel.ResumeLayout(false);
            this.apiPanel.PerformLayout();
            this.guidePanel.ResumeLayout(false);
            this.guidePanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubTitle;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Panel dbPanel;
        private System.Windows.Forms.Label lblDbSectionTitle;
        private System.Windows.Forms.Label lblDbDescription;
        private System.Windows.Forms.Label lblDbServer;
        private System.Windows.Forms.Label lblDbPort;
        private System.Windows.Forms.Label lblDbName;
        private System.Windows.Forms.Label lblDbUserId;
        private System.Windows.Forms.Label lblDbPassword;
        private System.Windows.Forms.TextBox txtDbServer;
        private System.Windows.Forms.TextBox txtDbPort;
        private System.Windows.Forms.TextBox txtDbName;
        private System.Windows.Forms.TextBox txtDbUserId;
        private System.Windows.Forms.TextBox txtDbPassword;
        private System.Windows.Forms.Button btnDbTest;
        private System.Windows.Forms.Button btnDbApply;
        private System.Windows.Forms.Panel apiPanel;
        private System.Windows.Forms.Label lblApiSectionTitle;
        private System.Windows.Forms.Label lblApiDescription;
        private System.Windows.Forms.Label lblApiPort;
        private System.Windows.Forms.Label lblApiKey;
        private System.Windows.Forms.TextBox txtApiPort;
        private System.Windows.Forms.TextBox txtApiKey;
        private System.Windows.Forms.Button btnApiTest;
        private System.Windows.Forms.Button btnApiApply;
        private System.Windows.Forms.Panel guidePanel;
        private System.Windows.Forms.Label lblGuideTitle;
        private System.Windows.Forms.Label lblGuide;
    }
}
