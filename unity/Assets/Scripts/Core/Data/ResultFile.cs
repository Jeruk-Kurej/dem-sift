using System;
using System.Collections.Generic;
using System.IO;
using DEMSIFT.Player;
using UnityEngine;

namespace DEMSIFT.Data
{
    public class ResultFile
    {
        private const string FolderName = "Hasil";
        private const string ExportFolderName = "Export";
        private const string ArchiveFolderName = "Arsip";
        private const string Separator = "\t";
        private const string TimestampFormat = "yyyy-MM-dd HH:mm:ss";
        private const string FileTimestampFormat = "yyyy-MM-dd_HH-mm-ss";
        private static readonly char[] FormulaStarts = { '=', '+', '-', '@' };
        private const int NameColumn = 1;
        private const int ClassColumn = 2;

        public static readonly ResultFile Pengetahuan = new("hasil-pengetahuan", new PengetahuanResultFormat());
        public static readonly ResultFile Pelatihan = new("hasil-pelatihan", new PelatihanResultFormat());
        public static readonly ResultFile[] All = { Pengetahuan, Pelatihan };

        private readonly string baseName;
        private readonly ResultFormat format;
        private readonly int columnCount;

        private ResultFile(string fileBaseName, ResultFormat resultFormat)
        {
            baseName = fileBaseName;
            format = resultFormat;
            columnCount = Header().Count;
        }

        public static string FolderPath
        {
            get
            {
                string folderPath = Path.Combine(Application.persistentDataPath, FolderName);
                Directory.CreateDirectory(folderPath);
                return folderPath;
            }
        }

        public string FileName => baseName + ".txt";

        public string FilePath => Path.Combine(FolderPath, FileName);

        public bool Exists => File.Exists(FilePath);

        private string BackupFilePath => Path.Combine(FolderPath, baseName + "_cadangan.txt");

        public void AppendCurrentPlayer()
        {
            string line = string.Join(Separator, CurrentPlayerColumns()) + Environment.NewLine;

            try
            {
                if (!Exists) File.WriteAllText(FilePath, string.Join(Separator, Header()) + Environment.NewLine);
                File.AppendAllText(FilePath, line);
            }
            catch (IOException exception)
            {
                Debug.LogError($"[ResultFile] {FileName} is locked, saving to backup: {exception.Message}");
                File.AppendAllText(BackupFilePath, line);
            }
        }

        public List<PlayerResult> ReadAll()
        {
            var results = new List<PlayerResult>();
            if (!Exists) return results;

            string[] lines;
            try
            {
                lines = File.ReadAllLines(FilePath);
            }
            catch (IOException exception)
            {
                Debug.LogError($"[ResultFile] Could not read {FileName}: {exception.Message}");
                return results;
            }
            for (int i = 1; i < lines.Length; i++)
            {
                string[] columns = lines[i].Split(Separator);
                if (columns.Length != columnCount) continue;
                if (!int.TryParse(columns[columnCount - 1], out int total)) continue;

                results.Add(new PlayerResult(columns[NameColumn], columns[ClassColumn], total));
            }

            return results;
        }

        // --- Admin ---
        public string Archive()
        {
            string archivePath = StampedPath(ArchiveFolderName, ".txt");
            File.Move(FilePath, Path.Combine(FolderPath, archivePath));
            return archivePath;
        }

        public string ExportCsv()
        {
            string csvPath = StampedPath(ExportFolderName, ".csv");
            var csvLines = new List<string>();

            foreach (string line in File.ReadAllLines(FilePath))
            {
                var fields = new List<string>();
                foreach (string field in line.Split(Separator)) fields.Add(CsvField(field));
                csvLines.Add(string.Join(",", fields));
            }

            File.WriteAllLines(Path.Combine(FolderPath, csvPath), csvLines);
            return csvPath;
        }

        private string StampedPath(string subfolderName, string extension)
        {
            Directory.CreateDirectory(Path.Combine(FolderPath, subfolderName));

            string stampedName = $"{baseName}_{DateTime.Now.ToString(FileTimestampFormat)}{extension}";
            return Path.Combine(subfolderName, stampedName);
        }

        // --- Line format ---
        private List<string> Header()
        {
            var columns = new List<string> { "Waktu", "Nama", "Kelas" };
            columns.AddRange(format.ScoreHeaders());
            columns.Add("Total Skor");
            return columns;
        }

        private List<string> CurrentPlayerColumns()
        {
            var columns = new List<string>
            {
                DateTime.Now.ToString(TimestampFormat),
                Clean(PlayerSession.PlayerName),
                Clean(PlayerSession.ClassName)
            };
            columns.AddRange(format.CurrentScores());
            columns.Add(format.CurrentTotal.ToString());
            return columns;
        }

        private static string CsvField(string field)
        {
            if (field.Length > 0 && Array.IndexOf(FormulaStarts, field[0]) >= 0) field = "'" + field;
            return $"\"{field.Replace("\"", "\"\"")}\"";
        }

        private static string Clean(string value)
        {
            return value.Replace('\t', ' ').Replace('\n', ' ').Replace('\r', ' ');
        }
    }
}
