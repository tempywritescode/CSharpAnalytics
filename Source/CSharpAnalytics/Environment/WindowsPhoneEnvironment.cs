// Copyright (c) Attack Pattern LLC. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

using System.Globalization;
using System.Windows;

namespace CSharpAnalytics.Environment
{
    /// <summary>
    /// Implements the IEnvironment interface required by analytics to track details of the machine
    /// in a Windows Phone application.
    /// </summary>
    internal class WindowsPhoneEnvironment : IEnvironment
    {
        internal WindowsPhoneEnvironment() {
            _viewportWidth = (uint)Application.Current.Host.Content.ActualWidth;
            _viewportHeight = (uint)Application.Current.Host.Content.ActualHeight;
            _screenHeight = (uint)(_viewportHeight * (Application.Current.Host.Content.ScaleFactor / 100));
            _screenWidth = (uint)(_viewportWidth * (Application.Current.Host.Content.ScaleFactor / 100));
        }

        public string CharacterSet { get { return "UTF-8"; } }

        public string LanguageCode { get { return CultureInfo.CurrentCulture.ToString(); } }

        public string FlashVersion { get { return null; } }
        public bool? JavaEnabled { get { return null; } }

        public uint ScreenColorDepth { get { return 32; } }

        private uint _screenHeight;
        public uint ScreenHeight
        {
            get { return _screenHeight; }
        }

        private uint _screenWidth;
        public uint ScreenWidth
        {
            get { return _screenWidth; }
        }

        private uint _viewportHeight;
        public uint ViewportHeight
        {
            get { return _viewportHeight; }
        }

        private uint _viewportWidth;
        public uint ViewportWidth
        {
            get { return _viewportWidth; }
        }
    }
}