# Scoria

Scoria is a cross-platform TUI framework for .NET built directly on the terminal with no external dependencies.

It provides the low-level primitives for building terminal user interfaces — rendering character grids with per-cell color, style, and alpha blending, handling keyboard and mouse input, and managing the terminal lifecycle. The framework is designed to be lightweight and composable, giving applications full control over the rendering pipeline without imposing a specific widget model or layout system.

Platform support is handled through internal drivers, with separate backends for Windows (P/Invoke to the Win32 console API) and Linux (direct terminal interaction via ANSI escape sequences and ioctl).

The project is in its early stages, and the public API reflects that — only the foundational layer has been shaped so far.
