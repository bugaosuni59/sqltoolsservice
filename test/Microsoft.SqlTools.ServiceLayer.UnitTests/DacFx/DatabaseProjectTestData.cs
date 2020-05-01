﻿//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

namespace Microsoft.SqlTools.ServiceLayer.UnitTests.DacFx.DatabaseProject
{
    internal static class DatabaseProjectTestData
    {
        public const string ValidProjectXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Project DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"" ToolsVersion=""4.0"">
  <PropertyGroup>
    <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>
    <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>
    <Name>BuildSimpleProject</Name>
    <SchemaVersion>2.0</SchemaVersion>
    <ProjectVersion>4.1</ProjectVersion>
    <ProjectGuid>{9827e7bd-44b2-4576-ba10-293746e1fff5}</ProjectGuid>
    <DSP>Microsoft.Data.Tools.Schema.Sql.Sql130DatabaseSchemaProvider</DSP>
    <OutputType>Database</OutputType>
    <RootPath>
    </RootPath>
    <RootNamespace>BuildSimpleProject</RootNamespace>
    <AssemblyName>BuildSimpleProject</AssemblyName>
    <ModelCollation>1033, CI</ModelCollation>
    <DefaultFileStructure>BySchemaAndSchemaType</DefaultFileStructure>
    <DeployToDatabase>True</DeployToDatabase>
    <TargetFrameworkVersion>v4.5</TargetFrameworkVersion>
    <TargetLanguage>CS</TargetLanguage>
    <AppDesignerFolder>Properties</AppDesignerFolder>
    <SqlServerVerification>False</SqlServerVerification>
    <IncludeCompositeObjects>True</IncludeCompositeObjects>
    <TargetDatabaseSet>True</TargetDatabaseSet>
  </PropertyGroup>
  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' "">
    <OutputPath>bin\Release\</OutputPath>
    <BuildScriptName>$(MSBuildProjectName).sql</BuildScriptName>
    <TreatWarningsAsErrors>False</TreatWarningsAsErrors>
    <DebugType>pdbonly</DebugType>
    <Optimize>true</Optimize>
    <DefineDebug>false</DefineDebug>
    <DefineTrace>true</DefineTrace>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
  </PropertyGroup>
  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' "">
    <OutputPath>bin\TestDacPacPath\</OutputPath>
    <BuildScriptName>$(MSBuildProjectName).sql</BuildScriptName>
    <TreatWarningsAsErrors>false</TreatWarningsAsErrors>
    <DebugSymbols>true</DebugSymbols>
    <DebugType>full</DebugType>
    <Optimize>false</Optimize>
    <DefineDebug>true</DefineDebug>
    <DefineTrace>true</DefineTrace>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
  </PropertyGroup>
  <PropertyGroup>
    <VisualStudioVersion Condition=""'$(VisualStudioVersion)' == ''"">11.0</VisualStudioVersion>
    <!-- Default to the v11.0 targets path if the targets file for the current VS version is not found -->
    <SSDTExists Condition=""Exists('$(MSBuildExtensionsPath)\Microsoft\VisualStudio\v$(VisualStudioVersion)\SSDT\Microsoft.Data.Tools.Schema.SqlTasks.targets')"">True</SSDTExists>
    <VisualStudioVersion Condition=""'$(SSDTExists)' == ''"">11.0</VisualStudioVersion>
  </PropertyGroup>
  <!-- chnage in import condition -->
  <Import Condition=""'$(NetCoreBuild)' == 'true'"" Project=""$(NETCoreTargetsPath)\Microsoft.Data.Tools.Schema.SqlTasks.targets"" />
  <Import Condition=""'$(SQLDBExtensionsRefPath)' != '' And '$(NetCoreBuild)' != 'true'"" Project=""$(SQLDBExtensionsRefPath)\Microsoft.Data.Tools.Schema.SqlTasks.targets"" />
  <Import Condition=""'$(SQLDBExtensionsRefPath)' == '' And '$(NetCoreBuild)' != 'true'"" Project=""$(MSBuildExtensionsPath)\Microsoft\VisualStudio\v$(VisualStudioVersion)\SSDT\Microsoft.Data.Tools.Schema.SqlTasks.targets"" />
  <ItemGroup>
    <Folder Include=""Properties"" />
  </ItemGroup>
  <ItemGroup>
    <Build Include=""RefTable.sql"" />
    <Build Include=""Link\RefTable2.sql"">
      <Link>RefTable2.sql</Link>
    </Build>
  </ItemGroup>
</Project>";

        public const string InvalidProjectXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Project DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"" ToolsVersion=""4.0"">
  <PropertyGroup>
    <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>
    <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>
    <Name>BuildSimpleProject</Name>
    <SchemaVersion>2.0</SchemaVersion>
    <ProjectVersion>4.1</ProjectVersion>
    <ProjectGuid>{9827e7bd-44b2-4576-ba10-293746e1fff5}</ProjectGuid>
    <DSP>Microsoft.Data.Tools.Schema.Sql.Sql130DatabaseSchemaProvider</DSP>
    <OutputType>Database</OutputType>
    <RootPath>
    </RootPath>
    <RootNamespace>BuildSimpleProject</RootNamespace>
    <AssemblyName>BuildSimpleProject</AssemblyName>
    <ModelCollation>1033, CI</ModelCollation>
    <DefaultFileStructure>BySchemaAndSchemaType</DefaultFileStructure>
    <DeployToDatabase>True</DeployToDatabase>
    <TargetFrameworkVersion>v4.5</TargetFrameworkVersion>
    <TargetLanguage>CS</TargetLanguage>
    <AppDesignerFolder>Properties</AppDesignerFolder>
    <SqlServerVerification>False</SqlServerVerification>
    <IncludeCompositeObjects>True</IncludeCompositeObjects>
    <TargetDatabaseSet>True</TargetDatabaseSet>
  </PropertyGroup>
  <PropertyGroup>
    <VisualStudioVersion Condition=""'$(VisualStudioVersion)' == ''"">11.0</VisualStudioVersion>
    <!-- Default to the v11.0 targets path if the targets file for the current VS version is not found -->
    <SSDTExists Condition=""Exists('$(MSBuildExtensionsPath)\Microsoft\VisualStudio\v$(VisualStudioVersion)\SSDT\Microsoft.Data.Tools.Schema.SqlTasks.targets')"">True</SSDTExists>
    <VisualStudioVersion Condition=""'$(SSDTExists)' == ''"">11.0</VisualStudioVersion>
  </PropertyGroup>
  <!-- chnage in import condition -->
  <Import Condition=""'$(NetCoreBuild)' == 'true'"" Project=""$(NETCoreTargetsPath)\Microsoft.Data.Tools.Schema.SqlTasks.targets"" />
  <Import Condition=""'$(SQLDBExtensionsRefPath)' != '' And '$(NetCoreBuild)' != 'true'"" Project=""$(SQLDBExtensionsRefPath)\Microsoft.Data.Tools.Schema.SqlTasks.targets"" />
  <Import Condition=""'$(SQLDBExtensionsRefPath)' == '' And '$(NetCoreBuild)' != 'true'"" Project=""$(MSBuildExtensionsPath)\Microsoft\VisualStudio\v$(VisualStudioVersion)\SSDT\Microsoft.Data.Tools.Schema.SqlTasks.targets"" />
  <ItemGroup>
    <Folder Include=""Properties"" />
  </ItemGroup>
  <ItemGroup>
    <Build Include=""RefTable.sql"" />
    <Build Include=""Link\RefTable2.sql"">
      <Link>RefTable2.sql</Link>
    </Build>
  </ItemGroup>
</Project>";
    }
}
