using marcc.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace marcc
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void inputBrowse_Click(object sender, EventArgs e)
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = "Selecione o diretório dos arquivos que serão convertidos",
                UseDescriptionForTitle = true,
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                inputTextBox.Text = dialog.SelectedPath;

                if (string.IsNullOrEmpty(outputTextBox.Text))
                {
                    outputTextBox.Text = $"{dialog.SelectedPath}\\output";
                }
            }
        }

        private void outputBrowse_Click(object sender, EventArgs e)
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = "Selecione o diretório em que os arquivos convertidos irão ser salvos",
                UseDescriptionForTitle = true,
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                outputTextBox.Text = $"{dialog.SelectedPath}\\output";

                if (string.IsNullOrEmpty(inputTextBox.Text))
                {
                    inputTextBox.Text = dialog.SelectedPath;
                }
            }
        }

        private void pasteOutputBrowse_Click(object sender, EventArgs e)
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = "Selecione o diretório em que o texto convertido será ser salvo",
                UseDescriptionForTitle = true
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                pasteOutputTextBox.Text = dialog.SelectedPath;
            }
        }

        private void pasteClearAll_Click(object sender, EventArgs e)
        {
            pasteOutputTextBox.Text = null;
            pasteNameTextBox.Text = null;
            pasteRichTextBox.Text = null;
        }

        private void convertButton_Click(object sender, EventArgs e)
        {
            logRichTextBox.Clear();
            logRichTextBox.SelectionColor = Color.Black;

            var inputPath = inputTextBox.Text;
            var outputPath = outputTextBox.Text;

            if (string.IsNullOrWhiteSpace(inputPath))
            {
                MessageBox.Show(
                    "Informe o diretório de entrada.",
                    "Atenção",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            if (string.IsNullOrWhiteSpace(outputPath))
            {
                MessageBox.Show(
                    "Informe o diretório de saída.",
                    "Atenção",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            var files = Directory.GetFiles(inputPath, "*.txt");

            if (files.Length == 0)
            {
                MessageBox.Show(
                    "Nenhum arquivo para conversão foi encontrado.",
                    "Informação",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                return;
            }

            outputTextBox.Enabled = false;
            convertButton.Enabled = false;
            outputBrowse.Enabled = false;
            inputBrowse.Enabled = false;

            var service = new MarcConverterService();
            var successCount = 0;
            var errorCount = 0;

            foreach (var inputFile in files)
            {
                var fileName = Path.GetFileNameWithoutExtension(inputFile);
                var outputFile = Path.Combine(outputPath, $"{fileName}.mrc");

                logRichTextBox.AppendText($"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}] ");
                logRichTextBox.AppendText($"Convertendo: {fileName}...{Environment.NewLine}");

                var result = service.ConvertFile(inputFile, outputFile);

                if (result.Success)
                {
                    successCount++;
                    logRichTextBox.SelectionColor = Color.Green;
                    logRichTextBox.AppendText($"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}] ");
                    logRichTextBox.AppendText($"{fileName} convertido com sucesso.{Environment.NewLine}");
                }
                else
                {
                    errorCount++;
                    logRichTextBox.SelectionColor = Color.Red;
                    logRichTextBox.AppendText($"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}] ");
                    logRichTextBox.AppendText($"Não foi possível converter {fileName}: {result.Error}{Environment.NewLine}");
                }
            }

            logRichTextBox.SelectionColor = Color.Black;
            logRichTextBox.AppendText(Environment.NewLine);
            logRichTextBox.AppendText($"Conversão de arquivos finalizada{Environment.NewLine}");

            logRichTextBox.SelectionColor = Color.Green;
            logRichTextBox.AppendText($"Sucessos: {successCount}{Environment.NewLine}");

            logRichTextBox.SelectionColor = Color.Red;
            logRichTextBox.AppendText($"Erros: {errorCount}{Environment.NewLine}");

            outputTextBox.Enabled = true;
            convertButton.Enabled = true;
            outputBrowse.Enabled = true;
            inputBrowse.Enabled = true;
        }

        private void pasteConvert_Click(object sender, EventArgs e)
        {
            pasteLogRichTextBox.Clear();
            pasteLogRichTextBox.SelectionColor = Color.Black;

            var outputPath = pasteOutputTextBox.Text;
            var text = pasteRichTextBox.Text;
            var name = pasteNameTextBox.Text;

            if (string.IsNullOrWhiteSpace(outputPath))
            {
                MessageBox.Show(
                    "Informe o diretório de saída.",
                    "Atenção",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                MessageBox.Show(
                    "Informe o texto a ser convertido.",
                    "Atenção",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            pasteOutputTextBox.Enabled = false;
            pasteNameTextBox.Enabled = false;
            pasteRichTextBox.Enabled = false;
            pasteConvert.Enabled = false;
            pasteClearAll.Enabled = false;

            var service = new MarcConverterService();

            pasteLogRichTextBox.AppendText($"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}] ");
            pasteLogRichTextBox.AppendText($"Convertendo texto para MARC...{Environment.NewLine}");

            var result = service.ConvertText(text, outputPath, name);

            if (result.Success)
            {
                pasteLogRichTextBox.SelectionColor = Color.Green;
                pasteLogRichTextBox.AppendText($"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}] ");
                pasteLogRichTextBox.AppendText($"Texto convertido com sucesso.{Environment.NewLine}");
            }
            else
            {
                pasteLogRichTextBox.SelectionColor = Color.Red;
                pasteLogRichTextBox.AppendText($"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}] ");
                pasteLogRichTextBox.AppendText($"Não foi possível converter o texto: {result.Error}{Environment.NewLine}");
            }

            pasteLogRichTextBox.SelectionColor = Color.Black;
            pasteLogRichTextBox.AppendText(Environment.NewLine);
            pasteLogRichTextBox.AppendText($"Conversão de texto finalizada");

            pasteOutputTextBox.Enabled = true;
            pasteNameTextBox.Enabled = true;
            pasteRichTextBox.Enabled = true;
            pasteConvert.Enabled = true;
            pasteClearAll.Enabled = true;
        }
    }
}