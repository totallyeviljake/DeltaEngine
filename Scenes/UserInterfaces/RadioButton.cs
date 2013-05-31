using System;
using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Input;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Sprites;

namespace DeltaEngine.Scenes.UserInterfaces
{
	/// <summary>
	/// A set of RadioButtons form a RadioDialog
	/// </summary>
	public class RadioButton : Label, Interact.Clickable
	{
		internal RadioButton(Theme theme, string text = "")
			: this(theme, Rectangle.Zero, text) {}

		public RadioButton(Theme theme, Rectangle drawArea, string text = "")
			: base(theme.RadioButtonBackground, theme.Font, drawArea)
		{
			Text = text;
			Add(theme);
			Add(new Interact.State());
			Add<Interact, Interact.Clicking>();
			Add(new Data());
			Add(new Sprite(theme.RadioButtonNotSelected.Image, theme.RadioButtonNotSelected.Color));
			Add<UpdateAppearance>();
		}

		public void Clicking()
		{
			IsSelected = true;
			if (Clicked != null)
				Clicked();
		}

		public event Action Clicked;

		public bool IsSelected
		{
			get { return Get<Data>().IsSelected; }
			set { Get<Data>().IsSelected = value; }
		}

		public class Data
		{
			public bool IsSelected { get; set; }
		}

		public class UpdateAppearance : EntityHandler
		{
			public override void Handle(List<Entity> entities)
			{
				foreach (RadioButton button in entities.OfType<RadioButton>())
					UpdateSelectorGraphics(button, button.Get<Sprite>());
			}

			private static void UpdateSelectorGraphics(RadioButton button, Sprite selector)
			{
				UpdateSelectorDrawArea(button, selector);
				UpdateSelectorAppearance(button, selector);
			}

			private static void UpdateSelectorDrawArea(Entity2D button, Sprite selector)
			{
				Rectangle drawArea = button.DrawArea;
				var size = new Size(selector.Image.PixelSize.AspectRatio * drawArea.Height, drawArea.Height);
				selector.DrawArea = new Rectangle(drawArea.TopLeft, size);
				selector.RenderLayer = button.RenderLayer + 1;
			}

			private static void UpdateSelectorAppearance(RadioButton button, Entity selector)
			{
				var isInside = button.Get<Interact.State>().IsInside;
				var theme = button.Get<Theme>();
				if (isInside && button.IsSelected)
					SetAppearance(selector, theme.RadioButtonSelectedMouseover);
				else if (isInside)
					SetAppearance(selector, theme.RadioButtonNotSelectedMouseover);
				else if (button.IsSelected)
					SetAppearance(selector, theme.RadioButtonSelected);
				else
					SetAppearance(selector, theme.RadioButtonNotSelected);
			}

			private static void SetAppearance(Entity entity, Theme.Appearance appearance)
			{
				entity.Set(appearance.Image);
				entity.Set(appearance.Color);
			}
		}
	}
}