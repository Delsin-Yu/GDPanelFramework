# GD Panel Framework [Work In Progress]

[![GitHub Release](https://img.shields.io/github/v/release/Delsin-Yu/GDPanelFramework)](https://github.com/Delsin-Yu/GDPanelFramework/releases/Latest) [![NuGet Version](https://img.shields.io/nuget/v/GDPanelFramework)](https://www.nuget.org/packages/GDPanelFramework) ![NuGet Downloads](https://img.shields.io/nuget/dt/GDPanelFramework) [![Stars](https://img.shields.io/github/stars/Delsin-Yu/GDPanelFramework?color=brightgreen)](https://github.com/Delsin-Yu/GDPanelFramework/stargazers) [![License](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/Delsin-Yu/GDPanelFramework/blob/main/LICENSE)

## Introduction

Tested in `Godot 4.2/4.3` with .Net module.
***GD Panel Framework*** is a `Godot 4` UI Management System designed to provide a `flexible`, `panel-based`, `single-focus point`, `Gamepad + Keyboard + Keyboard&Mouse friendly` UI programming experience.

This framework groups `sets` of `user interactions` into a `UIPanel`, which includes a combination of the following:

1. `Controls`, such as `button`, `label`, and `container`.
2. `Inputs`, which is a set of developer-defined input actions binds with this panel.

These `user interactions` are `panel-scoped`, which means they only stay active when the `panel` is active; this simplifies the workflow for maintaining large amounts of discrete `Controls` and `Global Input Actions` and allows developers to focus on programming game logic (*not collecting and toggling `Controls` or adding more `if`s into a global `_Input` method*).

## Simple API Usage

```csharp
// WIP: API usage should be simple and intuitive
```

## Framework Documentation

### Panel Instantiation and Caching

WIP: Introduction to creating panel instance from packed scene
WIP: Introduction to framework-level panel caching

### Panel Lifetime Explained

WIP: Introduction to the control flow: `The caller opens a panel and transfers the control flow to it; the panel closes itself and returns the control flow to the caller`
WIP: Ways of opening a panel: `async/await` style API, `delegate/callback` style API, and `forget` style API
WIP: The `PreviousPanelVisual` enum argument, the ability to configure the visual behavior of the previous panel
WIP: The `ClosePolicy` enum argument, choose to cache or delete the closing panel
WIP: Invalid operations and error reporting

### API Documentation

#### Panel vs PanelArg

WIP: Introduction to passing/returning arguments to/from a panel
WIP: The `Empty` Struct

#### Panel Parent Container Management

WIP: Introduction to configuring the parent container for the opening panels
WIP: The `RootPanelContainer` Node
WIP: The `PushPanelParent` Method
WIP: The `PopPanelParent` Method

#### Input Binding / Routing

WIP: Introduction to the framework level Input Routing
WIP: The `RegisterInput` Method
WIP: The `RemoveInput` Method
WIP: The `GodotBuiltinActionNames` Static Class
WIP: The `PanelManager.UICancelActionName` Property
WIP: The `RegisterCancelInput` Method
WIP: The `RemoveCancelInput` Method
WIP: The `EnableCloseWithCancelKey` Method
WIP: The `DisableCloseWithCancelKey` Method

#### Lifetime Methods

WIP: Introduction to the `Protected` panel lifetime methods
WIP: The `_OnPanelInitialize` Method
WIP: The `_OnPanelOpen` Method
WIP: The `_OnPanelClose` Method
WIP: The `_OnPanelPredelete` Method
WIP: The `_OnPanelNotification` Method
WIP: The `ClosePanel` Method

#### Panel Visual Behavior

WIP: Introduction to the `PanelTweener` and `IPanelTweener` interface
WIP: The `PanelManager.IPanelTweener` Property
WIP: The `IPanelTweener` Property in Panel
WIP: The Built-in `NonePanelTweener`
WIP: The Built-in `FadePanelTweener`

#### AsyncInterop Class

WIP: Introduction to converting a `delegate/callback` style api into `async/await` style api
