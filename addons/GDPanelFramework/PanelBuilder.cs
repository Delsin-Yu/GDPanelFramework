using System;
using System.Collections.Generic;
using System.Threading;
using GDPanelFramework.Panels;
using Godot;

namespace GDPanelFramework;

/// <summary>
/// Provides a lightweight DSL for creating runtime panel trees with Godot <see cref="Control"/> nodes.
/// </summary>
public interface IPanelBuilderDsl
{
    /// <summary>
    /// Declares a control reference that will be initialized later while building the control tree.
    /// </summary>
    /// <typeparam name="T">The concrete control type to reference later.</typeparam>
    /// <returns>The default value of <typeparamref name="T"/>, used only to satisfy nullability analysis.</returns>
    /// <example>
    /// <code>
    /// var titleEdit = builder.LateInit&lt;LineEdit&gt;();
    /// var confirmButton = builder.LateInit&lt;Button&gt;();
    ///
    /// var root = builder.VBox(
    ///     titleEdit = builder.LineEdit("Draft"),
    ///     confirmButton = builder.Button("Confirm")
    /// );
    ///
    /// builder.OnPanelInitialized += panel =&gt;
    /// {
    ///     confirmButton.Pressed += panel.Close;
    /// };
    /// </code>
    /// </example>
    /// <remarks>
    /// Use this method when you want a variable to be assigned from a nested builder expression, such as
    /// <c>label = builder.Label()</c>. This method does not create a control instance by itself.
    /// </remarks>
    T LateInit<T>();

    /// <summary>
    /// Creates a <see cref="global::Godot.MarginContainer"/>, applies optional initialization, and adds the provided children.
    /// </summary>
    MarginContainer MarginContainer(Action<MarginContainer>? init = null, params Control[] children);

    /// <summary>
    /// Creates a <see cref="global::Godot.MarginContainer"/> and adds the provided children.
    /// </summary>
    MarginContainer MarginContainer(params Control[] children);

    /// <summary>
    /// Creates a <see cref="global::Godot.VBoxContainer"/>, applies optional initialization, and adds the provided children.
    /// </summary>
    VBoxContainer VBox(Action<VBoxContainer>? init = null, params Control[] children);

    /// <summary>
    /// Creates a <see cref="global::Godot.VBoxContainer"/> and adds the provided children.
    /// </summary>
    VBoxContainer VBox(params Control[] children);

    /// <summary>
    /// Creates a <see cref="global::Godot.HBoxContainer"/>, applies optional initialization, and adds the provided children.
    /// </summary>
    HBoxContainer HBox(Action<HBoxContainer>? init = null, params Control[] children);

    /// <summary>
    /// Creates a <see cref="global::Godot.HBoxContainer"/> and adds the provided children.
    /// </summary>
    HBoxContainer HBox(params Control[] children);

    /// <summary>
    /// Creates a <see cref="global::Godot.HFlowContainer"/>, applies optional initialization, and adds the provided children.
    /// </summary>
    HFlowContainer HFlow(Action<HFlowContainer>? init = null, params Control[] children);

    /// <summary>
    /// Creates a <see cref="global::Godot.HFlowContainer"/> and adds the provided children.
    /// </summary>
    HFlowContainer HFlow(params Control[] children);

    /// <summary>
    /// Creates a <see cref="global::Godot.VFlowContainer"/>, applies optional initialization, and adds the provided children.
    /// </summary>
    VFlowContainer VFlow(Action<VFlowContainer>? init = null, params Control[] children);

    /// <summary>
    /// Creates a <see cref="global::Godot.VFlowContainer"/> and adds the provided children.
    /// </summary>
    VFlowContainer VFlow(params Control[] children);

    /// <summary>
    /// Creates a <see cref="global::Godot.GridContainer"/>, applies optional initialization, and adds the provided children.
    /// </summary>
    GridContainer Grid(Action<GridContainer>? init = null, params Control[] children);

    /// <summary>
    /// Creates a <see cref="global::Godot.GridContainer"/> and adds the provided children.
    /// </summary>
    GridContainer Grid(params Control[] children);

    /// <summary>
    /// Creates a <see cref="global::Godot.GridContainer"/>, sets its column count, applies optional initialization, and adds the provided children.
    /// </summary>
    GridContainer Grid(int columns, Action<GridContainer>? init = null, params Control[] children);

    /// <summary>
    /// Creates a <see cref="global::Godot.GridContainer"/>, sets its column count, and adds the provided children.
    /// </summary>
    GridContainer Grid(int columns, params Control[] children);

    /// <summary>
    /// Creates a <see cref="global::Godot.PanelContainer"/>, applies optional initialization, and adds the provided children.
    /// </summary>
    PanelContainer Panel(Action<PanelContainer>? init = null, params Control[] children);

    /// <summary>
    /// Creates a <see cref="global::Godot.PanelContainer"/> and adds the provided children.
    /// </summary>
    PanelContainer Panel(params Control[] children);

    /// <summary>
    /// Creates a <see cref="global::Godot.ScrollContainer"/>, applies optional initialization, and adds the provided children.
    /// </summary>
    ScrollContainer Scroll(Action<ScrollContainer>? init = null, params Control[] children);

    /// <summary>
    /// Creates a <see cref="global::Godot.ScrollContainer"/> and adds the provided children.
    /// </summary>
    ScrollContainer Scroll(params Control[] children);

    /// <summary>
    /// Creates a <see cref="global::Godot.HSplitContainer"/>, applies optional initialization, and adds the provided children.
    /// </summary>
    HSplitContainer HSplit(Action<HSplitContainer>? init = null, params Control[] children);

    /// <summary>
    /// Creates a <see cref="global::Godot.HSplitContainer"/> and adds the provided children.
    /// </summary>
    HSplitContainer HSplit(params Control[] children);

    /// <summary>
    /// Creates a <see cref="global::Godot.VSplitContainer"/>, applies optional initialization, and adds the provided children.
    /// </summary>
    VSplitContainer VSplit(Action<VSplitContainer>? init = null, params Control[] children);

    /// <summary>
    /// Creates a <see cref="global::Godot.VSplitContainer"/> and adds the provided children.
    /// </summary>
    VSplitContainer VSplit(params Control[] children);

    /// <summary>
    /// Creates a <see cref="global::Godot.CenterContainer"/>, applies optional initialization, and adds the provided children.
    /// </summary>
    CenterContainer Center(Action<CenterContainer>? init = null, params Control[] children);

    /// <summary>
    /// Creates a <see cref="global::Godot.CenterContainer"/> and adds the provided children.
    /// </summary>
    CenterContainer Center(params Control[] children);

    /// <summary>
    /// Creates a <see cref="global::Godot.HSeparator"/> and optionally initializes it.
    /// </summary>
    HSeparator HSeparator(Action<HSeparator>? init = null);

    /// <summary>
    /// Creates a <see cref="global::Godot.VSeparator"/> and optionally initializes it.
    /// </summary>
    VSeparator VSeparator(Action<VSeparator>? init = null);

    /// <summary>
    /// Creates a <see cref="global::Godot.Label"/> and optionally sets its text and custom initialization.
    /// </summary>
    Label Label(string? text = null, Action<Label>? init = null);

    /// <summary>
    /// Creates a <see cref="global::Godot.Label"/> with custom initialization.
    /// </summary>
    Label Label(Action<Label>? init);

    /// <summary>
    /// Creates a <see cref="global::Godot.RichTextLabel"/> and optionally sets its text and custom initialization.
    /// </summary>
    RichTextLabel RichTextLabel(string? text = null, Action<RichTextLabel>? init = null);

    /// <summary>
    /// Creates a <see cref="global::Godot.RichTextLabel"/> with custom initialization.
    /// </summary>
    RichTextLabel RichTextLabel(Action<RichTextLabel>? init);

    /// <summary>
    /// Creates a <see cref="global::Godot.Button"/>, optionally sets its text, and binds a pressed callback.
    /// </summary>
    Button Button(string? text = null, Action? onPressed = null, Action<Button>? init = null);

    /// <summary>
    /// Creates a <see cref="global::Godot.Button"/> and binds a pressed callback.
    /// </summary>
    Button Button(Action? onPressed, Action<Button>? init = null);

    /// <summary>
    /// Creates a <see cref="global::Godot.Button"/> with custom initialization.
    /// </summary>
    Button Button(Action<Button>? init);

    /// <summary>
    /// Creates a <see cref="global::Godot.TextureButton"/>, optionally sets its normal texture, and binds a pressed callback.
    /// </summary>
    TextureButton TextureButton(Texture2D? textureNormal = null, Action? onPressed = null, Action<TextureButton>? init = null);

    /// <summary>
    /// Creates a <see cref="global::Godot.TextureButton"/> and binds a pressed callback.
    /// </summary>
    TextureButton TextureButton(Action? onPressed, Action<TextureButton>? init = null);

    /// <summary>
    /// Creates a <see cref="global::Godot.TextureButton"/> with custom initialization.
    /// </summary>
    TextureButton TextureButton(Action<TextureButton>? init);

    /// <summary>
    /// Creates a <see cref="global::Godot.CheckButton"/>, optionally sets its text and value, and binds a toggled callback.
    /// </summary>
    CheckButton CheckButton(string? text = null, bool isPressed = false, Action<bool>? onToggled = null, Action<CheckButton>? init = null);

    /// <summary>
    /// Creates a <see cref="global::Godot.ColorPickerButton"/>, sets its initial color, and binds a color changed callback.
    /// </summary>
    ColorPickerButton ColorPickerButton(Color color, Action<Color>? onColorChanged = null, Action<ColorPickerButton>? init = null);

    /// <summary>
    /// Creates a <see cref="global::Godot.ColorPickerButton"/> with custom initialization.
    /// </summary>
    ColorPickerButton ColorPickerButton(Action<ColorPickerButton>? init);

    /// <summary>
    /// Creates a <see cref="global::Godot.CheckButton"/> with an initial value and toggled callback.
    /// </summary>
    CheckButton CheckButton(bool isPressed, Action<bool>? onToggled = null, Action<CheckButton>? init = null);

    /// <summary>
    /// Creates a <see cref="global::Godot.LineEdit"/>, optionally sets its initial text, and binds a text changed callback.
    /// </summary>
    LineEdit LineEdit(string? text = null, Action<string>? onTextChanged = null, Action<LineEdit>? init = null);

    /// <summary>
    /// Creates a <see cref="global::Godot.LineEdit"/> and binds a text changed callback.
    /// </summary>
    LineEdit LineEdit(Action<string>? onTextChanged, Action<LineEdit>? init = null);

    /// <summary>
    /// Creates a <see cref="global::Godot.TextEdit"/>, optionally sets its initial text, and binds a text changed callback.
    /// </summary>
    TextEdit TextEdit(string? text = null, Action<string>? onTextChanged = null, Action<TextEdit>? init = null);

    /// <summary>
    /// Creates a <see cref="global::Godot.TextEdit"/> and binds a text changed callback.
    /// </summary>
    TextEdit TextEdit(Action<string>? onTextChanged, Action<TextEdit>? init = null);

    /// <summary>
    /// Creates a <see cref="global::Godot.TextureRect"/>, optionally sets its texture, and applies sensible display defaults.
    /// </summary>
    TextureRect TextureRect(Texture2D? texture = null, Action<TextureRect>? init = null);

    /// <summary>
    /// Creates a floating-point <see cref="global::Godot.SpinBox"/> and binds a value changed callback.
    /// </summary>
    SpinBox FloatSpinBox(float value, float min, float max, float step, Action<float>? onValueChanged = null, Action<SpinBox>? init = null);

    /// <summary>
    /// Creates an integer <see cref="global::Godot.SpinBox"/> and binds a value changed callback.
    /// </summary>
    SpinBox IntSpinBox(int value, int min, int max, int step, Action<int>? onValueChanged = null, Action<SpinBox>? init = null);

    /// <summary>
    /// Creates a <see cref="global::Godot.HSlider"/>, sets its range, and binds a value changed callback.
    /// </summary>
    HSlider HSlider(double value, double min, double max, double step = 1, Action<double>? onValueChanged = null, Action<HSlider>? init = null);

    /// <summary>
    /// Creates a <see cref="global::Godot.VSlider"/>, sets its range, and binds a value changed callback.
    /// </summary>
    VSlider VSlider(double value, double min, double max, double step = 1, Action<double>? onValueChanged = null, Action<VSlider>? init = null);

    /// <summary>
    /// Creates a <see cref="global::Godot.ProgressBar"/> and sets its current value and range.
    /// </summary>
    ProgressBar ProgressBar(double value = 0, double min = 0, double max = 100, Action<ProgressBar>? init = null);

    /// <summary>
    /// Creates a <see cref="Godot.Tree"/> and optionally initializes it.
    /// </summary>
    Tree Tree(Action<Tree>? init = null);

    /// <summary>
    /// Creates a <see cref="Godot.Tree"/>, sets its column count, and optionally initializes it.
    /// </summary>
    Tree Tree(int columns, Action<Tree>? init = null);

    /// <summary>
    /// Creates a <see cref="Godot.Tree"/> from column titles and enables title visibility automatically.
    /// </summary>
    Tree Tree(ReadOnlySpan<string> columnTitles, Action<Tree>? init = null);

    /// <summary>
    /// Creates a <see cref="Godot.Tree"/>, sets its column count and titles, and enables title visibility automatically.
    /// </summary>
    Tree Tree(int columns, ReadOnlySpan<string> columnTitles, Action<Tree>? init = null);

    /// <summary>
    /// Creates the root <see cref="global::Godot.TreeItem"/> for a <see cref="Godot.Tree"/>.
    /// </summary>
    TreeItem TreeRoot(Tree tree, Action<TreeItem>? init = null);

    /// <summary>
    /// Creates a child <see cref="global::Godot.TreeItem"/> under the specified parent item.
    /// </summary>
    TreeItem TreeItem(TreeItem parent, Action<TreeItem>? init = null);

    /// <summary>
    /// Creates a <see cref="global::Godot.TreeItem"/> in the specified <see cref="Godot.Tree"/>, optionally under a parent item.
    /// </summary>
    TreeItem TreeItem(Tree tree, TreeItem? parent = null, Action<TreeItem>? init = null);

    /// <summary>
    /// Creates an <see cref="Godot.ItemList"/> and optionally initializes it.
    /// </summary>
    ItemList ItemList(Action<ItemList>? init = null);

    /// <summary>
    /// Creates an <see cref="Godot.ItemList"/> from string items.
    /// </summary>
    ItemList ItemList(ReadOnlySpan<string> items, Action<string>? onSelected = null, Action<ItemList>? init = null);

    /// <summary>
    /// Creates an <see cref="Godot.ItemList"/> from typed items.
    /// </summary>
    ItemList ItemList<T>(ReadOnlySpan<T> items, Action<T>? onSelected = null, Action<ItemList>? init = null, Func<T, string>? textSelector = null);

    /// <summary>
    /// Creates an <see cref="global::Godot.OptionButton"/> from typed options.
    /// </summary>
    OptionButton OptionButton<T>(T? selectedValue, ReadOnlySpan<T> options, Action<T>? onSelected = null, Action<OptionButton>? init = null, Func<T, string>? textSelector = null);

    /// <summary>
    /// Creates an <see cref="global::Godot.OptionButton"/> from typed options without a preselected value.
    /// </summary>
    OptionButton OptionButton<T>(ReadOnlySpan<T> options, Action<T>? onSelected = null, Action<OptionButton>? init = null, Func<T, string>? textSelector = null);

    /// <summary>
    /// Creates an <see cref="global::Godot.OptionButton"/> from typed options using a selected index.
    /// </summary>
    OptionButton OptionButton<T>(int selectedIndex, ReadOnlySpan<T> options, Action<T>? onSelected = null, Action<OptionButton>? init = null, Func<T, string>? textSelector = null);

    /// <summary>
    /// Creates an <see cref="global::Godot.OptionButton"/> from typed values paired with display text.
    /// </summary>
    OptionButton OptionButton<T>(T? selectedValue, ReadOnlySpan<KeyValuePair<T, string>> options, Action<T>? onSelected = null, Action<OptionButton>? init = null);

    /// <summary>
    /// Creates an <see cref="global::Godot.OptionButton"/> from typed values paired with display text using a selected index.
    /// </summary>
    OptionButton OptionButton<T>(int selectedIndex, ReadOnlySpan<KeyValuePair<T, string>> options, Action<T>? onSelected = null, Action<OptionButton>? init = null);

    /// <summary>
    /// Creates an <see cref="global::Godot.OptionButton"/> from display text paired with optional icons.
    /// </summary>
    OptionButton OptionButton(string? selectedText, ReadOnlySpan<(string Text, Texture2D? Icon)> options, Action<string>? onSelected = null, Action<OptionButton>? init = null);

    /// <summary>
    /// Creates an <see cref="global::Godot.OptionButton"/> from display text paired with optional icons without a preselected value.
    /// </summary>
    OptionButton OptionButton(ReadOnlySpan<(string Text, Texture2D? Icon)> options, Action<string>? onSelected = null, Action<OptionButton>? init = null);

    /// <summary>
    /// Creates an <see cref="global::Godot.OptionButton"/> from display text paired with optional icons using a selected index.
    /// </summary>
    OptionButton OptionButton(int selectedIndex, ReadOnlySpan<(string Text, Texture2D? Icon)> options, Action<string>? onSelected = null, Action<OptionButton>? init = null);
}

/// <summary>
/// Exposes the runtime input registration APIs of a panel created through <see cref="PanelBuilder"/>.
/// </summary>
public interface IRuntimePanelInputScope
{
    /// <inheritdoc cref="UIPanelBaseCore.RegisterAnyKeyInput(Action{InputEvent}, InputActionPhase?)"/>
    void RegisterAnyKeyInput(Action<InputEvent> callback, InputActionPhase? actionPhase = null);

    /// <inheritdoc cref="UIPanelBaseCore.RegisterInput(StringName, Action{InputEvent}, InputActionPhase?)"/>
    void RegisterInput(StringName inputName, Action<InputEvent> callback, InputActionPhase? actionPhase = null);

    /// <inheritdoc cref="UIPanelBaseCore.RemoveInput(StringName, Action{InputEvent}, InputActionPhase?)"/>
    void RemoveInput(StringName inputName, Action<InputEvent> callback, InputActionPhase? actionPhase = null);

    /// <inheritdoc cref="UIPanelBaseCore.RegisterInputCancel(Action, InputActionPhase?)"/>
    void RegisterInputCancel(Action callback, InputActionPhase? actionPhase = null);

    /// <inheritdoc cref="UIPanelBaseCore.RemoveInputCancel(Action, InputActionPhase?)"/>
    void RemoveInputCancel(Action callback, InputActionPhase? actionPhase = null);

    /// <inheritdoc cref="UIPanelBaseCore.RegisterEchoedInput(StringName, Action)"/>
    void RegisterEchoedInput(StringName inputName, Action callback);

    /// <inheritdoc cref="UIPanelBaseCore.RemoveEchoedInput(StringName, Action)"/>
    void RemoveEchoedInput(StringName inputName, Action callback);

    /// <inheritdoc cref="UIPanelBaseCore.RegisterInputAxis(StringName, StringName, Action{float}, CompositeInputActionState)"/>
    void RegisterInputAxis(StringName negativeInputName, StringName positiveInputName, Action<float> callback, CompositeInputActionState actionState);

    /// <inheritdoc cref="UIPanelBaseCore.RemoveInputAxis(StringName, StringName, Action{float}, CompositeInputActionState)"/>
    void RemoveInputAxis(StringName negativeInputName, StringName positiveInputName, Action<float> callback, CompositeInputActionState actionState);

    /// <inheritdoc cref="UIPanelBaseCore.RegisterInputVector(StringName, StringName, StringName, StringName, Action{Vector2}, CompositeInputActionState)"/>
    void RegisterInputVector(StringName upInputName, StringName downInputName, StringName leftInputName, StringName rightInputName, Action<Vector2> callback, CompositeInputActionState actionState);

    /// <inheritdoc cref="UIPanelBaseCore.RemoveInputVector(StringName, StringName, StringName, StringName, Action{Vector2}, CompositeInputActionState)"/>
    void RemoveInputVector(StringName upInputName, StringName downInputName, StringName leftInputName, StringName rightInputName, Action<Vector2> callback, CompositeInputActionState actionState);
}

/// <summary>
/// Represents the runtime handle for a panel without open or close arguments.
/// </summary>
public interface IRuntimePanelHandle : IRuntimePanelInputScope
{
    /// <inheritdoc cref="UIPanel.ClosePanel"/>
    void Close();

    /// <inheritdoc cref="UIPanelBaseCore.PanelCloseToken"/>
    CancellationToken PanelCloseToken { get; }

    /// <inheritdoc cref="UIPanelBaseCore.PanelOpenTweenFinishToken"/>
    CancellationToken PanelOpenTweenFinishToken { get; }

    /// <inheritdoc cref="UIPanelBaseCore.PanelCloseTweenFinishToken"/>
    CancellationToken PanelCloseTweenFinishToken { get; }

    /// <inheritdoc cref="UIPanel.EnableCloseWithCancelKey"/>
    void EnableCancelToClose(InputActionPhase? actionPhase = null);

    /// <inheritdoc cref="UIPanel.DisableCloseWithCancelKey"/>
    void DisableCancelToClose();
}

/// <summary>
/// Represents the runtime handle for a panel that accepts one open argument.
/// </summary>
/// <typeparam name="TOpenArg">The type of the open argument passed when the panel is opened.</typeparam>
public interface IRuntimePanelArg1Handle<TOpenArg> : IRuntimePanelHandle
{
    /// <inheritdoc cref="UIPanelBase{TOpenArg, Empty}.OpenArg"/>
    TOpenArg? CurrentOpenArg { get; }
}

/// <summary>
/// Represents the runtime handle for a panel that accepts an open argument and returns a close argument.
/// </summary>
/// <typeparam name="TOpenArg">The type of the open argument passed when the panel is opened.</typeparam>
/// <typeparam name="TCloseArg">The type of the close argument supplied when the panel is closed.</typeparam>
public interface IRuntimePanelArg2Handle<TOpenArg, TCloseArg> : IRuntimePanelInputScope
{
    /// <inheritdoc cref="UIPanelBase{TOpenArg, TCloseArg}.OpenArg"/>
    TOpenArg? CurrentOpenArg { get; }

    /// <inheritdoc cref="UIPanelBaseCore.PanelCloseToken"/>
    CancellationToken PanelCloseToken { get; }

    /// <inheritdoc cref="UIPanelBaseCore.PanelOpenTweenFinishToken"/>
    CancellationToken PanelOpenTweenFinishToken { get; }

    /// <inheritdoc cref="UIPanelBaseCore.PanelCloseTweenFinishToken"/>
    CancellationToken PanelCloseTweenFinishToken { get; }

    /// <inheritdoc cref="UIPanelArg2{TOpenArg, TCloseArg}.ClosePanel(TCloseArg)"/>
    void Close(TCloseArg closeArg);
}

/// <summary>
/// Defines the builder contract for creating a runtime panel without open or close arguments.
/// </summary>
public interface IPanelBuilder : IPanelBuilderDsl
{
    /// <summary>
    /// Occurs after the runtime panel instance has been initialized.
    /// </summary>
    event Action<IRuntimePanelHandle>? OnPanelInitialized;

    /// <summary>
    /// Occurs when the runtime panel is opened.
    /// </summary>
    event Action<IRuntimePanelHandle>? OnPanelOpen;

    /// <summary>
    /// Occurs when the runtime panel is closed through the framework lifecycle.
    /// </summary>
    event Action<IRuntimePanelHandle>? OnPanelClose;

    /// <summary>
    /// Occurs when the runtime panel is closed externally instead of by its own handle.
    /// </summary>
    event Action<IRuntimePanelHandle>? OnPanelExternalClose;

    /// <summary>
    /// Occurs immediately before the runtime panel is deleted.
    /// </summary>
    event Action<IRuntimePanelHandle>? OnPanelPredelete;

    /// <summary>
    /// Occurs whenever Godot sends a notification to the runtime panel.
    /// </summary>
    event Action<IRuntimePanelHandle, int>? OnPanelNotification;
}

/// <summary>
/// Defines the builder contract for creating a runtime panel with one open argument.
/// </summary>
/// <typeparam name="TOpenArg">The type of the open argument passed when the panel is opened.</typeparam>
public interface IPanelBuilderArg1<TOpenArg> : IPanelBuilderDsl
{
    /// <summary>
    /// Occurs after the runtime panel instance has been initialized.
    /// </summary>
    event Action<IRuntimePanelArg1Handle<TOpenArg>>? OnPanelInitialized;

    /// <summary>
    /// Occurs when the runtime panel is opened.
    /// </summary>
    event Action<IRuntimePanelArg1Handle<TOpenArg>>? OnPanelOpen;

    /// <summary>
    /// Occurs when the runtime panel is closed through the framework lifecycle.
    /// </summary>
    event Action<IRuntimePanelArg1Handle<TOpenArg>>? OnPanelClose;

    /// <summary>
    /// Occurs when the runtime panel is closed externally instead of by its own handle.
    /// </summary>
    event Action<IRuntimePanelArg1Handle<TOpenArg>>? OnPanelExternalClose;

    /// <summary>
    /// Occurs immediately before the runtime panel is deleted.
    /// </summary>
    event Action<IRuntimePanelArg1Handle<TOpenArg>>? OnPanelPredelete;

    /// <summary>
    /// Occurs whenever Godot sends a notification to the runtime panel.
    /// </summary>
    event Action<IRuntimePanelArg1Handle<TOpenArg>, int>? OnPanelNotification;
}

/// <summary>
/// Defines the builder contract for creating a runtime panel with open and close arguments.
/// </summary>
/// <typeparam name="TOpenArg">The type of the open argument passed when the panel is opened.</typeparam>
/// <typeparam name="TCloseArg">The type of the close argument supplied when the panel is closed.</typeparam>
public interface IPanelBuilderArg2<TOpenArg, TCloseArg> : IPanelBuilderDsl
{
    /// <summary>
    /// Occurs after the runtime panel instance has been initialized.
    /// </summary>
    event Action<IRuntimePanelArg2Handle<TOpenArg, TCloseArg>>? OnPanelInitialized;

    /// <summary>
    /// Occurs when the runtime panel is opened.
    /// </summary>
    event Action<IRuntimePanelArg2Handle<TOpenArg, TCloseArg>>? OnPanelOpen;

    /// <summary>
    /// Occurs when the runtime panel is closed through the framework lifecycle.
    /// </summary>
    event Action<IRuntimePanelArg2Handle<TOpenArg, TCloseArg>>? OnPanelClose;

    /// <summary>
    /// Occurs when the runtime panel is closed externally instead of by its own handle.
    /// </summary>
    event Action<IRuntimePanelArg2Handle<TOpenArg, TCloseArg>>? OnPanelExternalClose;

    /// <summary>
    /// Occurs immediately before the runtime panel is deleted.
    /// </summary>
    event Action<IRuntimePanelArg2Handle<TOpenArg, TCloseArg>>? OnPanelPredelete;

    /// <summary>
    /// Occurs whenever Godot sends a notification to the runtime panel.
    /// </summary>
    event Action<IRuntimePanelArg2Handle<TOpenArg, TCloseArg>, int>? OnPanelNotification;
}

/// <summary>
/// Creates runtime panels from declarative builder callbacks.
/// </summary>
public static class PanelBuilder
{
    /// <summary>
    /// Creates a runtime panel without open or close arguments.
    /// </summary>
    /// <param name="build">A callback that uses the builder DSL to create the root control tree.</param>
    /// <returns>A runtime <see cref="UIPanel"/> instance configured with the generated control tree.</returns>
    public static UIPanel CreatePanel(Func<IPanelBuilder, Control> build)
    {
        ArgumentNullException.ThrowIfNull(build);
        var builder = new PanelBuilderContext();
        var root = build(builder);
        ArgumentNullException.ThrowIfNull(root);
        return PanelManager.CreateRuntimePanel<RuntimePanel>(panel => builder.ConfigurePanel(panel, root));
    }

    /// <summary>
    /// Creates a runtime panel that accepts one open argument.
    /// </summary>
    /// <typeparam name="TOpenArg">The type of the open argument passed when the panel is opened.</typeparam>
    /// <param name="build">A callback that uses the builder DSL to create the root control tree.</param>
    /// <returns>A runtime <see cref="UIPanelArg1{TOpenArg}"/> instance configured with the generated control tree.</returns>
    public static UIPanelArg1<TOpenArg> CreatePanelArg1<TOpenArg>(Func<IPanelBuilderArg1<TOpenArg>, Control> build)
    {
        ArgumentNullException.ThrowIfNull(build);
        var builder = new PanelBuilderContextArg1<TOpenArg>();
        var root = build(builder);
        ArgumentNullException.ThrowIfNull(root);
        return PanelManager.CreateRuntimePanel<RuntimePanelArg1<TOpenArg>>(panel => builder.ConfigurePanel(panel, root));
    }

    /// <summary>
    /// Creates a runtime panel that accepts an open argument and returns a close argument.
    /// </summary>
    /// <typeparam name="TOpenArg">The type of the open argument passed when the panel is opened.</typeparam>
    /// <typeparam name="TCloseArg">The type of the close argument supplied when the panel is closed.</typeparam>
    /// <param name="build">A callback that uses the builder DSL to create the root control tree.</param>
    /// <returns>A runtime <see cref="UIPanelArg2{TOpenArg, TCloseArg}"/> instance configured with the generated control tree.</returns>
    public static UIPanelArg2<TOpenArg, TCloseArg> CreatePanelArg2<TOpenArg, TCloseArg>(Func<IPanelBuilderArg2<TOpenArg, TCloseArg>, Control> build)
    {
        ArgumentNullException.ThrowIfNull(build);
        var builder = new PanelBuilderContextArg2<TOpenArg, TCloseArg>();
        var root = build(builder);
        ArgumentNullException.ThrowIfNull(root);
        return PanelManager.CreateRuntimePanel<RuntimePanelArg2<TOpenArg, TCloseArg>>(panel => builder.ConfigurePanel(panel, root));
    }

    private abstract class PanelBuilderDslBase : IPanelBuilderDsl
    {
        public T LateInit<T>() => default!;

        private static T Use<T>(Action<T>? init, params Control[] children) where T : Control, new()
        {
            var control = new T();
            ArgumentNullException.ThrowIfNull(control);
            init?.Invoke(control);
            AddChildren(control, children);
            return control;
        }

        public MarginContainer MarginContainer(Action<MarginContainer>? init = null, params Control[] children) => Use(init, children);
        public MarginContainer MarginContainer(params Control[] children) => Use<MarginContainer>(null, children);
        public VBoxContainer VBox(Action<VBoxContainer>? init = null, params Control[] children) => Use(init, children);
        public VBoxContainer VBox(params Control[] children) => Use<VBoxContainer>(null, children);
        public HBoxContainer HBox(Action<HBoxContainer>? init = null, params Control[] children) => Use(init, children);
        public HBoxContainer HBox(params Control[] children) => Use<HBoxContainer>(null, children);
        public HFlowContainer HFlow(Action<HFlowContainer>? init = null, params Control[] children) => Use(init, children);
        public HFlowContainer HFlow(params Control[] children) => Use<HFlowContainer>(null, children);
        public VFlowContainer VFlow(Action<VFlowContainer>? init = null, params Control[] children) => Use(init, children);
        public VFlowContainer VFlow(params Control[] children) => Use<VFlowContainer>(null, children);
        public GridContainer Grid(Action<GridContainer>? init = null, params Control[] children) => Use(init, children);
        public GridContainer Grid(params Control[] children) => Use<GridContainer>(null, children);
        public GridContainer Grid(int columns, Action<GridContainer>? init = null, params Control[] children) =>
            Use<GridContainer>(grid =>
            {
                grid.Columns = columns;
                init?.Invoke(grid);
            }, children);
        public GridContainer Grid(int columns, params Control[] children) => Grid(columns, null, children);
        public PanelContainer Panel(Action<PanelContainer>? init = null, params Control[] children) => Use(init, children);
        public PanelContainer Panel(params Control[] children) => Use<PanelContainer>(null, children);
        public ScrollContainer Scroll(Action<ScrollContainer>? init = null, params Control[] children) => Use(init, children);
        public ScrollContainer Scroll(params Control[] children) => Use<ScrollContainer>(null, children);
        public HSplitContainer HSplit(Action<HSplitContainer>? init = null, params Control[] children) => Use(init, children);
        public HSplitContainer HSplit(params Control[] children) => Use<HSplitContainer>(null, children);
        public VSplitContainer VSplit(Action<VSplitContainer>? init = null, params Control[] children) => Use(init, children);
        public VSplitContainer VSplit(params Control[] children) => Use<VSplitContainer>(null, children);
        public CenterContainer Center(Action<CenterContainer>? init = null, params Control[] children) => Use(init, children);
        public CenterContainer Center(params Control[] children) => Use<CenterContainer>(null, children);
        public HSeparator HSeparator(Action<HSeparator>? init = null) => Create(init);
        public VSeparator VSeparator(Action<VSeparator>? init = null) => Create(init);

        public Label Label(string? text = null, Action<Label>? init = null)
        {
            var label = new Label();
            if (text is not null) label.Text = text;
            init?.Invoke(label);
            return label;
        }

        public Label Label(Action<Label>? init) => Label(null, init);

        public RichTextLabel RichTextLabel(string? text = null, Action<RichTextLabel>? init = null)
        {
            var richTextLabel = new RichTextLabel();
            if (text is not null) richTextLabel.Text = text;
            init?.Invoke(richTextLabel);
            return richTextLabel;
        }

        public RichTextLabel RichTextLabel(Action<RichTextLabel>? init) => RichTextLabel(null, init);

        public Button Button(string? text = null, Action? onPressed = null, Action<Button>? init = null)
        {
            var button = new Button();
            if (text is not null) button.Text = text;
            if (onPressed is not null) button.Pressed += onPressed;
            init?.Invoke(button);
            return button;
        }

        public Button Button(Action? onPressed, Action<Button>? init = null) => Button(null, onPressed, init);
        public Button Button(Action<Button>? init) => Button(null, null, init);

        public TextureButton TextureButton(Texture2D? textureNormal = null, Action? onPressed = null, Action<TextureButton>? init = null)
        {
            var textureButton = new TextureButton();
            if (textureNormal is not null) textureButton.TextureNormal = textureNormal;
            if (onPressed is not null) textureButton.Pressed += onPressed;
            init?.Invoke(textureButton);
            return textureButton;
        }

        public TextureButton TextureButton(Action? onPressed, Action<TextureButton>? init = null) => TextureButton(null, onPressed, init);
        public TextureButton TextureButton(Action<TextureButton>? init) => TextureButton(null, null, init);

        public CheckButton CheckButton(string? text = null, bool isPressed = false, Action<bool>? onToggled = null, Action<CheckButton>? init = null)
        {
            var button = new CheckButton();
            if (text is not null) button.Text = text;
            button.ButtonPressed = isPressed;
            if (onToggled is not null) button.Toggled += toggled => onToggled(toggled);
            init?.Invoke(button);
            return button;
        }

        public CheckButton CheckButton(bool isPressed, Action<bool>? onToggled = null, Action<CheckButton>? init = null) =>
            CheckButton(null, isPressed, onToggled, init);

        public ColorPickerButton ColorPickerButton(Color color, Action<Color>? onColorChanged = null, Action<ColorPickerButton>? init = null)
        {
            var colorPickerButton = new ColorPickerButton { Color = color };
            if (onColorChanged is not null) colorPickerButton.ColorChanged += selectedColor => onColorChanged(selectedColor);
            init?.Invoke(colorPickerButton);
            return colorPickerButton;
        }

        public ColorPickerButton ColorPickerButton(Action<ColorPickerButton>? init) => ColorPickerButton(Colors.White, null, init);

        public LineEdit LineEdit(string? text = null, Action<string>? onTextChanged = null, Action<LineEdit>? init = null)
        {
            var lineEdit = new LineEdit();
            if (text is not null) lineEdit.Text = text;
            if (onTextChanged is not null) lineEdit.TextChanged += newText => onTextChanged(newText);
            init?.Invoke(lineEdit);
            return lineEdit;
        }

        public LineEdit LineEdit(Action<string>? onTextChanged, Action<LineEdit>? init = null) => LineEdit(null, onTextChanged, init);

        public TextEdit TextEdit(string? text = null, Action<string>? onTextChanged = null, Action<TextEdit>? init = null)
        {
            var textEdit = new TextEdit();
            if (text is not null) textEdit.Text = text;
            if (onTextChanged is not null) textEdit.TextChanged += () => onTextChanged(textEdit.Text);
            init?.Invoke(textEdit);
            return textEdit;
        }

        public TextEdit TextEdit(Action<string>? onTextChanged, Action<TextEdit>? init = null) => TextEdit(null, onTextChanged, init);

        public TextureRect TextureRect(Texture2D? texture = null, Action<TextureRect>? init = null)
        {
            var textureRect = new TextureRect
            {
                StretchMode = Godot.TextureRect.StretchModeEnum.KeepAspectCentered,
                ExpandMode = Godot.TextureRect.ExpandModeEnum.IgnoreSize,
            };
            if (texture is not null) textureRect.Texture = texture;
            init?.Invoke(textureRect);
            return textureRect;
        }

        public SpinBox FloatSpinBox(float value, float min, float max, float step, Action<float>? onValueChanged = null, Action<SpinBox>? init = null)
        {
            var spinBox = new SpinBox
            {
                Value = value,
                MinValue = min,
                MaxValue = max,
                Step = step,
            };
            if (onValueChanged is not null) spinBox.ValueChanged += newValue => onValueChanged((float)newValue);
            init?.Invoke(spinBox);
            return spinBox;
        }

        public SpinBox IntSpinBox(int value, int min, int max, int step, Action<int>? onValueChanged = null, Action<SpinBox>? init = null)
        {
            var spinBox = new SpinBox
            {
                Value = value,
                MinValue = min,
                MaxValue = max,
                Step = step,
                Rounded = true,
            };
            if (onValueChanged is not null) spinBox.ValueChanged += newValue => onValueChanged(Mathf.RoundToInt(newValue));
            init?.Invoke(spinBox);
            return spinBox;
        }

        public HSlider HSlider(double value, double min, double max, double step = 1, Action<double>? onValueChanged = null, Action<HSlider>? init = null)
        {
            var slider = new HSlider
            {
                Value = value,
                MinValue = min,
                MaxValue = max,
                Step = step,
            };
            if (onValueChanged is not null) slider.ValueChanged += newValue => onValueChanged(newValue);
            init?.Invoke(slider);
            return slider;
        }

        public VSlider VSlider(double value, double min, double max, double step = 1, Action<double>? onValueChanged = null, Action<VSlider>? init = null)
        {
            var slider = new VSlider
            {
                Value = value,
                MinValue = min,
                MaxValue = max,
                Step = step,
            };
            if (onValueChanged is not null) slider.ValueChanged += newValue => onValueChanged(newValue);
            init?.Invoke(slider);
            return slider;
        }

        public ProgressBar ProgressBar(double value = 0, double min = 0, double max = 100, Action<ProgressBar>? init = null)
        {
            var progressBar = new ProgressBar
            {
                Value = value,
                MinValue = min,
                MaxValue = max,
            };
            init?.Invoke(progressBar);
            return progressBar;
        }

        public Tree Tree(Action<Tree>? init = null)
        {
            var tree = new Tree();
            init?.Invoke(tree);
            return tree;
        }

        public Tree Tree(int columns, Action<Tree>? init = null)
        {
            var tree = new Tree { Columns = columns };
            init?.Invoke(tree);
            return tree;
        }

        public Tree Tree(ReadOnlySpan<string> columnTitles, Action<Tree>? init = null) => Tree(columnTitles.Length, columnTitles, init);

        public Tree Tree(int columns, ReadOnlySpan<string> columnTitles, Action<Tree>? init = null)
        {
            var tree = new Tree { Columns = columns, ColumnTitlesVisible = columnTitles.Length > 0 };
            for (var index = 0; index < columnTitles.Length && index < columns; index++)
                tree.SetColumnTitle(index, columnTitles[index]);
            init?.Invoke(tree);
            return tree;
        }

        public TreeItem TreeRoot(Tree tree, Action<TreeItem>? init = null)
        {
            ArgumentNullException.ThrowIfNull(tree);
            var item = tree.CreateItem();
            init?.Invoke(item);
            return item;
        }

        public TreeItem TreeItem(TreeItem parent, Action<TreeItem>? init = null)
        {
            ArgumentNullException.ThrowIfNull(parent);
            var item = parent.CreateChild();
            init?.Invoke(item);
            return item;
        }

        public TreeItem TreeItem(Tree tree, TreeItem? parent = null, Action<TreeItem>? init = null)
        {
            ArgumentNullException.ThrowIfNull(tree);
            var item = tree.CreateItem(parent);
            init?.Invoke(item);
            return item;
        }

        public ItemList ItemList(Action<ItemList>? init = null) => Create(init);

        public ItemList ItemList(ReadOnlySpan<string> items, Action<string>? onSelected = null, Action<ItemList>? init = null) =>
            ItemList<string>(items, onSelected, init);

        public ItemList ItemList<T>(ReadOnlySpan<T> items, Action<T>? onSelected = null, Action<ItemList>? init = null, Func<T, string>? textSelector = null)
        {
            var itemList = new ItemList();
            textSelector ??= value => value?.ToString() ?? string.Empty;

            for (var index = 0; index < items.Length; index++)
                itemList.AddItem(textSelector(items[index]));

            if (onSelected is not null)
            {
                var itemArray = items.ToArray();
                itemList.ItemSelected += index =>
                {
                    if (index < 0 || index >= itemArray.Length) return;
                    onSelected(itemArray[index]);
                };
            }

            init?.Invoke(itemList);
            return itemList;
        }

        public OptionButton OptionButton<T>(T? selectedValue, ReadOnlySpan<T> options, Action<T>? onSelected = null, Action<OptionButton>? init = null, Func<T, string>? textSelector = null)
        {
            var optionButton = new OptionButton();
            textSelector ??= value => value?.ToString() ?? string.Empty;

            var selectedIndex = -1;
            for (var index = 0; index < options.Length; index++)
            {
                var option = options[index];
                optionButton.AddItem(textSelector(option));
                if (selectedIndex >= 0) continue;
                if (Equals(option, selectedValue)) selectedIndex = index;
            }

            if (selectedIndex >= 0) optionButton.Selected = selectedIndex;
            if (onSelected is not null)
            {
                var optionArray = options.ToArray();
                optionButton.ItemSelected += index =>
                {
                    if (index < 0 || index >= optionArray.Length) return;
                    onSelected(optionArray[index]);
                };
            }

            init?.Invoke(optionButton);
            return optionButton;
        }

        public OptionButton OptionButton<T>(ReadOnlySpan<T> options, Action<T>? onSelected = null, Action<OptionButton>? init = null, Func<T, string>? textSelector = null) =>
            OptionButton(default(T), options, onSelected, init, textSelector);

        public OptionButton OptionButton<T>(int selectedIndex, ReadOnlySpan<T> options, Action<T>? onSelected = null, Action<OptionButton>? init = null, Func<T, string>? textSelector = null)
        {
            if (selectedIndex < 0 || selectedIndex >= options.Length)
                return OptionButton(default(T), options, onSelected, init, textSelector);
            return OptionButton(options[selectedIndex], options, onSelected, init, textSelector);
        }

        public OptionButton OptionButton<T>(T? selectedValue, ReadOnlySpan<KeyValuePair<T, string>> options, Action<T>? onSelected = null, Action<OptionButton>? init = null)
        {
            var optionButton = new OptionButton();
            var selectedIndex = -1;

            for (var index = 0; index < options.Length; index++)
            {
                var option = options[index];
                optionButton.AddItem(option.Value);
                if (selectedIndex >= 0) continue;
                if (Equals(option.Key, selectedValue)) selectedIndex = index;
            }

            if (selectedIndex >= 0) optionButton.Selected = selectedIndex;
            if (onSelected is not null)
            {
                var optionArray = options.ToArray();
                optionButton.ItemSelected += index =>
                {
                    if (index < 0 || index >= optionArray.Length) return;
                    onSelected(optionArray[index].Key);
                };
            }

            init?.Invoke(optionButton);
            return optionButton;
        }

        public OptionButton OptionButton<T>(int selectedIndex, ReadOnlySpan<KeyValuePair<T, string>> options, Action<T>? onSelected = null, Action<OptionButton>? init = null)
        {
            if (selectedIndex < 0 || selectedIndex >= options.Length)
                return OptionButton(default(T), options, onSelected, init);
            return OptionButton(options[selectedIndex].Key, options, onSelected, init);
        }

        public OptionButton OptionButton(string? selectedText, ReadOnlySpan<(string Text, Texture2D? Icon)> options, Action<string>? onSelected = null, Action<OptionButton>? init = null)
        {
            var optionButton = new OptionButton();
            var selectedIndex = -1;

            for (var index = 0; index < options.Length; index++)
            {
                var option = options[index];
                if (option.Icon is not null) optionButton.AddIconItem(option.Icon, option.Text);
                else optionButton.AddItem(option.Text);

                if (selectedIndex >= 0) continue;
                if (option.Text == selectedText) selectedIndex = index;
            }

            if (selectedIndex >= 0) optionButton.Selected = selectedIndex;
            if (onSelected is not null)
            {
                var optionArray = options.ToArray();
                optionButton.ItemSelected += index =>
                {
                    if (index < 0 || index >= optionArray.Length) return;
                    onSelected(optionArray[index].Text);
                };
            }

            init?.Invoke(optionButton);
            return optionButton;
        }

        public OptionButton OptionButton(ReadOnlySpan<(string Text, Texture2D? Icon)> options, Action<string>? onSelected = null, Action<OptionButton>? init = null) =>
            OptionButton(null, options, onSelected, init);

        public OptionButton OptionButton(int selectedIndex, ReadOnlySpan<(string Text, Texture2D? Icon)> options, Action<string>? onSelected = null, Action<OptionButton>? init = null)
        {
            if (selectedIndex < 0 || selectedIndex >= options.Length)
                return OptionButton(null, options, onSelected, init);
            return OptionButton(options[selectedIndex].Text, options, onSelected, init);
        }

        private static T Create<T>(Action<T>? init = null) where T : Control, new()
        {
            var control = new T();
            init?.Invoke(control);
            return control;
        }

        private static void AddChildren(Control parent, ReadOnlySpan<Control> children)
        {
            foreach (var child in children)
            {
                if (child.GetParent() == parent) continue;
                parent.AddChild(child);
            }
        }
    }

    private sealed class PanelBuilderContext : PanelBuilderDslBase, IPanelBuilder
    {
        public event Action<IRuntimePanelHandle>? OnPanelInitialized;
        public event Action<IRuntimePanelHandle>? OnPanelOpen;
        public event Action<IRuntimePanelHandle>? OnPanelClose;
        public event Action<IRuntimePanelHandle>? OnPanelExternalClose;
        public event Action<IRuntimePanelHandle>? OnPanelPredelete;
        public event Action<IRuntimePanelHandle, int>? OnPanelNotification;

        public void ConfigurePanel(RuntimePanel panel, Control root) =>
            panel.Configure(root, OnPanelInitialized, OnPanelOpen, OnPanelClose, OnPanelExternalClose, OnPanelPredelete, OnPanelNotification);
    }

    private sealed class PanelBuilderContextArg1<TOpenArg> : PanelBuilderDslBase, IPanelBuilderArg1<TOpenArg>
    {
        public event Action<IRuntimePanelArg1Handle<TOpenArg>>? OnPanelInitialized;
        public event Action<IRuntimePanelArg1Handle<TOpenArg>>? OnPanelOpen;
        public event Action<IRuntimePanelArg1Handle<TOpenArg>>? OnPanelClose;
        public event Action<IRuntimePanelArg1Handle<TOpenArg>>? OnPanelExternalClose;
        public event Action<IRuntimePanelArg1Handle<TOpenArg>>? OnPanelPredelete;
        public event Action<IRuntimePanelArg1Handle<TOpenArg>, int>? OnPanelNotification;

        public void ConfigurePanel(RuntimePanelArg1<TOpenArg> panel, Control root) =>
            panel.Configure(root, OnPanelInitialized, OnPanelOpen, OnPanelClose, OnPanelExternalClose, OnPanelPredelete, OnPanelNotification);
    }

    private sealed class PanelBuilderContextArg2<TOpenArg, TCloseArg> : PanelBuilderDslBase, IPanelBuilderArg2<TOpenArg, TCloseArg>
    {
        public event Action<IRuntimePanelArg2Handle<TOpenArg, TCloseArg>>? OnPanelInitialized;
        public event Action<IRuntimePanelArg2Handle<TOpenArg, TCloseArg>>? OnPanelOpen;
        public event Action<IRuntimePanelArg2Handle<TOpenArg, TCloseArg>>? OnPanelClose;
        public event Action<IRuntimePanelArg2Handle<TOpenArg, TCloseArg>>? OnPanelExternalClose;
        public event Action<IRuntimePanelArg2Handle<TOpenArg, TCloseArg>>? OnPanelPredelete;
        public event Action<IRuntimePanelArg2Handle<TOpenArg, TCloseArg>, int>? OnPanelNotification;

        public void ConfigurePanel(RuntimePanelArg2<TOpenArg, TCloseArg> panel, Control root) =>
            panel.Configure(root, OnPanelInitialized, OnPanelOpen, OnPanelClose, OnPanelExternalClose, OnPanelPredelete, OnPanelNotification);
    }
}