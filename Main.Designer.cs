namespace marcc
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            tabControl = new TabControl();
            bulkConvertPage = new TabPage();
            panel1 = new Panel();
            outputBrowse = new Button();
            inputBrowse = new Button();
            convertButton = new Button();
            logRichTextBox = new RichTextBox();
            outputTextBox = new TextBox();
            inputTextBox = new TextBox();
            outputLabel = new Label();
            inputLabel = new Label();
            pasteConvertPage = new TabPage();
            pasteLogRichTextBox = new RichTextBox();
            panel2 = new Panel();
            pasteNameTextBox = new TextBox();
            pasteNameLabel = new Label();
            pasteOutputBrowse = new Button();
            pasteOutputTextBox = new TextBox();
            pasteOutputLabel = new Label();
            pasteLabel = new Label();
            pasteConvert = new Button();
            pasteClearAll = new Button();
            pasteRichTextBox = new RichTextBox();
            tabControl.SuspendLayout();
            bulkConvertPage.SuspendLayout();
            pasteConvertPage.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl
            // 
            tabControl.Controls.Add(bulkConvertPage);
            tabControl.Controls.Add(pasteConvertPage);
            tabControl.Dock = DockStyle.Fill;
            tabControl.Location = new Point(0, 0);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(700, 710);
            tabControl.TabIndex = 0;
            // 
            // bulkConvertPage
            // 
            bulkConvertPage.BackColor = Color.Transparent;
            bulkConvertPage.Controls.Add(panel1);
            bulkConvertPage.Controls.Add(outputBrowse);
            bulkConvertPage.Controls.Add(inputBrowse);
            bulkConvertPage.Controls.Add(convertButton);
            bulkConvertPage.Controls.Add(logRichTextBox);
            bulkConvertPage.Controls.Add(outputTextBox);
            bulkConvertPage.Controls.Add(inputTextBox);
            bulkConvertPage.Controls.Add(outputLabel);
            bulkConvertPage.Controls.Add(inputLabel);
            bulkConvertPage.Location = new Point(4, 29);
            bulkConvertPage.Name = "bulkConvertPage";
            bulkConvertPage.Padding = new Padding(3);
            bulkConvertPage.Size = new Size(692, 677);
            bulkConvertPage.TabIndex = 0;
            bulkConvertPage.Text = "Converter Arquivos";
            // 
            // panel1
            // 
            panel1.BackColor = Color.LightGray;
            panel1.Location = new Point(18, 212);
            panel1.Name = "panel1";
            panel1.Size = new Size(655, 1);
            panel1.TabIndex = 9;
            // 
            // outputBrowse
            // 
            outputBrowse.Location = new Point(471, 110);
            outputBrowse.Name = "outputBrowse";
            outputBrowse.Size = new Size(202, 29);
            outputBrowse.TabIndex = 8;
            outputBrowse.Text = "Definir diretório de saída";
            outputBrowse.UseVisualStyleBackColor = true;
            outputBrowse.Click += outputBrowse_Click;
            // 
            // inputBrowse
            // 
            inputBrowse.Location = new Point(471, 41);
            inputBrowse.Name = "inputBrowse";
            inputBrowse.Size = new Size(202, 29);
            inputBrowse.TabIndex = 7;
            inputBrowse.Text = "Definir diretório de entrada";
            inputBrowse.UseVisualStyleBackColor = true;
            inputBrowse.Click += inputBrowse_Click;
            // 
            // convertButton
            // 
            convertButton.Location = new Point(18, 165);
            convertButton.Name = "convertButton";
            convertButton.Size = new Size(655, 29);
            convertButton.TabIndex = 5;
            convertButton.Text = "Converter arquivos para MARC (.mrc)";
            convertButton.UseVisualStyleBackColor = true;
            convertButton.Click += convertButton_Click;
            // 
            // logRichTextBox
            // 
            logRichTextBox.BackColor = Color.White;
            logRichTextBox.BorderStyle = BorderStyle.None;
            logRichTextBox.Location = new Point(18, 230);
            logRichTextBox.Name = "logRichTextBox";
            logRichTextBox.ReadOnly = true;
            logRichTextBox.Size = new Size(655, 428);
            logRichTextBox.TabIndex = 4;
            logRichTextBox.Text = "";
            // 
            // outputTextBox
            // 
            outputTextBox.BackColor = Color.White;
            outputTextBox.Location = new Point(18, 112);
            outputTextBox.Name = "outputTextBox";
            outputTextBox.Size = new Size(447, 27);
            outputTextBox.TabIndex = 3;
            // 
            // inputTextBox
            // 
            inputTextBox.BackColor = Color.White;
            inputTextBox.Enabled = false;
            inputTextBox.Location = new Point(18, 41);
            inputTextBox.Name = "inputTextBox";
            inputTextBox.ReadOnly = true;
            inputTextBox.Size = new Size(447, 27);
            inputTextBox.TabIndex = 2;
            // 
            // outputLabel
            // 
            outputLabel.AutoSize = true;
            outputLabel.Location = new Point(18, 89);
            outputLabel.Name = "outputLabel";
            outputLabel.Size = new Size(131, 20);
            outputLabel.TabIndex = 1;
            outputLabel.Text = "Diretório de Saída";
            // 
            // inputLabel
            // 
            inputLabel.AutoSize = true;
            inputLabel.Location = new Point(18, 18);
            inputLabel.Name = "inputLabel";
            inputLabel.Size = new Size(145, 20);
            inputLabel.TabIndex = 0;
            inputLabel.Text = "Diretório de Entrada";
            // 
            // pasteConvertPage
            // 
            pasteConvertPage.BackColor = Color.Transparent;
            pasteConvertPage.Controls.Add(pasteLogRichTextBox);
            pasteConvertPage.Controls.Add(panel2);
            pasteConvertPage.Controls.Add(pasteNameTextBox);
            pasteConvertPage.Controls.Add(pasteNameLabel);
            pasteConvertPage.Controls.Add(pasteOutputBrowse);
            pasteConvertPage.Controls.Add(pasteOutputTextBox);
            pasteConvertPage.Controls.Add(pasteOutputLabel);
            pasteConvertPage.Controls.Add(pasteLabel);
            pasteConvertPage.Controls.Add(pasteConvert);
            pasteConvertPage.Controls.Add(pasteClearAll);
            pasteConvertPage.Controls.Add(pasteRichTextBox);
            pasteConvertPage.Location = new Point(4, 29);
            pasteConvertPage.Name = "pasteConvertPage";
            pasteConvertPage.Padding = new Padding(3);
            pasteConvertPage.Size = new Size(692, 677);
            pasteConvertPage.TabIndex = 1;
            pasteConvertPage.Text = "Converter Texto";
            // 
            // pasteLogRichTextBox
            // 
            pasteLogRichTextBox.BackColor = Color.White;
            pasteLogRichTextBox.BorderStyle = BorderStyle.None;
            pasteLogRichTextBox.Location = new Point(18, 469);
            pasteLogRichTextBox.Name = "pasteLogRichTextBox";
            pasteLogRichTextBox.ReadOnly = true;
            pasteLogRichTextBox.Size = new Size(654, 192);
            pasteLogRichTextBox.TabIndex = 10;
            pasteLogRichTextBox.Text = "";
            // 
            // panel2
            // 
            panel2.BackColor = Color.LightGray;
            panel2.Location = new Point(18, 445);
            panel2.Name = "panel2";
            panel2.Size = new Size(654, 1);
            panel2.TabIndex = 9;
            // 
            // pasteNameTextBox
            // 
            pasteNameTextBox.Location = new Point(18, 105);
            pasteNameTextBox.Name = "pasteNameTextBox";
            pasteNameTextBox.Size = new Size(654, 27);
            pasteNameTextBox.TabIndex = 8;
            // 
            // pasteNameLabel
            // 
            pasteNameLabel.AutoSize = true;
            pasteNameLabel.Location = new Point(18, 82);
            pasteNameLabel.Name = "pasteNameLabel";
            pasteNameLabel.Size = new Size(186, 20);
            pasteNameLabel.TabIndex = 7;
            pasteNameLabel.Text = "Nome do arquivo de saída";
            // 
            // pasteOutputBrowse
            // 
            pasteOutputBrowse.Location = new Point(485, 38);
            pasteOutputBrowse.Name = "pasteOutputBrowse";
            pasteOutputBrowse.Size = new Size(187, 29);
            pasteOutputBrowse.TabIndex = 6;
            pasteOutputBrowse.Text = "Definir diretório de saída";
            pasteOutputBrowse.UseVisualStyleBackColor = true;
            pasteOutputBrowse.Click += pasteOutputBrowse_Click;
            // 
            // pasteOutputTextBox
            // 
            pasteOutputTextBox.Location = new Point(18, 38);
            pasteOutputTextBox.Name = "pasteOutputTextBox";
            pasteOutputTextBox.Size = new Size(461, 27);
            pasteOutputTextBox.TabIndex = 5;
            // 
            // pasteOutputLabel
            // 
            pasteOutputLabel.AutoSize = true;
            pasteOutputLabel.Location = new Point(18, 15);
            pasteOutputLabel.Name = "pasteOutputLabel";
            pasteOutputLabel.Size = new Size(131, 20);
            pasteOutputLabel.TabIndex = 4;
            pasteOutputLabel.Text = "Diretório de Saída";
            // 
            // pasteLabel
            // 
            pasteLabel.AutoSize = true;
            pasteLabel.Location = new Point(18, 151);
            pasteLabel.Name = "pasteLabel";
            pasteLabel.Size = new Size(315, 20);
            pasteLabel.TabIndex = 3;
            pasteLabel.Text = "Cole o texto para conversão no campo abaixo";
            // 
            // pasteConvert
            // 
            pasteConvert.Location = new Point(341, 397);
            pasteConvert.Name = "pasteConvert";
            pasteConvert.Size = new Size(331, 29);
            pasteConvert.TabIndex = 2;
            pasteConvert.Text = "Converter para MARC (.mrc)";
            pasteConvert.UseVisualStyleBackColor = true;
            pasteConvert.Click += pasteConvert_Click;
            // 
            // pasteClearAll
            // 
            pasteClearAll.Location = new Point(18, 397);
            pasteClearAll.Name = "pasteClearAll";
            pasteClearAll.Size = new Size(317, 29);
            pasteClearAll.TabIndex = 1;
            pasteClearAll.Text = "Limpar campos";
            pasteClearAll.UseVisualStyleBackColor = true;
            pasteClearAll.Click += pasteClearAll_Click;
            // 
            // pasteRichTextBox
            // 
            pasteRichTextBox.BackColor = Color.White;
            pasteRichTextBox.BorderStyle = BorderStyle.None;
            pasteRichTextBox.Location = new Point(18, 176);
            pasteRichTextBox.Name = "pasteRichTextBox";
            pasteRichTextBox.Size = new Size(654, 205);
            pasteRichTextBox.TabIndex = 0;
            pasteRichTextBox.Text = "";
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(700, 710);
            Controls.Add(tabControl);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Main";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Conversor MARC";
            tabControl.ResumeLayout(false);
            bulkConvertPage.ResumeLayout(false);
            bulkConvertPage.PerformLayout();
            pasteConvertPage.ResumeLayout(false);
            pasteConvertPage.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControl;
        private TabPage bulkConvertPage;
        private TabPage pasteConvertPage;
        private Button convertButton;
        private TextBox outputTextBox;
        private TextBox inputTextBox;
        private Label outputLabel;
        private Label inputLabel;
        private Button outputBrowse;
        private Button inputBrowse;
        private RichTextBox logRichTextBox;
        private Button pasteConvert;
        private Button pasteClearAll;
        private RichTextBox pasteRichTextBox;
        private Label pasteLabel;
        private Button pasteOutputBrowse;
        private TextBox pasteOutputTextBox;
        private Label pasteOutputLabel;
        private TextBox pasteNameTextBox;
        private Label pasteNameLabel;
        private Panel panel1;
        private RichTextBox pasteLogRichTextBox;
        private Panel panel2;
    }
}