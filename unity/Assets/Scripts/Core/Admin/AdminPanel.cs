using System;
using System.Collections.Generic;
using DEMSIFT.Data;
using TMPro;
using UnityEngine;

namespace DEMSIFT.Admin
{
    public class AdminPanel : MonoBehaviour
    {
        [SerializeField] private GameObject resetConfirm;
        [SerializeField] private TMP_Text statusLabel;

        // --- Called by buttons ---
        public void Open()
        {
            resetConfirm.SetActive(false);
            statusLabel.text = string.Empty;
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        public void AskReset()
        {
            if (HasData()) resetConfirm.SetActive(true);
        }

        public void CancelReset()
        {
            resetConfirm.SetActive(false);
        }

        public void ConfirmReset()
        {
            resetConfirm.SetActive(false);

            var archived = new List<string>();
            foreach (ResultFile file in ResultFile.All)
            {
                if (file.Exists) archived.Add(file.Archive());
            }
            statusLabel.text = $"Data direset. Arsip: {string.Join(", ", archived)}";
        }

        public void Export()
        {
            if (!HasData()) return;

            var exported = new List<string>();
            foreach (ResultFile file in ResultFile.All)
            {
                if (file.Exists) exported.Add(file.ExportCsv());
            }
            statusLabel.text = $"Tersimpan: {string.Join(", ", exported)}";
        }

        public void OpenFolder()
        {
            Application.OpenURL(new Uri(ResultFile.FolderPath).AbsoluteUri);
        }

        private bool HasData()
        {
            foreach (ResultFile file in ResultFile.All)
            {
                if (file.Exists) return true;
            }

            statusLabel.text = "Belum ada data.";
            return false;
        }
    }
}
