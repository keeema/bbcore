﻿using JavaScriptEngineSwitcher.Core;

namespace Lib.ToolsDir
{
    public interface IToolsDir
    {
        string Path { get; }
        string GetTypeScriptVersion();
        void InstallTypeScriptVersion(string version = "*");
        void RunYarn(string dir, string aParams);
        string TypeScriptLibDir { get; }
        string TypeScriptJsContent { get; }
        string LoaderJs { get; }
        string JasmineCoreJs { get; }
        string JasmineBootJs { get; }
        string JasmineDts { get; }
        string WebtIndexHtml { get; }
        string WebtAJs { get; }
        string WebIndexHtml { get; }
        string WebAJs { get; }
        string JasmineDtsPath { get; }

        IJsEngine CreateJsEngine();
    }
}
