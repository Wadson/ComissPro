﻿namespace ComissPro
{
    partial class FrmPrincipal
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmPrincipal));
            this.kryptonPalette1 = new ComponentFactory.Krypton.Toolkit.KryptonPalette(this.components);
            this.kryptonStatusStrip1 = new Krypton.Toolkit.KryptonStatusStrip();
            this.lblEstacao = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel4 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblData = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblHoraAtual = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel5 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblUsuarioLogadoo = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel6 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblTipoUsuarioo = new System.Windows.Forms.ToolStripStatusLabel();
            this.kryptonPanel2 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.btnFerramentas = new System.Windows.Forms.Button();
            this.btnRelatorios = new System.Windows.Forms.Button();
            this.btnUsuario = new System.Windows.Forms.Button();
            this.btnVendedor = new System.Windows.Forms.Button();
            this.btnSair = new System.Windows.Forms.Button();
            this.btnPrestacaoContas = new System.Windows.Forms.Button();
            this.btnProduto = new System.Windows.Forms.Button();
            this.btnManutencaoEntregas = new System.Windows.Forms.Button();
            this.panelConteiner = new System.Windows.Forms.Panel();
            this.timerH = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.cadastrosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.usuáriosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.vendedoresToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.produtosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.kryptonPanel1 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel2)).BeginInit();
            this.kryptonPanel2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.SuspendLayout();
            // 
            // kryptonPalette1
            // 
            this.kryptonPalette1.FormStyles.FormMain.StateCommon.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.kryptonPalette1.FormStyles.FormMain.StateCommon.Back.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.kryptonPalette1.FormStyles.FormMain.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.kryptonPalette1.FormStyles.FormMain.StateCommon.Border.GraphicsHint = ComponentFactory.Krypton.Toolkit.PaletteGraphicsHint.None;
            this.kryptonPalette1.FormStyles.FormMain.StateCommon.Border.Rounding = 12;
            this.kryptonPalette1.HeaderStyles.HeaderForm.StateCommon.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.kryptonPalette1.HeaderStyles.HeaderForm.StateCommon.Back.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.kryptonPalette1.HeaderStyles.HeaderForm.StateCommon.ButtonEdgeInset = 10;
            this.kryptonPalette1.HeaderStyles.HeaderForm.StateCommon.Content.Padding = new System.Windows.Forms.Padding(10, -1, -1, -1);
            // 
            // kryptonStatusStrip1
            // 
            this.kryptonStatusStrip1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.kryptonStatusStrip1.Location = new System.Drawing.Point(0, 707);
            this.kryptonStatusStrip1.Name = "kryptonStatusStrip1";
            this.kryptonStatusStrip1.ProgressBars = null;
            this.kryptonStatusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode;
            this.kryptonStatusStrip1.Size = new System.Drawing.Size(1008, 22);
            this.kryptonStatusStrip1.TabIndex = 611;
            this.kryptonStatusStrip1.Text = "kryptonStatusStrip1";
            // 
            // lblEstacao
            // 
            this.lblEstacao.Name = "lblEstacao";
            this.lblEstacao.Size = new System.Drawing.Size(12, 17);
            this.lblEstacao.Text = "-";
            // 
            // toolStripStatusLabel4
            // 
            this.toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            this.toolStripStatusLabel4.Size = new System.Drawing.Size(10, 17);
            this.toolStripStatusLabel4.Text = "|";
            // 
            // lblData
            // 
            this.lblData.Name = "lblData";
            this.lblData.Size = new System.Drawing.Size(31, 17);
            this.lblData.Text = "Data";
            // 
            // lblHoraAtual
            // 
            this.lblHoraAtual.Name = "lblHoraAtual";
            this.lblHoraAtual.Size = new System.Drawing.Size(33, 17);
            this.lblHoraAtual.Text = "Hora";
            // 
            // toolStripStatusLabel5
            // 
            this.toolStripStatusLabel5.Name = "toolStripStatusLabel5";
            this.toolStripStatusLabel5.Size = new System.Drawing.Size(10, 17);
            this.toolStripStatusLabel5.Text = "|";
            // 
            // lblUsuarioLogadoo
            // 
            this.lblUsuarioLogadoo.Name = "lblUsuarioLogadoo";
            this.lblUsuarioLogadoo.Size = new System.Drawing.Size(12, 17);
            this.lblUsuarioLogadoo.Text = "-";
            // 
            // toolStripStatusLabel6
            // 
            this.toolStripStatusLabel6.Name = "toolStripStatusLabel6";
            this.toolStripStatusLabel6.Size = new System.Drawing.Size(10, 17);
            this.toolStripStatusLabel6.Text = "|";
            // 
            // lblTipoUsuarioo
            // 
            this.lblTipoUsuarioo.Name = "lblTipoUsuarioo";
            this.lblTipoUsuarioo.Size = new System.Drawing.Size(12, 17);
            this.lblTipoUsuarioo.Text = "-";
            // 
            // kryptonPanel2
            // 
            this.kryptonPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.kryptonPanel2.Controls.Add(this.btnFerramentas);
            this.kryptonPanel2.Controls.Add(this.btnRelatorios);
            this.kryptonPanel2.Controls.Add(this.btnUsuario);
            this.kryptonPanel2.Controls.Add(this.btnVendedor);
            this.kryptonPanel2.Controls.Add(this.btnSair);
            this.kryptonPanel2.Controls.Add(this.btnPrestacaoContas);
            this.kryptonPanel2.Controls.Add(this.btnProduto);
            this.kryptonPanel2.Controls.Add(this.btnManutencaoEntregas);
            this.kryptonPanel2.Location = new System.Drawing.Point(-1, 26);
            this.kryptonPanel2.Name = "kryptonPanel2";
            this.kryptonPanel2.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.kryptonPanel2.PanelBackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.GridHeaderColumnCustom1;
            this.kryptonPanel2.Size = new System.Drawing.Size(1010, 73);
            this.kryptonPanel2.TabIndex = 610;
            // 
            // btnFerramentas
            // 
            this.btnFerramentas.BackColor = System.Drawing.Color.Transparent;
            this.btnFerramentas.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(8)))), ((int)(((byte)(142)))), ((int)(((byte)(254)))));
            this.btnFerramentas.FlatAppearance.BorderSize = 0;
            this.btnFerramentas.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnFerramentas.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnFerramentas.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.btnFerramentas.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFerramentas.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnFerramentas.ImageIndex = 3;
            this.btnFerramentas.ImageList = this.imageList1;
            this.btnFerramentas.Location = new System.Drawing.Point(478, 1);
            this.btnFerramentas.Name = "btnFerramentas";
            this.btnFerramentas.Size = new System.Drawing.Size(75, 68);
            this.btnFerramentas.TabIndex = 11;
            this.btnFerramentas.Text = "&Ferramentas";
            this.btnFerramentas.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolTip1.SetToolTip(this.btnFerramentas, "Entrega de bilhetes e Prestação de contas");
            this.btnFerramentas.UseVisualStyleBackColor = false;
            this.btnFerramentas.Click += new System.EventHandler(this.btnFerramentas_Click);
            // 
            // btnRelatorios
            // 
            this.btnRelatorios.BackColor = System.Drawing.Color.Transparent;
            this.btnRelatorios.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(8)))), ((int)(((byte)(142)))), ((int)(((byte)(254)))));
            this.btnRelatorios.FlatAppearance.BorderSize = 0;
            this.btnRelatorios.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnRelatorios.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnRelatorios.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.btnRelatorios.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRelatorios.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnRelatorios.ImageIndex = 1;
            this.btnRelatorios.ImageList = this.imageList1;
            this.btnRelatorios.Location = new System.Drawing.Point(400, 4);
            this.btnRelatorios.Name = "btnRelatorios";
            this.btnRelatorios.Size = new System.Drawing.Size(75, 68);
            this.btnRelatorios.TabIndex = 10;
            this.btnRelatorios.Text = "&Relatórios";
            this.btnRelatorios.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolTip1.SetToolTip(this.btnRelatorios, "Prestação de contas");
            this.btnRelatorios.UseVisualStyleBackColor = false;
            this.btnRelatorios.Click += new System.EventHandler(this.btnRelatorios_Click);
            // 
            // btnUsuario
            // 
            this.btnUsuario.BackColor = System.Drawing.Color.Transparent;
            this.btnUsuario.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(8)))), ((int)(((byte)(142)))), ((int)(((byte)(254)))));
            this.btnUsuario.FlatAppearance.BorderSize = 0;
            this.btnUsuario.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnUsuario.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnUsuario.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.btnUsuario.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUsuario.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnUsuario.ImageIndex = 0;
            this.btnUsuario.ImageList = this.imageList1;
            this.btnUsuario.Location = new System.Drawing.Point(10, 3);
            this.btnUsuario.Name = "btnUsuario";
            this.btnUsuario.Size = new System.Drawing.Size(75, 68);
            this.btnUsuario.TabIndex = 1;
            this.btnUsuario.Text = "&Usuários";
            this.btnUsuario.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolTip1.SetToolTip(this.btnUsuario, "Cadastro de Usuários");
            this.btnUsuario.UseVisualStyleBackColor = false;
            // 
            // btnVendedor
            // 
            this.btnVendedor.BackColor = System.Drawing.Color.Transparent;
            this.btnVendedor.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(8)))), ((int)(((byte)(142)))), ((int)(((byte)(254)))));
            this.btnVendedor.FlatAppearance.BorderSize = 0;
            this.btnVendedor.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnVendedor.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnVendedor.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.btnVendedor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVendedor.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnVendedor.ImageIndex = 9;
            this.btnVendedor.ImageList = this.imageList1;
            this.btnVendedor.Location = new System.Drawing.Point(88, 2);
            this.btnVendedor.Name = "btnVendedor";
            this.btnVendedor.Size = new System.Drawing.Size(75, 68);
            this.btnVendedor.TabIndex = 2;
            this.btnVendedor.Text = "&Vendedor";
            this.btnVendedor.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolTip1.SetToolTip(this.btnVendedor, "Cadastro de Vendedores");
            this.btnVendedor.UseVisualStyleBackColor = false;
            this.btnVendedor.Click += new System.EventHandler(this.btnVendedor_Click);
            // 
            // btnSair
            // 
            this.btnSair.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSair.BackColor = System.Drawing.Color.Transparent;
            this.btnSair.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(8)))), ((int)(((byte)(142)))), ((int)(((byte)(254)))));
            this.btnSair.FlatAppearance.BorderSize = 0;
            this.btnSair.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnSair.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnSair.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.btnSair.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSair.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnSair.ImageIndex = 7;
            this.btnSair.ImageList = this.imageList1;
            this.btnSair.Location = new System.Drawing.Point(931, 3);
            this.btnSair.Name = "btnSair";
            this.btnSair.Size = new System.Drawing.Size(75, 68);
            this.btnSair.TabIndex = 7;
            this.btnSair.Text = "&Sair";
            this.btnSair.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnSair.UseVisualStyleBackColor = false;
            this.btnSair.Click += new System.EventHandler(this.btnSair_Click);
            // 
            // btnPrestacaoContas
            // 
            this.btnPrestacaoContas.BackColor = System.Drawing.Color.Transparent;
            this.btnPrestacaoContas.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(8)))), ((int)(((byte)(142)))), ((int)(((byte)(254)))));
            this.btnPrestacaoContas.FlatAppearance.BorderSize = 0;
            this.btnPrestacaoContas.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnPrestacaoContas.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnPrestacaoContas.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.btnPrestacaoContas.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrestacaoContas.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnPrestacaoContas.ImageIndex = 2;
            this.btnPrestacaoContas.ImageList = this.imageList1;
            this.btnPrestacaoContas.Location = new System.Drawing.Point(322, 4);
            this.btnPrestacaoContas.Name = "btnPrestacaoContas";
            this.btnPrestacaoContas.Size = new System.Drawing.Size(75, 68);
            this.btnPrestacaoContas.TabIndex = 9;
            this.btnPrestacaoContas.Text = "&Prestação Concluída";
            this.btnPrestacaoContas.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolTip1.SetToolTip(this.btnPrestacaoContas, "Prestação de contas");
            this.btnPrestacaoContas.UseVisualStyleBackColor = false;
            this.btnPrestacaoContas.Click += new System.EventHandler(this.btnPrestacaoContas_Click);
            // 
            // btnProduto
            // 
            this.btnProduto.BackColor = System.Drawing.Color.Transparent;
            this.btnProduto.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnProduto.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(8)))), ((int)(((byte)(142)))), ((int)(((byte)(254)))));
            this.btnProduto.FlatAppearance.BorderSize = 0;
            this.btnProduto.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnProduto.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnProduto.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.btnProduto.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnProduto.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnProduto.ImageIndex = 10;
            this.btnProduto.ImageList = this.imageList1;
            this.btnProduto.Location = new System.Drawing.Point(166, 3);
            this.btnProduto.Name = "btnProduto";
            this.btnProduto.Size = new System.Drawing.Size(75, 68);
            this.btnProduto.TabIndex = 4;
            this.btnProduto.Text = "&Produto";
            this.btnProduto.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolTip1.SetToolTip(this.btnProduto, "Cadastro de Produtos / Bilhetes");
            this.btnProduto.UseVisualStyleBackColor = false;
            this.btnProduto.Click += new System.EventHandler(this.btnProduto_Click);
            // 
            // btnManutencaoEntregas
            // 
            this.btnManutencaoEntregas.BackColor = System.Drawing.Color.Transparent;
            this.btnManutencaoEntregas.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(8)))), ((int)(((byte)(142)))), ((int)(((byte)(254)))));
            this.btnManutencaoEntregas.FlatAppearance.BorderSize = 0;
            this.btnManutencaoEntregas.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnManutencaoEntregas.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnManutencaoEntregas.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.btnManutencaoEntregas.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnManutencaoEntregas.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnManutencaoEntregas.ImageIndex = 8;
            this.btnManutencaoEntregas.ImageList = this.imageList1;
            this.btnManutencaoEntregas.Location = new System.Drawing.Point(244, 4);
            this.btnManutencaoEntregas.Name = "btnManutencaoEntregas";
            this.btnManutencaoEntregas.Size = new System.Drawing.Size(75, 68);
            this.btnManutencaoEntregas.TabIndex = 6;
            this.btnManutencaoEntregas.Text = "&Vendas";
            this.btnManutencaoEntregas.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.toolTip1.SetToolTip(this.btnManutencaoEntregas, "Entrega de bilhetes e Prestação de contas");
            this.btnManutencaoEntregas.UseVisualStyleBackColor = false;
            this.btnManutencaoEntregas.Click += new System.EventHandler(this.btnManutencaoEntregas_Click);
            // 
            // panelConteiner
            // 
            this.panelConteiner.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelConteiner.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.panelConteiner.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.panelConteiner.Location = new System.Drawing.Point(3, 152);
            this.panelConteiner.Name = "panelConteiner";
            this.panelConteiner.Size = new System.Drawing.Size(1002, 519);
            this.panelConteiner.TabIndex = 609;
            // 
            // timerH
            // 
            this.timerH.Interval = 1000;
            this.timerH.Tick += new System.EventHandler(this.timerH_Tick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cadastrosToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1008, 24);
            this.menuStrip1.TabIndex = 612;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // cadastrosToolStripMenuItem
            // 
            this.cadastrosToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.usuáriosToolStripMenuItem,
            this.vendedoresToolStripMenuItem,
            this.produtosToolStripMenuItem});
            this.cadastrosToolStripMenuItem.Name = "cadastrosToolStripMenuItem";
            this.cadastrosToolStripMenuItem.Size = new System.Drawing.Size(71, 20);
            this.cadastrosToolStripMenuItem.Text = "Cadastros";
            // 
            // usuáriosToolStripMenuItem
            // 
            this.usuáriosToolStripMenuItem.Name = "usuáriosToolStripMenuItem";
            this.usuáriosToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.usuáriosToolStripMenuItem.Text = "&Usuários";
            // 
            // vendedoresToolStripMenuItem
            // 
            this.vendedoresToolStripMenuItem.Name = "vendedoresToolStripMenuItem";
            this.vendedoresToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.vendedoresToolStripMenuItem.Text = "&Vendedores";
            this.vendedoresToolStripMenuItem.Click += new System.EventHandler(this.vendedoresToolStripMenuItem_Click);
            // 
            // produtosToolStripMenuItem
            // 
            this.produtosToolStripMenuItem.Name = "produtosToolStripMenuItem";
            this.produtosToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.produtosToolStripMenuItem.Text = "&Produtos";
            this.produtosToolStripMenuItem.Click += new System.EventHandler(this.produtosToolStripMenuItem_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "adicionar-usuario.png");
            this.imageList1.Images.SetKeyName(1, "comissao.png");
            this.imageList1.Images.SetKeyName(2, "Comissao64.png");
            this.imageList1.Images.SetKeyName(3, "engrenagem64.png");
            this.imageList1.Images.SetKeyName(4, "EntregaBilhete64.png");
            this.imageList1.Images.SetKeyName(5, "PrestacaoDeconta64.png");
            this.imageList1.Images.SetKeyName(6, "Realizado64.png");
            this.imageList1.Images.SetKeyName(7, "Ssair64.png");
            this.imageList1.Images.SetKeyName(8, "Venda64.png");
            this.imageList1.Images.SetKeyName(9, "Vendedores64.png");
            this.imageList1.Images.SetKeyName(10, "Bilhete64.png");
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.kryptonPanel1.Location = new System.Drawing.Point(-2, 100);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.kryptonPanel1.PanelBackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.GridBackgroundCustom1;
            this.kryptonPanel1.Size = new System.Drawing.Size(1010, 50);
            this.kryptonPanel1.TabIndex = 613;
            // 
            // FrmPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(1008, 729);
            this.Controls.Add(this.kryptonPanel1);
            this.Controls.Add(this.kryptonStatusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.kryptonPanel2);
            this.Controls.Add(this.panelConteiner);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FrmPrincipal";
            this.Palette = this.kryptonPalette1;
            this.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ComissPro - Sistema de Controle de Comissão";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FrmPrincipal_Load);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel2)).EndInit();
            this.kryptonPanel2.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPalette kryptonPalette1;
        private Krypton.Toolkit.KryptonStatusStrip kryptonStatusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblEstacao;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel4;
        private System.Windows.Forms.ToolStripStatusLabel lblData;
        private System.Windows.Forms.ToolStripStatusLabel lblHoraAtual;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel5;
        private System.Windows.Forms.ToolStripStatusLabel lblUsuarioLogadoo;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel6;
        private System.Windows.Forms.ToolStripStatusLabel lblTipoUsuarioo;
        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel2;
        private System.Windows.Forms.Button btnUsuario;
        private System.Windows.Forms.Button btnVendedor;
        private System.Windows.Forms.Button btnSair;
        private System.Windows.Forms.Button btnPrestacaoContas;
        private System.Windows.Forms.Button btnProduto;
        private System.Windows.Forms.Button btnManutencaoEntregas;
        private System.Windows.Forms.Panel panelConteiner;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Timer timerH;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem cadastrosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem usuáriosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem vendedoresToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem produtosToolStripMenuItem;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnRelatorios;
        private System.Windows.Forms.Button btnFerramentas;
        private System.Windows.Forms.ImageList imageList1;
        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
    }
}