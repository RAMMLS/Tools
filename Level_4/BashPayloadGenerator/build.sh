#!/bin/bash
cd src
mcs -pkg:dotnet -out:BashPayloadGenerator.exe \
  Program.cs MainForm.cs PayloadGenerator.cs \
  -r:System.Windows.Forms -r:System.Drawing
mono BashPayloadGenerator.exe
