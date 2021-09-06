using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Terraclient.Cheats;
using Terraria.Terraclient.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.UI.Gamepad;

namespace Terraria.Terraclient.GameContent.UI.States
{
	public class UIManageCheats : UIState
	{
		public float UIScaleOnCtor;
		private static readonly List<ModCheat> CheatsFullLine = new();
		private readonly List<UIElement> _bindsKeyboard = new();
		private UIElement _outerContainer;
		private UIList _uiList;

		public UIManageCheats() {
			UIScaleOnCtor = Main.UIScale;
		}

		public override void OnInitialize() {
			foreach (ModCheat cheat in ModContent.GetContent<ModCheat>().Where(x => x.LargeBar))
				CheatsFullLine.Add(cheat);

			_outerContainer = new UIElement();
			_outerContainer.Width.Set(0f, 0.8f);
			_outerContainer.MaxWidth.Set(600f, 0f);
			_outerContainer.Top.Set(220f, 0f);
			_outerContainer.Height.Set(-200f, 1f);
			_outerContainer.HAlign = 0.5f;

			UIPanel panel = new UIPanel();
			panel.Width.Set(0f, 1f);
			panel.Height.Set(-110f, 1f);
			panel.BackgroundColor = new Color(33, 43, 79) * 0.8f;
			_outerContainer.Append(panel);

			_uiList = new UIList();
			_uiList.Width.Set(-25f, 1f);
			_uiList.Height.Set(-10f, 1f);
			_uiList.VAlign = 1f;
			_uiList.PaddingBottom = 5f;
			_uiList.ListPadding = 20f;
			panel.Append(_uiList);

			AssembleBindPanels();
			FillList();

			UIScrollbar scrollbar = new UIScrollbar();
			scrollbar.SetView(100f, 1000f);
			scrollbar.Height.Set(-67f, 1f);
			scrollbar.HAlign = 1f;
			scrollbar.VAlign = 1f;
			scrollbar.MarginBottom = 11f;
			panel.Append(scrollbar);
			_uiList.SetScrollbar(scrollbar);

			UITextPanel<LocalizedText> localizedCheatsTextPanel = new UITextPanel<LocalizedText>(
				Language.GetText("Terraclient.UICheats"),
				0.7f, true) {HAlign = 0.5f};
			localizedCheatsTextPanel.Top.Set(-45f, 0f);
			localizedCheatsTextPanel.Left.Set(-10f, 0f);
			localizedCheatsTextPanel.SetPadding(15f);
			localizedCheatsTextPanel.BackgroundColor = new Color(73, 94, 171);
			_outerContainer.Append(localizedCheatsTextPanel);

			UITextPanel<LocalizedText> localizedBackTextPanel = new UITextPanel<LocalizedText>(
				Language.GetText("UI.Back"),
				0.7f, true);
			localizedBackTextPanel.Width.Set(-10f, 0.5f);
			localizedBackTextPanel.Height.Set(50f, 0f);
			localizedBackTextPanel.VAlign = 1f;
			localizedBackTextPanel.HAlign = 0.5f;
			localizedBackTextPanel.Top.Set(-45f, 0f);
			localizedBackTextPanel.OnMouseOver += FadedMouseOver;
			localizedBackTextPanel.OnMouseOut += FadedMouseOut;
			localizedBackTextPanel.OnClick += GoBackClick;
			_outerContainer.Append(localizedBackTextPanel);

			Append(_outerContainer);
		}

		private void AssembleBindPanels() {
			List<ModCheat> cheats = ModContent.GetContent<ModCheat>().ToList();
			Dictionary<ModCheatCategory, List<ModCheat>> categories = new();

			foreach (ModCheat cheat in cheats) {
				ModCheatCategory category = cheat.Category;

				if (!categories.ContainsKey(category))
					categories[category] = new List<ModCheat>();

				categories[category].Add(cheat);
			}

			int group = 0;

			foreach ((ModCheatCategory category, List<ModCheat> cheatList) in categories)
				_bindsKeyboard.Add(CreateBindingGroup(group++, category, cheatList));
		}

		private static UISortableElement CreateBindingGroup(int elementIndex, ModCheatCategory category,
			List<ModCheat> cheats) {
			UISortableElement sortableElementIndex = new UISortableElement(elementIndex) {HAlign = 0.5f};
			sortableElementIndex.Width.Set(0f, 1f);
			sortableElementIndex.Height.Set(2000f, 0f);

			UIPanel panel = new UIPanel();
			panel.Width.Set(0f, 1f);
			panel.Height.Set(-16f, 1f);
			panel.VAlign = 1f;
			panel.BackgroundColor = new Color(33, 43, 79) * 0.8f;
			sortableElementIndex.Append(panel);

			UIList list = new UIList {OverflowHidden = false};
			list.Width.Set(0f, 1f);
			list.Height.Set(-8f, 1f);
			list.VAlign = 1f;
			list.ListPadding = 5f;
			panel.Append(list);

			panel.BackgroundColor = Color.Lerp(panel.BackgroundColor, new Color(63, 82, 151), 0.18f);

			CreateElementGroup(list, panel.BackgroundColor, cheats);

			panel.BackgroundColor = panel.BackgroundColor.MultiplyRGBA(new Color(111, 111, 111));

			UITextPanel<string> localizedPanel = new UITextPanel<string>(
				category.DisplayName.GetTranslation(Language.ActiveCulture),
				0.7f) {VAlign = 0f, HAlign = 0.5f};

			sortableElementIndex.Append(localizedPanel);
			sortableElementIndex.Recalculate();
			sortableElementIndex.Width.Set(0f, 1f);
			sortableElementIndex.Height.Set(list.GetTotalHeight() + 30f + 16f, 0f);

			return sortableElementIndex;
		}

		private static void CreateElementGroup(UIList parent, Color color, List<ModCheat> cheats) {
			for (int i = 0; i < cheats.Count; i++) {
				UISortableElement sortableIndex = new UISortableElement(i);
				sortableIndex.Width.Set(0f, 1f);
				sortableIndex.Height.Set(30f, 0f);
				sortableIndex.HAlign = 0.5f;
				parent.Add(sortableIndex);

				if (CheatsFullLine.Contains(cheats[i])) {
					UIElement panel = CreatePanel(color, cheats[i]);
					panel.Width.Set(0f, 1f);
					panel.Height.Set(0f, 1f);
					sortableIndex.Append(panel);
				}
				else {
					UIElement panel = CreatePanel(color, cheats[i]);
					panel.Width.Set(-5f, 0.5f);
					panel.Height.Set(0f, 1f);
					sortableIndex.Append(panel);

					i++;

					if (i >= cheats.Count)
						continue;

					panel = CreatePanel(color, cheats[i]);
					panel.Width.Set(-5f, 0.5f);
					panel.Height.Set(0f, 1f);
					panel.HAlign = 1f;
					sortableIndex.Append(panel);
				}
			}
		}

		public static UIElement CreatePanel(Color color, ModCheat cheat) => new UICheatsListItem(color, cheat);

		public override void OnActivate() {
			if (Main.gameMenu) {
				_outerContainer.Top.Set(220f, 0f);
				_outerContainer.Height.Set(-220f, 1f);
			}
			else {
				_outerContainer.Top.Set(120f, 0f);
				_outerContainer.Height.Set(-120f, 1f);
			}

			if (PlayerInput.UsingGamepadUI)
				UILinkPointNavigator.ChangePoint(3002);
		}

		private void FillList() {
			_uiList.Clear();

			foreach (UIElement item in _bindsKeyboard)
				_uiList.Add(item);
		}

		private static void FadedMouseOver(UIMouseEvent evt, UIElement listeningElement) {
			SoundEngine.PlaySound(12);
			((UIPanel)evt.Target).BackgroundColor = new Color(73, 94, 171);
			((UIPanel)evt.Target).BorderColor = Colors.FancyUIFatButtonMouseOver;
		}

		private static void FadedMouseOut(UIMouseEvent evt, UIElement listeningElement) {
			((UIPanel)evt.Target).BackgroundColor = new Color(63, 82, 151) * 0.7f;
			((UIPanel)evt.Target).BorderColor = Color.Black;
		}

		private static void GoBackClick(UIMouseEvent evt, UIElement listeningElement) {
			Main.menuMode = 11;
			Main.SaveSettings();
			IngameFancyUI.Close();
		}
	}
}