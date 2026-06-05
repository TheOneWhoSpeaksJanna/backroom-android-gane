# Unity Cloud Build Setup

Unity Build Automation should point at this subfolder:

```text
UnityProject
```

Recommended configuration:

```text
Target name: Desolation Android
Branch: main
Project subfolder path: UnityProject
Platform: Android
Bundle ID: com.desolation.thebackrooms
Machine: Micro
Auto-build: Off
```

Use the manual Build button after Unity finishes indexing the repo.

The Unity scaffold is separate from the working Godot project.
