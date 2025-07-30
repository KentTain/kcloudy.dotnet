﻿using KC.Common;
using KC.Framework.Util;
using KC.IdentityModel;
using KC.IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace KC.Service.Extension
{
    public static class ConsoleExtensions
    {
        public static void Show(this TokenResponse response)
        {
            if (!response.IsError)
            {
                "Token response:".ConsoleGreen();
                Console.WriteLine(response.Json);
                LogUtil.LogDebug(SerializeHelper.ToJson(response));

                if (response.AccessToken.Contains("."))
                {
                    "\nAccess Token (decoded):".ConsoleGreen();

                    var parts = response.AccessToken.Split('.');
                    var header = parts[0];
                    var claims = parts[1];

                    Console.WriteLine(SerializeHelper.ToJson(Encoding.UTF8.GetString(Base64Url.Decode(header))));
                    Console.WriteLine(SerializeHelper.ToJson(Encoding.UTF8.GetString(Base64Url.Decode(claims))));

                    LogUtil.LogDebug("\nheader (decoded):" + SerializeHelper.ToJson(Encoding.UTF8.GetString(Base64Url.Decode(header))));
                    LogUtil.LogDebug("\nclaims (decoded):" + SerializeHelper.ToJson(Encoding.UTF8.GetString(Base64Url.Decode(claims))));
                }
            }
            else
            {
                if (response.ErrorType == ResponseErrorType.Http)
                {
                    "HTTP error: ".ConsoleGreen();
                    Console.WriteLine(response.Error);
                    LogUtil.LogDebug("HTTP error: " + response.Error);
                    "HTTP status code: ".ConsoleGreen();
                    Console.WriteLine(response.HttpStatusCode);
                    LogUtil.LogDebug("HTTP status code: " + response.HttpStatusCode);
                }
                else
                {
                    "Protocol error response:".ConsoleGreen();
                    Console.WriteLine(response.Raw);
                    LogUtil.LogDebug("Protocol error response: " + response.Raw);
                }
            }
        }

        /// <summary>
        /// Writes green text to the console.
        /// </summary>
        /// <param name="text">The text.</param>
        [DebuggerStepThrough]
        public static void ConsoleGreen(this string text)
        {
            text.ColoredWriteLine(ConsoleColor.Green);
        }

        /// <summary>
        /// Writes red text to the console.
        /// </summary>
        /// <param name="text">The text.</param>
        [DebuggerStepThrough]
        public static void ConsoleRed(this string text)
        {
            text.ColoredWriteLine(ConsoleColor.Red);
        }

        /// <summary>
        /// Writes yellow text to the console.
        /// </summary>
        /// <param name="text">The text.</param>
        [DebuggerStepThrough]
        public static void ConsoleYellow(this string text)
        {
            text.ColoredWriteLine(ConsoleColor.Yellow);
        }

        /// <summary>
        /// Writes out text with the specified ConsoleColor.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="color">The color.</param>
        [DebuggerStepThrough]
        public static void ColoredWriteLine(this string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }
    }

}
