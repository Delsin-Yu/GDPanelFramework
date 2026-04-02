using System.Collections.Generic;
using GDPanelFramework.Panels;
using Godot;
using GodotTask;

namespace GDPanelFramework.Example._04;

/// <summary>
/// Demonstrates composing multiple runtime-built panels into a stateful workflow.
/// </summary>
public partial class Example04_Main : Node
{
	private const string PlannerCanceledMessage = "Planner canceled.";

	private sealed class ExpeditionTemplate
	{
		public required string MissionName { get; init; }
		public required string Commander { get; init; }
		public required string Notes { get; init; }
		public int TeamSize { get; init; }
		public int Risk { get; init; }
	}

	private sealed class ExpeditionState
	{
		public string MissionName = string.Empty;
		public string Commander = string.Empty;
		public string Notes = string.Empty;
		public string Biome = "Forest";
		public string Approach = "Balanced";
		public string Specialist = "Scout";
		public int TeamSize = 4;
		public int Risk = 35;
		public bool IncludeMedic = true;
		public bool DeployDrone = true;
		public Color AccentColor = new(0.87f, 0.53f, 0.35f);
		public string Status = "";
	}

	private sealed class PlannerRefs
	{
		public LineEdit? MissionNameEdit;
		public OptionButton? BiomeOption;
		public OptionButton? ApproachOption;
		public SpinBox? TeamSizeSpinBox;
		public HSlider? RiskSlider;
		public ProgressBar? RiskBar;
		public CheckButton? MedicToggle;
		public CheckButton? DroneToggle;
		public ColorPickerButton? AccentPicker;
		public ItemList? SpecialistList;
		public TextureRect? PreviewTexture;
		public Label? SummaryLabel;
		public Label? StatusLabel;
		public RichTextLabel? BriefingLabel;
		public TextEdit? NotesEdit;
		public TreeItem? ObjectiveRoot;
		public TreeItem? RouteItem;
		public TreeItem? SquadItem;
		public TreeItem? ExtractionItem;
		public Button? LaunchButton;
		public Button? CancelButton;
	}

	private sealed class ConfirmationRefs
	{
		public RichTextLabel? SummaryLabel;
		public Button? ConfirmButton;
		public Button? BackButton;
	}

	private sealed class ResultRefs
	{
		public Label? ResultLabel;
		public Button? CloseButton;
	}

	public override void _Ready() =>
		GDTask.NextFrame().ContinueWith(OpenDemo);

	private void OpenDemo() => OpenDemoAsync().Forget();

	private async GDTask OpenDemoAsync()
	{
		var template = new ExpeditionTemplate
		{
			MissionName = "Operation Ember Tide",
			Commander = "Mira Voss",
			Notes = "Recover the relay core before dawn and avoid waking the shoreline sentries.",
			TeamSize = 4,
			Risk = 35,
		};

		var launchSummary = await CreatePlannerPanel().OpenPanelAsync(template, closePolicy: ClosePolicy.Delete);

		if (launchSummary == PlannerCanceledMessage)
		{
			GetTree().Quit();
			return;
		}

		await CreateResultPanel(launchSummary).OpenPanelAsync(closePolicy: ClosePolicy.Delete);
		GetTree().Quit();
	}

	private UIPanelArg2<ExpeditionTemplate, string> CreatePlannerPanel()
	{
		var state = new ExpeditionState();
		var refs = new PlannerRefs();

		var forestTexture = CreatePatternTexture(new Color(0.18f, 0.32f, 0.2f), new Color(0.48f, 0.73f, 0.49f));
		var desertTexture = CreatePatternTexture(new Color(0.42f, 0.29f, 0.12f), new Color(0.86f, 0.72f, 0.43f));
		var coastTexture = CreatePatternTexture(new Color(0.13f, 0.25f, 0.39f), new Color(0.49f, 0.8f, 0.94f));

		var biomeOptions = new (string Text, Texture2D? Icon)[]
		{
			("Forest", forestTexture),
			("Desert", desertTexture),
			("Coast", coastTexture),
		};

		var biomeTextures = new Dictionary<string, Texture2D>
		{
			["Forest"] = forestTexture,
			["Desert"] = desertTexture,
			["Coast"] = coastTexture,
		};

		var approachOptions = new[] { "Stealth", "Balanced", "Assault" };
		var specialistOptions = new[] { "Scout", "Engineer", "Medic", "Sniper" };

		string BuildSummary()
		{
			var modifiers = new List<string>();
			if (state.IncludeMedic)
			{
				modifiers.Add("medic onboard");
			}
			if (state.DeployDrone)
			{
				modifiers.Add("recon drone");
			}

			var modifierText = modifiers.Count == 0 ? "no support modules" : string.Join(", ", modifiers);
			return string.Join(
				"\n",
				$"Mission: {state.MissionName}",
				$"Commander: {state.Commander}",
				$"Biome: {state.Biome}",
				$"Approach: {state.Approach}",
				$"Squad: {state.TeamSize} operators with {state.Specialist} lead",
				$"Risk: {state.Risk}%",
				$"Support: {modifierText}",
				$"Notes: {state.Notes}"
			);
		}

		void RefreshUi()
		{
			if (refs.SummaryLabel is null || refs.BriefingLabel is null || refs.RiskBar is null || refs.StatusLabel is null)
			{
				return;
			}

			refs.PreviewTexture!.Texture = biomeTextures[state.Biome];
			refs.SummaryLabel.Text = $"{state.MissionName} | {state.Biome} | {state.TeamSize} operators | {state.Risk}% risk";
			refs.SummaryLabel.Modulate = state.AccentColor;
			refs.StatusLabel.Text = state.Status;
			refs.StatusLabel.Modulate = state.AccentColor;
			refs.BriefingLabel.Text =
				$"[b]{state.MissionName}[/b]\n" +
				$"Commander {state.Commander} is preparing a {state.Approach.ToLowerInvariant()} approach through the {state.Biome.ToLowerInvariant()}.\n" +
				$"{state.Specialist} leads a {state.TeamSize}-operator squad with {(state.IncludeMedic ? "medical" : "no medical")} backup and {(state.DeployDrone ? "a recon drone" : "no drone support")}.";
			refs.BriefingLabel.Modulate = state.AccentColor;
			refs.RiskBar.Value = state.Risk;

			UpdateTreeItem(refs.RouteItem, "Route", $"{state.Biome} insertion, {state.Approach.ToLowerInvariant()} pace");
			UpdateTreeItem(refs.SquadItem, "Squad", $"{state.Specialist} lead, {state.TeamSize} operators");
			UpdateTreeItem(refs.ExtractionItem, "Extraction", state.DeployDrone ? "Drone-guided pickup" : "Manual flare pickup");
		}

		async GDTask ConfirmLaunchAsync(IRuntimePanelArg2Handle<ExpeditionTemplate, string> runtimePanel)
		{
			var accepted = await CreateConfirmationPanel(state.AccentColor).OpenPanelAsync(BuildSummary(), closePolicy: ClosePolicy.Delete);

			if (!accepted)
			{
				state.Status = "Launch confirmation dismissed.";
				RefreshUi();
				return;
			}

			runtimePanel.Close(BuildSummary());
		}

		return PanelBuilder.CreatePanelArg2<ExpeditionTemplate, string>(builder =>
		{
			refs.MissionNameEdit = builder.LateInit<LineEdit>();
			refs.BiomeOption = builder.LateInit<OptionButton>();
			refs.ApproachOption = builder.LateInit<OptionButton>();
			refs.TeamSizeSpinBox = builder.LateInit<SpinBox>();
			refs.RiskSlider = builder.LateInit<HSlider>();
			refs.RiskBar = builder.LateInit<ProgressBar>();
			refs.MedicToggle = builder.LateInit<CheckButton>();
			refs.DroneToggle = builder.LateInit<CheckButton>();
			refs.AccentPicker = builder.LateInit<ColorPickerButton>();
			refs.SpecialistList = builder.LateInit<ItemList>();
			refs.PreviewTexture = builder.LateInit<TextureRect>();
			refs.SummaryLabel = builder.LateInit<Label>();
			refs.StatusLabel = builder.LateInit<Label>();
			refs.BriefingLabel = builder.LateInit<RichTextLabel>();
			refs.NotesEdit = builder.LateInit<TextEdit>();
			refs.LaunchButton = builder.LateInit<Button>();
			refs.CancelButton = builder.LateInit<Button>();

			builder.OnPanelInitialized += panel =>
			{
				refs.CancelButton!.Pressed += () => panel.Close(PlannerCanceledMessage);
				refs.LaunchButton!.Pressed += () => ConfirmLaunchAsync(panel).Forget();
				panel.RegisterInputCancel(() => panel.Close(PlannerCanceledMessage));
			};

			builder.OnPanelOpen += panel =>
			{
				var template = panel.CurrentOpenArg;
				if (template is null)
				{
					return;
				}

				state.MissionName = template.MissionName;
				state.Commander = template.Commander;
				state.Notes = template.Notes;
				state.TeamSize = template.TeamSize;
				state.Risk = template.Risk;
				state.Biome = "Forest";
				state.Approach = "Balanced";
				state.Specialist = "Scout";
				state.IncludeMedic = true;
				state.DeployDrone = true;
				state.AccentColor = new Color(0.87f, 0.53f, 0.35f);
				state.Status = "Tune the plan, then launch the expedition.";

				refs.MissionNameEdit!.Text = state.MissionName;
				SetOptionSelection(refs.BiomeOption!, state.Biome);
				SetOptionSelection(refs.ApproachOption!, state.Approach);
				refs.TeamSizeSpinBox!.Value = state.TeamSize;
				refs.RiskSlider!.Value = state.Risk;
				refs.MedicToggle!.ButtonPressed = state.IncludeMedic;
				refs.DroneToggle!.ButtonPressed = state.DeployDrone;
				refs.AccentPicker!.Color = state.AccentColor;
				refs.NotesEdit!.Text = state.Notes;
				SelectItem(refs.SpecialistList!, state.Specialist);
				RefreshUi();
				refs.MissionNameEdit.GrabFocus();
			};

			return builder.MarginContainer(
				margin =>
				{
					margin.AddThemeConstantOverride("margin_left", 24);
					margin.AddThemeConstantOverride("margin_top", 24);
					margin.AddThemeConstantOverride("margin_right", 24);
					margin.AddThemeConstantOverride("margin_bottom", 24);
				},
				builder.Center(
					builder.Panel(
						panelContainer => panelContainer.CustomMinimumSize = new Vector2(920, 0),
						builder.VBox(
							box => box.AddThemeConstantOverride("separation", 14),
							builder.Label("Runtime Expedition Planner", label =>
							{
								label.HorizontalAlignment = HorizontalAlignment.Center;
								label.AddThemeFontSizeOverride("font_size", 20);
							}),
							builder.Label(
								"A larger PanelBuilder example with live state, nested dialogs, open arguments, and a close result.",
								label => label.HorizontalAlignment = HorizontalAlignment.Center),
							builder.HSeparator(),
							builder.HSplit(
								builder.VBox(
									box =>
									{
										box.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
										box.AddThemeConstantOverride("separation", 10);
									},
									refs.PreviewTexture = builder.TextureRect(forestTexture, textureRect =>
									{
										textureRect.CustomMinimumSize = new Vector2(0, 180);
										textureRect.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
									}),
									builder.Grid(
										2,
										grid =>
										{
											grid.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
											grid.AddThemeConstantOverride("h_separation", 10);
											grid.AddThemeConstantOverride("v_separation", 8);
										},
										builder.Label("Mission Name"),
										refs.MissionNameEdit = builder.LineEdit(value =>
										{
											state.MissionName = value;
											state.Status = "Mission name updated.";
											RefreshUi();
										}, lineEdit => lineEdit.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill),
										builder.Label("Biome"),
										refs.BiomeOption = builder.OptionButton("Forest", biomeOptions, value =>
										{
											state.Biome = value;
											state.Status = $"Biome set to {value}.";
											RefreshUi();
										}, option => option.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill),
										builder.Label("Approach"),
										refs.ApproachOption = builder.OptionButton("Balanced", approachOptions, value =>
										{
											state.Approach = value;
											state.Status = $"Approach set to {value}.";
											RefreshUi();
										}, option => option.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill),
										builder.Label("Team Size"),
										refs.TeamSizeSpinBox = builder.IntSpinBox(4, 2, 8, 1, value =>
										{
											state.TeamSize = value;
											state.Status = $"Squad resized to {value}.";
											RefreshUi();
										}, spinBox => spinBox.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill)
									),
									builder.Label("Risk Threshold"),
									refs.RiskSlider = builder.HSlider(35, 0, 100, 5, value =>
									{
										state.Risk = (int)value;
										state.Status = $"Risk threshold adjusted to {state.Risk}%.";
										RefreshUi();
									}, slider => slider.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill),
									refs.RiskBar = builder.ProgressBar(35, init: progressBar =>
									{
										progressBar.ShowPercentage = true;
										progressBar.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
									}),
									builder.HFlow(
										flow => flow.AddThemeConstantOverride("h_separation", 10),
										refs.MedicToggle = builder.CheckButton("Medic", true, value =>
										{
											state.IncludeMedic = value;
											state.Status = value ? "Medic support enabled." : "Medic support removed.";
											RefreshUi();
										}),
										refs.DroneToggle = builder.CheckButton("Recon Drone", true, value =>
										{
											state.DeployDrone = value;
											state.Status = value ? "Drone support enabled." : "Drone support removed.";
											RefreshUi();
										}),
										refs.AccentPicker = builder.ColorPickerButton(state.AccentColor, color =>
										{
											state.AccentColor = color;
											state.Status = "Accent color updated.";
											RefreshUi();
										})
									),
									builder.Label("Specialist Lead"),
									refs.SpecialistList = builder.ItemList(specialistOptions, value =>
									{
										state.Specialist = value;
										state.Status = $"{value} assigned as lead specialist.";
										RefreshUi();
									}, itemList => itemList.CustomMinimumSize = new Vector2(0, 100))
								),
								builder.VSplit(
									split =>
									{
										split.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
										split.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
										split.SplitOffsets = [240];
									},
									builder.VBox(
										box =>
										{
											box.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
											box.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
											box.AddThemeConstantOverride("separation", 8);
										},
										builder.Label("Snapshot"),
										refs.SummaryLabel = builder.Label(label =>
										{
											label.AutowrapMode = TextServer.AutowrapMode.WordSmart;
											label.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
										}),
										builder.Label("Briefing"),
										refs.BriefingLabel = builder.RichTextLabel(richText =>
										{
											richText.BbcodeEnabled = true;
											richText.FitContent = true;
											richText.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
										})
									),
									builder.VBox(
										box =>
										{
											box.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
											box.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
											box.AddThemeConstantOverride("separation", 8);
										},
										builder.Label("Execution Board"),
										builder.Tree(new[] { "Stage", "Plan" }, tree =>
										{
											tree.CustomMinimumSize = new Vector2(0, 140);
											tree.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
											refs.ObjectiveRoot = builder.TreeRoot(tree, item => item.SetText(0, "Expedition"));
											refs.RouteItem = builder.TreeItem(refs.ObjectiveRoot, item => item.SetText(0, "Route"));
											refs.SquadItem = builder.TreeItem(refs.ObjectiveRoot, item => item.SetText(0, "Squad"));
											refs.ExtractionItem = builder.TreeItem(refs.ObjectiveRoot, item => item.SetText(0, "Extraction"));
										}),
										builder.Label("Command Notes"),
										refs.NotesEdit = builder.TextEdit(value =>
										{
											state.Notes = value;
											state.Status = "Command notes updated.";
											RefreshUi();
										}, textEdit =>
										{
											textEdit.CustomMinimumSize = new Vector2(0, 120);
											textEdit.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
										})
									)
								)
							),
							builder.HSeparator(),
							builder.HBox(
								box =>
								{
									box.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
									box.AddThemeConstantOverride("separation", 10);
								},
								refs.StatusLabel = builder.Label(label =>
								{
									label.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
									label.AutowrapMode = TextServer.AutowrapMode.WordSmart;
								}),
								refs.CancelButton = builder.Button("Cancel"),
								refs.LaunchButton = builder.Button("Launch")
							)
						)
					)
				)
			);
		});
	}

	private static UIPanelArg2<string, bool> CreateConfirmationPanel(Color accentColor)
	{
		var refs = new ConfirmationRefs();

		return PanelBuilder.CreatePanelArg2<string, bool>(builder =>
		{
			refs.SummaryLabel = builder.LateInit<RichTextLabel>();
			refs.ConfirmButton = builder.LateInit<Button>();
			refs.BackButton = builder.LateInit<Button>();

			builder.OnPanelInitialized += panel =>
			{
				refs.ConfirmButton!.Pressed += () => panel.Close(true);
				refs.BackButton!.Pressed += () => panel.Close(false);
				panel.RegisterInputCancel(() => panel.Close(false));
			};

			builder.OnPanelOpen += panel =>
			{
				refs.SummaryLabel!.Text = panel.CurrentOpenArg ?? string.Empty;
				refs.SummaryLabel.Modulate = accentColor;
				refs.ConfirmButton!.GrabFocus();
			};

			return builder.MarginContainer(
				margin =>
				{
					margin.AddThemeConstantOverride("margin_left", 36);
					margin.AddThemeConstantOverride("margin_top", 36);
					margin.AddThemeConstantOverride("margin_right", 36);
					margin.AddThemeConstantOverride("margin_bottom", 36);
				},
				builder.Center(
					builder.Panel(
						panelContainer => panelContainer.CustomMinimumSize = new Vector2(540, 0),
						builder.VBox(
							box => box.AddThemeConstantOverride("separation", 12),
							builder.Label("Confirm Launch", label => label.HorizontalAlignment = HorizontalAlignment.Center),
							refs.SummaryLabel = builder.RichTextLabel(richText =>
							{
								richText.CustomMinimumSize = new Vector2(0, 180);
								richText.FitContent = true;
							}),
							builder.HBox(
								box =>
								{
									box.Alignment = BoxContainer.AlignmentMode.End;
									box.AddThemeConstantOverride("separation", 10);
								},
								refs.BackButton = builder.Button("Back"),
								refs.ConfirmButton = builder.Button("Confirm")
							)
						)
					)
				)
			);
		});
	}

	private static UIPanel CreateResultPanel(string launchSummary)
	{
		var refs = new ResultRefs();

		return PanelBuilder.CreatePanel(builder =>
		{
			refs.ResultLabel = builder.LateInit<Label>();
			refs.CloseButton = builder.LateInit<Button>();

			builder.OnPanelInitialized += panel =>
			{
				refs.CloseButton!.Pressed += panel.Close;
				panel.RegisterInputCancel(panel.Close);
			};

			builder.OnPanelOpen += _ =>
			{
				refs.ResultLabel!.Text = $"Launch submitted.\n\n{launchSummary}";
				refs.CloseButton!.GrabFocus();
			};

			return builder.MarginContainer(
				margin =>
				{
					margin.AddThemeConstantOverride("margin_left", 36);
					margin.AddThemeConstantOverride("margin_top", 36);
					margin.AddThemeConstantOverride("margin_right", 36);
					margin.AddThemeConstantOverride("margin_bottom", 36);
				},
				builder.Center(
					builder.Panel(
						panelContainer => panelContainer.CustomMinimumSize = new Vector2(560, 0),
						builder.VBox(
							box => box.AddThemeConstantOverride("separation", 12),
							builder.Label("Runtime Workflow Complete", label => label.HorizontalAlignment = HorizontalAlignment.Center),
							refs.ResultLabel = builder.Label(label =>
							{
								label.AutowrapMode = TextServer.AutowrapMode.WordSmart;
								label.CustomMinimumSize = new Vector2(0, 180);
							}),
							builder.HBox(
								box => box.Alignment = BoxContainer.AlignmentMode.End,
								refs.CloseButton = builder.Button("Close")
							)
						)
					)
				)
			);
		});
	}

	private static Texture2D CreatePatternTexture(Color baseColor, Color accentColor)
	{
		const int width = 128;
		const int height = 128;
		var image = Image.CreateEmpty(width, height, false, Image.Format.Rgba8);
		image.Fill(baseColor);

		for (var y = 0; y < height; y++)
		{
			for (var x = 0; x < width; x++)
			{
				if ((x + y) % 17 < 6)
				{
					image.SetPixel(x, y, accentColor);
				}
			}
		}

		return ImageTexture.CreateFromImage(image);
	}

	private static void UpdateTreeItem(TreeItem? item, string stage, string plan)
	{
		if (item is null)
		{
			return;
		}

		item.SetText(0, stage);
		item.SetText(1, plan);
	}

	private static void SetOptionSelection(OptionButton button, string text)
	{
		for (var index = 0; index < button.ItemCount; index++)
		{
			if (button.GetItemText(index) != text)
			{
				continue;
			}

			button.Select(index);
			return;
		}
	}

	private static void SelectItem(ItemList itemList, string text)
	{
		itemList.DeselectAll();

		for (var index = 0; index < itemList.ItemCount; index++)
		{
			if (itemList.GetItemText(index) != text)
			{
				continue;
			}

			itemList.Select(index);
			return;
		}
	}
}