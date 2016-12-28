﻿using file_msbt.Properties;
using KuriimuContract;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace file_msbt
{
	public class MsbtAdapter : IFileAdapter
	{
		private FileInfo _fileInfo = null;
		private MSBT _msbt = null;
		private List<Entry> _entries = null;

		#region Properties

		// Information
		public string Name => "MSBT";

		public string Description => "Message Binary Text";

		public string Extension => " *.msbt";

		public string About => "This is the MSBT file adapter for Kuriimu.";

		// Feature Support
		public bool FileHasExtendedProperties => false;

		public bool CanSave => true;

		public bool CanAddEntries => true;

		public bool CanRenameEntries => true;

		public bool CanRemoveEntries => true;

		public bool EntriesHaveSubEntries => false;

		public bool OnlySubEntriesHaveText => false;

		public bool EntriesHaveUniqueNames => true;

		public bool EntriesHaveExtendedProperties => false;

		public FileInfo FileInfo
		{
			get
			{
				return _fileInfo;
			}
			set
			{
				_fileInfo = value;
			}
		}

		#endregion

		public LoadResult Load(string filename)
		{
			LoadResult result = LoadResult.Success;

			_fileInfo = new FileInfo(filename);

			if (_fileInfo.Exists)
			{
				try
				{
					_msbt = new MSBT(_fileInfo.FullName);
				}
				catch (Exception)
				{
					result = LoadResult.Failure;
				}
			}
			else
				result = LoadResult.FileNotFound;

			return result;
		}

		public SaveResult Save(string filename = "")
		{
			SaveResult result = SaveResult.Success;

			if (filename.Trim() != string.Empty)
				_fileInfo = new FileInfo(filename);

			try
			{
				_msbt.Save(_fileInfo.FullName);
			}
			catch (Exception)
			{
				result = SaveResult.Failure;
			}

			return result;
		}

		public bool Identify(string filename)
		{
			bool result = true;

			try
			{
				new MSBT(filename);
			}
			catch (Exception)
			{
				result = false;
			}

			return result;
		}

		// Entries
		public IEnumerable<IEntry> Entries
		{
			get
			{
				_entries = new List<Entry>();

				// Create the entry objects for Kuriimu
				foreach (Label label in _msbt.LBL1.Labels)
				{
					Entry entry = new Entry(_msbt.FileEncoding);
					entry.EditedLabel = label;
					_entries.Add(entry);
				}

				return _entries;
			}
		}

		public List<string> NameList
		{
			get
			{
				List<string> names = new List<string>();
				foreach (Entry entry in Entries)
					names.Add(entry.Name);
				return names;
			}
		}

		public string NameFilter => MSBT.LabelFilter;

		public int NameMaxLength => MSBT.LabelMaxLength;

		// Features
		public bool ShowProperties(Icon icon) => false;

		public IEntry NewEntry() => new Entry(_msbt.FileEncoding);

		public bool AddEntry(IEntry entry)
		{
			bool result = true;

			try
			{
				_msbt.AddEntry((Entry)entry);
			}
			catch (Exception)
			{
				result = false;
			}

			return result;
		}

		public bool RenameEntry(IEntry entry, string newName)
		{
			bool result = true;

			try
			{
				_msbt.RenameEntry((Entry)entry, newName);
			}
			catch (Exception)
			{
				result = false;
			}

			return result;
		}

		public bool RemoveEntry(IEntry entry)
		{
			bool result = true;

			try
			{
				_msbt.RemoveEntry((Entry)entry);
			}
			catch (Exception)
			{
				result = false;
			}

			return result;
		}

		public bool ShowEntryProperties(IEntry entry, Icon icon) => false;

		// Settings
		public bool SortEntries
		{
			get { return Settings.Default.SortEntries; }
			set
			{
				Settings.Default.SortEntries = value;
				Settings.Default.Save();
			}
		}
	}

	public class Entry : IEntry
	{
		public Encoding Encoding { get; set; }

		public string Name
		{
			get { return EditedLabel.Name; }
			set { EditedLabel.Name = value; }
		}

		public byte[] OriginalText
		{
			get { return OriginalLabel.String.Text; }
			set {; }
		}

		public string OriginalTextString
		{
			get { return Encoding.GetString(OriginalLabel.String.Text); }
			set {; }
		}

		public byte[] EditedText
		{
			get { return EditedLabel.String.Text; }
			set { EditedLabel.String.Text = value; }
		}

		public string EditedTextString
		{
			get { return Encoding.GetString(EditedLabel.String.Text); }
			set { EditedLabel.String.Text = Encoding.GetBytes(value); }
		}

		public int MaxLength { get; set; }

		public bool IsResizable
		{
			get { return true; }
		}

		public List<IEntry> SubEntries { get; set; }
		public bool IsSubEntry { get; set; }

		public Label OriginalLabel { get; set; }
		public Label EditedLabel { get; set; }

		public Entry()
		{
			Encoding = Encoding.Unicode;
			EditedLabel = new Label();
			OriginalLabel = new Label();
			Name = string.Empty;
			MaxLength = 0;
			OriginalText = new byte[] { };
			EditedText = new byte[] { };
			SubEntries = new List<IEntry>();
		}

		public Entry(Encoding encoding) : this()
		{
			Encoding = encoding;
		}

		public override string ToString()
		{
			return Name == string.Empty ? EditedLabel.String.Index.ToString() : Name;
		}

		public int CompareTo(IEntry rhs)
		{
			int result = Name.CompareTo(rhs.Name);
			if (result == 0)
				result = EditedLabel.String.Index.CompareTo(((Entry)rhs).EditedLabel.String.Index);
			return result;
		}
	}
}