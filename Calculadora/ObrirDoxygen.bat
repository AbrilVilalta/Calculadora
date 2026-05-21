@echo off
doxygen Doxyfile
start chrome "%~dp0docs\html\index.html"