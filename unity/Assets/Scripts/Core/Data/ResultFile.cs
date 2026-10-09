using System;
using System.Collections.Generic;
using System.IO;
using DEMSIFT.Player;
using DEMSIFT.Scoring;
using UnityEngine;

namespace DEMSIFT.Data
{
    public static class ResultFile
    {
        private const string FolderName = "Hasil";
        private const string FileName = "hasil-pengetahuan.txt";
        private const string ExportFolderName = "Export";
        private const string ArchiveFolderName = "Arsip";
        private const string Separator = "\t";
        private const string TimestampFormat = "yyyy-MM-dd HH:mm:ss";
        private const string FileTimestampFormat = "yyyy-MM-dd_HH-mm-ss";
        private const int NameColumn = 1;
        private const int ClassColumn = 2;
        private const int ColumnCount = 3 + ScoreSession.SoalCount * 2 + 1;

        public static string FolderPath
        {
            get
            {
                string folderPath = Path.Combine(Application.persistentDataPath, FolderName);
                Directory.CreateDirectory(folderPath);
                return folderPath;
            }
        }

        public static string FilePath => Path.Combine(FolderPath, FileName);

        public static bool Exists => File.Exists(FilePath);

        public static void AppendCurrentPlayer()
        {
            if (!Exists)
            {
                File.WriteAllText(FilePath, Header() + Environment.NewLine);
            }

            File.AppendAllText(FilePath, CurrentPlayerLine() + Environment.NewLine);
        }

        public static List<PlayerResult> ReadAll()
        {
            var results = new List<PlayerResult>();
            if (!Exists) return results;

            string[] lines = File.ReadAllLines(FilePath);
            for (int i = 1; i < lines.Length; i++)
            {
                string[] columns = lines[i].Split(Separator);
                if (columns.Length != ColumnCount) continue;
                if (!int.TryParse(columns[ColumnCount - 1], out int total)) continue;

                results.Add(new PlayerResult(columns[NameColumn], columns[ClassColumn], total));
            }

            return results;
        }

        // --- Admin ---
        public static string Archive()
        {
            string archivePath = StampedPath(ArchiveFolderName, ".txt");
            File.Move(FilePath, Path.Combine(FolderPath, archivePath));
            return archivePath;
        }

        public static string ExportCsv()
        {
            string csvPath = StampedPath(ExportFolderName, ".csv");
            var csvLines = new List<string>();

            foreach (string line in File.ReadAllLines(FilePath))
            {
                var fields = new List<string>();
                foreach (string field in line.Split(Separator)) fields.Add($"\"{field.Replace("\"", "\"\"")}\"");
                csvLines.Add(string.Join(",", fields));
            }

            File.WriteAllLines(Path.Combine(FolderPath, csvPath), csvLines);
            return csvPath;
        }

        private static string StampedPath(string subfolderName, string extension)
        {
            Directory.CreateDirectory(Path.Combine(FolderPath, subfolderName));

            string stampedName = $"{Path.GetFileNameWithoutExtension(FileName)}_{DateTime.Now.ToString(FileTimestampFormat)}{extension}";
            return Path.Combine(subfolderName, stampedName);
        }

        // --- Line format ---
        private static string Header()
        {
            var columns = new List<string> { "Waktu", "Nama", "Kelas" };
            for (int soal = 1; soal <= ScoreSession.SoalCount; soal++) columns.Add($"Skor Soal {soal}");
            for (int soal = 1; soal <= ScoreSession.SoalCount; soal++) columns.Add($"Jumlah Salah Soal {soal}");
            columns.Add("Total Skor");
            return string.Join(Separator, columns);
        }

        private static string CurrentPlayerLine()
        {
            var columns = new List<string>
            {
                DateTime.Now.ToString(TimestampFormat),
                Clean(PlayerSession.PlayerName),
                Clean(PlayerSession.ClassName)
            };
            for (int soal = 1; soal <= ScoreSession.SoalCount; soal++) columns.Add(ScoreSession.ScoreOf(soal).ToString());
            for (int soal = 1; soal <= ScoreSession.SoalCount; soal++) columns.Add(ScoreSession.WrongAttemptsOf(soal).ToString());
            columns.Add(ScoreSession.Total.ToString());
            return string.Join(Separator, columns);
        }

        private static string Clean(string value)
        {
            return value.Replace('\t', ' ').Replace('\n', ' ').Replace('\r', ' ');
        }
    }
}
