#region Copyright

/*
 * Developer    : Willy Kimura (WK).
 * Library      : HotkeySelector.
 * License      : MIT.
 * 
 */

#endregion

using System;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Controls;

namespace WK.Libraries.HotkeyListenerNS
{
    /// <summary>
    /// Provides support for enabling standard Windows controls 
    /// and User controls to select hotkeys at runtime. 
    /// Combined with the <see cref="HotkeyListener"/> class, 
    /// you can easily enable the selection and registering of 
    /// hotkeys for a seamless end-user experience.
    /// </summary>
    //[DebuggerStepThrough]
    [Description("Provides support for enabling standard Windows controls " +
                 "and User controls to select hotkeys at runtime.")]

    ///This class is addition for HotkeyListenerNS library,
    ///here is a link to author: https://github.com/Willy-Kimura/HotkeyListener/issues/12

    public partial class HotkeySelectorWPF : Component
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="HotkeySelector"/> class.
        /// </summary>
        public HotkeySelectorWPF() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HotkeySelector"/> class.
        /// </summary>
        public HotkeySelectorWPF(IContainer container)
        {
            container.Add(this);

            //InitializeComponent();
        }

        #endregion

        #region Fields

        // These variables store the selected hotkey and modifier key(s).
        private Keys _hotkey = Keys.None;
        private Keys _modifiers = Keys.None;

        // ArrayLists used to enforce the use of proper modifiers.
        // Shift+A isn't a valid hotkey, for instance.
        private ArrayList _needNonShiftModifier = null;
        private ArrayList _needNonAltGrModifier = null;

        // Stores the list of enabled hotkey selection controls.
        private List<System.Windows.Controls.Control> _controls = new List<System.Windows.Controls.Control>();

        #endregion

        #region Properties

        #region Public

        /// <summary>
        /// Gets or sets the text to be displayed in a 
        /// control when no hotkey has been set. 
        /// (Preferred default text is "None")
        /// </summary>
        public string EmptyHotkeyText { get; set; } = "None";

        /// <summary>
        /// Gets or sets the text to be displayed in a control 
        /// when an invalid or unsupported hotkey is pressed.
        /// (Preferred default text is "(Unsupported)")
        /// </summary>
        public string InvalidHotkeyText { get; set; } = "Unsupported";

        #endregion

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// Enables a control for hotkey selection and previewing.
        /// This will make use of the control's Text property to 
        /// preview the current hotkey selected.
        /// </summary>
        /// <param name="control">The control to enable.</param>
        public bool Enable(System.Windows.Controls.TextBox control)
        {
            try
            {
                control.Text = EmptyHotkeyText;

                control.KeyDown += new System.Windows.Input.KeyEventHandler(OnKeyDown);
                control.KeyUp += new System.Windows.Input.KeyEventHandler(OnKeyUp);

                ResetModifiers();

                try
                {
                    _controls.Add(control);
                }
                catch (Exception) { }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Enables a control for hotkey selection and previewing.
        /// This will make use of the control's Text property to 
        /// preview the current hotkey selected.
        /// </summary>
        /// <param name="control">The control to enable.</param>
        /// <param name="hotkey">Assign the default hotkey to be previewed in the control.</param>
        public bool Enable(System.Windows.Controls.TextBox control, Hotkey hotkey)
        {
            try
            {
                Enable(control);

                _hotkey = hotkey.KeyCode;
                _modifiers = hotkey.Modifiers;

                Refresh(control);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Disables a control for hotkey selection and previewing.
        /// </summary>
        /// <param name="control">The control to disable.</param>
        /// <param name="clearKeys">Clear the control's previewed keys?</param>
        public bool Disable(System.Windows.Controls.TextBox control, bool clearKeys = true)
        {
            try
            {
                control.KeyDown -= OnKeyDown;
                control.KeyUp -= OnKeyUp;

                if (clearKeys)
                    control.Text = string.Empty;

                try
                {
                    if (_controls.Contains(control))
                        _controls.Remove(control);
                }
                catch (Exception) { }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether a specific 
        /// control is enabled for hotkey selection.
        /// </summary>
        /// <param name="control">The control to determine.</param>
        public bool IsEnabled(System.Windows.Controls.TextBox control)
        {
            if (_controls.Contains(control))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Sets a hotkey selection to be previewed in a control. 
        /// Thsi does not automatically enable the control for 
        /// hotkey selection. For this, please use the <see cref="Enable(Control)"/> method.
        /// </summary>
        /// <param name="control">The control to set.</param>
        /// <param name="hotkey">Provide a standard key or key combination string.</param>
        public bool Set(System.Windows.Controls.TextBox control, Hotkey hotkey)
        {
            try
            {
                Refresh(control);

                control.Text = Convert(hotkey);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Sets a hotkey selection to be previewed in a control. 
        /// Thsi does not automatically enable the control for 
        /// hotkey selection. For this, please use the <see cref="Enable(Control)"/> method.
        /// </summary>
        /// <param name="control">The control to set.</param>
        /// <param name="key">Provide a standard key selection.</param>
        /// <param name="modifiers">Provide a modifier key selection.</param>
        public bool Set(System.Windows.Controls.TextBox control, Keys key, Keys modifiers)
        {
            try
            {
                _hotkey = key;
                _modifiers = modifiers;

                Refresh(control);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Clears the currently previewed hotkey 
        /// selection displayed in a control.
        /// </summary>
        public void Clear(System.Windows.Controls.TextBox control)
        {
            this._hotkey = Keys.None;
            this._modifiers = Keys.None;

            Refresh(control);
        }

        /// <summary>
        /// (Variant of the <see cref="Clear(System.Windows.Controls.TextBox)"/> method) 
        /// Clears the currently previewed hotkey 
        /// selection displayed in a control.
        /// </summary>
        public void Reset(System.Windows.Controls.TextBox control)
        {
            this._hotkey = Keys.None;
            this._modifiers = Keys.None;

            Refresh(control);
        }

        /// <summary>
        /// [Helper] Converts keys or key combinations to their string types.
        /// </summary>
        /// <param name="hotkey">The hotkey to convert.</param>
        public string Convert(Hotkey hotkey)
        {
            try
            {
                _hotkey = hotkey.KeyCode;
                _modifiers = hotkey.Modifiers;

                string parsedHotkey = string.Empty;

                // No modifier or shift only, and a hotkey that needs another modifier.
                if ((_modifiers == Keys.Shift || _modifiers == Keys.None))
                {
                    if (_needNonShiftModifier != null && _needNonShiftModifier.Contains((int)this._hotkey))
                    {
                        if (this._modifiers == Keys.None)
                        {
                            // Set Ctrl+Alt as the modifier unless Ctrl+Alt+<key> won't work.
                            if (_needNonAltGrModifier.Contains((int)this._hotkey) == false)
                            {
                                this._modifiers = Keys.Alt | Keys.Control;
                            }
                            else
                            {
                                // ...In that case, use Shift+Alt instead.
                                this._modifiers = Keys.Alt | Keys.Shift;
                            }
                        }
                        else
                        {
                            // User pressed Shift and an invalid key (e.g. a letter or a number), 
                            // that needs another set of modifier keys.
                            this._hotkey = Keys.None;
                        }
                    }
                }

                // Without this code, pressing only Ctrl 
                // will show up as "Control + ControlKey", etc.
                if (this._hotkey == Keys.Menu || /* Alt */
                    this._hotkey == Keys.ShiftKey ||
                    this._hotkey == Keys.ControlKey)
                {
                    this._hotkey = Keys.None;
                }

                if (this._modifiers == Keys.None)
                {
                    // LWin/RWin don't work as hotkeys...
                    // (neither do they work as modifier keys in .NET 2.0).
                    if (_hotkey == Keys.None || _hotkey == Keys.LWin || _hotkey == Keys.RWin)
                    {
                        parsedHotkey = string.Empty;
                    }
                    else
                    {
                        parsedHotkey = this._hotkey.ToString();
                    }
                }
                else
                {
                    parsedHotkey = this._modifiers.ToString() + " + " + this._hotkey.ToString();
                }

                return parsedHotkey;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        #endregion

        #region Private

        /// <summary>
        /// Resets the hotkey modifiers to their defaults.
        /// </summary>
        private void ResetModifiers()
        {
            // Fill the ArrayLists that contain  
            // all invalid hotkey combinations.
            _needNonShiftModifier = new ArrayList();
            _needNonAltGrModifier = new ArrayList();

            PopulateModifierLists();
        }

        /// <summary>
        /// Populates the ArrayLists specifying disallowed Hotkeys 
        /// such as Shift+A, Ctrl+Alt+4 (produces 'dollar' sign).
        /// </summary>
        private void PopulateModifierLists()
        {
            // Shift + 0 - 9, A - Z.
            for (Keys k = Keys.D0; k <= Keys.Z; k++)
                _needNonShiftModifier.Add((int)k);

            // Shift + Numpad keys.
            for (Keys k = Keys.NumPad0; k <= Keys.NumPad9; k++)
                _needNonShiftModifier.Add((int)k);

            // Shift + Misc (,;<./ etc).
            for (Keys k = Keys.Oem1; k <= Keys.OemBackslash; k++)
                _needNonShiftModifier.Add((int)k);

            // Shift + Space, PgUp, PgDn, End, Home.
            for (Keys k = Keys.Space; k <= Keys.Home; k++)
                _needNonShiftModifier.Add((int)k);

            // Misc keys that we can't loop through.
            _needNonShiftModifier.Add((int)Keys.Insert);
            _needNonShiftModifier.Add((int)Keys.Help);
            _needNonShiftModifier.Add((int)Keys.Multiply);
            _needNonShiftModifier.Add((int)Keys.Add);
            _needNonShiftModifier.Add((int)Keys.Subtract);
            _needNonShiftModifier.Add((int)Keys.Divide);
            _needNonShiftModifier.Add((int)Keys.Decimal);
            _needNonShiftModifier.Add((int)Keys.Return);
            _needNonShiftModifier.Add((int)Keys.Escape);
            _needNonShiftModifier.Add((int)Keys.NumLock);
            _needNonShiftModifier.Add((int)Keys.Scroll);
            _needNonShiftModifier.Add((int)Keys.Pause);

            // Ctrl+Alt + 0 - 9.
            for (Keys k = Keys.D0; k <= Keys.D9; k++)
                _needNonAltGrModifier.Add((int)k);
        }

        /// <summary>
        /// Refreshes the previewed hotkey combination displayed in a control.
        /// </summary>
        /// <param name="control">
        /// The control providing hotkey selection.
        /// </param>
        private void Refresh(System.Windows.Controls.TextBox control)
        {
            Refresh(control, false);
        }

        /// <summary>
        /// Refreshes the previewed hotkey combination displayed in a control.
        /// </summary>
        /// <param name="control">
        /// The control providing hotkey selection.
        /// </param>
        /// <param name="internalCall">
        /// Specifies whether this function is 
        /// called internally or by the user.
        /// </param>
        private void Refresh(System.Windows.Controls.TextBox control, bool internalCall)
        {
            try
            {
                string parsedHotkey = string.Empty;

                // No hotkey set.
                if (this._hotkey == Keys.None && this._modifiers == Keys.None)
                {
                    control.Text = EmptyHotkeyText;
                    return;
                }

                // LWin/RWin don't work as hotkeys...
                // (neither do they work as modifier keys in .NET 2.0).
                if (this._hotkey == Keys.LWin || this._hotkey == Keys.RWin)
                {
                    control.Text = InvalidHotkeyText;

                    return;
                }

                if (this._modifiers == Keys.None)
                {
                    if (this._hotkey == Keys.None)
                    {
                        control.Text = EmptyHotkeyText;

                        return;
                    }
                    else
                    {
                        // We get here if we've got a hotkey that is valid without a modifier,
                        // like F1-F12, Media-keys etc.
                        control.Text = this._hotkey.ToString();

                        return;
                    }
                }
                else
                {
                    // Only validate input if it comes from the user.
                    if (internalCall == false)
                    {
                        // No modifier or shift only, and a hotkey that needs another modifier.
                        if ((this._modifiers == Keys.Shift || this._modifiers == Keys.None) &&
                            this._needNonShiftModifier.Contains((int)this._hotkey))
                        {
                            if (this._modifiers == Keys.None)
                            {
                                // Set Ctrl+Alt as the modifier unless Ctrl+Alt+<key> won't work.
                                if (_needNonAltGrModifier.Contains((int)this._hotkey) == false)
                                {
                                    this._modifiers = Keys.Alt | Keys.Control;
                                }
                                else
                                {
                                    // ...In that case, use Shift+Alt instead.
                                    this._modifiers = Keys.Alt | Keys.Shift;
                                }
                            }
                            else
                            {
                                // User pressed Shift and an invalid key (e.g. a letter or a number), 
                                // that needs another set of modifier keys.
                                this._hotkey = Keys.None;

                                control.Text = this._modifiers.ToString() + $" + {InvalidHotkeyText}";

                                return;
                            }
                        }
                    }
                }

                // Without this code, pressing only Ctrl 
                // will show up as "Control + ControlKey", etc.
                if (this._hotkey == Keys.Menu || /* Alt */
                    this._hotkey == Keys.ShiftKey ||
                    this._hotkey == Keys.Alt ||
                    this._hotkey == Keys.ControlKey ||
                    this._hotkey == Keys.LControlKey ||
                    this._hotkey == Keys.RControlKey ||
                    this._hotkey == Keys.LShiftKey ||
                    this._hotkey == Keys.RShiftKey)
                {
                    this._hotkey = Keys.None;
                }

                // A final compilation of the processed keys in string format.
                if (this._modifiers == Keys.None)
                    parsedHotkey = this._hotkey.ToString();
                else
                    parsedHotkey = this._modifiers.ToString() + " + " + this._hotkey.ToString();

                control.Text = parsedHotkey;

                return;
            }
            catch (Exception) { }
        }

        #endregion

        #endregion

        #region Events

        #region Private

        /// <summary>
        /// Fires when a key is pressed down. Here, we'll want to update the Text  
        /// property to notify the user what key combination is currently pressed.
        /// </summary>
        private void OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Delete || e.Key == (System.Windows.Input.Key.LeftCtrl | System.Windows.Input.Key.RightCtrl | System.Windows.Input.Key.Delete))
                Reset((System.Windows.Controls.TextBox)sender);

            if (e.Key == (System.Windows.Input.Key.LeftShift | System.Windows.Input.Key.RightShift | System.Windows.Input.Key.Insert))
            {
                this._modifiers = Keys.Shift;
                e.Handled = true;
            }

            // Clear the current hotkey.
            if (e.Key == System.Windows.Input.Key.Back || e.Key == System.Windows.Input.Key.Delete)
            {
                Reset((System.Windows.Controls.TextBox)sender);

                return;
            }
            else
            {
                this._modifiers = ToWinforms(e.KeyboardDevice.Modifiers);
                this._hotkey = (Keys)System.Windows.Input.KeyInterop.VirtualKeyFromKey(e.Key);

                Refresh((System.Windows.Controls.TextBox)sender);
            }
        }

        /// <summary>
        /// Source: https://stackoverflow.com/questions/1153009/how-can-i-convert-system-windows-input-key-to-system-windows-forms-keys
        /// </summary>
        /// <param name="modifier"></param>
        /// <returns></returns>
        private System.Windows.Forms.Keys ToWinforms(System.Windows.Input.ModifierKeys modifier)
        {
            var retVal = System.Windows.Forms.Keys.None;
            if (modifier.HasFlag(System.Windows.Input.ModifierKeys.Alt))
            {
                retVal |= System.Windows.Forms.Keys.Alt;
            }
            if (modifier.HasFlag(System.Windows.Input.ModifierKeys.Control))
            {
                retVal |= System.Windows.Forms.Keys.Control;
            }
            if (modifier.HasFlag(System.Windows.Input.ModifierKeys.None))
            {
                // Pointless I know
                retVal |= System.Windows.Forms.Keys.None;
            }
            if (modifier.HasFlag(System.Windows.Input.ModifierKeys.Shift))
            {
                retVal |= System.Windows.Forms.Keys.Shift;
            }
            if (modifier.HasFlag(System.Windows.Input.ModifierKeys.Windows))
            {
                // Not supported lel
            }
            return retVal;
        }

        /// <summary>
        /// Fires when all keys are released. If the current hotkey isn't valid, reset it.
        /// Otherwise, do nothing and keep the Text and hotkey as it was.
        /// </summary>
        private void OnKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {

            if (this._hotkey == Keys.None && e.KeyboardDevice.Modifiers == System.Windows.Input.ModifierKeys.None)
            {
                Reset((System.Windows.Controls.TextBox)sender);

                return;
            }
        }

        #endregion

        #endregion
    }
}