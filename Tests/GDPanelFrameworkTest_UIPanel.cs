using GdUnit4;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using GodotTask;

namespace GDPanelFramework.Tests;

[TestSuite, RequireGodotRuntime]
public class GDPanelFrameworkTest_UIPanel
{
    private sealed class EventFunctionMonitor
    {
        public bool Initialized { get; set; }
        public bool Opened { get; set; }
        public bool Closed { get; set; }
        public bool Predelete { get; set; }
        public bool Notification { get; set; }
    }

    private sealed class EventFunctionArgMonitor
    {
        public int OpenValue { get; set; }
        public int CloseValue { get; set; }
        public bool Initialized { get; set; }
        public bool Opened { get; set; }
        public bool Closed { get; set; }
        public bool Predelete { get; set; }
        public bool Notification { get; set; }
    }

    private sealed class TokenMonitor
    {
        public bool PanelCloseTokenCanceled { get; set; }
        public bool PanelOpenTweenFinishTokenCanceled { get; set; }
        public bool PanelCloseTweenFinishTokenCanceled { get; set; }
    }

    private sealed class InputMonitor
    {
        public bool UIAcceptPressed { get; set; }
        public bool UIAcceptReleased { get; set; }
        public bool UICancelPressed { get; set; }
        public bool UICancelReleased { get; set; }
    }

    private sealed class CompositeInputMonitor
    {
        public bool Composite_Axis_Started { get; set; }
        public bool Composite_Axis_Negative_Updated { get; set; }
        public bool Composite_Axis_Positive_Updated { get; set; }
        public bool Composite_Axis_Ended { get; set; }

        public bool Composite_Vector_Started { get; set; }
        public bool Composite_Vector_Up_Updated { get; set; }
        public bool Composite_Vector_Down_Updated { get; set; }
        public bool Composite_Vector_Left_Updated { get; set; }
        public bool Composite_Vector_Right_Updated { get; set; }
        public bool Composite_Vector_Ended { get; set; }
    }

    private sealed class RuntimePanelMonitor
    {
        public bool Initialized { get; set; }
        public bool Opened { get; set; }
        public bool Closed { get; set; }
        public bool Predelete { get; set; }
    }

    private sealed class ExternalCloseMonitor
    {
        public bool Opened { get; set; }
        public bool Closed { get; set; }
        public bool ExternalClosed { get; set; }
        public bool Predelete { get; set; }
    }

    private sealed class PanelRoutingMonitor
    {
        public int LowerPanelInputCount { get; set; }
        public int TopPanelInputCount { get; set; }
    }

    private sealed class RuntimePanelDslRefs
    {
        public Label? Label;
        public Label? Title;
        public RichTextLabel? RichText;
        public Button? Button;
        public TextureButton? TextureButton;
        public CheckButton? Toggle;
        public ColorPickerButton? ColorPickerButton;
        public LineEdit? LineEdit;
        public TextEdit? TextEdit;
        public TextureRect? TextureRect;
        public SpinBox? FloatSpinBox;
        public SpinBox? IntSpinBox;
        public HSlider? HSlider;
        public ProgressBar? ProgressBar;
        public OptionButton? Option;
        public OptionButton? IconOption;
        public Tree? Tree;
        public TreeItem? TreeRoot;
        public TreeItem? TreeChild;
        public GridContainer? Grid;
        public ItemList? ItemList;
    }

    private ISceneRunner? _sceneRunner;

    [BeforeTest]
    public void BeforeTest()
    {
        _sceneRunner = ISceneRunner.Load("res://Tests/test_entry.tscn", true).NotNull();
    }

    [TestCase, RequireGodotRuntime]
    public static async Task UIPanel_Test_EventFunction()
    {
        await GDTask.NextFrame();

        var monitor = new EventFunctionMonitor();

        await PanelBuilder.CreatePanel(builder =>
            {
                builder.OnPanelInitialized += _ => monitor.Initialized = true;
                builder.OnPanelOpen += panel =>
                {
                    monitor.Opened = true;
                    GDTask.NextFrame().ContinueWith(panel.Close);
                };
                builder.OnPanelClose += _ => monitor.Closed = true;
                builder.OnPanelPredelete += _ => monitor.Predelete = true;
                builder.OnPanelNotification += (_, _) => monitor.Notification = true;

                return builder.MarginContainer();
            })
            .OpenPanelAsync(closePolicy: ClosePolicy.Delete);

        await GDTask.NextFrame();

        Assertions.AssertThat(monitor.Initialized).IsTrue();
        Assertions.AssertThat(monitor.Opened).IsTrue();
        Assertions.AssertThat(monitor.Closed).IsTrue();
        Assertions.AssertThat(monitor.Predelete).IsTrue();
        Assertions.AssertThat(monitor.Notification).IsTrue();
    }

    [TestCase, RequireGodotRuntime]
    public static async Task UIPanelArg_Test_EventFunction()
    {
        var monitor = new EventFunctionArgMonitor();

        const int openArg = 10;

        var closeValue = await PanelBuilder.CreatePanelArg2<int, int>(builder =>
            {
                builder.OnPanelInitialized += _ => monitor.Initialized = true;
                builder.OnPanelOpen += panel =>
                {
                    monitor.Opened = true;
                    var openValue = panel.CurrentOpenArg;
                    monitor.OpenValue = openValue;
                    var closeValue1 = openValue * 2;
                    monitor.CloseValue = closeValue1;
                    GDTask.NextFrame().ContinueWith(() => panel.Close(closeValue1));
                };
                builder.OnPanelClose += _ => monitor.Closed = true;
                builder.OnPanelPredelete += _ => monitor.Predelete = true;
                builder.OnPanelNotification += (_, _) => monitor.Notification = true;

                return builder.MarginContainer();
            })
            .OpenPanelAsync(openArg, closePolicy: ClosePolicy.Delete);

        await GDTask.NextFrame();

        Assertions.AssertThat(monitor.Initialized).IsTrue();
        Assertions.AssertThat(monitor.Opened).IsTrue();
        Assertions.AssertThat(monitor.Closed).IsTrue();
        Assertions.AssertThat(monitor.Predelete).IsTrue();
        Assertions.AssertThat(monitor.Notification).IsTrue();

        Assertions.AssertThat(monitor.OpenValue).IsEqual(openArg);
        Assertions.AssertThat(monitor.CloseValue).IsEqual(openArg * 2);
        Assertions.AssertThat(monitor.CloseValue).IsEqual(closeValue);
    }

    [TestCase, RequireGodotRuntime]
    public static async Task UIPanel_Test_Token()
    {
        await GDTask.NextFrame();

        var monitor = new TokenMonitor();

        await PanelBuilder.CreatePanel(builder =>
            {
                builder.OnPanelInitialized += panel =>
                {
                    panel.PanelCloseToken.Register(() => monitor.PanelCloseTokenCanceled = true);
                    panel.PanelOpenTweenFinishToken.Register(() => monitor.PanelOpenTweenFinishTokenCanceled = true);
                    panel.PanelCloseTweenFinishToken.Register(() => monitor.PanelCloseTweenFinishTokenCanceled = true);
                };
                builder.OnPanelOpen += panel => GDTask.NextFrame().ContinueWith(panel.Close);

                return builder.MarginContainer();
            })
            .OpenPanelAsync(closePolicy: ClosePolicy.Delete);

        await GDTask.NextFrame();

        Assertions.AssertThat(monitor.PanelCloseTokenCanceled).IsTrue();
        Assertions.AssertThat(monitor.PanelCloseTweenFinishTokenCanceled).IsTrue();
        Assertions.AssertThat(monitor.PanelOpenTweenFinishTokenCanceled).IsTrue();
    }

    [TestCase, RequireGodotRuntime]
    public async Task UIPanel_Test_Input()
    {
        await GDTask.NextFrame();
        _sceneRunner.NotNull();

        var monitor = new InputMonitor();

        await PanelBuilder.CreatePanel(builder =>
            {
                builder.OnPanelOpen += panel => GDTask.Create(async () =>
                {
                    Action<InputEvent> call1 = _ => monitor.UIAcceptPressed = !monitor.UIAcceptPressed;
                    Action<InputEvent> call2 = _ => monitor.UIAcceptReleased = !monitor.UIAcceptReleased;
                    Action call3 = () => monitor.UICancelPressed = !monitor.UICancelPressed;
                    Action call4 = () => monitor.UICancelReleased = !monitor.UICancelReleased;

                    panel.RegisterInput(BuiltinInputNames.UIAccept, call1, InputActionPhase.Pressed);
                    panel.RegisterInput(BuiltinInputNames.UIAccept, call2, InputActionPhase.Released);

                    await Helpers.KeyPressedAsync(Key.Enter);

                    panel.RemoveInput(BuiltinInputNames.UIAccept, call1, InputActionPhase.Pressed);
                    panel.RemoveInput(BuiltinInputNames.UIAccept, call2, InputActionPhase.Released);

                    await Helpers.KeyPressedAsync(Key.Enter);

                    panel.RegisterInputCancel(call3, InputActionPhase.Pressed);
                    panel.RegisterInputCancel(call4, InputActionPhase.Released);

                    await Helpers.KeyPressedAsync(Key.Escape);

                    panel.RegisterInputCancel(call3, InputActionPhase.Pressed);
                    panel.RegisterInputCancel(call4, InputActionPhase.Released);

                    await Helpers.KeyPressedAsync(Key.Escape);

                    panel.Close();
                });
                return builder.MarginContainer();
            })
            .OpenPanelAsync(closePolicy: ClosePolicy.Delete);

        await GDTask.NextFrame();

        Assertions.AssertThat(monitor.UIAcceptPressed).IsTrue();
        Assertions.AssertThat(monitor.UIAcceptReleased).IsTrue();
        Assertions.AssertThat(monitor.UICancelPressed).IsTrue();
        Assertions.AssertThat(monitor.UICancelReleased).IsTrue();
    }

    [TestCase, RequireGodotRuntime]
    public async Task UIPanel_Test_Input_Composite()
    {
        await GDTask.NextFrame();
        _sceneRunner.NotNull();

        var monitor = new CompositeInputMonitor();

        await PanelBuilder.CreatePanel(builder =>
            {
                builder.OnPanelOpen += panel => RunCompositeInputPanelAsync(panel, monitor).Forget();
                return builder.MarginContainer();
            })
            .OpenPanelAsync(closePolicy: ClosePolicy.Delete);

        await GDTask.NextFrame();

        Assertions.AssertThat(monitor.Composite_Axis_Started).IsTrue();
        Assertions.AssertThat(monitor.Composite_Axis_Negative_Updated).IsTrue();
        Assertions.AssertThat(monitor.Composite_Axis_Positive_Updated).IsTrue();
        Assertions.AssertThat(monitor.Composite_Axis_Ended).IsTrue();

        Assertions.AssertThat(monitor.Composite_Vector_Started).IsTrue();
        Assertions.AssertThat(monitor.Composite_Vector_Up_Updated).IsTrue();
        Assertions.AssertThat(monitor.Composite_Vector_Down_Updated).IsTrue();
        Assertions.AssertThat(monitor.Composite_Vector_Left_Updated).IsTrue();
        Assertions.AssertThat(monitor.Composite_Vector_Right_Updated).IsTrue();
        Assertions.AssertThat(monitor.Composite_Vector_Ended).IsTrue();
    }

    [TestCase, RequireGodotRuntime]
    public async Task UIPanel_Test_TweenHide()
    {
        await GDTask.NextFrame();

        var panelA = PanelBuilder.CreatePanel(builder => builder.MarginContainer());
        panelA.OpenPanel(closePolicy: ClosePolicy.Delete);

        var panelB = PanelBuilder.CreatePanel(builder => builder.MarginContainer());
        panelB.OpenPanel(PreviousPanelVisual.Hidden, ClosePolicy.Delete);
        var panelAHidden = panelA.Visible;

        Assertions.AssertThat(panelAHidden).IsFalse();
        ((IRuntimePanelHandle)panelB).Close();
        Assertions.AssertThat(panelA.Visible).IsTrue();
        ((IRuntimePanelHandle)panelA).Close();
    }

    [TestCase, RequireGodotRuntime]
    public static async Task RuntimePanel_Test_Lifecycle()
    {
        await GDTask.NextFrame();

        var monitor = new RuntimePanelMonitor();

        var panel = PanelBuilder.CreatePanel(builder =>
        {
            var label = builder.Label("Runtime Panel");

            builder.OnPanelInitialized += _ => { monitor.Initialized = label.GetParent() is not null; };
            builder.OnPanelOpen += runtimePanel =>
            {
                monitor.Opened = true;
                runtimePanel.Close();
            };
            builder.OnPanelClose += _ => monitor.Closed = true;
            builder.OnPanelPredelete += _ => monitor.Predelete = true;

            return builder.MarginContainer(children: label);
        });

        await panel.OpenPanelAsync(closePolicy: ClosePolicy.Delete);
        await GDTask.NextFrame();

        Assertions.AssertThat(monitor.Initialized).IsTrue();
        Assertions.AssertThat(monitor.Opened).IsTrue();
        Assertions.AssertThat(monitor.Closed).IsTrue();
        Assertions.AssertThat(monitor.Predelete).IsTrue();
    }

    [TestCase, RequireGodotRuntime]
    public static async Task RuntimePanelArg2_Test_ReturnValue()
    {
        await GDTask.NextFrame();

        const int openArg = 10;
        var refs = new RuntimePanelDslRefs();

        var panel = PanelBuilder.CreatePanelArg2<int, int>(builder =>
        {
            refs.Label = builder.LateInit<Label>();

            builder.OnPanelOpen += runtimePanel =>
            {
                refs.Label.Text = runtimePanel.CurrentOpenArg.ToString();
                runtimePanel.Close(runtimePanel.CurrentOpenArg * 2);
            };

            return builder.MarginContainer(refs.Label = builder.Label());
        });

        var closeValue = await panel.OpenPanelAsync(openArg, closePolicy: ClosePolicy.Delete);

        Assertions.AssertThat(closeValue).IsEqual(openArg * 2);
    }

    [TestCase, RequireGodotRuntime]
    public static async Task RuntimePanel_Test_CachePolicy_NotSupported()
    {
        await GDTask.NextFrame();

        var panel = PanelBuilder.CreatePanel(builder => builder.MarginContainer());

        InvalidOperationException? exception = null;
        try
        {
            panel.OpenPanel(closePolicy: ClosePolicy.Cache);
        }
        catch (InvalidOperationException ex)
        {
            exception = ex;
        }
        finally
        {
            panel.QueueFree();
        }

        await GDTask.NextFrame();

        Assertions.AssertThat(exception).IsNotNull();
    }

    [TestCase, RequireGodotRuntime]
    public static async Task RuntimePanel_Test_ExternalCancellation()
    {
        await GDTask.NextFrame();

        var cancellationTokenSource = new CancellationTokenSource();
        var monitor = new ExternalCloseMonitor();

        var panel = PanelBuilder.CreatePanel(builder =>
        {
            builder.OnPanelOpen += _ =>
            {
                monitor.Opened = true;
                GDTask.NextFrame().ContinueWith(cancellationTokenSource.Cancel);
            };
            builder.OnPanelClose += _ => monitor.Closed = true;
            builder.OnPanelExternalClose += _ => monitor.ExternalClosed = true;
            builder.OnPanelPredelete += _ => monitor.Predelete = true;

            return builder.MarginContainer();
        });

        OperationCanceledException? exception = null;
        try
        {
            await panel.OpenPanelAsync(closePolicy: ClosePolicy.Delete, cancellationToken: cancellationTokenSource.Token);
        }
        catch (OperationCanceledException ex)
        {
            exception = ex;
        }

        await GDTask.NextFrame();

        Assertions.AssertThat(exception).IsNotNull();
        Assertions.AssertThat(cancellationTokenSource.IsCancellationRequested).IsTrue();
        Assertions.AssertThat(monitor.Opened).IsTrue();
        Assertions.AssertThat(monitor.Closed).IsFalse();
        Assertions.AssertThat(monitor.ExternalClosed).IsTrue();
        Assertions.AssertThat(monitor.Predelete).IsTrue();
    }

    [TestCase, RequireGodotRuntime]
    public async Task UIPanel_Test_InputRouting_OnlyTopPanelReceivesInput()
    {
        await GDTask.NextFrame();
        _sceneRunner.NotNull();

        var monitor = new PanelRoutingMonitor();

        var lowerPanel = PanelBuilder.CreatePanel(builder =>
        {
            builder.OnPanelOpen += panel =>
                panel.RegisterInput(BuiltinInputNames.UIAccept, _ => monitor.LowerPanelInputCount++, InputActionPhase.Pressed);

            return builder.MarginContainer();
        });

        lowerPanel.OpenPanel(closePolicy: ClosePolicy.Delete);
        await GDTask.NextFrame();

        var topPanel = PanelBuilder.CreatePanel(builder =>
        {
            builder.OnPanelOpen += panel =>
                panel.RegisterInput(BuiltinInputNames.UIAccept, _ => monitor.TopPanelInputCount++, InputActionPhase.Pressed);

            return builder.MarginContainer();
        });

        topPanel.OpenPanel(PreviousPanelVisual.Visible, ClosePolicy.Delete);

        await Helpers.KeyPressedAsync(Key.Enter);

        Assertions.AssertThat(monitor.LowerPanelInputCount).IsEqual(0);
        Assertions.AssertThat(monitor.TopPanelInputCount).IsEqual(1);

        ((IRuntimePanelHandle)topPanel).Close();
        await GDTask.NextFrame();
        await Helpers.KeyPressedAsync(Key.Enter);

        Assertions.AssertThat(monitor.LowerPanelInputCount).IsEqual(1);
        Assertions.AssertThat(monitor.TopPanelInputCount).IsEqual(1);

        ((IRuntimePanelHandle)lowerPanel).Close();
    }

    [TestCase, RequireGodotRuntime]
    public async Task UIPanel_Test_DefaultInputRegistrationBehavior_Release()
    {
        await GDTask.NextFrame();
        _sceneRunner.NotNull();

        var originalBehavior = PanelManager.DefaultInputRegistrationBehavior;
        var callbackTriggered = false;
        var callbackTriggeredBeforeRelease = false;

        try
        {
            PanelManager.DefaultInputRegistrationBehavior = PanelManager.InputRegistrationBehavior.Release;

            await PanelBuilder.CreatePanel(builder =>
                {
                    builder.OnPanelOpen += panel => GDTask.Create(async () =>
                    {
                        panel.RegisterInput(BuiltinInputNames.UIAccept, _ => callbackTriggered = true);

                        await Helpers.KeyPressAsync(Key.Enter);
                        callbackTriggeredBeforeRelease = callbackTriggered;

                        await Helpers.KeyReleaseAsync(Key.Enter);
                        panel.Close();
                    });

                    return builder.MarginContainer();
                })
                .OpenPanelAsync(closePolicy: ClosePolicy.Delete);

            await GDTask.NextFrame();
        }
        finally
        {
            PanelManager.DefaultInputRegistrationBehavior = originalBehavior;
        }

        Assertions.AssertThat(callbackTriggeredBeforeRelease).IsFalse();
        Assertions.AssertThat(callbackTriggered).IsTrue();
    }

    [TestCase, RequireGodotRuntime]
    public static async Task RuntimePanel_Test_DslHelpers()
    {
        await GDTask.NextFrame();

        var optionValues = new[] { "Backlog", "Done" };
        var mappedOptionValues = new[]
        {
            new KeyValuePair<int, string>(10, "Low"),
            new KeyValuePair<int, string>(20, "High")
        };
        var itemValues = new[] { "One", "Two" };
        var toggledValue = false;
        var selectedValue = string.Empty;
        var mappedSelectedValue = 0;
        var selectedIconValue = string.Empty;
        var selectedItem = string.Empty;
        var lineValue = string.Empty;
        var textValue = string.Empty;
        var floatValue = 0f;
        var intValue = 0;
        var sliderValue = 0d;
        var selectedColor = Colors.Transparent;
        var pressed = false;
        var texturePressed = false;
        var titleText = string.Empty;
        var richTextContent = string.Empty;
        var gridColumns = 0;
        var treeColumns = 0;
        var treeColumnTitlesVisible = false;
        var hasTreeRoot = false;
        var hasTreeChild = false;
        var treeChildText = string.Empty;
        Texture2D? textureRectTexture = null;
        Texture2D? textureButtonTexture = null;
        var textureRectStretchMode = TextureRect.StretchModeEnum.Scale;
        var textureRectExpandMode = TextureRect.ExpandModeEnum.KeepSize;
        var floatSpinBoxStep = 0d;
        var intSpinBoxRounded = false;
        var progressBarValue = 0d;
        var iconOptionSelected = -1;
        var iconTexture = new GradientTexture2D();
        var iconOptions = new (string Text, Texture2D? Icon)[]
        {
            ("Backlog", null),
            ("Done", iconTexture)
        };
        var refs = new RuntimePanelDslRefs();

        var panel = PanelBuilder.CreatePanel(builder =>
        {
            refs.Title = builder.LateInit<Label>();
            refs.RichText = builder.LateInit<RichTextLabel>();
            refs.Button = builder.LateInit<Button>();
            refs.TextureButton = builder.LateInit<TextureButton>();
            refs.Toggle = builder.LateInit<CheckButton>();
            refs.ColorPickerButton = builder.LateInit<ColorPickerButton>();
            refs.LineEdit = builder.LateInit<LineEdit>();
            refs.TextEdit = builder.LateInit<TextEdit>();
            refs.TextureRect = builder.LateInit<TextureRect>();
            refs.FloatSpinBox = builder.LateInit<SpinBox>();
            refs.IntSpinBox = builder.LateInit<SpinBox>();
            refs.HSlider = builder.LateInit<HSlider>();
            refs.ProgressBar = builder.LateInit<ProgressBar>();
            refs.Option = builder.LateInit<OptionButton>();
            refs.IconOption = builder.LateInit<OptionButton>();
            refs.Tree = builder.LateInit<Tree>();
            refs.Grid = builder.LateInit<GridContainer>();
            refs.ItemList = builder.LateInit<ItemList>();

            builder.OnPanelOpen += runtimePanel =>
            {
                refs.Button.EmitSignal(BaseButton.SignalName.Pressed);
                refs.TextureButton.EmitSignal(BaseButton.SignalName.Pressed);
                refs.Toggle.EmitSignal(BaseButton.SignalName.Toggled, true);
                refs.ColorPickerButton.EmitSignal(ColorPickerButton.SignalName.ColorChanged, new Color(0.2f, 0.4f, 0.6f));
                refs.LineEdit.EmitSignal(LineEdit.SignalName.TextChanged, "updated-line");
                refs.TextEdit.Text = "updated-text";
                refs.TextEdit.EmitSignal(TextEdit.SignalName.TextChanged);
                refs.FloatSpinBox.EmitSignal(Godot.Range.SignalName.ValueChanged, 2.5);
                refs.IntSpinBox.EmitSignal(Godot.Range.SignalName.ValueChanged, 7.0);
                refs.HSlider.EmitSignal(Godot.Range.SignalName.ValueChanged, 42.0);
                refs.Option.EmitSignal(OptionButton.SignalName.ItemSelected, 1L);
                refs.IconOption.EmitSignal(OptionButton.SignalName.ItemSelected, 1L);
                refs.ItemList.EmitSignal(ItemList.SignalName.ItemSelected, 1L);
                titleText = refs.Title.Text;
                richTextContent = refs.RichText.Text;
                gridColumns = refs.Grid.Columns;
                treeColumns = refs.Tree.Columns;
                treeColumnTitlesVisible = refs.Tree.ColumnTitlesVisible;
                hasTreeRoot = refs.TreeRoot is not null;
                hasTreeChild = refs.TreeChild is not null;
                treeChildText = refs.TreeChild?.GetText(1) ?? string.Empty;
                textureRectTexture = refs.TextureRect.Texture;
                textureButtonTexture = refs.TextureButton.TextureNormal;
                textureRectStretchMode = refs.TextureRect.StretchMode;
                textureRectExpandMode = refs.TextureRect.ExpandMode;
                floatSpinBoxStep = refs.FloatSpinBox.Step;
                intSpinBoxRounded = refs.IntSpinBox.Rounded;
                progressBarValue = refs.ProgressBar.Value;
                iconOptionSelected = refs.IconOption.Selected;
                runtimePanel.Close();
            };

            return builder.MarginContainer(
                builder.VBox(
                    refs.Title = builder.Label(label => label.Text = "Helpers"),
                    refs.RichText = builder.RichTextLabel("[b]Runtime[/b] helpers", richText => richText.BbcodeEnabled = true),
                    builder.HSeparator(),
                    builder.HFlow(
                        builder.Label("A"),
                        builder.Label("B")
                    ),
                    refs.Button = builder.Button(() => pressed = true),
                    refs.TextureButton = builder.TextureButton(iconTexture, () => texturePressed = true),
                    refs.Grid = builder.Grid(2,
                        builder.Label("State"),
                        refs.Option = builder.OptionButton<string>(0, optionValues, value => selectedValue = value),
                        builder.Label("Flag"),
                        refs.Toggle = builder.CheckButton("Blocked", false, value => toggledValue = value)
                    ),
                    refs.ColorPickerButton = builder.ColorPickerButton(Colors.White, color => selectedColor = color),
                    refs.LineEdit = builder.LineEdit(value => lineValue = value),
                    refs.TextEdit = builder.TextEdit(value => textValue = value),
                    refs.TextureRect = builder.TextureRect(iconTexture),
                    refs.FloatSpinBox = builder.FloatSpinBox(1.5f, 0f, 10f, 0.5f, value => floatValue = value),
                    refs.IntSpinBox = builder.IntSpinBox(2, 0, 10, 1, value => intValue = value),
                    refs.HSlider = builder.HSlider(10, 0, 100, 5, value =>
                    {
                        sliderValue = value;
                        refs.ProgressBar.Value = value;
                    }),
                    refs.ProgressBar = builder.ProgressBar(10),
                    refs.Option = builder.OptionButton<int>(1, mappedOptionValues, value => mappedSelectedValue = value, option => option.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill),
                    refs.IconOption = builder.OptionButton("Backlog", iconOptions, value => selectedIconValue = value),
                    refs.Tree = builder.Tree(new[] { "Id", "Name", "State" }, tree =>
                    {
                        tree.CustomMinimumSize = new Vector2(0, 80);
                        refs.TreeRoot = builder.TreeRoot(tree, item =>
                        {
                            item.SetText(0, "0");
                            item.SetText(1, "Root");
                            item.SetText(2, "Ready");
                        });
                        refs.TreeChild = builder.TreeItem(refs.TreeRoot, item =>
                        {
                            item.SetText(0, "1");
                            item.SetText(1, "Child");
                            item.SetText(2, "Queued");
                        });
                    }),
                    refs.ItemList = builder.ItemList(itemValues, value => selectedItem = value)
                )
            );
        });

        await panel.OpenPanelAsync(closePolicy: ClosePolicy.Delete);
        await GDTask.NextFrame();

        Assertions.AssertThat(titleText).IsEqual("Helpers");
        Assertions.AssertThat(richTextContent).IsEqual("[b]Runtime[/b] helpers");
        Assertions.AssertThat(pressed).IsTrue();
        Assertions.AssertThat(texturePressed).IsTrue();
        Assertions.AssertThat(gridColumns).IsEqual(2);
        Assertions.AssertThat(treeColumns).IsEqual(3);
        Assertions.AssertThat(treeColumnTitlesVisible).IsTrue();
        Assertions.AssertThat(hasTreeRoot).IsTrue();
        Assertions.AssertThat(hasTreeChild).IsTrue();
        Assertions.AssertThat(treeChildText).IsEqual("Child");
        Assertions.AssertThat(toggledValue).IsTrue();
        Assertions.AssertThat(selectedValue).IsEqual(string.Empty);
        Assertions.AssertThat(mappedSelectedValue).IsEqual(20);
        Assertions.AssertThat(selectedIconValue).IsEqual("Done");
        Assertions.AssertThat(selectedItem).IsEqual("Two");
        Assertions.AssertThat(lineValue).IsEqual("updated-line");
        Assertions.AssertThat(textValue).IsEqual("updated-text");
        Assertions.AssertThat(floatValue).IsEqual(2.5f);
        Assertions.AssertThat(intValue).IsEqual(7);
        Assertions.AssertThat(sliderValue).IsEqual(42.0);
        Assertions.AssertThat(selectedColor).IsEqual(new Color(0.2f, 0.4f, 0.6f));
        Assertions.AssertThat(textureRectTexture).IsSame(iconTexture);
        Assertions.AssertThat(textureButtonTexture).IsSame(iconTexture);
        Assertions.AssertThat(textureRectStretchMode).IsEqual(TextureRect.StretchModeEnum.KeepAspectCentered);
        Assertions.AssertThat(textureRectExpandMode).IsEqual(TextureRect.ExpandModeEnum.IgnoreSize);
        Assertions.AssertThat(floatSpinBoxStep).IsEqual(0.5);
        Assertions.AssertThat(intSpinBoxRounded).IsTrue();
        Assertions.AssertThat(progressBarValue).IsEqual(42.0);
        Assertions.AssertThat(iconOptionSelected).IsEqual(0);
    }

    private static async GDTask RunCompositeInputPanelAsync(IRuntimePanelHandle panel, CompositeInputMonitor monitor)
    {
        Action<float> callAxisStarted = _ => monitor.Composite_Axis_Started = !monitor.Composite_Axis_Started;
        Action<float> callAxisUpdated = inputDirection =>
        {
            switch (inputDirection)
            {
                case < 0:
                    monitor.Composite_Axis_Negative_Updated = !monitor.Composite_Axis_Negative_Updated;
                    break;
                case > 0:
                    monitor.Composite_Axis_Positive_Updated = !monitor.Composite_Axis_Positive_Updated;
                    break;
            }
        };
        Action<float> callAxisEnded = _ => monitor.Composite_Axis_Ended = !monitor.Composite_Axis_Ended;

        Action<Vector2> callVectorStarted = _ => monitor.Composite_Vector_Started = !monitor.Composite_Vector_Started;
        Action<Vector2> callVectorUpdated = inputDirection =>
        {
            if (inputDirection == Vector2.Up)
                monitor.Composite_Vector_Up_Updated = !monitor.Composite_Vector_Up_Updated;
            if (inputDirection == Vector2.Down)
                monitor.Composite_Vector_Down_Updated = !monitor.Composite_Vector_Down_Updated;
            if (inputDirection == Vector2.Left)
                monitor.Composite_Vector_Left_Updated = !monitor.Composite_Vector_Left_Updated;
            if (inputDirection == Vector2.Right)
                monitor.Composite_Vector_Right_Updated = !monitor.Composite_Vector_Right_Updated;
        };
        Action<Vector2> callVectorEnded = _ => monitor.Composite_Vector_Ended = !monitor.Composite_Vector_Ended;

        panel.RegisterInputAxis(BuiltinInputNames.UILeft, BuiltinInputNames.UIRight, callAxisStarted, CompositeInputActionState.Start);
        panel.RegisterInputAxis(BuiltinInputNames.UILeft, BuiltinInputNames.UIRight, callAxisUpdated, CompositeInputActionState.Update);
        panel.RegisterInputAxis(BuiltinInputNames.UILeft, BuiltinInputNames.UIRight, callAxisEnded, CompositeInputActionState.End);

        await Helpers.KeyPressAsync(Key.Left);
        await Helpers.KeyPressAsync(Key.Right);
        await Helpers.KeyReleaseAsync(Key.Left);
        await Helpers.KeyReleaseAsync(Key.Right);

        panel.RemoveInputAxis(BuiltinInputNames.UILeft, BuiltinInputNames.UIRight, callAxisStarted, CompositeInputActionState.Start);
        panel.RemoveInputAxis(BuiltinInputNames.UILeft, BuiltinInputNames.UIRight, callAxisUpdated, CompositeInputActionState.Update);
        panel.RemoveInputAxis(BuiltinInputNames.UILeft, BuiltinInputNames.UIRight, callAxisEnded, CompositeInputActionState.End);

        await Helpers.KeyPressAsync(Key.Left);
        await Helpers.KeyPressAsync(Key.Right);
        await Helpers.KeyReleaseAsync(Key.Left);
        await Helpers.KeyReleaseAsync(Key.Right);

        panel.RegisterInputVector(BuiltinInputNames.UIUp, BuiltinInputNames.UIDown, BuiltinInputNames.UILeft, BuiltinInputNames.UIRight, callVectorStarted, CompositeInputActionState.Start);
        panel.RegisterInputVector(BuiltinInputNames.UIUp, BuiltinInputNames.UIDown, BuiltinInputNames.UILeft, BuiltinInputNames.UIRight, callVectorUpdated, CompositeInputActionState.Update);
        panel.RegisterInputVector(BuiltinInputNames.UIUp, BuiltinInputNames.UIDown, BuiltinInputNames.UILeft, BuiltinInputNames.UIRight, callVectorEnded, CompositeInputActionState.End);

        await Helpers.KeyPressAsync(Key.Up);
        await Helpers.KeyPressAsync(Key.Right);
        await Helpers.KeyReleaseAsync(Key.Up);
        await Helpers.KeyPressAsync(Key.Down);
        await Helpers.KeyReleaseAsync(Key.Right);
        await Helpers.KeyPressAsync(Key.Left);
        await Helpers.KeyReleaseAsync(Key.Down);
        await Helpers.KeyReleaseAsync(Key.Left);

        panel.RemoveInputVector(BuiltinInputNames.UIUp, BuiltinInputNames.UIDown, BuiltinInputNames.UILeft, BuiltinInputNames.UIRight, callVectorStarted, CompositeInputActionState.Start);
        panel.RemoveInputVector(BuiltinInputNames.UIUp, BuiltinInputNames.UIDown, BuiltinInputNames.UILeft, BuiltinInputNames.UIRight, callVectorUpdated, CompositeInputActionState.Update);
        panel.RemoveInputVector(BuiltinInputNames.UIUp, BuiltinInputNames.UIDown, BuiltinInputNames.UILeft, BuiltinInputNames.UIRight, callVectorEnded, CompositeInputActionState.End);

        await Helpers.KeyPressAsync(Key.Up);
        await Helpers.KeyPressAsync(Key.Right);
        await Helpers.KeyReleaseAsync(Key.Up);
        await Helpers.KeyPressAsync(Key.Down);
        await Helpers.KeyReleaseAsync(Key.Right);
        await Helpers.KeyPressAsync(Key.Left);
        await Helpers.KeyReleaseAsync(Key.Down);
        await Helpers.KeyReleaseAsync(Key.Left);

        panel.Close();
    }
}