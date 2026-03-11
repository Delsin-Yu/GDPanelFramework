using Godot;
using GodotTask;

namespace GDPanelFramework.Examples;

/// <summary>
/// Demonstrates building a richer runtime panel entirely with <see cref="PanelBuilder"/>.
/// </summary>
public partial class Example03_Main : Node
{
	private sealed class ExampleRefs
	{
		public Label? StatusLabel;
		public Label? QuantityLabel;
		public Button? CloseButton;
		public ProgressBar? ProgressBar;
	}

	/// <summary>
	/// Executes the demo one frame after startup so the panel root exists in the scene tree.
	/// </summary>
	public override void _Ready() =>
		GDTask.NextFrame().ContinueWith(OpenDemo);

	private void OpenDemo() => OpenDemoAsync().Forget();

	private async GDTask OpenDemoAsync()
	{
		var previewTexture = new GradientTexture2D
		{
			Width = 128,
			Height = 128,
		};
		var accentColor = new Color(0.38f, 0.74f, 0.92f);
		double progress = 35;
		var quantity = 3;
		var status = "Ready";
		var refs = new ExampleRefs();

		var panel = PanelBuilder.CreatePanel(builder =>
		{
			refs.StatusLabel = builder.LateInit<Label>();
			refs.QuantityLabel = builder.LateInit<Label>();
			refs.CloseButton = builder.LateInit<Button>();
			refs.ProgressBar = builder.LateInit<ProgressBar>();

			void Refresh()
			{
				if (!IsInstanceValid(refs.StatusLabel)) return;
				refs.StatusLabel.Text = $"Status: {status}";
				refs.StatusLabel.Modulate = accentColor;
				refs.QuantityLabel!.Text = $"Quantity: {quantity}";
				refs.ProgressBar!.Value = progress;
			}

			builder.OnPanelInitialized += runtimePanel =>
			{
				refs.CloseButton!.Pressed += runtimePanel.Close;
				runtimePanel.RegisterInputCancel(runtimePanel.Close);
			};
			builder.OnPanelOpen += _ => Refresh();

			return builder.MarginContainer(
				margin =>
				{
					margin.AddThemeConstantOverride("margin_left", 18);
					margin.AddThemeConstantOverride("margin_top", 18);
					margin.AddThemeConstantOverride("margin_right", 18);
					margin.AddThemeConstantOverride("margin_bottom", 18);
				},
				builder.Center(
					builder.Panel(
						panelContainer => panelContainer.CustomMinimumSize = new Vector2(640, 0),
						builder.VBox(
							box => box.AddThemeConstantOverride("separation", 12),
							builder.Label("Runtime Builder Showcase", label => label.HorizontalAlignment = HorizontalAlignment.Center),
							builder.RichTextLabel(
								"[b]Built in C#[/b]\nRichTextLabel, TextureButton, ColorPickerButton, HSlider and ProgressBar are all created through PanelBuilder.",
								richText =>
								{
									richText.BbcodeEnabled = true;
									richText.CustomMinimumSize = new Vector2(0, 72);
								}),
							builder.HSplit(
								builder.VBox(
									box => box.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
									builder.Label("Preview"),
									builder.TextureRect(previewTexture, textureRect =>
									{
										textureRect.CustomMinimumSize = new Vector2(180, 180);
										textureRect.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
									}),
									builder.HBox(
										builder.TextureButton(previewTexture, () =>
										{
											quantity++;
											status = "Texture action triggered";
											Refresh();
										}, textureButton =>
										{
											textureButton.CustomMinimumSize = new Vector2(48, 48);
											textureButton.IgnoreTextureSize = true;
											textureButton.StretchMode = TextureButton.StretchModeEnum.KeepAspectCentered;
										}),
										builder.Button("Reset", () =>
										{
											quantity = 3;
											progress = 35;
											status = "Values reset";
											Refresh();
										})
									),
									builder.Label("Accent Color"),
									builder.ColorPickerButton(accentColor, color =>
									{
										accentColor = color;
										status = "Accent color updated";
										Refresh();
									})
								),
								builder.VBox(
									box => box.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
									refs.StatusLabel = builder.Label(),
									refs.QuantityLabel = builder.Label(),
									refs.ProgressBar = builder.ProgressBar(progress, init: bar =>
									{
										bar.ShowPercentage = true;
										bar.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
									}),
									builder.HSlider(progress, 0, 100, 5, value =>
									{
										progress = value;
										status = $"Progress set to {value:0}%";
										Refresh();
									}, slider => slider.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill),
									builder.IntSpinBox(quantity, 1, 9, 1, value =>
									{
										quantity = value;
										status = $"Quantity set to {value}";
										Refresh();
									}),
									refs.CloseButton = builder.Button("Close")
								)
							)
						)
					)
				)
			);
		});

		await panel.OpenPanelAsync(closePolicy: ClosePolicy.Delete);
		GetTree().Quit();
	}
}
