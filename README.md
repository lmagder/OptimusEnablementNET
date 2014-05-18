OptimusEnablementNET
===================
This is an example of adding a data export to a C# program to select the correct GPU on an NVIDIA Optimus System.

It works around the fact that you can't directly export a symbol in C# by editing the IL after compilation and the fact you can only export code in .NET by editing the export's value at module load.
