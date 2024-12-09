namespace GodotPanelFramework;

/// <summary>
/// Stores a list of pre-defined Godot's built-in ui action names. 
/// </summary>
public static class BuiltinInputNames
{
    /// <summary>Corresponds to the built-in ui action: ui_accept.</summary>
    public const string UIAccept = "ui_accept";

    /// <summary>Corresponds to the built-in ui action: ui_select.</summary>
    public const string UISelect = "ui_select";

    /// <summary>Corresponds to the built-in ui action: ui_cancel.</summary>
    public const string UICancel = "ui_cancel";

    /// <summary>Corresponds to the built-in ui action: ui_focus_next.</summary>
    public const string UIFocusNext = "ui_focus_next";

    /// <summary>Corresponds to the built-in ui action: ui_focus_prev.</summary>
    public const string UIFocusPrev = "ui_focus_prev";

    /// <summary>Corresponds to the built-in ui action: ui_left.</summary>
    public const string UILeft = "ui_left";

    /// <summary>Corresponds to the built-in ui action: ui_right.</summary>
    public const string UIRight = "ui_right";

    /// <summary>Corresponds to the built-in ui action: ui_up.</summary>
    public const string UIUp = "ui_up";

    /// <summary>Corresponds to the built-in ui action: ui_down.</summary>
    public const string UIDown = "ui_down";

    /// <summary>Corresponds to the built-in ui action: ui_page_up.</summary>
    public const string UIPageUp = "ui_page_up";

    /// <summary>Corresponds to the built-in ui action: ui_page_down.</summary>
    public const string UIPageDown = "ui_page_down";

    /// <summary>Corresponds to the built-in ui action: ui_home.</summary>
    public const string UIHome = "ui_home";

    /// <summary>Corresponds to the built-in ui action: ui_end.</summary>
    public const string UIEnd = "ui_end";

    /// <summary>Corresponds to the built-in ui action: ui_cut.</summary>
    public const string UICut = "ui_cut";

    /// <summary>Corresponds to the built-in ui action: ui_copy.</summary>
    public const string UICopy = "ui_copy";

    /// <summary>Corresponds to the built-in ui action: ui_paste.</summary>
    public const string UIPaste = "ui_paste";

    /// <summary>Corresponds to the built-in ui action: ui_undo.</summary>
    public const string UIUndo = "ui_undo";

    /// <summary>Corresponds to the built-in ui action: ui_redo.</summary>
    public const string UIRedo = "ui_redo";

    /// <summary>Corresponds to the built-in ui action: ui_text_completion_query.</summary>
    public const string UITextCompletionQuery = "ui_text_completion_query";

    /// <summary>Corresponds to the built-in ui action: ui_text_newline.</summary>
    public const string UITextNewline = "ui_text_newline";

    /// <summary>Corresponds to the built-in ui action: ui_text_newline_blank.</summary>
    public const string UITextNewlineBlank = "ui_text_newline_blank";

    /// <summary>Corresponds to the built-in ui action: ui_text_newline_above.</summary>
    public const string UITextNewlineAbove = "ui_text_newline_above";

    /// <summary>Corresponds to the built-in ui action: ui_text_indent.</summary>
    public const string UITextIndent = "ui_text_indent";

    /// <summary>Corresponds to the built-in ui action: ui_text_dedent.</summary>
    public const string UITextDedent = "ui_text_dedent";

    /// <summary>Corresponds to the built-in ui action: ui_text_backspace.</summary>
    public const string UITextBackspace = "ui_text_backspace";

    /// <summary>Corresponds to the built-in ui action: ui_text_backspace_word.</summary>
    public const string UITextBackspaceWord = "ui_text_backspace_word";

    /// <summary>Corresponds to the built-in ui action: ui_text_backspace_all_to_left.</summary>
    public const string UITextBackspaceAllToLeft = "ui_text_backspace_all_to_left";

    /// <summary>Corresponds to the built-in ui action: ui_text_delete.</summary>
    public const string UITextDelete = "ui_text_delete";

    /// <summary>Corresponds to the built-in ui action: ui_text_delete_word.</summary>
    public const string UITextDeleteWord = "ui_text_delete_word";

    /// <summary>Corresponds to the built-in ui action: ui_text_delete_all_to_right.</summary>
    public const string UITextDeleteAllToRight = "ui_text_delete_all_to_right";

    /// <summary>Corresponds to the built-in ui action: ui_text_caret_left.</summary>
    public const string UITextCaretLeft = "ui_text_caret_left";

    /// <summary>Corresponds to the built-in ui action: ui_text_caret_word_left.</summary>
    public const string UITextCaretWordLeft = "ui_text_caret_word_left";

    /// <summary>Corresponds to the built-in ui action: ui_text_caret_right.</summary>
    public const string UITextCaretRight = "ui_text_caret_right";

    /// <summary>Corresponds to the built-in ui action: ui_text_caret_word_right.</summary>
    public const string UITextCaretWordRight = "ui_text_caret_word_right";

    /// <summary>Corresponds to the built-in ui action: ui_text_caret_up.</summary>
    public const string UITextCaretUp = "ui_text_caret_up";

    /// <summary>Corresponds to the built-in ui action: ui_text_caret_down.</summary>
    public const string UITextCaretDown = "ui_text_caret_down";

    /// <summary>Corresponds to the built-in ui action: ui_text_caret_line_start.</summary>
    public const string UITextCaretLineStart = "ui_text_caret_line_start";

    /// <summary>Corresponds to the built-in ui action: ui_text_caret_line_end.</summary>
    public const string UITextCaretLineEnd = "ui_text_caret_line_end";

    /// <summary>Corresponds to the built-in ui action: ui_text_caret_page_up.</summary>
    public const string UITextCaretPageUp = "ui_text_caret_page_up";

    /// <summary>Corresponds to the built-in ui action: ui_text_caret_page_down.</summary>
    public const string UITextCaretPageDown = "ui_text_caret_page_down";

    /// <summary>Corresponds to the built-in ui action: ui_text_caret_document_start.</summary>
    public const string UITextCaretDocumentStart = "ui_text_caret_document_start";

    /// <summary>Corresponds to the built-in ui action: ui_text_caret_document_end.</summary>
    public const string UITextCaretDocumentEnd = "ui_text_caret_document_end";

    /// <summary>Corresponds to the built-in ui action: ui_text_caret_add_below.</summary>
    public const string UITextCaretAddBelow = "ui_text_caret_add_below";

    /// <summary>Corresponds to the built-in ui action: ui_text_caret_add_above.</summary>
    public const string UITextCaretAddAbove = "ui_text_caret_add_above";

    /// <summary>Corresponds to the built-in ui action: ui_text_scroll_up.</summary>
    public const string UITextScrollUp = "ui_text_scroll_up";

    /// <summary>Corresponds to the built-in ui action: ui_text_scroll_down.</summary>
    public const string UITextScrollDown = "ui_text_scroll_down";

    /// <summary>Corresponds to the built-in ui action: ui_text_select_all.</summary>
    public const string UITextSelectAll = "ui_text_select_all";

    /// <summary>Corresponds to the built-in ui action: ui_text_select_word_under_caret.</summary>
    public const string UITextSelectWordUnderCaret = "ui_text_select_word_under_caret";

    /// <summary>Corresponds to the built-in ui action: ui_text_add_selection_for_next_occurrence.</summary>
    public const string UITextAddSelectionForNextOccurrence = "ui_text_add_selection_for_next_occurrence";

    /// <summary>Corresponds to the built-in ui action: ui_text_clear_carets_and_selection.</summary>
    public const string UITextClearCaretsAndSelection = "ui_text_clear_carets_and_selection";

    /// <summary>Corresponds to the built-in ui action: ui_text_toggle_insert_mode.</summary>
    public const string UITextToggleInsertMode = "ui_text_toggle_insert_mode";

    /// <summary>Corresponds to the built-in ui action: ui_text_submit.</summary>
    public const string UITextSubmit = "ui_text_submit";

    /// <summary>Corresponds to the built-in ui action: ui_graph_duplicate.</summary>
    public const string UIGraphDuplicate = "ui_graph_duplicate";

    /// <summary>Corresponds to the built-in ui action: ui_graph_delete.</summary>
    public const string UIGraphDelete = "ui_graph_delete";

    /// <summary>Corresponds to the built-in ui action: ui_filedialog_up_one_level.</summary>
    public const string UIFiledialogUpOneLevel = "ui_filedialog_up_one_level";

    /// <summary>Corresponds to the built-in ui action: ui_filedialog_refresh.</summary>
    public const string UIFiledialogRefresh = "ui_filedialog_refresh";

    /// <summary>Corresponds to the built-in ui action: ui_filedialog_show_hidden.</summary>
    public const string UIFiledialogShowHidden = "ui_filedialog_show_hidden";

    /// <summary>Corresponds to the built-in ui action: ui_swap_input_direction.</summary>
    public const string UISwapInputDirection = "ui_swap_input_direction";
}