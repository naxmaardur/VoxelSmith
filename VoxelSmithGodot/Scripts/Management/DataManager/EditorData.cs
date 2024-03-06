﻿using System;
using System.Collections.Generic;

[Serializable]
public class EditorData
{
    public string version;
    public Guid? lastProject;
    public Dictionary<Guid, string> palettePaths;
    public Dictionary<Guid, string> savePaths;
    public Dictionary<Guid, string> exportPaths;
    public EditorData() 
    {
        version = "0.1";
        lastProject = null;
        palettePaths = new Dictionary<Guid, string>();
        savePaths = new Dictionary<Guid, string>();
        exportPaths = new Dictionary<Guid, string>();
    }
}

