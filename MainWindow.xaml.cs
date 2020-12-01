using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Microsoft.Win32;

using static KeyFloat.NativeMethods;

namespace KeyFloat {
    /// <summary>d
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private static MainWindow mainW;
        private LowLevelKeyboardListener _listener;
        public MainWindow() {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
             _listener = new LowLevelKeyboardListener();
             _listener.OnKeyPressed += _listener_OnKeyPressed;

             _listener.HookKeyboard();
            mainW = this;
            BlurBG(true);
        }
        
        void _listener_OnKeyPressed(object sender, KeyPressedArgs e) {
            string key = e.KeyPressed.ToString();
            string ch = e.Ch.ToString();
            this.textBox_DisplayKeyboardInput.Content = key;
            this.textBox_DisplayKeyboardInput.Content += " ( " + ch+" ) ";
            this.textBox_DisplayKeyboardInput.Content += !e.Ev?"-UP":"";

            foreach (TextBox tb in mainW.Grid.Children.OfType<TextBox>()) {
                if (tb.Text == key || tb.Text == ch) {
                    if (e.Ev) {
                        tb.Background = new SolidColorBrush(Color.FromArgb(204, 255, 255, 255));
                        tb.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                    } else {
                        tb.Background = new SolidColorBrush(Color.FromArgb(63, 255, 255, 255));
                        tb.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    }
                }
            }
            
            _listener.UnHookKeyboard();
            _listener.HookKeyboard();
        }
        
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            _listener.UnHookKeyboard();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e) {
            mainW.Focus();
            // Kill logical focus
            FocusManager.SetFocusedElement(FocusManager.GetFocusScope(mainW), null);
            // Kill keyboard focus
            Keyboard.ClearFocus();
            //move
            if (Mouse.LeftButton == MouseButtonState.Pressed) {
                try {
                    this.DragMove();
                } catch (Exception ex) {
                    #if DEBUG
                    MessageBox.Show("DEBUG: " + ex.ToString());
                    //throw;
                    #endif
                    //System.InvalidOperationException
                    //dragdrop with only leftclick
                    //dragdrop must be with pressed click
                }
            }
        }
        ////////////////////////////////////////////

        public static void StartWithWindows(bool doStart = false) {
            string execPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            const string name = nameof(KeyFloat);
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            if (doStart) {
                key.SetValue(name, execPath);
            } else {
                key.DeleteValue(name);
            }
        }
        
        public static void BlurBG(bool enableBlur) {
                if (enableBlur) {
                    NativeMethods.EnableBlur(AccentState.ACCENT_ENABLE_BLURBEHIND, mainW);
                } else {
                    NativeMethods.EnableBlur(AccentState.ACCENT_ENABLE_GRADIENT, mainW);
                }
            
        }


        private void CheckBox_Click(object sender, RoutedEventArgs e) {
            BlurBG(Blur.IsChecked==true);
        }

        private void Close_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }
   
}
