using DiffPatch;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PatchReviewer
{
	public class ResultViewModel : INotifyPropertyChanged
	{
		public FilePatcherViewModel File { get; }
		private Patcher.Result Result { get; }
		private int origIndex = 0;

		public ResultViewModel(FilePatcherViewModel file, Patcher.Result result, int origIndex) {
			File = file;
			Result = result;
			this.origIndex = origIndex;
		}

		private Patch _editingPatch;
		public Patch EditingPatch {
			get => _editingPatch;
			set {
				var start1 = Start1;
				_editingPatch = value;

				if (start1 != Start1)
					OnPropertyChanged(nameof(Start1));
			}
		}

		// Patch which is active in the editor
		public Patch ViewPatch => EditingPatch ?? Result.appliedPatch;

		public int Start1 => ViewPatch?.start1 ?? (Result.patch.start1 + Result.searchOffset);
		public int Start2 => ViewPatch.start2;
		public int End1 => Start1 + (ViewPatch ?? Result.patch).length1;
		public int End2 => Start2 + ViewPatch.length2;
		public int SearchOffset => Result.searchOffset;

		// should be a 'friend method' of FilePatcherViewModel
		// FilePatcherViewModel is responsible for making sure that Start1 and Start2 actually line up with the patch list
		public void MoveTo(int start2) {
			if (Result.appliedPatch == null)
				throw new NullReferenceException(nameof(Result.appliedPatch));

			Result.appliedPatch.start2 = start2;
			OnPropertyChanged(nameof(Start2));
		}

		public bool IsRemoved => Result.success && Result.appliedPatch == null;

		public ResultStatus Status {
			get {
				if (!Result.success)
					return ResultStatus.FAILED;
				if (Result.mode == Patcher.Mode.FUZZY && Result.fuzzyQuality < 0.5f)
					return ResultStatus.BAD;
				if (Result.offsetWarning || Result.mode == Patcher.Mode.FUZZY && Result.fuzzyQuality < 0.85f)
					return ResultStatus.WARNING;
				if (Result.mode == Patcher.Mode.FUZZY)
					return ResultStatus.GOOD;
				if (Result.mode == Patcher.Mode.OFFSET)
					return ResultStatus.OFFSET;
				return ResultStatus.EXACT;
			}
		}

		// shouldn't slow things down much, and worth offering some 'immutability'
		public Patch OriginalPatch => Result.patch;
		public Patch AppliedPatch => Result.appliedPatch == null ? null : Result.appliedPatch;
		public Patch ApprovedPatch => Status >= ResultStatus.OFFSET ? AppliedPatch : OriginalPatch;

		private bool _modifiedInEditor;
		public bool ModifiedInEditor {
			get => _modifiedInEditor;
			set {
				if (_modifiedInEditor == value) return;

				_modifiedInEditor = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(LabelWithModifiedIndicator));
			}
		}

		public string LabelWithModifiedIndicator => Label + (ModifiedInEditor ? " *" : "");

		public string Label => IsRemoved ? $"REMOVED: {Result.patch.Header}" : Result.Summary();
		public string Title => $"{File.Label} {Label}";

		public string MovedPatchCountText { get; private set; } = "";

		private int _appliedIndex = -1;
		public int AppliedIndex {
			get => _appliedIndex;
			set {
				if (_appliedIndex == value)
					return;

				_appliedIndex = value;

				int moved = AppliedIndex - origIndex;
				MovedPatchCountText = moved > 0 ? $"▼{moved}" : moved < 0 ? $"▲{-moved}" : "";
				OnPropertyChanged(nameof(MovedPatchCountText));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

		internal void Approve() {
			Result.success = true;
			Result.mode = Patcher.Mode.EXACT;
			Result.offsetWarning = false;
			Result.appliedPatch = EditingPatch;

			ModifiedInEditor = false;
			// trigger reordering in the collection view
			OnPropertyChanged(nameof(Start1));
			OnPropertyChanged(nameof(Status));
			OnPropertyChanged(nameof(Label));
			OnPropertyChanged(nameof(LabelWithModifiedIndicator));
			OnPropertyChanged(nameof(Title));

			File.ResultsModified = true;
			File.RecalculateOffsets();
		}

		internal void UndoRemove() {
			Result.success = false; //convert to FAILED
			OnPropertyChanged(nameof(Status));
			OnPropertyChanged(nameof(Label));
			OnPropertyChanged(nameof(Title));
			OnPropertyChanged(nameof(LabelWithModifiedIndicator));
		}
	}
}