# GD Panel Framework

[English](https://github.com/Delsin-Yu/GDPanelFramework/blob/main/README.md)

[![GitHub Release](https://img.shields.io/github/v/release/Delsin-Yu/GDPanelFramework)](https://github.com/Delsin-Yu/GDPanelFramework/releases/latest) [![Stars](https://img.shields.io/github/stars/Delsin-Yu/GDPanelFramework?color=brightgreen)](https://github.com/Delsin-Yu/GDPanelFramework/stargazers) [![License](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/Delsin-Yu/GDPanelFramework/blob/main/LICENSE)

## 引言

支持包括 `mono (.Net)` 模块的 `Godot 4.5+`
***GD Panel Framework***是为`Godot 4`设计的UI管理系统，旨在提供灵活、基于面板、单焦点、多输入设备兼容的UI编程体验。

该框架将`用户交互`的集合总结为`UIPanel`，其中包括以下内容的组合：

1. `控件`，如`按钮`、`标签`和`容器`。
2. `输入`，一组开发人员定义的，与该面板绑定的输入操作。

这些`用户交互`是`面板范围的`，这意味着它们只有在`面板`处于活动状态时才保持活动状态；这简化了维护大量离散`控件`和`全局输入动作`的工作量，并允许开发人员专注于编程游戏逻辑（*而不是收集和开关`控件`或将更多`if`添加到全局`_Input`方法中*）。

## 安装

由于 Godot 对外部程序集的支持不佳，`GD Panel Framework` 现在不再作为 Nuget 程序包，而是使用 `zip/add` 来分发。

1. 确保项目的 C# 语言版本至少为 `<LangVersion>12</LangVersion>`。
2. 从最新的 [release](https://github.com/Delsin-Yu/GDPanelFramework/releases) 页面下载 `GDPanelFramework.zip`。
3. 解压下载的文件，并且将其中的 `addons` 目录放置在项目的根目录（`res://`）中。
4. 在开始使用任何 API 之前，调用 `PanelManager.Initialize()`。

这个插件只包含用于运行时使用的源代码文件；所以你不需要在 `插件` 窗口中启用任何项。

---

<!-- START doctoc generated TOC please keep comment here to allow auto update -->
<!-- DON'T EDIT THIS SECTION, INSTEAD RE-RUN doctoc TO UPDATE -->
## Table of Contents

- [API简单用法](#api%E7%AE%80%E5%8D%95%E7%94%A8%E6%B3%95)
  - [创建一个简单的面板](#%E5%88%9B%E5%BB%BA%E4%B8%80%E4%B8%AA%E7%AE%80%E5%8D%95%E7%9A%84%E9%9D%A2%E6%9D%BF)
  - [创建一个支持传参及返回值的面板](#%E5%88%9B%E5%BB%BA%E4%B8%80%E4%B8%AA%E6%94%AF%E6%8C%81%E4%BC%A0%E5%8F%82%E5%8F%8A%E8%BF%94%E5%9B%9E%E5%80%BC%E7%9A%84%E9%9D%A2%E6%9D%BF)
- [框架文档](#%E6%A1%86%E6%9E%B6%E6%96%87%E6%A1%A3)
  - [框架概念](#%E6%A1%86%E6%9E%B6%E6%A6%82%E5%BF%B5)
  - [`UIPanel` 类型](#uipanel-%E7%B1%BB%E5%9E%8B)
    - [实例化一个面板](#%E5%AE%9E%E4%BE%8B%E5%8C%96%E4%B8%80%E4%B8%AA%E9%9D%A2%E6%9D%BF)
    - [开启一个面板](#%E5%BC%80%E5%90%AF%E4%B8%80%E4%B8%AA%E9%9D%A2%E6%9D%BF)
    - [关闭一个面板](#%E5%85%B3%E9%97%AD%E4%B8%80%E4%B8%AA%E9%9D%A2%E6%9D%BF)
    - [输入绑定/路由](#%E8%BE%93%E5%85%A5%E7%BB%91%E5%AE%9A%E8%B7%AF%E7%94%B1)
      - [输入注册](#%E8%BE%93%E5%85%A5%E6%B3%A8%E5%86%8C)
        - [基本用法](#%E5%9F%BA%E6%9C%AC%E7%94%A8%E6%B3%95)
        - [默认输入触发阶段](#%E9%BB%98%E8%AE%A4%E8%BE%93%E5%85%A5%E8%A7%A6%E5%8F%91%E9%98%B6%E6%AE%B5)
        - [变体: `RegisterAnyKeyInput`](#%E5%8F%98%E4%BD%93-registeranykeyinput)
        - [变体: `RegisterInputToggle`](#%E5%8F%98%E4%BD%93-registerinputtoggle)
        - [变体: `RegisterEchoedInput`/`RemoveEchoedInput`](#%E5%8F%98%E4%BD%93-registerechoedinputremoveechoedinput)
        - [变体: `RegisterInputCancel`/`RemoveInputCancel`/`ToggleInputCancel`](#%E5%8F%98%E4%BD%93-registerinputcancelremoveinputcanceltoggleinputcancel)
        - [变体: `EnableCloseWithCancelKey` and `DisableCloseWithCancelKey`](#%E5%8F%98%E4%BD%93-enableclosewithcancelkey-and-disableclosewithcancelkey)
        - [变体: `RegisterInputAxis`/`RemoveInputAxis`/`ToggleInputAxis`](#%E5%8F%98%E4%BD%93-registerinputaxisremoveinputaxistoggleinputaxis)
        - [变体: `RegisterInputVector`/`RemoveInputVector`/`ToggleInputVector`](#%E5%8F%98%E4%BD%93-registerinputvectorremoveinputvectortoggleinputvector)
      - [BuiltinInputNames类](#builtininputnames%E7%B1%BB)
      - [全局输入监听器](#%E5%85%A8%E5%B1%80%E8%BE%93%E5%85%A5%E7%9B%91%E5%90%AC%E5%99%A8)
    - [面板栈/Panel Stack](#%E9%9D%A2%E6%9D%BF%E6%A0%88panel-stack)
    - [框架级别缓存](#%E6%A1%86%E6%9E%B6%E7%BA%A7%E5%88%AB%E7%BC%93%E5%AD%98)
    - [作用域面板缓冲](#%E4%BD%9C%E7%94%A8%E5%9F%9F%E9%9D%A2%E6%9D%BF%E7%BC%93%E5%86%B2)
    - [面板事件方法概述](#%E9%9D%A2%E6%9D%BF%E4%BA%8B%E4%BB%B6%E6%96%B9%E6%B3%95%E6%A6%82%E8%BF%B0)
    - [配置上一个面板的视觉行为](#%E9%85%8D%E7%BD%AE%E4%B8%8A%E4%B8%80%E4%B8%AA%E9%9D%A2%E6%9D%BF%E7%9A%84%E8%A7%86%E8%A7%89%E8%A1%8C%E4%B8%BA)
  - [`UIPanelArg1` 和 `UIPanelArg2` 类型](#uipanelarg1-%E5%92%8C-uipanelarg2-%E7%B1%BB%E5%9E%8B)
  - [面板容器管理](#%E9%9D%A2%E6%9D%BF%E5%AE%B9%E5%99%A8%E7%AE%A1%E7%90%86)
    - [使用`async/await`风格的API时请注意](#%E4%BD%BF%E7%94%A8asyncawait%E9%A3%8E%E6%A0%BC%E7%9A%84api%E6%97%B6%E8%AF%B7%E6%B3%A8%E6%84%8F)
    - [使用此框架时请注意](#%E4%BD%BF%E7%94%A8%E6%AD%A4%E6%A1%86%E6%9E%B6%E6%97%B6%E8%AF%B7%E6%B3%A8%E6%84%8F)
  - [`面板过渡控制器/PanelTweener`](#%E9%9D%A2%E6%9D%BF%E8%BF%87%E6%B8%A1%E6%8E%A7%E5%88%B6%E5%99%A8paneltweener)
    - [内置过渡器](#%E5%86%85%E7%BD%AE%E8%BF%87%E6%B8%A1%E5%99%A8)
    - [自定义过渡器](#%E8%87%AA%E5%AE%9A%E4%B9%89%E8%BF%87%E6%B8%A1%E5%99%A8)
- [运行时 PanelBuilder DSL](#%E8%BF%90%E8%A1%8C%E6%97%B6-panelbuilder-dsl)
  - [这个 DSL 是拿来做什么的](#%E8%BF%99%E4%B8%AA-dsl-%E6%98%AF%E6%8B%BF%E6%9D%A5%E5%81%9A%E4%BB%80%E4%B9%88%E7%9A%84)
  - [构建运行时面板](#%E6%9E%84%E5%BB%BA%E8%BF%90%E8%A1%8C%E6%97%B6%E9%9D%A2%E6%9D%BF)
  - [如何理解 builder 回调](#%E5%A6%82%E4%BD%95%E7%90%86%E8%A7%A3-builder-%E5%9B%9E%E8%B0%83)
  - [生命周期与状态组织](#%E7%94%9F%E5%91%BD%E5%91%A8%E6%9C%9F%E4%B8%8E%E7%8A%B6%E6%80%81%E7%BB%84%E7%BB%87)
  - [这套 DSL 目前能覆盖什么](#%E8%BF%99%E5%A5%97-dsl-%E7%9B%AE%E5%89%8D%E8%83%BD%E8%A6%86%E7%9B%96%E4%BB%80%E4%B9%88)
  - [运行时面板特有的能力](#%E8%BF%90%E8%A1%8C%E6%97%B6%E9%9D%A2%E6%9D%BF%E7%89%B9%E6%9C%89%E7%9A%84%E8%83%BD%E5%8A%9B)
  - [当前限制](#%E5%BD%93%E5%89%8D%E9%99%90%E5%88%B6)

<!-- END doctoc generated TOC please keep comment here to allow auto update -->

---

## API简单用法

### 创建一个简单的面板

你可以在Godot编辑器中运行 ***[RunMe_Example00.tscn](https://github.com/Delsin-Yu/GDPanelFramework.Test/blob/main/Examples/00/RunMe_Example00.tscn)***。

```csharp
using Godot;
using GodotTask;

namespace GDPanelFramework.Examples;

/// <summary>
/// 创建并打开面板的引导程序脚本。
/// </summary>
public partial class Example00_Main : Node
{
    /// <summary>
    /// 以PackedScene引用的面板Prefab
    /// </summary>
    [Export] private PackedScene _panelPrefab;

    /// <summary>
    /// 自游戏开始一帧后执行主逻辑。
    /// 这是框架将其面板根添加到场景树中所必需的。
    /// </summary>
    public override void _Ready() =>
        GDTask.NextFrame().ContinueWith(OnReady);

    private void OnReady()
    {
        _panelPrefab
            .CreatePanel<Example00_MyPanel>() // 这个扩展方法告诉框架创建或重用这个面板的实例。
            .OpenPanel( // 这个方法告诉框架打开面板。
                onPanelCloseCallback: // 当面板本身调用ClosePanel()时，当该面板关闭时，会调用此委托。
                () => GetTree().Quit() // 关闭此面板后终止应用程序。
            );
    }
}
```

```csharp
using GDPanelFramework.Panels;
using Godot;

namespace GDPanelFramework.Examples;

/// <summary>
/// 将此脚本附加到控件以使其成为“UIPanel”。
/// </summary>
public partial class Example00_MyPanel : UIPanel
{
    // 这三个字段是在Godot编辑器中通过 inspector 分配的。
    [Export] private Label _text;
    [Export] private Button _updateButton;
    [Export] private Button _closeButton;

    // 存储单击次数。
    private int _clickCount = 0;

    /// <summary>
    /// 当创建该面板实例时由框架调用，
    /// 一个实例只能被创建一次。
    /// </summary>
    protected override void _OnPanelInitialize()
    {
        _updateButton.Pressed += OnClick; // 当_updateButton被按下时调用OnClick。
        _closeButton.Pressed += ClosePanel; // 当_closeButton被按下时关闭当前面板。
    }

    /// <summary>
    /// 被注册到<see cref="_updateButton"/>中.
    /// </summary>
    private void OnClick()
    {
        _clickCount++;
        _text.Text = $"Clicked {_clickCount} time(s).";
    }

    /// <summary>
    /// 当面板的此实例打开时由框架调用。
    /// 该框架支持自动面板缓存，因此，你可以在一个面板的实例被关闭并缓存后重新打开它。
    /// </summary>
    protected override void _OnPanelOpen()
    {
        _text.Text = "Hello World";
        _updateButton.GrabFocus();
    }
}
```

### 创建一个支持传参及返回值的面板

你可以在Godot编辑器中运行 ***[RunMe_Example01.tscn](https://github.com/Delsin-Yu/GDPanelFramework.Test/blob/main/Examples/01/RunMe_Example01.tscn)***。

```csharp
using Godot;
using GodotTask;

namespace GDPanelFramework.Examples;

/// <summary>
/// 创建并打开面板的引导程序脚本。
/// </summary>
public partial class Example01_Main : Node
{
    /// <summary>
    /// 以PackedScene引用的面板Prefab
    /// </summary>
    [Export] private PackedScene _panelPrefab;

    /// <summary>
    /// 自游戏开始一帧后执行主逻辑。
    /// 这是框架将其面板根添加到场景树中所必需的。
    /// </summary>
    public override void _Ready() =>
        GDTask.NextFrame().ContinueWith(OnReady);

    private void OnReady()
    {
        _panelPrefab
            .CreatePanel<Example01_MyPanel>() // 这个扩展方法告诉框架创建或重用这个面板的实例。
            .OpenPanel( // // 这个方法告诉框架打开面板。
                "Hello World!", // 将参数传递给面板。
                onPanelCloseCallback: // 当面板本身调用ClosePanel()时，当该面板关闭时，会调用此委托。
                result => // 在此面板关闭时打印返回值，并终止应用程序。
                {
                    GD.Print($"Clicked {result.Unwrap()} time(s) before closed.");
                    GetTree().Quit();
                }
            );
    }
}
```

```csharp
using GDPanelFramework.Panels;
using Godot;

namespace GDPanelFramework.Examples;

/// <summary>
/// 将此脚本附加到控件以使其成为“UIPanelArg”。
/// </summary>
public partial class Example01_MyPanel : UIPanelArg2<string, string>
{
    // 这三个字段是在Godot编辑器中通过 inspector 分配的。
    [Export] private Label _text;
    [Export] private Button _updateButton;
    [Export] private Button _closeButton;

    // 存储单击次数。
    private int _clickCount = 0;

    /// <summary>
    /// 当创建该面板实例时由框架调用，
    /// 一个实例只能被创建一次。
    /// </summary>
    protected override void _OnPanelInitialize()
    {
        _updateButton.Pressed += OnClick; // 当_updateButton被按下时调用OnClick。
        _closeButton.Pressed += () => ClosePanel(_clickCount.ToString()); // 当_closeButton被按下时关闭当前面板。
    }

    /// <summary>
    /// 被注册到<see cref="_updateButton"/>中.
    /// </summary>
    private void OnClick()
    {
        _clickCount++;
        _text.Text = $"Clicked {_clickCount} time(s).";
    }

    /// <summary>
    /// 当面板的此实例打开时由框架调用。
    /// 该框架支持自动面板缓存，因此，你可以在一个面板的实例被关闭并缓存后重新打开它。
    /// </summary>
    protected override void _OnPanelOpen(string openArg)
    {
        _text.Text = openArg;
        _updateButton.GrabFocus();
    }
}
```

## 框架文档

### 框架概念

在诸如游戏之类的典型GUI应用程序中，`基于面板/页面的控制流`是一种常见的做法。

当从`主逻辑`打开面板时，开发人员可能希望面板执行其自己的`面板逻辑`，并在完成时`自动关闭`，然后继续执行`主要逻辑`（如文件对话框或警告）。

这种设计`将控制流从主逻辑转移到面板，完成后面板将控制流返回主逻辑`简化了面板编程的工作流程，处理了管理ui焦点的要求，在设计与游戏板兼容的游戏时至关重要。

该框架通过`基于面板堆栈的控制管理`、`异步/回调样式的API`和`面板输入绑定`设计来实现此实践。

### `UIPanel` 类型

`UIPanel/面板`是该框架的基本组件，它为简化编程工作流程提供了`面板级输入绑定`、`子控件访问管理`功能，还支持可配置的`面板过渡控制器`，用于界面打开/关闭的动态美术需求。

- `面板级输入绑定`功能允许开发人员为此面板注册/取消注册一组输入绑定，注册的输入在面板级被隔离开，这样当面板处于非活动状态时，它们就不会影响到其他逻辑。

- 当面板激活/停用时，`子控件访问管理`功能会自动禁用/恢复每个子控件的`FocusMode`和`MouseFilter`属性，从而防止不需要的UI导航和鼠标交互“泄漏到”当前激活的面板之外。

#### 实例化一个面板

通过调用`CreatePanel<TPanel>`以从给定的PackedScene实例化一个界面，和内置的`PackedScene.Instantiate`相比，此API会使用缓存，并且处理必要的界面初始化操作。

```csharp
// 在调用类中。
[Export] private PackedScene _panelPrefab;

// 在方法中。
var panelInstance = 
        _panelPrefab
            .CreatePanel<TypeOfScriptAttachedToThePanel>();
```

#### 开启一个面板

UIPanel有三种OpenPanel方法，每种方法都是为特定的编程风格设计的。

在异步方法中，`async/await`风格的打开方法返回 `PanelAwaitable` / `PanelAwaitable<TCloseArg>`，允许开发人员 `await` 一个面板的关闭。这些 awaitable 是`单次使用`的，行为类似 `ValueTask`；同时，`OpenPanelAsync` 现在还支持可选的 `CancellationToken`，用于从外部关闭面板。

```csharp
// 在异步方法中开启面板时。
await panelInstance.OpenPanelAsync();
GD.Print("The panel has closed!");

using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
await panelInstance.OpenPanelAsync(cancellationToken: cts.Token);
```

`回调风格`的打开方法允许开发人员提供一个委托，以便在面板关闭时得到通知。对于 `UIPanelArg2`，回调接收的是 `PanelResult<TCloseArg>`，因此你可以区分正常关闭和被取消令牌外部关闭这两种情况。

```csharp
// 开启面板时。
panelInstance
    .OpenPanel(
        onPanelCloseCallback: // 该lambda表达式在面板关闭时被调用。
           () => GD.Print("The panel has closed!")
    );

argPanelInstance
    .OpenPanel(
        10,
        onPanelCloseCallback: result =>
        {
            if (result.TryGetValue(out var value))
                GD.Print($"Returned: {value}");
            else
                GD.Print("The panel was closed by cancellation.");
        }
    );
```

`遗忘风格`的打开方法只打开面板，当面板关闭的时机不重要时，它很有用。

```csharp
// 开启面板时。
panelInstance.OpenPanel();
```

#### 关闭一个面板

在面板脚本中调用`ClosePanel()`将关闭打开的面板。默认情况下，此方法是`protected`，开发人员可以通过用公共方法包装此方法来提升可见性。

如果面板是被传入 `OpenPanel` / `OpenPanelAsync` 的 `cancellationToken` 关闭的，框架会调用 `_OnPanelExternalClose()`，而不是 `_OnPanelClose(...)`。

> 请注意，在关闭面板之前，必须先打开面板；此外，关闭不在面板堆栈顶部的面板被认为是一个错误，会使框架崩溃。

```csharp
// 在面板脚本中
protected override void _OnPanelOpen()
{
    // 在面板打开一帧后关闭面板。
    GDTask.NextFrame().ContinueWith(ClosePanel);
}
```

#### 输入绑定/路由

所有Godot输入事件都被`root/RootPanelViewport`截获，并直接分派到当前活动的面板。当面板`停用/激活`时，绑定到面板的一组输入会被`自动关闭或打开`。

##### 输入注册

###### 基本用法

在面板中调用`RegisterInput`可以将委托绑定到特定的输入事件，当面板释放时，已注册的委托将自动释放。

```csharp
// 在面板中
RegisterInput( // 向关联的inputName注册回调
    BuiltinInputNames.UIAccept, // 要关联的输入名称，此名称应与InputManager中的名称相对应。
    inputEvent => GD.Print(inputEvent.AsText()), // 要被关联的委托。
    InputActionPhase.Pressed // 要关注的输入状态。
);
```

###### 默认输入触发阶段

`RegisterInput`、`RemoveInput`、`ToggleInput` 以及相关辅助 API 的 `actionPhase` 参数现在都是可选的。当你省略该参数时，框架会使用 `PanelManager.DefaultInputRegistrationBehavior`，其默认值是 `InputRegistrationBehavior.Press`。

```csharp
PanelManager.DefaultInputRegistrationBehavior = PanelManager.InputRegistrationBehavior.Release;

RegisterInput(
    BuiltinInputNames.UIAccept,
    inputEvent => GD.Print($"Released: {inputEvent.AsText()}")
); // 使用全局默认触发阶段。
```

在某些需要解除委托绑定的情况下，开发者应调用`RemoveInput`并传入对应的注册信息。

> 请注意，在处理输入注销时，如果要正确注销`lambda表达式`，则必须在注册时`将lambda表达式分配给变量`并且`将该变量传递给API`。

```csharp
// 将此lambda表达式赋给变量。
Action<InputEvent> myDelegate = inputEvent => GD.Print(inputEvent.AsText());

// 将此回调注册到关联的inputName。
RegisterInput(BuiltinInputNames.UIAccept, myDelegate);
// 使用同样的信息来注销。
RemoveInput(BuiltinInputNames.UIAccept, myDelegate);
```

或者，您可以使用`ToggleInput`API。

```csharp
ToggleInput( // 这个api支持基于第一个bool参数的值在注册和注销之间切换。
    true, // 如设置为false的话就执行注销。
    BuiltinInputNames.UIAccept,
    inputEvent => GD.Print(inputEvent.AsText()) // 此lambda表达式由编译器缓存。
);
```

为了实现某些目的，输入注册API还有其他几种变体。

###### 变体: `RegisterAnyKeyInput`

将一个委托绑定到当前活动面板接收到的任意按键/按钮类输入。

```csharp
RegisterAnyKeyInput(inputEvent => GD.Print($"Any key: {inputEvent.AsText()}"));
```

###### 变体: `RegisterInputToggle`

将一个委托绑定为按下时收到 `true`，抬起时收到 `false`。你可以绑定单个输入名，也可以绑定一组输入名，以观察它们的组合按下状态。

```csharp
RegisterInputToggle(BuiltinInputNames.UIAccept, pressed => GD.Print($"Accept: {pressed}"));

RegisterInputToggle(
    [BuiltinInputNames.UILeft, BuiltinInputNames.UIRight],
    pressed => GD.Print($"Any horizontal input pressed: {pressed}")
);
```

###### 变体: `RegisterEchoedInput`/`RemoveEchoedInput`

将一个委托绑定到持续按住时的重复触发输入，效果类似键盘长按连发。首次按下会立即触发，之后会在 `InputEchoing.InitialDelay` 后开始重复，并以 `InputEchoing.RepeatInterval` 继续触发。

```csharp
InputEchoing.InitialDelay = 250;
InputEchoing.RepeatInterval = 100;

Action moveSelection = () => GD.Print("Move selection");

RegisterEchoedInput(BuiltinInputNames.UIDown, moveSelection);
RemoveEchoedInput(BuiltinInputNames.UIDown, moveSelection);
```

###### 变体: `RegisterInputCancel`/`RemoveInputCancel`/`ToggleInputCancel`

将委托直接与`ui_cancel`输入事件关联，开发人员可以通过修改`PanelManager.UICancelActionName`来变更这个值。

```csharp
RegisterInputCancel(() => GD.Print("Canceled!"));

Action myDelegate = () => GD.Print("Canceled!");
RegisterInputCancel(myDelegate);
RemoveInputCancel(myDelegate);

ToggleInputCancel(true, () => GD.Print("Canceled!"));
```

###### 变体: `EnableCloseWithCancelKey` and `DisableCloseWithCancelKey`

`UIPanel`附带了两个额外的输入绑定API：`EnableCloseWithCancelKey`和`DisableCloseWithCancelKey`，调用`EnableCloseWithCancelKey`允许玩家使用`ui_cancel`（`PanelManager.UICancelActionName`）关闭当前面板，调用`DisableCloseWithCancelKey`会取消此行为。

###### 变体: `RegisterInputAxis`/`RemoveInputAxis`/`ToggleInputAxis`

将委托与两个输入的组合相关联，类似于`Input.GetAxis`的行为。

```csharp
RegisterInputAxis(
    BuiltinInputNames.UILeft,
    BuiltinInputNames.UIRight,
    value => GD.Print(value),
    CompositeInputActionState.Update // Start, End
);

Action<float> myDelegate = value => GD.Print(value);
RegisterInputAxis(
    BuiltinInputNames.UILeft,
    BuiltinInputNames.UIRight,
    myDelegate,
    CompositeInputActionState.Update
);
RemoveInputAxis(
    BuiltinInputNames.UILeft,
    BuiltinInputNames.UIRight,
    myDelegate,
    CompositeInputActionState.Update
);

ToggleInputAxis(
    true,
    BuiltinInputNames.UILeft,
    BuiltinInputNames.UIRight,
    value => GD.Print(value),
    CompositeInputActionState.Update
);
```

###### 变体: `RegisterInputVector`/`RemoveInputVector`/`ToggleInputVector`

将委托与四个输入的组合相关联，类似于`Input.GetVector`的行为。

```csharp
RegisterInputVector(
    BuiltinInputNames.UIUp,
    BuiltinInputNames.UIDown,
    BuiltinInputNames.UILeft,
    BuiltinInputNames.UIRight,
    value => GD.Print(value),
    CompositeInputActionState.Update // Start, End
);

Action<Vector2> myDelegate = value => GD.Print(value);
RegisterInputVector(
    BuiltinInputNames.UIUp,
    BuiltinInputNames.UIDown,
    BuiltinInputNames.UILeft,
    BuiltinInputNames.UIRight,
    myDelegate,
    CompositeInputActionState.Update
);
RemoveInputVector(
    BuiltinInputNames.UIUp,
    BuiltinInputNames.UIDown,
    BuiltinInputNames.UILeft,
    BuiltinInputNames.UIRight,
    myDelegate,
    CompositeInputActionState.Update
);

ToggleInputVector(
    true,
    BuiltinInputNames.UIUp,
    BuiltinInputNames.UIDown,
    BuiltinInputNames.UILeft,
    BuiltinInputNames.UIRight,
    value => GD.Print(value),
    CompositeInputActionState.Update
);
```

##### BuiltinInputNames类

Godot提供了一个列表的内置ui输入事件，开发人员可以从`BuiltinInputNames`类访问这些输入事件的名称。

##### 全局输入监听器

除了活动面板继续接收面板级输入路由之外，框架现在还支持通过 `IGlobalInputListener` 广播每一个被 `PanelManager` 处理到的输入事件。

```csharp
public partial class MyGlobalInputLogger : Node, IGlobalInputListener
{
    public void OnGlobalInput(InputEvent inputEvent)
    {
        GD.Print($"Global input: {inputEvent.AsText()}");
    }
}

PanelManager.AddGlobalInputListener(this);
PanelManager.RemoveGlobalInputListener(this);
```

#### 面板栈/Panel Stack

`面板栈/Panel Stack`用于维护打开的面板的顺序，当打开面板时，框架会检视顶部面板的面板堆栈，禁用其下的每个控件（并缓存它们之前的打开状态），并将此新实例推送到堆栈。当关闭顶部面板时，框架将其从面板堆栈中弹出，并重新激活其下方面板的所有控件，它还将焦点恢复为该面板变为非活动状态之前的最后一个选定的项目。

以下示例显示了以下操作一系列操作时面板栈的状态：

```mermaid
timeline
   无面板
   打开 主菜单 : 主菜单 (激活)
   打开 设置菜单 : 设置菜单 (激活) : 主菜单 (睡眠)
   打开 确认设置菜单 : 确认设置菜单 (激活) : 设置菜单 (睡眠): 主菜单 (睡眠)
   关闭 确认设置菜单 : 设置菜单 (重新激活) : 主菜单 (睡眠)
   关闭 设置菜单 : 主菜单 (重新激活)
   关闭 主菜单l

```

#### 框架级别缓存

在某些情况下，面板在设计中就需要`频繁打开和关闭`*（例如某些游戏中的库存面板）*，在这种情况下，每次实例化面板并在关闭时删除它可能会带来较高负担。为了解决这个性能问题，该框架会`自动缓存面板`，您可以`在每次打开/关闭界面时进行配置`。

在创建面板时，通过指定`createPolicy`，您可以选择强制框架`创建面板的新实例`（`CreatePolicy.ForceCreate`），或者让框架尽可能`重用缓存的实例（默认值）`（`CreatePolicy.TryReuse`），当然，如果没有现有缓存，无论如何都会创建一个新实例。

```csharp
// 在创建面板时
var panelInstance = 
        _panelPrefab
            .CreatePanel<TPanel>(
                createPolicy: CreatePolicy.ForceCreate // CreatePolicy.TryReuse
            );
```

打开面板时，通过指定`closePolicy`，您可以选择指示框架在关闭的过渡动画完成后`删除此实例`（`ClosePolicy.Delete`），或者让框架`缓存此实例（默认值）`（`ClosePolicy.Cache`），并且下次在同一`PackedScene`上调用`CreatePanel`时重复使用。

```csharp
// 在开启面板时
panelInstance
    .OpenPanel(
        closePolicy: ClosePolicy.Delete // ClosePolicy.Cache
    );

```

#### 作用域面板缓冲

对于临时游戏状态或某些模态流程，你可以创建一个`作用域面板缓冲`。当该作用域激活时，被缓存的面板不会进入全局缓存，而是进入一个独立的缓冲区；当作用域结束时，这些被缓存的面板会被统一释放。

```csharp
var scopeToken = PanelManager.BeginScopedPanelManagement("Gameplay");

try
{
    var panel = _panelPrefab.CreatePanel<MyPanel>();
    panel.OpenPanel(closePolicy: ClosePolicy.Cache);
}
finally
{
    PanelManager.EndScopedPanelManagement(scopeToken);
}

```

#### 面板事件方法概述

在使用`UIPanel`时，某些方法会在面板的特定生存期内被框架调用，简要示意图可以总结如下。

```mermaid
---
title: UIPanel整个生命周期中的事件方法摘要
---
flowchart TD

id1["_OnPanelInitialize()"]
id2["_OnPanelOpen()"]
id3(["ClosePanel()"])
id4["_OnPanelClose()"]
id5["_OnPanelPredelete()"]
id6["_OnPanelNotification()"]


id0[["框架调用"]] -.-> id1
id1 -.->|框架调用|id2

subgraph 在面板被释放之前多次调用
id2 --> id3
id3 -.->|框架调用|id4
id4 -.->|框架调用|id2
end
id6 -.->|框架调用|id5
id7[["Godot调用"]] -.-> id6
```

1. 当调用`CreatePanel<TPanel>(PackedScene)`并导致新实例被创建时，在框架完成基本初始化后，会调用该实例的`_OnPanelInitialize`方法。在整个面板生命周期中，此方法只被调用一次；这意味着，如果`CreatePanel`重用了面板的实例，则不会再次调用此方法。
2. 当在未打开的面板实例上调用任何`OpenPanel`时，在框架完成打开此面板的准备工作后，将调用`_OnPanelOpen`方法。对于缓存的已关闭面板，当面板重新打开时，将再次调用`_OnPanelOpen`。
3. 当调用`ClosePanel`时，在框架完成关闭此面板的准备工作后，将调用`_OnPanelClose`方法。对于缓存的面板，当面板重新打开和关闭时，将再次调用`_OnPanelClose`。
4. `UIPanel`将`_Notification`引擎事件委托给`_OnPanelNotification`，并在必要时调用`_OnPanelPredelete`。

#### 配置上一个面板的视觉行为

打开新面板时，当前活动面板将变为`不可用（例如按钮将不再可点击或可聚焦）`，您还可以控制当前面板是保持可见还是隐藏。

在`OpenPanel`中将`previousPanelVisual`设置为`PreviousPanelVisual.Hidden`，将指示框架使用`上一个面板`的`PanelTweener`隐藏自身，否则上一个面板将`保持可见（默认）`（`PreviousPanelVisual.Visible`）。

```csharp
// 打开面板时。
panelInstance
    .OpenPanel( // 任何面板打开方法。
        previousPanelVisual: PreviousPanelVisual.Hidden // PreviousPanelVisual.Visible
    );
```

### `UIPanelArg1` 和 `UIPanelArg2` 类型

将参数传递给面板，以及从面板接收返回值，现在被拆分为两个基类：

- `UIPanelArg1<TOpenArg>`：适用于只需要开启参数、不需要返回值的面板。
- `UIPanelArg2<TOpenArg, TCloseArg>`：适用于既需要开启参数、又需要关闭返回值的面板。

如果两者都不需要，直接继承 `UIPanel`。如果你只需要其中一侧，但仍希望使用 `UIPanelArg2` 统一类型参数，可以用 `Empty` 作为占位类型。

```csharp
// MyArgumentPanel.cs
// 定义一个面板，该面板接受int作为开始参数，并使用string作为返回值。
public partial class MyArgumentPanel : UIPanelArg2<int, string>
{
    protected override void _OnPanelOpen(int openArg) // 从调用方传递的开启参数。
    {
        GD.Print($"Opened with argument: {openArg}");
        ClosePanel(openArg.ToString()); // ClosePanel方法需要提供一个返回值。
    }
}
```

与常规的 `UIPanel` 不同，`UIPanelArg1` 和 `UIPanelArg2` 的 `OpenPanel` 方法都会接收一个额外参数，并将其传递给 `_OnPanelOpen(TOpenArg)`。其中，`UIPanelArg2` 还会额外提供异步/回调风格的重载，用于接收关闭返回值。

```csharp
// 在调用类中。
[Export] private PackedScene _panelPrefab;

// 在调用方法中。
var argPanelInstance = _panelPrefab.CreatePanel<MyArgumentPanel>();

//异步/Await风格的开启方法。
string returnValue = await argPanelInstance.OpenPanelAsync(10); // returnValue为字符串的“10”。

// 回调/委托风格的开启方法。
argPanelInstance.OpenPanel(
    10,
    onPanelCloseCallback: result => GD.Print(result.Unwrap() == "10")
); // 面板正常关闭时打印true。
```

`UIPanelArg1` 是只需要开启参数时更简洁的选择：

```csharp
public partial class MyArgumentPanel : UIPanelArg1<int>
{
    protected override void _OnPanelOpen(int openArg)
    {
        GD.Print($"Opened with argument: {openArg}");
        ClosePanel();
    }
}
```

`UIPanelArg2` 同时支持`传递开启参数`和`返回值`。如果不需要其中一个功能，则可以使用 `Empty` 结构体作为占位符。

```csharp
// 定义一个不需要开启参数的界面（仅返回值）
public partial class MyArgumentPanel : UIPanelArg2<Empty, string>
{
    protected override void _OnPanelOpen(Empty _)
    {
        ClosePanel("Hello World!");
    }
}

// 在调用方法中。
argPanelInstance.OpenPanelAsync(Empty.Default);
```

```csharp
// 定义一个不需要返回值的界面（仅传递开启参数）
public partial class MyArgumentPanel : UIPanelArg2<int, Empty>
{
    protected override void _OnPanelOpen(int openArg)
    {
        GD.Print($"Opened with argument: {openArg}");
        ClosePanel(Empty.Default);
    }
}
```

### 面板容器管理

默认情况下，所有面板都在`root/RootPanelViewport/PanelRoot`下实例化，开发人员可以通过一系列API为打开的面板配置容器。

与`面板栈`类似，`面板容器栈`是为管理`面板容器`而设计的，开发人员可以使用`PanelManager.PushPanelContainer`将控件推送到面板容器堆栈，然后通过`PanelManager.PopPanelContainer`。与打开和关闭面板的限制相同，开发人员只允许弹出最上面的容器，然后才能弹出其他容器。

为了防止意外弹出容器，每个`PushPanelContainer`操作都由一个节点`授权`，也就是说，当推送新容器时，您需要提供一个`键`，并使用相同的`键`弹出容器。

```csharp
// 在调用类中。
[Export] private Control _myContainer;

// 在方法中。

// 此行之后的每个打开的面板都将实例化在/被移动到_myContainer下。
PanelManager.PushPanelContainer(this, _myContainer);

// 此行之后的每个打开的面板都将实例化在/被移动到默认容器下。
PanelManager.PopPanelContainer(this);
```

> 请注意，在使用自定义面板容器时，当`在将来会被删除的面板/自定义容器下生成面板`时要小心，虽然框架会尽力处理已删除的面板，但部分操作还是会无法避免的`删除具有活动面板的自定义面板容器`，这种行为可能会使框架崩溃，建议开发人员在弹出/删除该容器之前先确保自定义容器下的每个面板都已关闭。

### 使用`async/await`风格的API时请注意

`OpenPanelAsync` 现在返回 `PanelAwaitable` / `PanelAwaitable<T>`，这是一个专门用于跟踪面板生命周期的轻量级池化 awaitable。

这些 awaitable 都是单次使用的。对一个带取消令牌开启的面板执行 `await` 时，如果该令牌被取消，则会抛出 `OperationCanceledException`；而回调风格 API 则会收到 `PanelResult.None`。

此外，面板内部还可以通过 `PanelCancellationToken` 感知外部关闭请求，并通过 `_OnPanelExternalClose()` 为这种关闭路径编写自定义清理逻辑。

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

try
{
    var value = await argPanelInstance.OpenPanelAsync(10, cancellationToken: cts.Token);
    GD.Print($"Panel returned: {value}");
}
catch (OperationCanceledException)
{
    GD.Print("The panel was canceled externally.");
}
```

### 使用此框架时请注意

尽管我们已经采取了相应的预防措施，但仍有一些情况下，API的某些使用可能会不可避免地导致框架崩溃。

以下面板事件方法在`try ... catch 块`中执行，在这些方法的重写中抛出异常不会使框架崩溃。

- `_OnPanelInitialize`
- `_OnPanelOpen`
- `_OnPanelClose`
- `_OnPanelExternalClose`
- `_OnPanelPredelete`
- `_OnPanelNotification`
- 已注册的输入事件

以下用法 ***将会*** 导致框架崩溃：

- 通过指定不等于`脚本类型`的类型来创建面板。
- 打开一个未初始化的面板，通常代表该面板的实例不是通过`CreatePanel`API获取的。
- 打开一个已经开启的面板
- 关闭一个不是最后一次打开的面板的面板。
- 提供了无效的“CompositeInputActionState”枚举。
- 使用错误的`节点`来弹出被`另一个节点`推送的`面板容器`。
- 对一个已经使用过 `await` 的 `PanelAwaitable` 再次执行 `await`，或者在完成后继续访问它的 awaiter。
- 对尚未完成的 `PanelAwaitable` 调用 `GetResult()`。

### `面板过渡控制器/PanelTweener`

开发人员可以通过设置面板的`PanelTweener`属性来自定义面板的`打开/关闭时的视觉过渡行为`。或者，也可以通过修改`PanelManager.DefaultPanelTweener`来为全局所有面板设置默认的过渡器。

#### 内置过渡器

该框架提供了两个预配置的过渡器。

1. NonePanelTweener: 此过渡器在打开和关闭时立即隐藏和显示面板，它也是`PanelManager.DefaultPanelTweener`的默认值，您可以从`NonPanelTweener`访问它的全局实例。
2. FadePanelTweener: 此过渡器为面板打开和关闭执行透明度渐变过渡，在实例化之后，您可以通过访问其`FadeTime`属性来配置转换时间。

#### 自定义过渡器

开发者可以通过继承`IPanelTweenr`接口来自定义它们的转换效果。

```csharp
/// <summary>
/// 定义面板过渡的行为。
/// </summary>
public interface IPanelTweener
{
    /// <summary>
    /// 这将设置面板的默认视觉外观。
    /// </summary>
    /// <param name="panel">目标面板。</param>
    void Init(Control panel);
    
    /// <summary>
    /// 这个异步方法管理面板显示时的行为。
    /// </summary>
    /// <param name="panel">目标面板。</param>
    /// <param name="onFinish">当行为被认为完成时由方法调用，或者当行为被中断时根本不调用</param>
    void Show(Control panel, Action? onFinish);
    
    /// <summary>
    /// 这个异步方法管理面板隐藏时的行为。
    /// </summary>
    /// <param name="panel">目标面板。</param>
    /// <param name="onFinish">当行为被认为完成时由方法调用，或者当行为被中断时根本不调用</param>
    void Hide(Control panel, Action? onFinish);
}
```

## 运行时 PanelBuilder DSL

这一节专门说明由 `PanelBuilder` 提供的运行时面板 DSL，而不是上面基于 PackedScene 的常规面板用法。

### 这个 DSL 是拿来做什么的

`PanelBuilder` 本质上是 `UIPanel`、`UIPanelArg1` 和 `UIPanelArg2` 的运行时工厂。你不再需要先在编辑器里准备一个 `PackedScene`，而是直接在 C# 里描述控件树，再由框架把这棵树包进一个已经完成初始化的面板实例中。

需要注意的是，这并不是另一套独立的 UI 系统。通过 DSL 创建出来的仍然是框架里的标准面板，所以它依然遵循同一套面板栈、焦点管理、异步打开 API、回调式打开 API、输入路由以及关闭语义。DSL 改变的只是“面板内容如何被构造出来”，而不是“面板如何工作”。

它通常适合这些场景：

- UI 结构高度依赖运行时数据
- 面板更偏向工具、检查器、选择器或调试界面
- 你想快速组合一些临时对话框，而不是再维护很多独立场景资源
- 你希望让某些面板逻辑更贴近游戏逻辑或业务逻辑代码

如果一个界面高度依赖编辑器里的可视化排版、动画轨道、美术反复调整，或者复用大型视觉 prefab，那么 `PackedScene` 仍然通常是更合适的默认方案。

### 构建运行时面板

你也可以完全不依赖 PackedScene，直接在 C# 中构建一个简单面板。运行时代码构建的面板继续复用同一套打开和 await API，但当前只支持 `ClosePolicy.Delete`。

```csharp
using GDPanelFramework;
using Godot;

var panel = PanelBuilder.CreatePanel(builder =>
{
    var titleLabel = builder.Label("Runtime Panel", label => label.HorizontalAlignment = HorizontalAlignment.Center);
    var closeButton = builder.Button("Close");

    builder.OnPanelInitialized += runtimePanel =>
    {
        closeButton.Pressed += runtimePanel.Close;
        runtimePanel.RegisterInputCancel(runtimePanel.Close);
    };

    return builder.MarginContainer(
        builder.VBox(
            box => box.AddThemeConstantOverride("separation", 12),
            titleLabel,
            closeButton
        )
    );
});

await panel.OpenPanelAsync(closePolicy: ClosePolicy.Delete);

var argPanel = PanelBuilder.CreatePanelArg2<int, string>(builder =>
{
    var valueLabel = builder.LateInit<Label>();

    builder.OnPanelOpen += runtimePanel =>
    {
        valueLabel.Text = runtimePanel.CurrentOpenArg.ToString();
        runtimePanel.Close($"closed:{runtimePanel.CurrentOpenArg}");
    };

    return builder.MarginContainer(valueLabel = builder.Label());
});

var result = await argPanel.OpenPanelAsync(10, closePolicy: ClosePolicy.Delete);
```

这三个入口分别对应框架里的三类面板：

- `CreatePanel(...)` 对应 `UIPanel`
- `CreatePanelArg1<TOpenArg>(...)` 对应带一个打开参数的面板
- `CreatePanelArg2<TOpenArg, TCloseArg>(...)` 对应既接收打开参数、又返回强类型关闭值的面板

现在这套运行时 DSL 也覆盖了更适合工具面板的控件，包括 `TextureRect`、浮点与整数 `SpinBox` 辅助方法，以及支持图标项的 `OptionButton` 重载。

如果你想看更完整的运行时构建示例，可以运行 `Example/03/RunMe_Example03.tscn`，里面展示了通过 `PanelBuilder` 创建 `RichTextLabel`、`TextureButton`、`ColorPickerButton`、`HSlider` 和 `ProgressBar`。

### 如何理解 builder 回调

builder 回调可以理解为两件事同时发生的地方：

1. 你在这里构建出最终会成为面板内容的 `Control` 树。
2. 你在这里注册这个运行时面板的生命周期事件。

这两件事虽然写在一起，但时机并不相同。控件树是在运行时面板被创建时构造的，而 `OnPanelOpen` 则会在每次打开面板时触发。只要一个面板存在“可能重复打开”的可能性，你就应该把“一次性初始化”和“每次打开都要重置的逻辑”明确分开。

### 生命周期与状态组织

当你用 `PanelBuilder` 构建有状态的运行时界面时，比较稳定的组织方式通常是：

1. 用 `LateInit<T>()` 声明那些需要在回调或生命周期事件里继续访问的控件引用。
2. 用普通的 C# 变量或状态对象保存当前面板状态，并让这些状态被回调闭包捕获。
3. 在 `OnPanelInitialized` 里做一次性的接线工作，例如按钮事件、取消键行为、面板级输入绑定。
4. 在 `OnPanelOpen` 里读取 `CurrentOpenArg`，为本次打开重置状态，再把状态同步回控件。
5. 在流程完成时通过运行时句柄关闭面板，并在需要时返回强类型结果。

`LateInit<T>()` 很重要，因为复杂控件树经常会写在多层容器表达式里。它提供了一个类型安全的占位引用，让你可以一边在嵌套表达式里创建子控件，一边在稍后的回调和生命周期事件中继续访问这个控件。

`OnPanelInitialized` 可以把它当成运行时面板版本的 `_OnPanelInitialize()`。适合放在这里的通常是一劳永逸的事情，例如按钮点击委托、取消键逻辑、Token 注册、面板级输入绑定。

`OnPanelOpen` 则更接近 `_OnPanelOpen(...)`。如果面板有打开参数，这里通常就是读取 `CurrentOpenArg`、刷新当前状态和恢复默认焦点的位置。

对于 `CreatePanelArg2` 来说，运行时句柄同时还是一个强类型的关闭通道。这也是这套 DSL 很适合做选择器、步骤式对话框、临时编辑器的原因之一，因为整段交互流程可以保持在同一段 C# 逻辑里，同时又仍然保留一个明确的返回值。

### 这套 DSL 目前能覆盖什么

这些 helper 的目标不是替代 Godot，而是把运行时代码构建 UI 时最重复的那部分 `new`、属性赋值和 `AddChild` 流程压缩掉。

从大的类别上看，它目前已经覆盖：

- 布局容器，例如 `VBox`、`HBox`、`Grid`、`Scroll`、`Panel`、`Center`、`HSplit` 和 `VSplit`
- 文本与展示控件，例如 `Label`、`RichTextLabel`、`TextureRect`
- 交互控件，例如 `Button`、`TextureButton`、`CheckButton`、`LineEdit`、`TextEdit`
- 数值与编辑控件，例如 `HSlider`、`VSlider`、浮点与整数 `SpinBox`、`ColorPickerButton`、`ProgressBar`
- 列表与选择控件，例如 `OptionButton`、`ItemList`、`Tree`
- Tree 辅助 API，例如 `TreeRoot(...)` 和 `TreeItem(...)`

同时，helper 上的 `init` 回调仍然直接暴露 Godot 原生控件实例，所以当默认行为不够时，你依然可以继续设置原生属性、主题覆盖和各种 Godot 特性，而不是被 DSL 封死。

### 运行时面板特有的能力

除了生成控件树以外，这套 DSL 还把普通 `Control` 工厂本来没有的“面板能力”直接暴露了出来：

- `OnPanelInitialized`、`OnPanelOpen`、`OnPanelClose` 等事件直接映射到运行时面板生命周期
- 运行时句柄暴露了 `Close()`、强类型关闭值、取消 Token 以及开关面板补间完成 Token
- 运行时句柄可以注册面板级输入，例如 `RegisterInputCancel`、`RegisterInputAxis`、`RegisterInputVector`

所以它不只是“少写一点控件创建代码”的语法糖，它实际上是在同一个位置里同时处理“控件树”和“框架面板模型”的接合点。

### 当前限制

运行时构建的面板目前只支持 `ClosePolicy.Delete`。从使用方式上理解，就是更适合把它当作“运行时生成并销毁的面板对象”，而不是像编辑器里制作好的场景资源那样依赖缓存复用。

`Example/03/RunMe_Example03.tscn` 适合看这套 helper 控件的基本组合，`Example/04/RunMe_Example04.tscn` 则演示了更完整的流程，包括实时状态刷新、嵌套确认面板，以及带类型的关闭返回值。
